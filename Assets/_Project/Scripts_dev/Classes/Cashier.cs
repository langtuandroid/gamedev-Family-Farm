using System.Collections;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Cashier : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private UIManager _uiManager;
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("linePos")] public GameObject[] LinePositions;
        [SerializeField] private Money moneyPile;
        [SerializeField] private GameObject moneyPrefab;
        [SerializeField] private PlayerControl playerControl;
        [SerializeField] private Animator cashierAnimator;
        
        private float _money;
        private float _moneyTake;
        private AudioSource _audioSource;
        private float _customerTime;
        private CharactersAI _currentCustomer;
        public bool IsTaking { get; private set; }
        public bool IsCashier { get; set; }
        public bool IsPlayer { get; set; }
        public int Line { get; set; }
        private void Start()
        {
            InvokeRepeating("TakeMoney", -1, 1f);
            _audioSource = GetComponent<AudioSource>();

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

        private float Counter(Cart cart)
        {
            float sum=0;
            float multiple = 1;
            LevelMangament[] levelMangaments = FindObjectsOfType<LevelMangament>();
            foreach (GameObject item in cart.cart)
            {
                foreach (LevelMangament levelMangament in levelMangaments)
                {
                    if (levelMangament._goods.Id == item.GetComponent<Product>().Goods.Id)
                        multiple = levelMangament._multiply*levelMangament.Level;
                    if (multiple == 0) multiple = 1;
                }
                sum += item.GetComponent<Product>().Goods.Income * multiple * _gameManager.IncomeBoost *(_gameManager.Money>=1000?0.5f:1);
            }
            return sum;
        }

        private void TakeMoney()
        {
            if ((IsCashier||IsPlayer) && _currentCustomer!=null &&!IsTaking &&!playerControl.IsTakingMoney)
            {
                StartCoroutine(TakeMoneyRoutine());

                if (_uiManager.sound)
                {
                    _audioSource.Play();
                }
            }
        }

        private void ParabolicMovement(GameObject go, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] = go.transform.position;
            path[1] = (targetPosition + go.transform.position) / 2 + new Vector3(0, 2 * height, 0);
            path[2] = targetPosition;

            go.transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.red)
                .SetEase(Ease.OutQuad)
                .OnComplete(OnComplete);
        }

        private IEnumerator TakeMoneyRoutine()
        {
            IsTaking = true;
            float temp =_moneyTake;
            for(int i=0; i < Mathf.Ceil(_moneyTake / _gameManager.MoneyInPack); i++)
            {
                yield return null; //test
                int r = Random.Range(0, 2);
                if (r == 0)
                    _soundManager.CreateSound(_soundManager.Clips[5], transform.position, 0.5f);
                GameObject item = Instantiate(moneyPrefab,LinePositions[0].transform.position,Quaternion.identity);
                moneyPile.UpdatePosition(true);
            
                ParabolicMovement(item, moneyPile.NextPosition, 0.5f, 1.5f,
                    () => {
                        Destroy(item);
                        moneyPile.Quantity += temp > _gameManager.MoneyInPack ? _gameManager.MoneyInPack : temp;
                        temp -= 10;
                    });
            }
            _currentCustomer.GoHome();
            _currentCustomer = null;
            CharactersAI[] customers = FindObjectsOfType<CharactersAI>();
            Line--;

       
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
            moneyPile.CalculatePositions();
            IsTaking = false;
        }
   
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Customer"))
            {
                CharactersAI controller = other.GetComponent<CharactersAI>();
                if (controller.LineNumber == 0 && controller.CheckCompletePath()&&controller.IsMovingHome==false)
                {
                    _moneyTake = Counter(other.GetComponent<CharactersAI>().Cart);
                    _currentCustomer = controller;
                }
            }
        }
    }
}
