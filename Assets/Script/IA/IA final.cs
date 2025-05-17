/*using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Worq;

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AWSEntityIdentifier))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animation))]
    public class IA : MonoBehaviourPunCallbacks
    {
        [Header("Group")]
        [Space(10)]
        public WaypointRoute group;
        [HideInInspector] public int groupID = 0;

        [Header("Patrol")]
        [Space(10)]
        [Tooltip("Minimum amount of time to wait before moving to next patrol point")]
        public float minPatrolWaitTime = 1f;

        [Tooltip("Maximum amount of time to wait before moving to next patrol point")]
        public float maxPatrolWaitTime = 3f;

        [Tooltip("If or not all entities patrol waypoints at random or in sequence")]
        public bool randomPatroler = false;

        [Space(10)]
        [Header("Agent")]
        [Space(10)]
        [Tooltip("Speed by which the NavMesh agent moves")]
        public float moveSpeed = 3f;

        [Tooltip("The distance from destination the Navmesh agent stops")]
        public float stoppingDistance = 1f;

        [Tooltip("Turning speed of the NavMesh  agent")]
        public float angularSpeed = 500f;

        [Tooltip("Defines how high up the entity is. This is useful for creating flying entities")]
        public float distanceFromGround = 0.5f;

        [SerializeField] Transform? target = null;
        [SerializeField] float triggerRadius = 10f;

        //[SerializeField] private GameObject image;
        //[SerializeField] private GameObject menubackground;
        //[SerializeField] private GameObject menuprincipale;
        //[SerializeField] private GameObject textgameover;

        [SerializeField] Animator animator;

        NavMeshAgent agent;
        PhotonView photonView;

        // Variables internes
        private AWSManager mAWSManager;
        private Animation anim;
        private AudioSource src;
        private Transform[] patrolPoints;
        private bool isWaiting;
        private bool hasPlayedDetectSound;
        private int waypointCount;
        private bool interruptPatrol;
        private int destPoint;
        private bool reset;
        private bool resetPatrol;

        private bool isChasing = false;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
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

        void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            target = GameObject.FindGameObjectWithTag("Player")?.transform;

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
            if (!PhotonNetwork.IsMasterClient) return;

            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= triggerRadius)
                {
                    isChasing = true;
                    agent.SetDestination(target.position);

                    if (agent.velocity.magnitude > 0.1f)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                    }
                    else
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, false);
                    }

                    if (distanceToTarget <= 2f)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, false);
                        //image.SetActive(false);
                        //menuprincipale.SetActive(true);
                        //menubackground.SetActive(true);
                        //textgameover.SetActive(true);
                        return;
                    }
                }
                else
                {
                    isChasing = false;

                    if (resetPatrol || reset)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                        agent.isStopped = false;
                        goToNextPointDirect();
                        interruptPatrol = false;
                        resetPatrol = false;
                        reset = false;
                    }

                    if (interruptPatrol)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, false);
                        agent.isStopped = true;
                    }

                    if (!interruptPatrol && !isWaiting && agent.remainingDistance <= stoppingDistance && null != group)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                        GotoNextPoint();
                    }

                    agent.stoppingDistance = stoppingDistance;
                    agent.speed = moveSpeed;
                    agent.angularSpeed = angularSpeed;
                    agent.baseOffset = distanceFromGround;
                }
            } else
            {

            }
        }

        [PunRPC]
        public void SetWalkRPC(bool isWalking)
        {
            animator.SetBool("IsWalk", isWalking);
        }

        private void GotoNextPoint()
        {
            if (patrolPoints.Length == 0) return;
            StartCoroutine(pauseAndContinuePatrol());
        }

        IEnumerator pauseAndContinuePatrol()
        {
            isWaiting = true;
            float waitTime = UnityEngine.Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
            yield return new WaitForSeconds(waitTime);
            goToNextPointDirect();
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

        public void ResetPatrol()
        {
            reset = true;
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

        void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (target == null && other.CompareTag("Player"))
            {
                target = other.transform;
                isChasing = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (target != null && other.transform == target)
            {
                target = null;
                isChasing = false;
            }
        }
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PhotonView))]
    public class IA_Multiplayer : MonoBehaviourPunCallbacks
    {
        [Header("Patrol Settings")]
        public WaypointRoute group;
        public float minPatrolWaitTime = 1f;
        public float maxPatrolWaitTime = 3f;
        public bool randomPatroler = false;

        [Header("Agent Settings")]
        public float moveSpeed = 3f;
        public float stoppingDistance = 1f;
        public float angularSpeed = 500f;
        public float distanceFromGround = 0.5f;
        public float triggerRadius = 10f;
        public float catchDistance = 2f;

        [Header("Animation")]
        public Animator animator;

        private NavMeshAgent agent;
        private PhotonView photonView;
        private Transform[] patrolPoints;
        private Transform target;
        private bool isWaiting;
        private int destPoint;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            photonView = GetComponent<PhotonView>();

            if (group != null)
            {
                List<Transform> points = new List<Transform>();
                foreach (Transform child in group.transform)
                {
                    if (child.GetComponent<WaypointIdentifier>())
                    {
                        points.Add(child);
                        if (child.TryGetComponent(out MeshRenderer mr)) mr.enabled = false;
                        if (child.TryGetComponent(out Collider col)) col.enabled = false;
                    }
                }
                patrolPoints = points.ToArray();
            }
        }

        void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            agent.autoBraking = false;
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;

            if (patrolPoints.Length > 0)
            {
                GoToNextPoint();
            }
        }

        void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            FindClosestPlayer();

            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= triggerRadius)
                {
                    agent.SetDestination(target.position);
                    photonView.RPC("SetWalkRPC", RpcTarget.All, agent.velocity.magnitude > 0.1f);

                    if (distance <= catchDistance)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, false);
                        SwitchToSpectator(target.gameObject);
                        target = null;
                        return;
                    }
                }
                else
                {
                    target = null;
                }
            }

            if (target == null && agent.remainingDistance <= stoppingDistance && !isWaiting)
            {
                photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                GoToNextPoint();
            }
        }

        void FindClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float minDist = Mathf.Infinity;
            Transform closest = null;

            foreach (GameObject player in players)
            {
                if (!player.activeInHierarchy) continue;
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = player.transform;
                }
            }

            target = closest;
        }

        void GoToNextPoint()
        {
            StartCoroutine(PatrolCoroutine());
        }

        IEnumerator PatrolCoroutine()
        {
            isWaiting = true;
            yield return new WaitForSeconds(Random.Range(minPatrolWaitTime, maxPatrolWaitTime));
            if (patrolPoints.Length == 0) yield break;

            if (randomPatroler)
            {
                int next;
                do
                {
                    next = Random.Range(0, patrolPoints.Length);
                } while (next == destPoint);
                destPoint = next;
            }
            else
            {
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }

            agent.destination = patrolPoints[destPoint].position;
            isWaiting = false;
        }

        void SwitchToSpectator(GameObject player)
        {
            int viewID = player.GetComponent<PhotonView>().ViewID;
            photonView.RPC("KillPlayerRPC", RpcTarget.All, viewID);
        }

        [PunRPC]
        void KillPlayerRPC(int playerViewID)
        {
            PhotonView targetView = PhotonView.Find(playerViewID);
            if (targetView == null) return;

            GameObject playerObj = targetView.gameObject;

            if (targetView.IsMine)
            {
                // Stop controls
                var a = playerObj.GetComponent<PlayerScript>();
                var b = playerObj.GetComponent<CharacterInteraction>();

                if (a != null) a.enabled = false;
                if (b != null) b.enabled = false;

                // Hide mesh & colliders
                foreach (var r in playerObj.GetComponentsInChildren<Renderer>())
                    r.enabled = false;
                foreach (var c in playerObj.GetComponentsInChildren<Collider>())
                    c.enabled = false;

                // Switch camera to spectator mode
                StartCoroutine(SpectateAnotherPlayer());
            }

            // Let master decide if game over
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(CheckGameOver());
        }

        IEnumerator SpectateAnotherPlayer()
        {
            yield return new WaitForSeconds(0.2f);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                if (p.activeInHierarchy && p.GetComponent<PhotonView>().IsMine)
                {
                    Camera.main.transform.SetParent(p.transform);
                    Camera.main.transform.localPosition = new Vector3(0, 5, -5);
                    Camera.main.transform.localRotation = Quaternion.Euler(30, 0, 0);
                    yield break;
                }
            }

            // No player left — optional: go to a default scene camera or UI
            Camera.main.transform.SetParent(null);
        }

        IEnumerator CheckGameOver()
        {
            yield return new WaitForSeconds(1f);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            bool anyAlive = false;
            foreach (var p in players)
            {
                var renderers = p.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    if (r.enabled)
                    {
                        anyAlive = true;
                        break;
                    }
                }
                if (anyAlive) break;
            }

            if (!anyAlive)
            {
                PhotonNetwork.LoadLevel("Defaite");
            }
        }

        [PunRPC]
        void SetWalkRPC(bool isWalking)
        {
            if (animator != null)
                animator.SetBool("IsWalk", isWalking);
        }
    }
}