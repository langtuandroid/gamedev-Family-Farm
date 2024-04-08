using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cashier : MonoBehaviour
{
   
    public bool cashierIsHere,playerIsHere;
    public int inLine=0;
    public GameObject[] linePos;
    
    private float money, moneyToTake;
    public AIController currentCustomer;
    [SerializeField] MoneyPiile moneyPile;
    [SerializeField] GameObject moneyPrefab;
    public Animator cashierAnimator;
    //[SerializeField] GameObject bonusIcon; 
    [SerializeField] PlayerControl playerControl;
  
    public bool isTaking;
    AudioSource audioSource;
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
    /*private void Update()
    {

        if (!isTaking && currentCustomer == null && inLine > 0)
        {
            noCustomerTime += Time.deltaTime;
        }
        if (noCustomerTime >= 15)
        {
            noCustomerTime = 0;

           
            if (currentCustomer != null) {
                inLine--;
                currentCustomer.GoHome();
                AIController[] customers = FindObjectsOfType<AIController>();

                foreach (AIController customer in customers)
                {
                    if (customer.transform.CompareTag("Customer"))
                    {
                        customer.Reline();
                    }
                }
            } 
        }
        if (inLine == 0) noCustomerTime = 0;
    }*/
    float Counter(Cart cart)
    {
        float sum=0;
        float multiple = 1;
        LevelMangament[] levelMangaments = FindObjectsOfType<LevelMangament>();
        foreach (GameObject item in cart.cart)
        {

            foreach (LevelMangament levelMangament in levelMangaments)
            {
                if (levelMangament.goods.id == item.GetComponent<Farm.Product>().goods.id)
                    multiple = levelMangament.multiple*levelMangament.level;
                if (multiple == 0) multiple = 1;
            }
            sum += item.GetComponent<Farm.Product>().goods.income * multiple * GameManager.instance.incomeBoost *( GameManager.instance.money>=1000?0.5f:1);
            Debug.Log(sum);
        }
        return sum;
    }
    void TakeMoney()
    {
       // EffectManager.instance.NoInternet();
        //Debug.Log(currentCustomer.name);
        if ((cashierIsHere||playerIsHere) && currentCustomer!=null &&!isTaking &&!playerControl.takingMoney)
        {
            StartCoroutine(TakeMoneyDelay());

            if (UIManager.instance.sound)
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
        for(int i=0; i < Mathf.Ceil(moneyToTake / GameManager.instance.moneyPerPack); i++)
        {
            yield return null; //test
            int r = Random.Range(0, 2);
            if (r == 0)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[5], transform.position, 0.5f);
            GameObject item = Instantiate(moneyPrefab,linePos[0].transform.position,Quaternion.identity);
           // if(temp >= GameManager.instance.moneyPerPack)
            moneyPile.UpdateNextPos(true);
            
         //   else moneyPile.UpdateNextPos(true,false);
            

            ParabolicMovement(item, moneyPile.nextPos, 0.5f, 1.5f,
                () => {
                    Destroy(item);
                    moneyPile.currentQuantity += temp > GameManager.instance.moneyPerPack ? GameManager.instance.moneyPerPack : temp;
                    temp -= 10;
                });
        }
        currentCustomer.goHome = true;
        currentCustomer.GoHome();
        currentCustomer = null;
        AIController[] customers = FindObjectsOfType<AIController>();
        inLine--;

       
        foreach(AIController customer in customers)
        {
            if (customer.transform.CompareTag("Customer"))
            {
                if(customer.cashier.gameObject==gameObject)
                customer.Reline();
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
            AIController controller = other.GetComponent<AIController>();
            if (controller.lineNum == 0 && controller.CheckCompletePath()&&controller.goHome==false)
            {
                moneyToTake = Counter(other.GetComponent<AIController>().cart);
                currentCustomer = controller;
            }
        }
    }
}
