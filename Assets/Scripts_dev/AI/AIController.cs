using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;


public class AIController : MonoBehaviour
{
    [HideInInspector]
    public GameObject destination;
    [SerializeField] bool famer, customer,shipper;

    public Cart cart;
    [SerializeField] string tagFarm;
    [SerializeField] GameObject waitingSpot;
    [SerializeField] GameObject bongBong;
    [SerializeField] GameObject tool;
    [SerializeField] Sprite[] wishes;
    [SerializeField] Image customerWishPop;
    public Cashier cashier;

    public Animator animator;
    public int lineNum;
    public bool working;

    private NavMeshAgent agent; int need;
    public bool toCashier;
    public bool goHome;
    public LineRenderer line;
    private int producttNeed;
    private List<FarmSlot> currentWorkingFarms;
    private List<FarmSlot> currentHarvestFarms;
    public List<int> numberOfMats;
    private Vector3 targetPosition;
    private GameObject target;

    private List<FarmSlot> farms;
    public List<Stack> stacks;
    private List<GameObject> shelves;
    private bool takingMat,harvesting,shopChose,shipping,waiting;
    private int currentMatNeeded;
    private FarmSlot currentFarmWorkingOn;
    private FarmSlot currentHarvestingFarm;
    private float delaytime;
    public float stayTimer;
    private List<GameObject> shopsPassed;
    private bool choosing,preparing;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shopsPassed = new List<GameObject>();
        currentWorkingFarms = new List<FarmSlot>();
        currentHarvestFarms = new List<FarmSlot>();
        numberOfMats = new List<int>();
        farms = new List<FarmSlot>();
        stacks = new List<Stack>();
        shelves = new List<GameObject>();
      
    }
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Center");
        targetPosition = target.transform.position;
        GameObject[] cashiers = GameObject.FindGameObjectsWithTag("Cashier");
        cashier = GameObject.FindGameObjectsWithTag("Cashier")[Random.Range(0,cashiers.Length)].GetComponent<Cashier>();
        foreach (GameObject c in cashiers)
        {
            if (c.GetComponent<Cashier>().inLine < cashier.inLine) {
                cashier = c.GetComponent<Cashier>();
            }
        }
        
        // destination = cashier.gameObject;
    

        if (famer)

            InvokeRepeating("FindFarmsThatNeedsWorker", -1, 1f);
      
       if(shipper)
            InvokeRepeating("GetAllShelvesAndStacks", -1, 2);
    }
    private void Update()
    {
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            agent.enabled = true;
        }
        else
        {
            agent.enabled = false;
            return;
        }
        if(destination!=null&&!waiting)
            agent.SetDestination(destination.transform.position);
        if (delaytime > 1 && delaytime < 6)
        {
          transform.GetChild(0).localPosition = Vector3.zero;
 
        }
        if (delaytime < 6&&!customer)
        {
            delaytime += Time.deltaTime;
           
            return;
        }
        
        
        if (customer) {

            if ((stayTimer >= 21||shopsPassed.Count>=3)&& !goHome)
            {
                GoHome();
                customerWishPop.sprite = wishes[18];
                goHome = true;
            }
            if (toCashier)
            {//sua cho nay
                if (GameManager.instance.some1Taking==0&&!choosing)
                {
                    StartCoroutine(ChooseLine());
                }
            }
            if (!toCashier)
            {
                bongBong.SetActive(true);
            }
        } 
        if (cart.cart.Count == 0&&waiting) stayTimer += Time.deltaTime;
       
        if (!transform.GetChild(0).gameObject.activeInHierarchy) return;
        if (customer&&!shopChose)
            ChooseShop();
        if (CheckCompletePath()&&destination!=null&&!famer)
        {
            RotateObject(destination);
        }
        animator.SetFloat("Speed", agent.velocity.magnitude);
        /*  if (currentWorkingFarms.Count == 0 && currentHarvestFarms.Count == 0)
          {
              FindFarmsThatNeedsWorker();

          }*/

        if (famer)
        {
           // GetMats();
            WorkFarm();
            HarvestFarm();
            if (agent.velocity != Vector3.zero)
            {
                tool.SetActive(false);
            }
        }
        if (shipper)
        {
            GoToStacks();
            Ship();
            if (stacks.Count == 0 && cart.cart.Count == 0) {
                destination = waitingSpot;
                takingMat = false;
            } 
        }

     
    }
    IEnumerator ChooseLine()
    {
        choosing = true;
        GameManager.instance.some1Taking++;
        yield return new WaitForSeconds(0.05f);
        destination = cashier.linePos[lineNum];
        bongBong.SetActive(false);
        GameManager.instance.some1Taking--;
        choosing = true;
    }
    void FindFarmsThatNeedsWorker()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag(tagFarm);
        farms.Clear();
        stacks.Clear();
        foreach (GameObject farm in go)
        {
            farms.Add(farm.GetComponent<FarmSlot>());
        }
      
        go = GameObject.FindGameObjectsWithTag("Stack");
        foreach (GameObject stack in go)
        {
            stacks.Add(stack.GetComponent<Stack>());
        }
        currentHarvestFarms.Clear();
        currentWorkingFarms.Clear();
        numberOfMats.Clear();
        foreach (FarmSlot farm in farms)
        {
            if (farm.farmStatus == 0 && farm.workingFarmer == null)
            {
                bool shouldAdd = true;
                if (farm.materialsNeeded.Length > 0)
                {
                    Stack stackneed = null;
                    foreach (Stack staack in stacks)
                    {
                        foreach (int mat in farm.materialsNeeded)
                        {
                            if (mat == staack.productToShow.id)
                            {
                                stackneed = staack;
                            }
                        }
                    }
                    if (stackneed == null) shouldAdd = false;
                    else if (stackneed.currentQuantity == 0) shouldAdd = false;
                }
                if (shouldAdd)
                    currentWorkingFarms.Add(farm);
                /* if (farm.isFarm)
                 {
                     if (farm.materialsNeeded.Length == 0)
                     {
                         agent.SetDestination(farm.AIWorkingSpot.transform.position);
                     }
                 }*/
            }
            if (farm.farmStatus == 2 && farm.workingFarmer == null)
            {
                currentHarvestFarms.Add(farm);
            }
        }
        /*    foreach(FarmSlot farm in currentWorkingFarms)
            {
                Debug.Log(farm.transform.parent.name);

            }*/
        foreach (FarmSlot farm in farms)
        {
            if (farm.farmStatus == 2 && farm.workingFarmer == null)
            {
                currentHarvestFarms.Add(farm);

            }
        }
        foreach (FarmSlot farm in currentWorkingFarms)
        {
            if (farm.materialsNeeded.Length > 0)
            {
                foreach (int item in farm.materialsNeeded)
                {
                    bool shouldAdd = true;//bo khoi queue neu stack=0
                    foreach (GameObject cartItem in cart.cart)//co trong cart thi thoi
                    {
                        if (cartItem.GetComponent<Farm.Product>().goods.id == item)
                            shouldAdd = false;
                    }

                    Stack stackneed = null;
                    foreach (Stack stack in stacks)
                    {
                        foreach (int mat in farm.materialsNeeded)
                        {
                            if (mat == stack.productToShow.id)
                            {
                                stackneed = stack;
                            }
                        }
                    }
                    if (stackneed.currentQuantity == 0) shouldAdd = false;

                    if (farm.isFarm && shouldAdd) numberOfMats.Add(item);
                    else if (farm.isFactory && shouldAdd)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            numberOfMats.Add(item);
                        }
                    }
                }
            }
        }
    }
    IEnumerator GrowFarm()
    {
        if (!currentFarmWorkingOn.isCattle)
        {
            while (currentFarmWorkingOn.farmStatus == 0)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
       
        else
        {
            yield return new WaitForSeconds(currentFarmWorkingOn.plantTime+0.5f);
        }
        working = false;
        takingMat = false;
        harvesting = false;
     //   currentFarmWorkingOn = null;

    }
    //Farmer
    IEnumerator Harvest()
    {
        yield return new WaitForSeconds(currentHarvestingFarm.harvestTime+0.5f);
        if (currentHarvestingFarm.materialsNeeded.Length == 0)
            while (currentHarvestingFarm.farmStatus == 0)
            {
                yield return new WaitForSeconds(0.2f);
            }

        working = false;
        takingMat = false;
        harvesting = false;
    }
    bool GetMats()
    {
        
        if (numberOfMats.Count > 0&&!takingMat&&!working&&!harvesting)
        {
            foreach(Stack stack in stacks)
            {
               
                if (stack.productToShow.id == numberOfMats[0]&&stack.currentQuantity>0)
                {
                    takingMat = true;
                    destination = stack.gameObject;
                    
                    currentMatNeeded = numberOfMats[0];
                    
                }
            }
            return true;
        }
        return false;
    }
    void WorkFarm()
    {
        if (working) return; 
        if (currentWorkingFarms.Count>0&&!harvesting&&!takingMat)
        {
            int rnd = Random.Range(0, currentWorkingFarms.Count);
            FarmSlot farmSlot= currentWorkingFarms[rnd];
            if (farmSlot.workingFarmer != null) return;
          
            if (cart.cart.Count > 0)
            {
                foreach (FarmSlot farm in farms)
                {
                    if (farm.materialsNeeded.Length > 0 && farm.farmStatus == 0 && farm.workingFarmer == null)
                    {

                        if (farm.materialsNeeded[0]/*tam*/ == cart.cart[0].GetComponent<Farm.Product>().goods.id)
                        {
                            farmSlot = farm;
                            destination = farmSlot.AIWorkingSpot;
                            //agent.SetDestination(farmSlot.AIWorkingSpot.transform.position);

                            working = true;
                            currentFarmWorkingOn = farmSlot;
                            return;
                        }

                    }

                }
                cart.Clear();
            }
              
            if (farmSlot.materialsNeeded.Length > 0 && farmSlot.farmStatus == 0)
            {
                if (cart.cart.Count == 0)
                {
                    GetMats();

                    return;
                }
              
            }

            destination = farmSlot.AIWorkingSpot;
            //agent.SetDestination(farmSlot.AIWorkingSpot.transform.position);
            working = true;
            currentFarmWorkingOn = farmSlot;


        }
    }
    void HarvestFarm()
    {

        if (!harvesting&&currentHarvestFarms.Count>0 && currentWorkingFarms.Count == 0 && !working && !takingMat&& cart.inCart==0)
        {
            int rnd = Random.Range(0, currentHarvestFarms.Count);
            destination = currentHarvestFarms[rnd].AIWorkingSpot;
            //agent.SetDestination(currentHarvestFarms[rnd].AIWorkingSpot.transform.position);
            currentHarvestingFarm = currentHarvestFarms[rnd];
            harvesting = true;
        }
    }
    void RotateObject(GameObject targetObject)
    {
        float angle = Quaternion.Angle(transform.rotation, targetObject.transform.rotation);

        // Xác định góc xoay tối đa trong một frame
        float maxRotation = 500 * Time.deltaTime;

        // Xác định góc xoay thực tế sử dụng phương thức RotateTowards()
        float actualRotation = Mathf.Min(maxRotation, angle);

        // Lấy hướng xoay để xoay đối tượng
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetObject.transform.rotation, actualRotation);

        // Áp dụng xoay đối tượng
        transform.rotation = newRotation;
    }
    public bool CheckCompletePath()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }
    //customer 
    GameObject ChooseShop(bool reChoose=false)
    {
        GameManager.instance.UpdateCurrentShops();
        if (GameManager.instance.shops.Count == 0||goHome) return null;
       
        GameObject shop = GameManager.instance.shops[Random.Range(0, GameManager.instance.shops.Count)];
        if(reChoose==false)
            while (shopsPassed.Contains(shop) && GameManager.instance.shops.Count != shopsPassed.Count)
            {
                shop = GameManager.instance.shops[Random.Range(0, GameManager.instance.shops.Count)];

            }
        else
        {
            shop = null;
            foreach(GameObject go in GameManager.instance.shops)
            {
                if (go.GetComponent<AreaInfo>().shelf.currentQuantity > 0)
                {
                    shop = go;
                }
            }
            if (shop == null) {
                Debug.Log("Go home");
                return null;
            } 
        }
        if (GameManager.instance.shops.Count == shopsPassed.Count) return null;
        shopsPassed.Add(shop);
        destination =shop;
        if (shopsPassed.Count <3 )
            StartCoroutine(WaitForShop(shop.GetComponent<AreaInfo>().shelf));
        producttNeed = destination.GetComponent<AreaInfo>().shelf.productNeeded;
       
        customerWishPop.sprite = wishes[shop.GetComponent<AreaInfo>().shelf.productNeeded - 1];
        //agent.SetDestination(destination.transform.position);
        shopChose = true;
        return shop;
    }
    public void Reline()
    {
        if (goHome||!toCashier) return;
        lineNum--;
        destination = cashier.linePos[lineNum];
       // agent.SetDestination(destination.transform.position);
    }
    
    public void GoHome()
    {
        Debug.Log("tét");
        if (stayTimer < 20) customerWishPop.sprite = wishes[19];
        bongBong.SetActive(true);
        toCashier = false;
        destination = GameObject.FindGameObjectsWithTag("SweetHome")[Random.Range(0, GameObject.FindGameObjectsWithTag("SweetHome").Length)];
        gameObject.tag = "Untagged";
       // agent.SetDestination(destination.transform.position);
    }
    void CustomerGrab(Shelf shelf)
    {
        if (cart.inCart < need&&!waiting)
        {
            cart.AiAdd(shelf, need);
            if (destination.CompareTag("Shelf") && cart.inCart == 0&&CheckCompletePath())
            {
                stayTimer = 0;
               
                GameObject newShop = ChooseShop(true);
                Debug.Log(newShop);
               
                if (newShop == null||shopsPassed.Count==3) { stayTimer = 21;  StopAllCoroutines();  }
               
            }
              
        }
            
        else if(!toCashier && !waiting&&!goHome)
        {
           
            customerWishPop.sprite = wishes[20];
            toCashier = true;
            destination = cashier.linePos[cashier.inLine];
            
            lineNum = cashier.inLine;
            //agent.SetDestination(destination.transform.position);
            cashier.inLine++;
        }
    }
    IEnumerator WaitForShop(Shelf shelf,bool first =false)
    {
       
        waiting = true;
        while (shelf.currentQuantity == 0&&stayTimer<20&&cart.inCart==0)
        {

            if (CheckCompletePath() || first)
            {
                first = false;
                SetRandomDestination();
            }
             
            yield return new WaitForSeconds(Random.Range(2, 3));
        }
        waiting = false;
        if (stayTimer >= 20 )
        {
            stayTimer = 0;
          
            GameObject newShop = ChooseShop(true);
            Debug.Log(newShop);
            if (newShop != null)
            {

               
            }
            if (newShop == null || shopsPassed.Count == 3) { stayTimer = 21;StopAllCoroutines(); }
        }
        
    }
    void SetRandomDestination()
    {
        Vector3 randomPoint = GetRandomPointAroundTarget(targetPosition, 10);
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, 10, NavMesh.AllAreas))
        {
           
            agent.SetDestination(navMeshHit.position);
        }
    }
    Vector3 GetRandomPointAroundTarget(Vector3 targetPosition, float halfSideLength)
    {
        float randomX = 2*Random.Range(-halfSideLength, halfSideLength);
        float randomZ = Random.Range(-halfSideLength, halfSideLength);
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);
        return targetPosition + randomPosition;
    }
    //Shipper
    void GetAllShelvesAndStacks()
    {
        GameObject[] go;
       
        stacks.Clear();
        shelves.Clear();
        go = GameObject.FindGameObjectsWithTag("Stack").Concat(GameObject.FindGameObjectsWithTag("Stack2")).ToArray(); 
        foreach (GameObject stack in go)
        {
            if(stack.GetComponent<Stack>().currentQuantity>0)
            stacks.Add(stack.GetComponent<Stack>());
        }
        go = GameObject.FindGameObjectsWithTag("Shelf");
        foreach (GameObject shelf in go)
        {
            shelves.Add(shelf);
        }
        SortList();
    }
    void GoToStacks()
    {
        if (takingMat||stacks.Count==0||cart.cart.Count>0) return;
        int r = Random.Range(0, stacks.Count>3?3:stacks.Count);
        destination = stacks[r].gameObject;
        currentMatNeeded = stacks[r].productToShow.id;
       // agent.SetDestination(destination.transform.position);
        takingMat = true;
    }
    void Ship()
    {
        if (!shipping&&cart.cart.Count>0)
        {
            foreach(GameObject s in shelves)
            {
                if(s.GetComponent<AreaInfo>().shelf.productToShow.id == cart.cart[0].GetComponent<Farm.Product>().goods.id)
                {
                    destination = (s);
                   // agent.SetDestination(s.transform.position);
                    shipping = true;
                }
            }
        }
    }
    void SortList()
    {
        if(stacks.Count>0)
            stacks.Sort((a, b) => a.shelf.currentQuantity.CompareTo(b.shelf.currentQuantity));
       
        if (farms.Count > 0)
            farms.Sort((a, b) => b.stack.currentQuantity.CompareTo(a.stack.currentQuantity));
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Shelf"))
        {
            if (customer)
            {
                if (other.GetComponent<AreaInfo>().shelf.productNeeded != producttNeed) return;
                int quantity = other.GetComponent<AreaInfo>().shelf.currentQuantity;
                need = Random.Range(quantity / 5, quantity / 3);
                if (need < 2) need =Random.Range(1,3) ;
                if (need > cart.CartPos.Length) need = cart.CartPos.Length;
                CustomerGrab(other.GetComponent<AreaInfo>().shelf);
                if (cart.cart.Count > 0)
                {
                    animator.SetBool("Carry", true);
                    agent.stoppingDistance = 0f;
                }
                 

            }
            if(shipper&& cart.cart.Count>0)
            {
                if (other.GetComponent<AreaInfo>().shelf.productNeeded==cart.cart[0].GetComponent<Farm.Product>().goods.id)
                    shipping = false;
            }
        }
        if(other.transform.CompareTag("Stack")|| other.transform.CompareTag("Stack2"))
        {
            if (takingMat&& currentMatNeeded== other.GetComponent<Stack>().productToShow.id)
            {
                if (famer)
                {
                    cart.Clear();
                    int needNumber = numberOfMats.Count(n => n == other.GetComponent<Stack>().productToShow.id);
                    cart.FarmerAdd(other.GetComponent<Stack>(), needNumber);
                    for (int i = numberOfMats.Count - 1; i >= 0; i--)
                    {
                        if (numberOfMats[i] == currentMatNeeded)
                        {
                            numberOfMats.RemoveAt(i);
                        }
                    }
                    takingMat = false;
                    working = false;

                }

                if (shipper )
                {
                   // cart.Clear();
                    cart.FarmerAdd(other.GetComponent<Stack>(), 10);
                    takingMat = false;
                    shipping = false;
                }
            }
            if (shipper)
            {
                if(CheckCompletePath()&&currentMatNeeded != other.GetComponent<Stack>().productToShow.id)
                {
                    takingMat = false;
                    shipping = false;
                }
            }
           
        }
        if(famer)
            if (other.transform.CompareTag(tagFarm))
            {
                if (working && currentFarmWorkingOn.gameObject == other.gameObject && CheckCompletePath())
                {
                    currentWorkingFarms.Remove(currentFarmWorkingOn);

                    StartCoroutine(GrowFarm());
                }
                if (harvesting && CheckCompletePath())
                {
                    StartCoroutine(Harvest());
                  
                      
                    
                }
                if (other.GetComponent<FarmSlot>().workingFarmer == gameObject && agent.velocity == Vector3.zero)
                {
                    tool.SetActive(true);
                    animator.SetBool("Work", true);
                }
                else
                {
                    tool.SetActive(false);

                    animator.SetBool("Work", false);
                  
                }
            }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("SweetHome") && customer)
        {
            Destroy(gameObject);
        }
    }
}
