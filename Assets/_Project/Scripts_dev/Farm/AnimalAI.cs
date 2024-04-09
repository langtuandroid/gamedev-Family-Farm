using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Farm
{
    public class AnimalAI : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [Inject] private UIManager _uiManager;
        
        [FormerlySerializedAs("movementRadius")] [SerializeField] private float _moveRadius = 5f;
        [FormerlySerializedAs("target")] [SerializeField] private GameObject _target;
        [FormerlySerializedAs("animator")] [SerializeField] private Animator _animator;
        private FarmSlot _farmSlot;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _targetPosition;
        private float _waitTime;
        private bool _isFed;
        private GameObject _worker;
        private AudioSource _audioSource;
        private float _mooTime;
        private int _radius;
        private readonly float _speed = 3;
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = _speed;
            _targetPosition = _target.transform.position;
            _audioSource = GetComponent<AudioSource>();
            _farmSlot = GetComponent<FarmSlot>();
        }
        
        private void Update()
        {
            if (_uiManager.sound)
            {
                _mooTime += Time.deltaTime;

                if (_mooTime > _radius)
                {
                    _radius = Random.Range(20, 40);

                    if (_radius <= 30&&_gameManager.PlayTime>3)
                    {
                        _audioSource.Play();
                    }

                    _mooTime = 0;
                }
            }
            if (_farmSlot.farmStatus != 0)
            {

                _isFed = false;
                _animator.SetBool("Fed", false);
                _worker = null;
            }
           
            if (_isFed)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.velocity = Vector3.zero;
                _animator.SetFloat("Speed", 0);
                return;
            }
            else _navMeshAgent.isStopped = false;
            if (_navMeshAgent.remainingDistance < 0.5f)
            {
                _waitTime += Time.deltaTime;
            
            }
            if(_waitTime>= Random.Range(3, 5))
            {
                _waitTime = 0;
                MoveToRandomDestination();
            }
            _animator.SetFloat("Speed",_navMeshAgent.velocity.magnitude);
        }
        private void MoveToRandomDestination()
        {
            Vector3 randomPoint = GetPosInRadius(_targetPosition, _moveRadius);
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(randomPoint, out navMeshHit, _moveRadius, NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(navMeshHit.position);
            }
        }
        private Vector3 GetPosInRadius(Vector3 targetPosition, float radius)
        {
            Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;
            Vector3 randomPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
            return targetPosition + randomPosition;
        }
        private void OnTriggerStay(Collider other)
        {
            if (_farmSlot.farmStatus == 1) return; 
            if (other.CompareTag("Player") || other.CompareTag("Farmer"))
            {
                _worker = other.gameObject;
                _isFed = true;
                if(_farmSlot.farmStatus == 0)
                    _animator.SetBool("Fed", true);
                _navMeshAgent.SetDestination(transform.position);
            }
            if (other.CompareTag("Player"))
            {
                _mooTime = 0;
                _audioSource.Play();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject== _worker)
            {
                _isFed = false;
                _animator.SetBool("Fed", false);
                _worker = null;
            }
        }
    }
}
