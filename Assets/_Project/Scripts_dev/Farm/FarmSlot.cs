using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
using Stack = _Project.Scripts_dev.Classes.Stack;

namespace _Project.Scripts_dev.Farm
{
    public class FarmSlot : MonoBehaviour
    {
        [Inject] private EffectManager _effectManager;
        [Inject] private UIManager _uiManager;
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("products")] [SerializeField] private GameObject[] _products;
        [FormerlySerializedAs("fruits")] [SerializeField] private GameObject[] _fruits;
        [FormerlySerializedAs("need")] [SerializeField] private GameObject _need;
        [FormerlySerializedAs("harvstMeIcon")] [SerializeField] private GameObject _harvestIcon;
        [FormerlySerializedAs("noFruit")] [SerializeField] private bool _noFruit;
        [FormerlySerializedAs("animator")] [SerializeField] private Animator _animator;
        [FormerlySerializedAs("matPlace")] [SerializeField] private MaterialPlace _materialPlace;
        [FormerlySerializedAs("goodsPrefabs")] [SerializeField] private GameObject _goodsPrefabs;
        [FormerlySerializedAs("feedMax")] [SerializeField] private int _maxFeed;
        [FormerlySerializedAs("quantity")] [SerializeField] private int _quantity;
        [FormerlySerializedAs("growTime")] [SerializeField] private float _growTime;
        public int[] materialsNeeded;
        public float plantTime;
        public float harvestTime;
        [FormerlySerializedAs("isFarm")] [SerializeField] private bool _isFarm;
        [FormerlySerializedAs("isFactory")] [SerializeField] private bool _isFactory;
        [FormerlySerializedAs("isCattle")] [SerializeField] private bool _isCattle;
        public GameObject AIWorkingSpot;
        public Stack stack;
        
        private float time;
        private float machineTime;
        private Image timeImage;
        private float completeTime;
        private bool load;
        
        public int productID { get; private set; }
        public int farmStatus { get; private set; }
        public GameObject workingFarmer { get; private set; }
        public bool IsFarm => _isFarm;
        public bool IsFactory => _isFactory;
        public bool IsCattle => _isCattle;
        private void Start()
        {
            productID = _goodsPrefabs.GetComponent<Product>().Goods.Id;
            StartTimerCanvas();
        }
        private void StartTimerCanvas()
        {
            GameObject canvas= Instantiate(_uiManager.timerPrefab, transform.GetChild(0));    
            canvas.transform.localPosition =  new Vector3(0, 5, 0);
            timeImage = canvas.transform.GetChild(0).GetComponent<Image>();
        }
  
