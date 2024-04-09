using System.Collections;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.UI;
using _Project.Scripts_dev.Сamera;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private FloatingJoystick _joystick;
        [FormerlySerializedAs("speed")] [SerializeField] private float _moveSpeed;
        [FormerlySerializedAs("cart")] [SerializeField] private Cart _cart;
        [FormerlySerializedAs("cashier")] [SerializeField] private Cashier _cahsier;
        [FormerlySerializedAs("money")] [SerializeField] private GameObject _money;
        [FormerlySerializedAs("ZZZ")] [SerializeField] private GameObject _idleParticle;
        [FormerlySerializedAs("Speedy")] [SerializeField] private GameObject _speedParticle;
        [FormerlySerializedAs("tool")] [SerializeField] private GameObject _tool;
        [FormerlySerializedAs("seat")] [SerializeField] private Transform _carSeat;
        [FormerlySerializedAs("car")] [SerializeField] private bool _isCar;
        [FormerlySerializedAs("animator")] [SerializeField] private Animator _animatorController;
        
        [Header("Car")]
        [FormerlySerializedAs("mainPlayer")] [SerializeField] private GameObject _playerObject;
        [FormerlySerializedAs("driver")] [SerializeField] private GameObject _driver;
        [FormerlySerializedAs("carCart")] [SerializeField] private Cart _carCart;
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;
        private readonly float _carRotateSpeed = 500;
        private bool _isPlayMoney;
        private float _idleTime;
        private Vector3 _velocity;
        private float _spawnTime;
        private Transform _transform;
        public bool IsTakingMoney { get; private set; }
        public Cart Cart => _cart;
        private void OnDisable()
        {
            _spawnTime = 0;
        }
        private void Start()
        {
            _transform = transform;
            _previousPosition = _transform.position;
            _previousRotation = _transform.rotation;
        }

        private void Update()
        {
            Vector3 movement = new Vector3(-_joystick.Horizontal, 0, -_joystick.Vertical);
            movement.Normalize();
            float finalSpeed = _moveSpeed * _gameManager.speedBoost;
            if (finalSpeed > 12) finalSpeed = 12;
            _velocity = movement * finalSpeed;
        
            _animatorController.SetFloat("Speed", _velocity.magnitude);
            if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
            {
                if (_isCar)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                    Quaternion newRotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, _carRotateSpeed * Time.deltaTime);
                    _transform.rotation = newRotation;
                }
                else
                {
                    _transform.rotation = Quaternion.LookRotation(_velocity);
                }
            }

            if (_velocity != Vector3.zero)
            {
                _tool.SetActive(false);
                _idleTime = 0;
                _idleParticle.SetActive(false);
            }
            _idleTime += Time.deltaTime;
            if (_idleTime > 15) _idleParticle.SetActive(true);
            if (_gameManager.speedBoostTime > 0) _speedParticle.SetActive(true);
            else _speedParticle.SetActive(false);


            ActivateGravity();
            _characterController.Move(_velocity * Time.deltaTime);
            if(_isCar&& _gameManager.truckTime <= 0)
            {
                Remove();
            }
            _spawnTime += Time.deltaTime;
        }
        private void Remove()
        {
            _gameManager.truckTime = -100;
            _playerObject.transform.position = _transform.position;
            _transform.tag = "Untagged";
            _transform.SetPositionAndRotation(_previousPosition, _previousRotation);
            _playerObject.SetActive(true);
            GetComponent<PlayerControl>().enabled = false;
            _camFollow.player = _playerObject;
            _playerObject.GetComponent<PlayerControl>().enabled = true;
            _driver.SetActive(false);
            TapKetHang tapKetHang = FindObjectOfType<TapKetHang>();
            tapKetHang.Stack(_cart);
        }
        private void ActivateGravity()
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

        private void Collect(Stack stack)
        {
            if (_isCar && _gameManager.truckTime <= 1) return;
            _cart.Add(stack,_gameManager. maxCart);
        }
        private void TakeMoney(Money piile)
        {
            if (!IsTakingMoney && piile.Quantity > 0&&!_cahsier.isTaking)
            {
                StartCoroutine(TakeRoutine(piile));
            }
        }
        private void Pay(Unlock unlock)
        {
            if (!_isPlayMoney && unlock.remain > 0)
            {
                StartCoroutine(PayRoutine(unlock));
            }
        }
        private IEnumerator PayRoutine(Unlock unlock)
        {
            _isPlayMoney = true;
            if (unlock.remain > 0 && _gameManager.money>0)
            {
                if (unlock.remain >= _gameManager.moneyPerPack && _gameManager.money> _gameManager.moneyPerPack)
                {
                    GameObject clone = Instantiate(_money, _transform.position, Quaternion.identity);
                    unlock.remain -= _gameManager.moneyPerPack;
                    _gameManager.money -= _gameManager.moneyPerPack;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }

                else if(_gameManager.money > unlock.remain)
                {
                    GameObject clone = Instantiate(_money, _transform.position, Quaternion.identity);
                    _gameManager.money -= unlock.remain;
                    unlock.remain = 0;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }

                else 
                {
                    GameObject clone = Instantiate(_money, _transform.position, Quaternion.identity);
                
                    unlock.remain -= _gameManager.money;
                    _gameManager.money =0;
                    ParabolicMovement(clone, unlock.transform.position, 0.3f, 1f, () => { Destroy(clone); });
                }
                int r = Random.Range(0, 2);
                if (r == 0&&_gameManager.money>0) 
                    _soundManager.CreateSound(_soundManager.sounds[5],_transform.position,0.5f);
                yield return new WaitForSeconds(5/unlock.price);
            }
            _isPlayMoney = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.CompareTag("MoneyPile"))
            {
                TakeMoney(other.GetComponent<Money>());
            }
            if (other.transform.CompareTag("Unlock"))
            {
          
                Pay(other.GetComponent<Unlock>());

            }
            if (other.transform.CompareTag("Farm")|| other.transform.CompareTag("Farm2")|| other.transform.CompareTag("Farm3"))
            {
                if (other.GetComponent<FarmSlot>().workingFarmer == gameObject&&!Input.anyKey)
                {
                    _tool.SetActive(true);
                    _animatorController.SetBool("Work", true);
                    _idleTime = 0;
                }
                else
                {
                    _tool.SetActive(false);
                
                    _animatorController.SetBool("Work", false);
               
                }
            }
            if (other.transform.CompareTag("Stack")||other.transform.CompareTag("Stack2"))
            {
                Debug.Log("Grabbb");
                Collect(other.GetComponent<Stack>());
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("MoneyPile"))
            {
                other.GetComponent<Money>().CalculatePositions();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("MoneyPile"))
            {
                other.GetComponent<Money>().CalculatePositions();
            }

            if (other.transform.CompareTag("Unlock"))
            {
                _uiManager.shouldShowUnlockReward = true;
            }

            if (other.name == "PlayerWithCar" && other.transform != _transform && _spawnTime > 3 &&
                _gameManager.level >= 3)
            {
                _uiManager.CarUI.SetActive(true);
            }
        }

        public void SitInCar(Collider other)
        {
            GetComponent<PlayerControl>().enabled = false;

            ParabolicMovement(gameObject, _carSeat.position, 0.5f, 1f, () =>
            {
                other.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                other.tag = "Player";
                other.GetComponent<PlayerControl>().enabled = true;
                other.GetComponent<CharacterController>().enabled = true;
                _gameManager.truckTime = 180;
                _soundManager.CreateSound(_soundManager.sounds[11], transform.position);
                for (int i = 0; i < _cart.cart.Count; i++)
                {
                    _carCart.cart.Add(_cart.cart[i]);
                    _cart.cart[i].transform.parent = _carCart.CartPos[i].transform;
                    _cart.cart[i].transform.rotation = _carCart.transform.rotation;
                    _cart.cart[i].transform.localPosition = Vector3.zero;
                }
                _cart.Clear();
                gameObject.SetActive(false);
            });
        }

        private IEnumerator TakeRoutine(Money moneyPiile)
        {
            IsTakingMoney = true;
        
      
            if (moneyPiile.Quantity > moneyPiile._positions.Length * _gameManager.moneyPerPack)
            {
                _gameManager.money += (moneyPiile.Quantity - moneyPiile._positions.Length * _gameManager.moneyPerPack);
                moneyPiile.Quantity -= (moneyPiile.Quantity - moneyPiile._positions.Length * _gameManager.moneyPerPack);
           
            }
            if (moneyPiile.Quantity>0)
            {
                GameObject clone = Instantiate(moneyPiile._moneyPrefab, moneyPiile.PreviousPosition, Quaternion.identity);
                moneyPiile.UpdatePosition(false);

                if (moneyPiile.Quantity >= _gameManager.moneyPerPack)
                {

                    moneyPiile.Quantity -= _gameManager.moneyPerPack;
                    _gameManager.money += _gameManager.moneyPerPack;
                }
              
                else
                {
                    _gameManager.money += moneyPiile.Quantity;
                    moneyPiile.Quantity =0;
                }
           
                Debug.Log("Next: " + moneyPiile.NewIndex);
                ParabolicMovement(clone, _transform.position, 0.2f, 1f, () => { Destroy(clone);  });
                int r = Random.Range(0, 4);
                if (r == 0)
                    _soundManager.CreateSound(_soundManager.sounds[5], _transform.position, 0.5f);
                yield return null;
            }
        
            IsTakingMoney = false;
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
