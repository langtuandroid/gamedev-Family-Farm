using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts_dev.Classes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
using Stack = _Project.Scripts_dev.Classes.Stack;

namespace _Project.Scripts_dev.AI
{
    public class CharactersAI : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        private GameObject destination;
        [FormerlySerializedAs("famer")] [SerializeField] private bool _isFarmer;
        [FormerlySerializedAs("customer")] [SerializeField] private bool _isCustomer;
        [FormerlySerializedAs("shipper")] [SerializeField] private bool _isShiper;
        [FormerlySerializedAs("cart")] [SerializeField] private Cart _cart;
        [FormerlySerializedAs("tagFarm")] [SerializeField] private string _farmTag;
        [FormerlySerializedAs("waitingSpot")] [SerializeField] private GameObject _waitingPlace;
        [FormerlySerializedAs("bongBong")] [SerializeField] private GameObject _wishCloud;
        [FormerlySerializedAs("tool")] [SerializeField] private GameObject _tool;
        [FormerlySerializedAs("wishes")] [SerializeField] private Sprite[] _wishList;
        [FormerlySerializedAs("customerWishPop")] [SerializeField] private Image _wishImage;
        [FormerlySerializedAs("animator")] [SerializeField] private Animator _animator;
        private List<int> _mats = new();
        private List<Stack> _stacks = new();
        private bool _isWorking;
        private Cashier _cashier;
        private NavMeshAgent _agent; 
        private int _needAmount;
        private bool _isToCashier;
        private int _productsAmount;
        private List<FarmSlot> _workingFarms = new();
        private List<FarmSlot> _harvestFarms = new();
        private Vector3 _targetPosition;
        private GameObject _target;
        private List<FarmSlot> _farmSlots;
       