        private void Update()
        {
            if (_dataManager.gameData != null&&!load&&_gameManager.load)
            {
                GameData gameData = _dataManager.gameData;
                int[] statuses = gameData.status.ToArray();
                if (_gameManager.playTime < 3)
                {
                    foreach (int status in statuses)
                    {
                
                        if (status== productID)
                        {
                   
                            farmStatus = 2;
                            foreach (GameObject product in _products)
                            {
                                product.SetActive(true);
                                product.transform.localScale = Vector3.one;
                            }

                            if (!_noFruit)
                            {
                                foreach (GameObject fruit in _fruits)
                                {
                                    fruit.SetActive(true);
                                    fruit.transform.localScale = Vector3.one;
                                }
                            }
                            break;
                        }
                    }
                }
            
                load = true;
            }
            if(_dataManager.gameData==null)
            {
                load = true;
            }
            if (!load)
            {
                return;
            }
            if (timeImage == null) StartTimerCanvas();
      
            if ((workingFarmer != null) )
            {
                string tagg = workingFarmer.tag;
                Cart cartt = tagg == "Farmer" ? workingFarmer.GetComponent<CharactersAI>().Cart : workingFarmer.GetComponent<PlayerControl>().Cart;
                if( CheckCart(cartt) )
                {
                    time += Time.deltaTime;
                    machineTime += Time.deltaTime;
                }
         
            }
            else if (farmStatus == 1)
            {
                time += Time.deltaTime;
                machineTime += Time.deltaTime;
            }
            if (workingFarmer != null)
            {
                if (Input.GetMouseButton(0) && workingFarmer.transform.CompareTag("Player"))
                {
               
                    workingFarmer = null;
                    if (farmStatus != 1)
                    {
                        time = 0;
                   
                    }
                }
                else if (workingFarmer.transform.CompareTag("Farmer"))
                {
                    if (workingFarmer.GetComponent<NavMeshAgent>().velocity != Vector3.zero)
                    {
                        workingFarmer = null;
                        if (farmStatus != 1)
                        {
                            time = 0;

                        }
                    }
                }
            }

            if (time > 0&&!IsFactory)
            {
                timeImage.gameObject.SetActive(true);
                timeImage.fillAmount = time / completeTime;
                if (timeImage.fillAmount<0.15) timeImage.gameObject.SetActive(false);
            }
           
            else
            {
                timeImage.gameObject.SetActive(false);

                timeImage.fillAmount = 0;
            }
            if (_need != null)
            {
                if(farmStatus == 0&&workingFarmer ==null)
                {
                    _need.SetActive(true);
                }
                else
                {
                    _need.SetActive(false);
                }
            }
            if (_harvestIcon != null)
            {
                if (farmStatus == 2 && workingFarmer == null)
                {
                    _harvestIcon.SetActive(true);
                }
                else
                {
                    _harvestIcon.SetActive(false);
                }
            }

            ManageFarmSlot();
            TreeGrowing();
        }
        void TreeGrowing()
        {
            if (!IsFarm || farmStatus != 1) return;
            if (_noFruit)
            {
                foreach(GameObject product in _products)
                {
                    product.transform.localScale =  Vector3.one * (0.25f + (time / _growTime)*0.75f);
                }
            }
            else
            {
                if(time < _growTime / 2)
                {
                    foreach (GameObject product in _products)
                    {
                        product.transform.localScale = 1.5f* Vector3.one * (0.25f + (time / (_growTime / 2)) * 0.75f);
                   

                    }
                    foreach (GameObject fruit in _fruits)
                    {
                        fruit.transform.localScale = Vector3.one*0.1f;
                    }
                }
         
                else if(time>_growTime/2)
                    foreach (GameObject fruit in _fruits)
                    {
                        fruit.transform.localScale = Vector3.one/2 * (0.05f+ (time /( _growTime / 2)) *0.95f);
                    }
            }
        }

