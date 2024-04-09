using System.Collections;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.UI;
using _Project.Scripts_dev.Сamera;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Stack = _Project.Scripts_dev.Classes.Stack;

namespace _Project.Scripts_dev.Control
{
    public class PlayerControl : MonoBehaviour
    {
        private const float Gravity = 9.8f;
        [Inject] private SoundManager _soundManager;
        [Inject] private UIManager _uiManager;
        [Inject] private GameManager _gameManager;
        [Inject] private CamFollow _camFollow;
        
        [Header("Character")]
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
        public Animator animator;
    
        [Header("Car")]
        public GameObject mainPlayer;
        [SerializeField] GameObject driver;
        [SerializeField] Vector3 oldPos;
        [SerializeField] Quaternion oldRotation;
        [SerializeField] Cart carCart;
        private bool payMoney;
        private float idleTime;
        private Vector3 _velocity;
        private float spawnTime;
        public bool takingMoney { get; private set; }
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
            float finalSpeed = speed * _gameManager.speedBoost;
            if (finalSpeed > 12) finalSpeed = 12;
            _velocity = movement * finalSpeed;
        
            animator.SetFloat("Speed", _velocity.magnitude);
            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
            {
                if (car)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                    Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
            if (_gameManager.speedBoostTime > 0) Speedy.SetActive(true);
            else Speedy.SetActive(false);


            ApplyGravity();
            _characterController.Move(_velocity * Time.deltaTime);
            if(car&& _gameManager.truckTime <= 0)
            {
                GetOff();
            }
            spawnTime += Time.deltaTime;
        }
        private void GetOff()
        {
            _gameManager.truckTime = -100;
            mainPlayer.transform.position = transform.position;
            transform.tag = "Untagged";
            transform.SetPositionAndRotation(oldPos, oldRotation);
            mainPlayer.SetActive(true);
            GetComponent<PlayerControl>().enabled = false;
            _camFollow.player = mainPlayer;
            mainPlayer.GetComponent<PlayerControl>().enabled = true;
            driver.SetActive(false);
            TapKetHang tapKetHang = FindObjectOfType<TapKetHang>();
            tapKetHang.CreateStacks(cart);
        }
        private void ApplyGravity()
        {
            if (!_characterController.isGrounded)
            {
                _velocity.y -= Gravity * Time.deltaTime;
            }
            else
            {
                _velocity.y = 0f;
            }
        }

        private void Grab(Stack stack)
        {
            if (car && _gameManager.truckTime <= 1) return;
            cart.Add(stack,_gameManager. maxCart);
        }
        private void TakeMyMoney(MoneyPiile piile)
        {
            if (!takingMoney && piile.currentQuantity > 0&&!cashier.isTaking)
            {
                StartCoroutine(DelayTake(piile));
            }
        }
        private void PayMoney(Unlock unlock)
        {
            if (!payMoney && unlock.remain > 0)
            {
                StartCoroutine(DelayPay(unlock));
            }
        }
        private IEnumerator DelayPay(Unlock unlock)
        {
            payMoney = true;
            if (unlock.remain > 0 && _gameManager.money>0)
            {
                if (unlock.remain >= _gameManager.moneyPerPack && _gameManager.money> _gameManager.moneyPerPack)
                {
                    GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                    unlock.remain -= _gameManager.moneyPerPack;
                    _gameManager.money -= _gameManager.moneyPerPack;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }

                else if(_gameManager.money > unlock.remain)
                {
                    GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                    _gameManager.money -= unlock.remain;
                    unlock.remain = 0;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }

                else 
                {
                    GameObject clone = Instantiate(money, transform.position, Quaternion.identity);
                
                    unlock.remain -= _gameManager.money;
                    _gameManager.money =0;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }
                int r = Random.Range(0, 2);
                if (r == 0&&_gameManager.money>0) 
                    _soundManager.CreateSound(_soundManager.sounds[5],transform.position,0.5f);
                yield return new WaitForSeconds(5/unlock.price);
            }
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("MoneyPile"))
            {
                other.GetComponent<MoneyPiile>().ReCalculatePos();
            }

            if (other.transform.CompareTag("Unlock"))
            {
                _uiManager.shouldShowUnlockReward = true;
            }

            if (other.name == "PlayerWithCar" && other.transform != transform && spawnTime > 3 &&
                _gameManager.level >= 3)
            {
                _uiManager.CarUI.SetActive(true);
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
                _gameManager.truckTime = 180;
                _soundManager.CreateSound(_soundManager.sounds[11], transform.position);
                for (int i = 0; i < cart.cart.Count; i++)
                {
                    carCart.cart.Add(cart.cart[i]);
                    cart.cart[i].transform.parent = carCart.CartPos[i].transform;
                    cart.cart[i].transform.rotation = carCart.transform.rotation;
                    cart.cart[i].transform.localPosition = Vector3.zero;
                }
                cart.Clear();
                gameObject.SetActive(false);
            });
        }

        private IEnumerator DelayTake(MoneyPiile moneyPiile)
        {
            takingMoney = true;
        
      
            if (moneyPiile.currentQuantity > moneyPiile.pos.Length * _gameManager.moneyPerPack)
            {
                _gameManager.money += (moneyPiile.currentQuantity - moneyPiile.pos.Length * _gameManager.moneyPerPack);
                moneyPiile.currentQuantity -= (moneyPiile.currentQuantity - moneyPiile.pos.Length * _gameManager.moneyPerPack);
           
            }
            if (moneyPiile.currentQuantity>0)
            {
                GameObject clone = Instantiate(moneyPiile.productToShow, moneyPiile.prevPos, Quaternion.identity);
                moneyPiile.UpdateNextPos(false);

                if (moneyPiile.currentQuantity >= _gameManager.moneyPerPack)
                {

                    moneyPiile.currentQuantity -= _gameManager.moneyPerPack;
                    _gameManager.money += _gameManager.moneyPerPack;
                }
              
                else
                {
                    _gameManager.money += moneyPiile.currentQuantity;
                    moneyPiile.currentQuantity =0;
                }
           
                Debug.Log("Next: " + moneyPiile.nextIndex);
                ParabolicMovement(clone, transform.position, 0.2f, 1f, () => { Destroy(clone);  });
                int r = Random.Range(0, 4);
                if (r == 0)
                    _soundManager.CreateSound(_soundManager.sounds[5],transform.position, 0.5f);
                yield return null;
            }
        
            takingMoney = false;
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
    
    }
}