        private List<GameObject> _shelvesList;
        private bool _isTakingMaterial;
        private bool _isHarvesting;
        private bool _isShopChoose;
        private bool _isShipping;
        private bool _isWaiting;
        private int _currentMaterialNeeded;
        private FarmSlot _currentFarmWorkingOn;
        private FarmSlot _currentHarvestingFarm;
        private float _delay;
        private float _stayTime;
        private List<GameObject> _shopsSkipped;
        private bool _isChoosing;
        private bool _isPreparing;
        public bool IsMovingHome { get; private set; }
        public int LineNumber { get; private set; }
        public Cart Cart => _cart;
        public Cashier Cashier => _cashier;
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _shopsSkipped = new List<GameObject>();
            _workingFarms = new List<FarmSlot>();
            _harvestFarms = new List<FarmSlot>();
            _mats = new List<int>();
            _farmSlots = new List<FarmSlot>();
            _stacks = new List<Stack>();
            _shelvesList = new List<GameObject>();
      
        }

        private void Start()
        {
            _target = GameObject.FindGameObjectWithTag("Center");
            _targetPosition = _target.transform.position;
            GameObject[] cashiers = GameObject.FindGameObjectsWithTag("Cashier");
            _cashier = GameObject.FindGameObjectsWithTag("Cashier")[Random.Range(0,cashiers.Length)].GetComponent<Cashier>();
            foreach (GameObject c in cashiers)
            {
                if (c.GetComponent<Cashier>().inLine < _cashier.inLine) {
                    _cashier = c.GetComponent<Cashier>();
                }
            }
            
            if (_isFarmer)

                InvokeRepeating("FindEmptyFarms", -1, 1f);
      
            if(_isShiper)
                InvokeRepeating("GetAllShelvesAndStacks", -1, 2);
        }
        private void Update()
        {
            if (transform.GetChild(0).gameObject.activeInHierarchy)
            {
                _agent.enabled = true;
            }
            else
            {
                _agent.enabled = false;
                return;
            }
            if(destination!=null&&!_isWaiting)
                _agent.SetDestination(destination.transform.position);
            if (_delay > 1 && _delay < 6)
            {
                transform.GetChild(0).localPosition = Vector3.zero;
 
            }
            if (_delay < 6&&!_isCustomer)
            {
                _delay += Time.deltaTime;
           
                return;
            }
        
        
            if (_isCustomer) {

                if ((_stayTime >= 21||_shopsSkipped.Count>=3)&& !IsMovingHome)
                {
                    GoHome();
                    _wishImage.sprite = _wishList[18];
                }
                if (_isToCashier)
                {
                    if (_gameManager.some1Taking==0&&!_isChoosing)
                    {
                        StartCoroutine(ConfigureLine());
                    }
                }
                if (!_isToCashier)
                {
                    _wishCloud.SetActive(true);
                }
            } 
            if (Cart.cart.Count == 0&&_isWaiting) _stayTime += Time.deltaTime;
       
            if (!transform.GetChild(0).gameObject.activeInHierarchy) return;
            if (_isCustomer&&!_isShopChoose)
                ChooseShop();
            if (CheckCompletePath()&&destination!=null&&!_isFarmer)
            {
                RotateObject(destination);
            }
            _animator.SetFloat("Speed", _agent.velocity.magnitude);

            if (_isFarmer)
            {
                WorkOnFarm();
                CollectPlantsFarm();
                if (_agent.velocity != Vector3.zero)
                {
                    _tool.SetActive(false);
                }
            }
            if (_isShiper)
            {
                GoToStacks();
                DeliverProduct();
                if (_stacks.Count == 0 && Cart.cart.Count == 0) {
                    destination = _waitingPlace;
                    _isTakingMaterial = false;
                } 
            }

     
        }

        private IEnumerator ConfigureLine()
        {
            _isChoosing = true;
            _gameManager.some1Taking++;
            yield return new WaitForSeconds(0.05f);
            destination = _cashier.linePos[LineNumber];
            _wishCloud.SetActive(false);
            _gameManager.some1Taking--;
            _isChoosing = true;
        }

        private void FindEmptyFarms()
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag(_farmTag);
            _farmSlots.Clear();
            _stacks.Clear();
            foreach (GameObject farm in go)
            {
                _farmSlots.Add(farm.GetComponent<FarmSlot>());
            }
      
            go = GameObject.FindGameObjectsWithTag("Stack");
            foreach (GameObject stack in go)
            {
                _stacks.Add(stack.GetComponent<Stack>());
            }
            _harvestFarms.Clear();
            _workingFarms.Clear();
            _mats.Clear();
            foreach (FarmSlot farm in _farmSlots)
            {
                if (farm.farmStatus == 0 && farm.workingFarmer == null)
                {
                    bool shouldAdd = true;
                    if (farm.materialsNeeded.Length > 0)
                    {
                        Stack stackneed = null;
                        foreach (Stack staack in _stacks)
                        {
                            foreach (int mat in farm.materialsNeeded)
                            {
                                if (mat == staack.productToShow.Id)
                                {
                                    stackneed = staack;
                                }
                            }
                        }
                        if (stackneed == null) shouldAdd = false;
                        else if (stackneed.currentQuantity == 0) shouldAdd = false;
                    }
                    if (shouldAdd)
                        _workingFarms.Add(farm);
                }
                if (farm.farmStatus == 2 && farm.workingFarmer == null)
                {
                    _harvestFarms.Add(farm);
                }
            }
   
            foreach (FarmSlot farm in _farmSlots)
            {
                if (farm.farmStatus == 2 && farm.workingFarmer == null)
                {
                    _harvestFarms.Add(farm);

                }
            }
            foreach (FarmSlot farm in _workingFarms)
            {
                if (farm.materialsNeeded.Length > 0)
                {
                    foreach (int item in farm.materialsNeeded)
                    {
                        bool shouldAdd = true;
                        foreach (GameObject cartItem in Cart.cart)
                        {
                            if (cartItem.GetComponent<Product>().Goods.Id == item)
                                shouldAdd = false;
                        }

                        Stack stackneed = null;
                        foreach (Stack stack in _stacks)
                        {
                            foreach (int mat in farm.materialsNeeded)
                            {
                                if (mat == stack.productToShow.Id)
                                {
                                    stackneed = stack;
                                }
                            }
                        }
                        if (stackneed.currentQuantity == 0) shouldAdd = false;

                        if (farm.isFarm && shouldAdd) _mats.Add(item);
                        else if (farm.isFactory && shouldAdd)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                _mats.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator PlantFarm()
        {
            if (!_currentFarmWorkingOn.isCattle)
            {
                while (_currentFarmWorkingOn.farmStatus == 0)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
       
            else
            {
                yield return new WaitForSeconds(_currentFarmWorkingOn.plantTime+0.5f);
            }
            _isWorking = false;
            _isTakingMaterial = false;
            _isHarvesting = false;
        }

        private IEnumerator CollectPlants()
        {
            yield return new WaitForSeconds(_currentHarvestingFarm.harvestTime+0.5f);
            if (_currentHarvestingFarm.materialsNeeded.Length == 0)
                while (_currentHarvestingFarm.farmStatus == 0)
                {
                    yield return new WaitForSeconds(0.2f);
                }

            _isWorking = false;
            _isTakingMaterial = false;
            _isHarvesting = false;
        }

        private void GetMaterials()
        {
            if (_mats.Count > 0&&!_isTakingMaterial&&!_isWorking&&!_isHarvesting)
            {
                foreach(Stack stack in _stacks)
                {
               
                    if (stack.productToShow.Id == _mats[0]&&stack.currentQuantity>0)
                    {
                        _isTakingMaterial = true;
                        destination = stack.gameObject;
                    
                        _currentMaterialNeeded = _mats[0];
                    
                    }
                }
            }
        }

        private void WorkOnFarm()
        {
            if (_isWorking) return; 
            if (_workingFarms.Count>0&&!_isHarvesting&&!_isTakingMaterial)
            {
                int rnd = Random.Range(0, _workingFarms.Count);
                FarmSlot farmSlot= _workingFarms[rnd];
                if (farmSlot.workingFarmer != null) return;
          
                if (Cart.cart.Count > 0)
                {
                    foreach (FarmSlot farm in _farmSlots)
                    {
                        if (farm.materialsNeeded.Length > 0 && farm.farmStatus == 0 && farm.workingFarmer == null)
                        {

                            if (farm.materialsNeeded[0]/*tam*/ == Cart.cart[0].GetComponent<Product>().Goods.Id)
                            {
                                farmSlot = farm;
                                destination = farmSlot.AIWorkingSpot;

                                _isWorking = true;
                                _currentFarmWorkingOn = farmSlot;
                                return;
                            }

                        }

                    }
                    Cart.Clear();
                }
              
                if (farmSlot.materialsNeeded.Length > 0 && farmSlot.farmStatus == 0)
                {
                    if (Cart.cart.Count == 0)
                    {
                        GetMaterials();

                        return;
                    }
              
                }

                destination = farmSlot.AIWorkingSpot;
                _isWorking = true;
                _currentFarmWorkingOn = farmSlot;


            }
        }

        private void CollectPlantsFarm()
        {
            if (!_isHarvesting&&_harvestFarms.Count>0 && _workingFarms.Count == 0 && !_isWorking && !_isTakingMaterial&& Cart.inCart==0)
            {
                int rnd = Random.Range(0, _harvestFarms.Count);
                destination = _harvestFarms[rnd].AIWorkingSpot;
                _currentHarvestingFarm = _harvestFarms[rnd];
                _isHarvesting = true;
            }
        }

        private void RotateObject(GameObject targetObject)
        {
            float angle = Quaternion.Angle(transform.rotation, targetObject.transform.rotation);
            float maxRotation = 500 * Time.deltaTime;
            float actualRotation = Mathf.Min(maxRotation, angle);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetObject.transform.rotation, actualRotation);
            transform.rotation = newRotation;
        }
        public bool CheckCompletePath()
        {
            if (!(_agent.remainingDistance <= _agent.stoppingDistance) || _agent.pathPending) return false;
            return !_agent.hasPath || _agent.velocity.sqrMagnitude == 0f;
        }
        private GameObject ChooseShop(bool reChoose=false)
        {
            _gameManager.UpdateCurrentShops();
            if (_gameManager.shops.Count == 0||IsMovingHome) return null;
       
            GameObject shop = _gameManager.shops[Random.Range(0, _gameManager.shops.Count)];
            if(reChoose==false)
                while (_shopsSkipped.Contains(shop) && _gameManager.shops.Count != _shopsSkipped.Count)
                {
                    shop = _gameManager.shops[Random.Range(0, _gameManager.shops.Count)];

                }
            else
            {
                shop = null;
                foreach(GameObject go in _gameManager.shops)
                {
                    if (go.GetComponent<AreaInfo>().shelf.Quantity > 0)
                    {
                        shop = go;
                    }
                }
                if (shop == null) {
                    Debug.Log("Go home");
                    return null;
                } 
            }
            if (_gameManager.shops.Count == _shopsSkipped.Count) return null;
            _shopsSkipped.Add(shop);
            destination =shop;
            if (_shopsSkipped.Count <3 )
                StartCoroutine(WaitTillShopOpen(shop.GetComponent<AreaInfo>().shelf));
            _productsAmount = destination.GetComponent<AreaInfo>().shelf._productsRequierment;
       
            _wishImage.sprite = _wishList[shop.GetComponent<AreaInfo>().shelf._productsRequierment - 1];
            _isShopChoose = true;
            return shop;
        }
        public void GoToOtherLine()
        {
            if (IsMovingHome||!_isToCashier) return;
            LineNumber--;
            destination = _cashier.linePos[LineNumber];
        }
    
        public void GoHome()
        {
            IsMovingHome = true;
            if (_stayTime < 20) _wishImage.sprite = _wishList[19];
            _wishCloud.SetActive(true);
            _isToCashier = false;
            destination = GameObject.FindGameObjectsWithTag("SweetHome")[Random.Range(0, GameObject.FindGameObjectsWithTag("SweetHome").Length)];
            gameObject.tag = "Untagged";
        }
        private void GrabProduct(Shelf shelf)
        {
            if (Cart.inCart < _needAmount&&!_isWaiting)
            {
                Cart.AiAdd(shelf, _needAmount);
                if (destination.CompareTag("Shelf") && Cart.inCart == 0&&CheckCompletePath())
                {
                    _stayTime = 0;
               
                    GameObject newShop = ChooseShop(true);
                    Debug.Log(newShop);
               
                    if (newShop == null||_shopsSkipped.Count==3) { _stayTime = 21;  StopAllCoroutines();  }
               
                }
            }
            
            else if(!_isToCashier && !_isWaiting&&!IsMovingHome)
            {
           
                _wishImage.sprite = _wishList[20];
                _isToCashier = true;
                destination = _cashier.linePos[_cashier.inLine];
            
                LineNumber = _cashier.inLine;
                _cashier.inLine++;
            }
        }

        private IEnumerator WaitTillShopOpen(Shelf shelf,bool first =false)
        {
            _isWaiting = true;
            while (shelf.Quantity == 0&&_stayTime<20&&Cart.inCart==0)
            {

                if (CheckCompletePath() || first)
                {
                    first = false;
                    SetRandomDestination();
                }
             
                yield return new WaitForSeconds(Random.Range(2, 3));
            }
            _isWaiting = false;
            if (_stayTime >= 20 )
            {
                _stayTime = 0;
          
                GameObject newShop = ChooseShop(true);
                Debug.Log(newShop);
            
                if (newShop == null || _shopsSkipped.Count == 3) 
                { 
                    _stayTime = 21;
                    StopAllCoroutines(); 
                }
            }
        }
        private void SetRandomDestination()
        {
            Vector3 randomPoint = GetRandomPointAroundTarget(_targetPosition, 10);
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(randomPoint, out navMeshHit, 10, NavMesh.AllAreas))
            {
           
                _agent.SetDestination(navMeshHit.position);
            }
        }
        private Vector3 GetRandomPointAroundTarget(Vector3 targetPosition, float halfSideLength)
        {
            float randomX = 2*Random.Range(-halfSideLength, halfSideLength);
            float randomZ = Random.Range(-halfSideLength, halfSideLength);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);
            return targetPosition + randomPosition;
        }

        private void GetAllShelvesAndStacks()
        {
            GameObject[] go;
       
            _stacks.Clear();
            _shelvesList.Clear();
            go = GameObject.FindGameObjectsWithTag("Stack").Concat(GameObject.FindGameObjectsWithTag("Stack2")).ToArray(); 
            foreach (GameObject stack in go)
            {
                if(stack.GetComponent<Stack>().currentQuantity>0)
                    _stacks.Add(stack.GetComponent<Stack>());
            }
            go = GameObject.FindGameObjectsWithTag("Shelf");
            foreach (GameObject shelf in go)
            {
                _shelvesList.Add(shelf);
            }
            SortList();
        }
        private void GoToStacks()
        {
            if (_isTakingMaterial||_stacks.Count==0||Cart.cart.Count>0) return;
            int r = Random.Range(0, _stacks.Count>3?3:_stacks.Count);
            destination = _stacks[r].gameObject;
            _currentMaterialNeeded = _stacks[r].productToShow.Id;
            _isTakingMaterial = true;
        }
        private void DeliverProduct()
        {
            if (!_isShipping&&Cart.cart.Count>0)
            {
                foreach(GameObject s in _shelvesList)
                {
                    if(s.GetComponent<AreaInfo>().shelf._productsToDisplay.Id == Cart.cart[0].GetComponent<Product>().Goods.Id)
                    {
                        destination = (s);
                        _isShipping = true;
                    }
                }
            }
        }
        private void SortList()
        {
            if(_stacks.Count>0)
                _stacks.Sort((a, b) => a.shelf.Quantity.CompareTo(b.shelf.Quantity));
       
            if (_farmSlots.Count > 0)
                _farmSlots.Sort((a, b) => b.stack.currentQuantity.CompareTo(a.stack.currentQuantity));
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.transform.CompareTag("Shelf"))
            {
                if (_isCustomer)
                {
                    if (other.GetComponent<AreaInfo>().shelf._productsRequierment != _productsAmount) return;
                    int quantity = other.GetComponent<AreaInfo>().shelf.Quantity;
                    _needAmount = Random.Range(quantity / 5, quantity / 3);
                    if (_needAmount < 2) _needAmount =Random.Range(1,3) ;
                    if (_needAmount > Cart.CartPos.Length) _needAmount = Cart.CartPos.Length;
                    GrabProduct(other.GetComponent<AreaInfo>().shelf);
                    if (Cart.cart.Count > 0)
                    {
                        _animator.SetBool("Carry", true);
                        _agent.stoppingDistance = 0f;
                    }
                 

                }
                if(_isShiper&& Cart.cart.Count>0)
                {
                    if (other.GetComponent<AreaInfo>().shelf._productsRequierment==Cart.cart[0].GetComponent<Product>().Goods.Id)
                        _isShipping = false;
                }
            }
            if(other.transform.CompareTag("Stack")|| other.transform.CompareTag("Stack2"))
            {
                if (_isTakingMaterial&& _currentMaterialNeeded== other.GetComponent<Stack>().productToShow.Id)
                {
                    if (_isFarmer)
                    {
                        Cart.Clear();
                        int needNumber = _mats.Count(n => n == other.GetComponent<Stack>().productToShow.Id);
                        Cart.FarmerAdd(other.GetComponent<Stack>(), needNumber);
                        for (int i = _mats.Count - 1; i >= 0; i--)
                        {
                            if (_mats[i] == _currentMaterialNeeded)
                            {
                                _mats.RemoveAt(i);
                            }
                        }
                        _isTakingMaterial = false;
                        _isWorking = false;

                    }

                    if (_isShiper )
                    {
                        Cart.FarmerAdd(other.GetComponent<Stack>(), 10);
                        _isTakingMaterial = false;
                        _isShipping = false;
                    }
                }
                if (_isShiper)
                {
                    if(CheckCompletePath()&&_currentMaterialNeeded != other.GetComponent<Stack>().productToShow.Id)
                    {
                        _isTakingMaterial = false;
                        _isShipping = false;
                    }
                }
           
            }
            if(_isFarmer)
                if (other.transform.CompareTag(_farmTag))
                {
                    if (_isWorking && _currentFarmWorkingOn.gameObject == other.gameObject && CheckCompletePath())
                    {
                        _workingFarms.Remove(_currentFarmWorkingOn);

                        StartCoroutine(PlantFarm());
                    }
                    if (_isHarvesting && CheckCompletePath())
                    {
                        StartCoroutine(CollectPlants());
                  
                      
                    
                    }
                    if (other.GetComponent<FarmSlot>().workingFarmer == gameObject && _agent.velocity == Vector3.zero)
                    {
                        _tool.SetActive(true);
                        _animator.SetBool("Work", true);
                    }
                    else
                    {
                        _tool.SetActive(false);

                        _animator.SetBool("Work", false);
                  
                    }
                }
        }
  
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("SweetHome") && _isCustomer)
            {
                Destroy(gameObject);
            }
        }
    }
}
