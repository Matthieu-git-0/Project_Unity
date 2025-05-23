/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AWSEntityIdentifier))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animation))]
    public class IA : MonoBehaviour
    {
        [Header("Group")] 
        [Space(10)]
        public WaypointRoute group;
        [HideInInspector] public int groupID = 0;
        
        [Header("Patrol")] [Space(10)] [Tooltip("Minimum amount of time to wait before moving to next patrol point")]
        public float minPatrolWaitTime = 1f;

        [Tooltip("Maximum amount of time to wait before moving to next patrol point")]
        public float maxPatrolWaitTime = 3f;

        [Tooltip("If or not all entities patrol waypoints at random or in sequence")]
        public bool randomPatroler = false;
        
        [Space(10)] [Header("Agent")] [Space(10)] [Tooltip("Speed by which the NavMesh agent moves")]
        public float moveSpeed = 3f;

        [Tooltip("The distance from destination the Navmesh agent stops")]
        public float stoppingDistance = 1f;

        [Tooltip("Turning speed of the NavMesh  agent")]
        public float angularSpeed = 500f;

        [Tooltip("Defines how high up the entity is. This is useful for creating flying entities")]
        public float distanceFromGround = 0.5f;
        
        [SerializeField] Transform target;
        [SerializeField] float triggerRadius = 10f;
        [Space]
    
        [SerializeField] private GameObject image;
        [SerializeField] private GameObject menubackground;
        [SerializeField] private GameObject menuprincipale;
        [SerializeField] private GameObject textgameover;
        [SerializeField] Animator animator;
        NavMeshAgent agent;
        
        [Space(10)] [Header("Debug")] [Space(10)]
        public bool resetPatrol;

        public bool interruptPatrol;
        public static bool reset;
        
        private AWSManager mAWSManager;
        private Animation anim;
        private AudioSource src;
        private Transform[] patrolPoints;
        private bool isWaiting;
        private bool hasPlayedDetectSound;
        private bool hasReachedGoTo;
        private int waypointCount;
        private int destPoint;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
            
        void Awake()
        {
            mAWSManager = GameObject.FindObjectOfType<AWSManager>();

            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animation>();
            src = GetComponent<AudioSource>();

            try
            {
                waypointCount = 0;
                Transform groupTransform = group.transform;
                int childrenCount = groupTransform.childCount;

                for (int i = 0; i < childrenCount; i++)
                {
                    if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                    {
                        waypointCount += 1;
                    }
                }

                patrolPoints = new Transform[waypointCount];
                int curIndex = 0;
                for (int i = 0; i < childrenCount; i++)
                {
                    if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                    {
                        patrolPoints[curIndex] = groupTransform.GetChild(i);
                        if (patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>())
                            patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>().enabled = false;
                        if (patrolPoints[curIndex].gameObject.GetComponent<Collider>())
                            patrolPoints[curIndex].gameObject.GetComponent<Collider>().enabled = false;
                        curIndex++;
                    }
                }
            }
            catch
            {
                Debug.LogWarning("Group not assigned for " + gameObject.name);
            }
        }

        public void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            target = GameObject.FindGameObjectWithTag("Player").transform;

            agent.autoBraking = false;
            agent.stoppingDistance = stoppingDistance;
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;

            try
            {
                GotoNextPoint();
            }
            catch
            {
            }
        }

        void Update()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= triggerRadius)
            {
                agent.SetDestination(target.position);

                if (agent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("IsWalk", true);
                }
                else
                {
                    animator.SetBool("IsWalk", false);
                }

                if (distanceToTarget <= 2f)
                {
                    animator.SetBool("IsWalk", false);
                    image.SetActive(false);
                    menuprincipale.SetActive(true);
                    menubackground.SetActive(true);
                    textgameover.SetActive(true);
                    return;
                }
            }
            else 
            {
                if (resetPatrol || reset)
                {
                    animator.SetBool("IsWalk", true);
                    agent.isStopped = false;
                    goToNextPointDirect();
                    interruptPatrol = false;
                    resetPatrol = false;
                    reset = false;
                }

                if (interruptPatrol)
                {
                    animator.SetBool("IsWalk", false);
                    agent.isStopped = true;
                }

                if (!interruptPatrol && !isWaiting && agent.remainingDistance <= stoppingDistance && null != group)
                {
                    animator.SetBool("IsWalk", true);
                    GotoNextPoint();
                }
                
                animator.SetBool("IsWalk", true);

                agent.stoppingDistance = stoppingDistance;
                agent.speed = this.moveSpeed;
                agent.angularSpeed = angularSpeed;
                agent.baseOffset = distanceFromGround;
            }
        }

        private void GotoNextPoint()
        {
            if (patrolPoints.Length == 0)
                return;
            StartCoroutine(pauseAndContinuePatrol());
        }

        IEnumerator pauseAndContinuePatrol()
        {
            isWaiting = true;

            float waitTime = UnityEngine.Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
            if (waitTime < 0f)
                waitTime = 1f;

            yield return new WaitForSeconds(waitTime);

            if (randomPatroler)
            {
                agent.destination = patrolPoints[destPoint].position;
                int nextPos;
                do
                {
                    nextPos = UnityEngine.Random.Range(0, patrolPoints.Length);
                }
                while (nextPos == destPoint);
                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }
            
            isWaiting = false;
        }

        void goToNextPointDirect()
        {
            if (randomPatroler)
            {
                agent.destination = patrolPoints[destPoint].position;
                int nextPos;
                do
                {
                    nextPos = UnityEngine.Random.Range(0, patrolPoints.Length);
                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }
            
        }

        void RestartPatrol()
        {
            hasPlayedDetectSound = false;
            resetPatrol = false;
            agent.speed = moveSpeed;

            agent.stoppingDistance = 1f;
            goToNextPointDirect();
        }

        public void ResetPatrol()
        {
            resetPatrol = true;
        }

        public void InterruptPatrol()
        {
            interruptPatrol = true;
        }

        public void SetDeatination(Transform t)
        {
            agent.destination = t.position;
            isWaiting = false;
        }
    }
}*/