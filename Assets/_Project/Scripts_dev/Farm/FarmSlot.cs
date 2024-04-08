using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Farm;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class FarmSlot : MonoBehaviour
    {
        [Inject] private EffectManager _effectManager;
        [Inject] private UIManager _uiManager;
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        [SerializeField] private GameObject[] products;
        [SerializeField] private GameObject[] fruits;
        [SerializeField] private GameObject need;
        [SerializeField] private GameObject harvstMeIcon;
        [SerializeField] private bool noFruit;
        [SerializeField] private Animator animator;
        [SerializeField] MatPlace matPlace;
        [SerializeField] GameObject goodsPrefabs;
        [SerializeField] int feedMax;
        
        public int quantity;
        public int[] materialsNeeded;
        public int productID;
        public float growTime, plantTime, harvestTime;
        public bool isFarm, isFactory,isCattle;
        public int farmStatus;
        public float time, machineTime;
        public GameObject AIWorkingSpot;
        public Stack stack;
        
        private Image timeImage;
        private float completeTime;
        private bool load;
   
        public GameObject workingFarmer { get; private set; }
        private void Start()
        {
            productID = goodsPrefabs.GetComponent<Product>().goods.id;
            SetupTimerCanvas();
        }
        private void SetupTimerCanvas()
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
                            foreach (GameObject product in products)
                            {
                                product.SetActive(true);
                                product.transform.localScale = Vector3.one;
                       

                            }

                            if (!noFruit)
                            {
                                foreach (GameObject fruit in fruits)
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
            if (timeImage == null) SetupTimerCanvas();
      
            if ((workingFarmer != null) )
            {
                string tagg = workingFarmer.tag;
                Cart cartt = tagg == "Farmer" ? workingFarmer.GetComponent<AIController>().cart : workingFarmer.GetComponent<PlayerControl>().cart;
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

            if (time > 0&&!isFactory)
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
            if (need != null)
            {
                if(farmStatus == 0&&workingFarmer ==null)
                {
                    need.SetActive(true);
                }
                else
                {
                    need.SetActive(false);
                }
            }
            if (harvstMeIcon != null)
            {
                if (farmStatus == 2 && workingFarmer == null)
                {
                    harvstMeIcon.SetActive(true);
                }
                else
                {
                    harvstMeIcon.SetActive(false);
                }
            }

            ManageFarm();
            TreeGrowing();
        }
        void TreeGrowing()
        {
            if (!isFarm || farmStatus != 1) return;
            if (noFruit)
            {
                foreach(GameObject product in products)
                {
                    product.transform.localScale =  Vector3.one * (0.25f + (time / growTime)*0.75f);
                }
            }
            else
            {
                if(time < growTime / 2)
                {
                    foreach (GameObject product in products)
                    {
                        product.transform.localScale = 1.5f* Vector3.one * (0.25f + (time / (growTime / 2)) * 0.75f);
                   

                    }
                    foreach (GameObject fruit in fruits)
                    {
                        fruit.transform.localScale = Vector3.one*0.1f;
                    }
                }
         
                else if(time>growTime/2)
                    foreach (GameObject fruit in fruits)
                    {
                        fruit.transform.localScale = Vector3.one/2 * (0.05f+ (time /( growTime / 2)) *0.95f);
                    }
            }
        }

        private void ManageFarm()
        {
            if (isFarm||isCattle)
            {
                completeTime = farmStatus == 0 ? plantTime : farmStatus == 1 ? growTime : harvestTime;
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
                                Cart cartt = tagg == "Farmer" ? workingFarmer. GetComponent<AIController>().cart : workingFarmer.GetComponent<PlayerControl>().cart;
                                if (!cartt.Remove(transform.name, matID, matPlace, feedMax,true))
                                {
                                    time = completeTime;
                                    return;
                                }
                            }
                        }

                        foreach (GameObject product in products)
                        {
                            product.SetActive(true);
                            if (!isCattle)
                                if (product.transform.GetChild(0).GetComponent<PlantPhysic>() != null)
                                    product.transform.GetChild(0).GetComponent<PlantPhysic>().bonesStimulator.enabled = false;
                        }
                  
                        string tag = workingFarmer.tag;
                        Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<AIController>().cart : workingFarmer.GetComponent<PlayerControl>().cart;
                   
                        if (CheckCart(cart)) {
                            UpdateFarmStautus();
                            workingFarmer = null;
                        }
                       
                        return;
                    }
                    if (farmStatus == 1)//cay dng lon
                    {
                        foreach (GameObject product in products)
                        {
                            if (!isCattle)
                                if (product.transform.GetChild(0).GetComponent<PlantPhysic>()!=null)
                                    product.transform.GetChild(0).GetComponent<PlantPhysic>().bonesStimulator.enabled = true;

                        }
                        UpdateFarmStautus();
                        return;
                    }
                    if (farmStatus == 2)
                    {
                        StartCoroutine(ToStack());
                        UpdateFarmStautus();
                        return;
                    }
                }
            }
            if (isFactory)
            {
                if (matPlace.currentQuantity > 0)
                {
                    farmStatus = 1;
                    animator.SetBool("Work", true);
                }
                else {
                    farmStatus = 0;
                    animator.SetBool("Work", false);
                }
           
                if(workingFarmer!=null)
                    if (time >= plantTime)
                    {
                        time = 0;
                        foreach (int matID in materialsNeeded)
                        {
                            string tag = workingFarmer.tag;
                            Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<AIController>().cart : workingFarmer.GetComponent<PlayerControl>().cart;
                            cart.Remove(transform.name, matID, matPlace, feedMax);
                        }
                    }
                if (machineTime >= growTime&&farmStatus==1&&workingFarmer==null)
                {
                    machineTime = 0;
                
                    for(int i=matPlace.pos.Length-1; i >=0; i--)
                    {
                        if (matPlace.pos[i].transform.childCount > 0)
                        {
                            Destroy(matPlace.pos[i].transform.GetChild(0).gameObject);
                            StartCoroutine(ToStack());
                            break;
                        }
                    }
                    matPlace.currentQuantity--;
                }
            }
        }
        private void UpdateFarmStautus()
        {
            farmStatus++; 
            time = 0;
            if (farmStatus > 2)
            {
                farmStatus = 0;
            }
        }
        private IEnumerator ToStack()
        {
       
            foreach (GameObject go in products)
            {
                GameObject clone = Instantiate(goodsPrefabs);

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
            
            float exp = stack.productToShow.exp * (_gameManager.currentUnlocked < 18 ? 1 : _gameManager.currentUnlocked < 35 ? 0.75f : 0.5f);
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

            stack.currentQuantity += quantity;
        }
        private void OnTriggerStay(Collider other)
        {
            if (workingFarmer == null)
            {
                if (other.CompareTag("Farmer") || other.CompareTag("Player"))
                {
                    if (isFarm && farmStatus == 1) return;
                    workingFarmer = other.gameObject;

                    string tag = workingFarmer.tag;
                    Cart cart = tag == "Farmer" ? workingFarmer.GetComponent<AIController>().cart : workingFarmer.GetComponent<PlayerControl>().cart;
              
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
                    if (go.GetComponent<Product>().goods.id == matID)
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
