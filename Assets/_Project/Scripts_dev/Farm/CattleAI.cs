using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.UI;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace _Project.Scripts_dev.Farm
{
    public class CattleAI : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [Inject] private UIManager _uiManager;
        private AudioSource audioSource;
        public float movementRadius = 5f;
        public float speed;
        public GameObject target;
        private NavMeshAgent navMeshAgent;
        private Vector3 targetPosition;
        private float waitTime;
        private bool beeingFed;
        private GameObject currentWorker;
        public Animator animator;
        public FarmSlot farmslot;
        private float mooTime;
        private int r;
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = speed;
            targetPosition = target.transform.position;
            audioSource = GetComponent<AudioSource>();
            farmslot = GetComponent<FarmSlot>();
        }
        
        private void Update()
        {
            if (_uiManager.sound)
            {
                mooTime += Time.deltaTime;

                if (mooTime > r)
                {
                    r = Random.Range(20, 40);

                    if (r <= 30&&_gameManager.playTime>3)
                    {
                        audioSource.Play();
                    }

                    mooTime = 0;
                }
            }
            if (farmslot.farmStatus != 0)
            {

                beeingFed = false;
                animator.SetBool("Fed", false);
                currentWorker = null;
            }
           
            if (beeingFed)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;
                animator.SetFloat("Speed", 0);
                return;
            }
            else navMeshAgent.isStopped = false;
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                waitTime += Time.deltaTime;
            
            }
            if(waitTime>= Random.Range(3, 5))
            {
                waitTime = 0;
                SetRandomDestination();
            }
            animator.SetFloat("Speed",navMeshAgent.velocity.magnitude);
        }
        private void SetRandomDestination()
        {
            Vector3 randomPoint = GetRandomPointAroundTarget(targetPosition, movementRadius);
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(randomPoint, out navMeshHit, movementRadius, NavMesh.AllAreas))
            {
            
                navMeshAgent.SetDestination(navMeshHit.position);
            }
        }
        private Vector3 GetRandomPointAroundTarget(Vector3 targetPosition, float radius)
        {
            Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;
            Vector3 randomPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
            return targetPosition + randomPosition;
        }
        private void OnTriggerStay(Collider other)
        {
            if (farmslot.farmStatus == 1) return; 
            if (other.CompareTag("Player") || other.CompareTag("Farmer"))
            {
                currentWorker = other.gameObject;
                beeingFed = true;
                if(farmslot.farmStatus == 0)
                    animator.SetBool("Fed", true);
                navMeshAgent.SetDestination(transform.position);
            }
            if (other.CompareTag("Player"))
            {
                mooTime = 0;
                audioSource.Play();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject== currentWorker)
            {
                beeingFed = false;
                animator.SetBool("Fed", false);
                currentWorker = null;
            }
        }
    }
}
