using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] CharacterController _characterController;
    [SerializeField] FloatingJoystick _joystick;
    [SerializeField] float speed;
    public Cart cart;
    [SerializeField] Cashier cashier;
    [SerializeField] GameObject money;
    [SerializeField] GameObject ZZZ;
    [SerializeField] GameObject Speedy;

    [SerializeField] GameObject tool;
    [SerializeField] Transform seat;
  
    [SerializeField] float rotationSpeed;
    [SerializeField] bool car;
    //car
    public GameObject mainPlayer;
    [SerializeField] GameObject driver;
    [SerializeField] Vector3 oldPos;
    [SerializeField] Quaternion oldRotation;
    [SerializeField] Cart carCart;
    //end car
    public Animator animator;
    [HideInInspector]
    public bool takingMoney,payMoney;

    float idleTime;

    public float gravity = 9.8f;
    private Vector3 _velocity;
    float spawnTime;
    private void OnDisable()
    {
        spawnTime = 0;
    }
    private void Start()
    {
        oldPos = transform.position;
        oldRotation = transform.rotation;
    }

    private void Update()
    {
        
        Vector3 movement = new Vector3(-_joystick.Horizontal, 0, -_joystick.Vertical);
        movement.Normalize();
        float finalSpeed = speed * GameManager.instance.speedBoost;
        if (finalSpeed > 12) finalSpeed = 12;
        _velocity = movement * finalSpeed;
        
        animator.SetFloat("Speed", _velocity.magnitude);
        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {

            if (car)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                // Tốc độ quay từ từ (có thể điều chỉnh)

                // Tính toán góc xoay từ hướng hiện tại đến hướng chỉ định
                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Áp dụng góc xoay mới
                transform.rotation = newRotation;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(_velocity);
            }
        }

        if (_velocity != Vector3.zero)
        {
            tool.SetActive(false);
            idleTime = 0;
            ZZZ.SetActive(false);
        }
        idleTime += Time.deltaTime;
        if (idleTime > 15) ZZZ.SetActive(true);
        if (GameManager.instance.speedBoostTime > 0) Speedy.SetActive(true);
        else Speedy.SetActive(false);


        ApplyGravity();
        _characterController.Move(_velocity * Time.deltaTime);
        if(car&& GameManager.instance.truckTime <= 0)
        {
            GetOff();
        }
        spawnTime += Time.deltaTime;
    }
    void GetOff()
    {
        GameManager.instance.truckTime = -100;
        mainPlayer.transform.position = transform.position;
        transform.tag = "Untagged";
        transform.SetPositionAndRotation(oldPos, oldRotation);
        mainPlayer.SetActive(true);
        GetComponent<PlayerControl>().enabled = false;
        CamFollow.instance.player = mainPlayer;
        mainPlayer.GetComponent<PlayerControl>().enabled = true;
        driver.SetActive(false);
        Transform telespot= GameObject.Find("TeleSpot").transform;
       /* mainPlayer.transform.position =new Vector3(telespot.position.x,mainPlayer.transform.position.y,telespot.position.z);*/

        TapKetHang tapKetHang = FindObjectOfType<TapKetHang>();
        tapKetHang.CreateStacks(cart);
    }
    private void ApplyGravity()
    {
        if (!_characterController.isGrounded)
        {
            _velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            _velocity.y = 0f;
        }
    }

    void Grab(Stack stack)
    {
        if (car && GameManager.instance.truckTime <= 1) return;
        cart.Add(stack,GameManager.instance. maxCart);
    }
    void TakeMyMoney(MoneyPiile piile)
    {
        if (!takingMoney && piile.currentQuantity > 0&&!cashier.isTaking)
        {
           
            StartCoroutine(DelayTake(piile));
          
        }
       
    }
    void PayMoney(Unlock unlock)
    {
        if (!payMoney && unlock.remain > 0)
        {
          
            StartCoroutine(DelayPay(unlock));

          
        }
            
    }
    IEnumerator DelayPay(Unlock unlock)
    {
        payMoney = true;


        if (unlock.remain > 0 && GameManager.instance.money>0)
        {
            // GameObject clone = Instantiate(stack.productToShow.prefab, pos.transform.position, pos.transform.rotation);
         /*   UIManager.instance.shouldShowUnlockReward = true;*/
            if (unlock.remain >= GameManager.instance.moneyPerPack && GameManager.instance.money> GameManager.instance.moneyPerPack)
            {
                GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                unlock.remain -= GameManager.instance.moneyPerPack;
                GameManager.instance.money -= GameManager.instance.moneyPerPack;
                ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
            }

            else if(GameManager.instance.money > unlock.remain)
            {
                GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                GameManager.instance.money -= unlock.remain;
                unlock.remain = 0;
                ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
            }

            else 
            {
                GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                
                unlock.remain -= GameManager.instance.money;
                GameManager.instance.money =0;
                ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
            }
            int r = Random.Range(0, 2);
            if (r == 0&&GameManager.instance.money>0) 
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[5],transform.position,0.5f);
            yield return new WaitForSeconds(5/unlock.price);
        }
       
        //moneyPiile.UpdateNextPos(true);
        payMoney = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("MoneyPile"))
        {
            TakeMyMoney(other.GetComponent<MoneyPiile>());
        }
        if (other.transform.CompareTag("Unlock"))
        {
          
            PayMoney(other.GetComponent<Unlock>());

        }
        if (other.transform.CompareTag("Farm")|| other.transform.CompareTag("Farm2")|| other.transform.CompareTag("Farm3"))
        {
            if (other.GetComponent<FarmSlot>().workingFarmer == gameObject&&!Input.anyKey)
            {
                tool.SetActive(true);
                animator.SetBool("Work", true);
                idleTime = 0;
            }
            else
            {
                tool.SetActive(false);
                
                animator.SetBool("Work", false);
               
            }
        }
        if (other.transform.CompareTag("Stack")||other.transform.CompareTag("Stack2"))
        {
            Debug.Log("Grabbb");
            Grab(other.GetComponent<Stack>());
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("MoneyPile"))
        {
            other.GetComponent<MoneyPiile>().ReCalculatePos();
        }
    }  private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("MoneyPile"))
        {
            other.GetComponent<MoneyPiile>().ReCalculatePos();
        }
        if (other.transform.CompareTag("Unlock"))
        {
            UIManager.instance.shouldShowUnlockReward = true;

        }if (other.name== "PlayerWithCar"&& other.transform!= transform&&spawnTime>3&&GameManager.instance.level>=3)
        {
            UIManager.instance.CarUI.SetActive(true);
           

        }
    }
    public void GetCar(Collider other)
    {
        GetComponent<PlayerControl>().enabled = false;

        ParabolicMovement(gameObject, seat.position, 0.5f, 1f, () =>
        {
            other.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            other.tag = "Player";
            other.GetComponent<PlayerControl>().enabled = true;
            other.GetComponent<CharacterController>().enabled = true;
            GameManager.instance.truckTime = 180;
            SoundManager.instance.CreateSound(SoundManager.instance.sounds[11], transform.position);
            for (int i = 0; i < cart.cart.Count; i++)
            {
                carCart.cart.Add(cart.cart[i]);
                //GameObject clone = Instantiate(cart.cart[i], carCart.CartPos[i].transform.position, carCart.transform.rotation);
                /*  clone.transform.Rotate(0, 0, 90);*/
                cart.cart[i].transform.parent = carCart.CartPos[i].transform;
                cart.cart[i].transform.rotation = carCart.transform.rotation;
                cart.cart[i].transform.localPosition = Vector3.zero;
                //  clone.transform.localScale = pos.transform.localScale/5;

            }
            cart.Clear();
            gameObject.SetActive(false);
        });
    }
  /*  IEnumerator GetOnCoroutine(Collider other)
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<PlayerControl>().enabled = false;
        ParabolicMovement(gameObject, seat.position, 0.5f, 0.5f, () =>
        {
            other.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
            other.tag = "Player";
            other.GetComponent<PlayerControl>().enabled = true;
            other.GetComponent<CharacterController>().enabled = true;
            GameManager.instance.truckTime = 60;
            gameObject.SetActive(false);
        });
    }*/

    IEnumerator DelayTake(MoneyPiile moneyPiile)
    {
        takingMoney = true;
        
      
        if (moneyPiile.currentQuantity > moneyPiile.pos.Length * GameManager.instance.moneyPerPack)
        {
            GameManager.instance.money += (moneyPiile.currentQuantity - moneyPiile.pos.Length * GameManager.instance.moneyPerPack);
            moneyPiile.currentQuantity -= (moneyPiile.currentQuantity - moneyPiile.pos.Length * GameManager.instance.moneyPerPack);
           
        }
        if (moneyPiile.currentQuantity>0)
        {
            // GameObject clone = Instantiate(stack.productToShow.prefab, pos.transform.position, pos.transform.rotation);
            GameObject clone = Instantiate(moneyPiile.productToShow, moneyPiile.prevPos, Quaternion.identity);
            moneyPiile.UpdateNextPos(false);

            if (moneyPiile.currentQuantity >= GameManager.instance.moneyPerPack)
            {

                moneyPiile.currentQuantity -= GameManager.instance.moneyPerPack;
                GameManager.instance.money += GameManager.instance.moneyPerPack;
            }
              
            else
            {
                GameManager.instance.money += moneyPiile.currentQuantity;
                moneyPiile.currentQuantity =0;
            }
           
            Debug.Log("Next: " + moneyPiile.nextIndex);
            ParabolicMovement(clone, transform.position, 0.2f, 1f, () => { Destroy(clone);  });
            int r = Random.Range(0, 4);
            if (r == 0)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[5],transform.position, 0.5f);
            yield return null;
        }
       
        //moneyPiile.UpdateNextPos(true);
        takingMoney = false;
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
    
}
