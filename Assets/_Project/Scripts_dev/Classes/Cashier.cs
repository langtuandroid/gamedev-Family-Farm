using System.Collections;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Cashier : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private UIManager _uiManager;
        [Inject] private GameManager _gameManager;
        [SerializeField] private MoneyPiile moneyPile;
        [SerializeField] private GameObject moneyPrefab;
        [SerializeField] private PlayerControl playerControl;
        private float money, moneyToTake;
        private AudioSource audioSource;
    
        public bool cashierIsHere,playerIsHere;
        public int inLine;
        public GameObject[] linePos;
        public CharactersAI currentCustomer;
        public Animator cashierAnimator;
        public bool isTaking;

        private float noCustomerTime;
        private void Start()
        {
            InvokeRepeating("TakeMoney", -1, 1f);
            audioSource = GetComponent<AudioSource>();

        }
        private void Update()
        {
            if (transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.tag = "Cashier";
            }
            else
            {
                transform.tag = "Untagged";
            }
        }
    
        float Counter(Cart cart)
        {
            float sum=0;
            float multiple = 1;
            LevelMangament[] levelMangaments = FindObjectsOfType<LevelMangament>();
            foreach (GameObject item in cart.cart)
            {

                foreach (LevelMangament levelMangament in levelMangaments)
                {
                    if (levelMangament.goods.id == item.GetComponent<Product>().goods.id)
                        multiple = levelMangament.multiple*levelMangament.level;
                    if (multiple == 0) multiple = 1;
                }
                sum += item.GetComponent<Product>().goods.income * multiple * _gameManager.incomeBoost *(_gameManager.money>=1000?0.5f:1);
                Debug.Log(sum);
            }
            return sum;
        }
        void TakeMoney()
        {
            if ((cashierIsHere||playerIsHere) && currentCustomer!=null &&!isTaking &&!playerControl.takingMoney)
            {
                StartCoroutine(TakeMoneyDelay());

                if (_uiManager.sound)
                {
                    audioSource.Play();
                }
            }
        }
        void ParabolicMovement(GameObject go, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] = go.transform.position;
            path[1] = (targetPosition + go.transform.position) / 2 + new Vector3(0, 2 * height, 0);
            path[2] = targetPosition;

            go.transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.red)
                .SetEase(Ease.OutQuad)
                .OnComplete(OnComplete);
        }
        IEnumerator TakeMoneyDelay()
        {
            isTaking = true;
            float temp =moneyToTake;
            for(int i=0; i < Mathf.Ceil(moneyToTake / _gameManager.moneyPerPack); i++)
            {
                yield return null; //test
                int r = Random.Range(0, 2);
                if (r == 0)
                    _soundManager.CreateSound(_soundManager.sounds[5], transform.position, 0.5f);
                GameObject item = Instantiate(moneyPrefab,linePos[0].transform.position,Quaternion.identity);
                moneyPile.UpdateNextPos(true);
            
                ParabolicMovement(item, moneyPile.nextPos, 0.5f, 1.5f,
                    () => {
                        Destroy(item);
                        moneyPile.currentQuantity += temp > _gameManager.moneyPerPack ? _gameManager.moneyPerPack : temp;
                        temp -= 10;
                    });
            }
            currentCustomer.GoHome();
            currentCustomer = null;
            CharactersAI[] customers = FindObjectsOfType<CharactersAI>();
            inLine--;

       
            foreach(CharactersAI customer in customers)
            {
                if (customer.transform.CompareTag("Customer"))
                {
                    if (customer.Cashier == this)
                    {
                        customer.GoToOtherLine();
                    }
                }
            }
            cashierAnimator.SetTrigger("Great");
            yield return new WaitForSeconds(0.5f);
            moneyPile.ReCalculatePos();
            isTaking = false;
        }
   
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Customer"))
            {
                CharactersAI controller = other.GetComponent<CharactersAI>();
                if (controller.LineNumber == 0 && controller.CheckCompletePath()&&controller.IsMovingHome==false)
                {
                    moneyToTake = Counter(other.GetComponent<CharactersAI>().Cart);
                    currentCustomer = controller;
                }
            }
        }
    }
}