        private void ManageFarmSlot()
        {
            if (IsFarm||IsCattle)
            {
                completeTime = farmStatus == 0 ? plantTime : farmStatus == 1 ? _growTime : harvestTime;
                if (time >= completeTime)
                {
                    time = 0;
                    if (farmStatus ==0)
                    {
                    
                        if (materialsNeeded.Length > 0)
                        {
               
                            foreach (int matID in materialsNeeded)
                            {
                                string tagg = workingFarmer.tag;
                                Cart cartt = tagg == "Farmer" ? workingFarmer. GetComponent<CharactersAI>().Cart : workingFarmer.GetComponent<PlayerControl>().Cart;
                                if (!cartt.Remove(transform.name, matID, _materialPlace, _maxFeed,true))
                                {
                                    time = completeTime;
                                    return;
                                }
                            }
                        }

                        foreach (GameObject product in _products)
                        {
                            product.SetActive(true);
                            if (!IsCattle)
                                if (product.transform.GetChild(0).GetComponent<PlantAnimation>() != null)
                                    product.transform.GetChild(0).GetComponent<PlantAnimation>()._bones.enabled = false;
                        }
                  
                        string tag = workingFarmer.tag;
                        Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<CharactersAI>().Cart : workingFarmer.GetComponent<PlayerControl>().Cart;
                   
                        if (CheckCart(cart)) {
                            FarmStatus();
                            workingFarmer = null;
                        }
                       
                        return;
                    }
                    if (farmStatus == 1)//cay dng lon
                    {
                        foreach (GameObject product in _products)
                        {
                            if (!IsCattle)
                                if (product.transform.GetChild(0).GetComponent<PlantAnimation>()!=null)
                                    product.transform.GetChild(0).GetComponent<PlantAnimation>()._bones.enabled = true;

                        }
                        FarmStatus();
                        return;
                    }
                    if (farmStatus == 2)
                    {
                        StartCoroutine(StackItems());
                        FarmStatus();
                        return;
                    }
                }
            }
            if (IsFactory)
            {
                if (_materialPlace.CurrentQuantity > 0)
                {
                    farmStatus = 1;
                    _animator.SetBool("Work", true);
                }
                else {
                    farmStatus = 0;
                    _animator.SetBool("Work", false);
                }
           
                if(workingFarmer!=null)
                    if (time >= plantTime)
                    {
                        time = 0;
                        foreach (int matID in materialsNeeded)
                        {
                            string tag = workingFarmer.tag;
                            Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<CharactersAI>().Cart : workingFarmer.GetComponent<PlayerControl>().Cart;
                            cart.Remove(transform.name, matID, _materialPlace, _maxFeed);
                        }
                    }
                if (machineTime >= _growTime&&farmStatus==1&&workingFarmer==null)
                {
                    machineTime = 0;
                
                    for(int i=_materialPlace._positions.Length-1; i >=0; i--)
                    {
                        if (_materialPlace._positions[i].transform.childCount > 0)
                        {
                            Destroy(_materialPlace._positions[i].transform.GetChild(0).gameObject);
                            StartCoroutine(StackItems());
                            break;
                        }
                    }
                    _materialPlace.CurrentQuantity--;
                }
            }
        }
        private void FarmStatus()
        {
            farmStatus++; 
            time = 0;
            if (farmStatus > 2)
            {
                farmStatus = 0;
            }
        }
        private IEnumerator StackItems()
        {
            foreach (GameObject go in _products)
            {
                GameObject clone = Instantiate(_goodsPrefabs);

                go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y / 2, go.transform.localScale.z);

                go.SetActive(false);

                clone.transform.position = go.transform.position;
                ParabolicMovement(clone, stack.transform.position, 0.5f, 1.5f, () =>
                    {
                        stack.currentQuantity++;
                        Destroy(clone);
                    }
                );
                yield return new WaitForSeconds(0.05f);
            }
            
            float exp = stack.productToShow.ExpNum * (_gameManager.currentUnlocked < 18 ? 1 : _gameManager.currentUnlocked < 35 ? 0.75f : 0.5f);
            _effectManager.GetExpEffect(exp, GameObject.FindGameObjectWithTag("Player").transform);
        }

        private void ParabolicMovement(GameObject go,Vector3 targetPosition,float duration, float height,TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] =go. transform.position;
            path[1] =( targetPosition + go.transform.position)/2 + new Vector3(0, 2*height, 0);
            path[2] = targetPosition;

            go.transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.red)
                .SetEase(Ease.OutQuad)
                .OnComplete(OnComplete);
        }
        private IEnumerator Stock()
        {
            yield return new WaitForSeconds(0.25f);

            stack.currentQuantity += _quantity;
        }
        private void OnTriggerStay(Collider other)
        {
            if (workingFarmer == null)
            {
                if (other.CompareTag("Farmer") || other.CompareTag("Player"))
                {
                    if (IsFarm && farmStatus == 1) return;
                    workingFarmer = other.gameObject;

                    string tag = workingFarmer.tag;
                    Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<CharactersAI>().Cart : workingFarmer.GetComponent<PlayerControl>().Cart;
              
                    if (!CheckCart(cart))
                    {
                        workingFarmer = null;
                    }
                }
            }
        }
        private bool CheckCart(Cart cart)
        {
            if (materialsNeeded.Length == 0) return true;
            List<GameObject> list = cart.cart;
            foreach (GameObject go in list)
            {
                foreach(int matID in materialsNeeded)
                {
                    if (go.GetComponent<Product>().Goods.Id == matID)
                    {
                        return true;
                    }

                }
            }
            if (farmStatus == 2) return true;
            return false;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == workingFarmer)
            {        
                workingFarmer = null;
           
            }
        }
    }
}
