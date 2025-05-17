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
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AWSEntityIdentifier))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animation))]
    public class IA : MonoBehaviourPunCallbacks
    {
        [Header("Group")]
        public WaypointRoute group;
        [HideInInspector] public int groupID = 0;

        [Header("Patrol")]
        public float minPatrolWaitTime = 1f;
        public float maxPatrolWaitTime = 3f;
        public bool randomPatroler = false;

        [Header("Agent")]
        public float moveSpeed = 3f;
        public float stoppingDistance = 1f;
        public float angularSpeed = 500f;
        public float distanceFromGround = 0.5f;

        [SerializeField] float triggerRadius = 10f;
        [SerializeField] Animator animator;

        private NavMeshAgent agent;
        private PhotonView photonView;

        private Transform[] patrolPoints;
        private Transform target;

        private bool isWaiting;
        private bool interruptPatrol;
        private bool reset;
        private bool resetPatrol;
        private int destPoint;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();
            agent = GetComponent<NavMeshAgent>();

            // Initialisation des points de patrouille
            if (group != null)
            {
                Transform groupTransform = group.transform;
                int count = groupTransform.childCount;

                var tempList = new System.Collections.Generic.List<Transform>();

                for (int i = 0; i < count; i++)
                {
                    Transform child = groupTransform.GetChild(i);
                    if (child.GetComponent<WaypointIdentifier>())
                    {
                        tempList.Add(child);
                        if (child.TryGetComponent(out MeshRenderer renderer)) renderer.enabled = false;
                        if (child.TryGetComponent(out Collider collider)) collider.enabled = false;
                    }
                }

                patrolPoints = tempList.ToArray();
            }
            else
            {
                Debug.LogWarning("Group not assigned for " + gameObject.name);
            }
        }

        void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            agent.autoBraking = false;
            agent.stoppingDistance = stoppingDistance;
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                GotoNextPoint();
            }
        }

        void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            FindClosestPlayer();

            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= triggerRadius)
                {
                    agent.SetDestination(target.position);
                    photonView.RPC("SetWalkRPC", RpcTarget.All, agent.velocity.magnitude > 0.1f);

                    if (distanceToTarget <= 2f)
                    {
                        photonView.RPC("SetWalkRPC", RpcTarget.All, false);

                        SwitchToSpectator(target.gameObject);

                        // Vérifier s'il reste des joueurs actifs
                        CheckActivePlayers();
                        return;
                    }
                }
                else
                {
                    target = null;
                }
            }

            if (target == null)
            {
                if (resetPatrol || reset)
                {
                    photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                    agent.isStopped = false;
                    goToNextPointDirect();
                    interruptPatrol = resetPatrol = reset = false;
                }

                if (interruptPatrol)
                {
                    photonView.RPC("SetWalkRPC", RpcTarget.All, false);
                    agent.isStopped = true;
                }

                if (!interruptPatrol && !isWaiting && agent.remainingDistance <= stoppingDistance && patrolPoints != null)
                {
                    photonView.RPC("SetWalkRPC", RpcTarget.All, true);
                    GotoNextPoint();
                }

                agent.stoppingDistance = stoppingDistance;
                agent.speed = moveSpeed;
                agent.angularSpeed = angularSpeed;
                agent.baseOffset = distanceFromGround;
            }
        }

        void FindClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float closestDistance = Mathf.Infinity;
            Transform closest = null;

            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance && distance <= triggerRadius)
                {
                    closestDistance = distance;
                    closest = player.transform;
                }
            }

            target = closest;
        }

        [PunRPC]
        public void SetWalkRPC(bool isWalking)
        {
            animator.SetBool("IsWalk", isWalking);
        }

        void GotoNextPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            StartCoroutine(PauseAndContinuePatrol());
        }

        IEnumerator PauseAndContinuePatrol()
        {
            isWaiting = true;
            float waitTime = Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
            yield return new WaitForSeconds(waitTime);
            goToNextPointDirect();
            isWaiting = false;
        }

        void goToNextPointDirect()
        {
            agent.destination = patrolPoints[destPoint].position;

            if (randomPatroler)
            {
                int nextPos;
                do
                {
                    nextPos = Random.Range(0, patrolPoints.Length);
                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }
        }

        public void ResetPatrol() => reset = true;
        public void InterruptPatrol() => interruptPatrol = true;

        void SwitchToSpectator(GameObject player)
        {
            if (player != null)
            {
                

                // On peut aussi activer une caméra de spectateur ici
                // Exemple: SpecCamera.SetActive(true);

                photonView.RPC("OnPlayerDiedRPC", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID);
            }
        }

        [PunRPC]
        void OnPlayerDiedRPC(int playerViewID)
        {
            PhotonView targetView = PhotonView.Find(playerViewID);

            if (targetView != null)
            {
                GameObject playerObject = targetView.gameObject;

                // Si c'est moi, je passe en mode spectateur
                if (targetView.IsMine)
                {
                    SpectateOtherPlayer();

                    // Optionnel : désactiver le contrôle du joueur au lieu de détruire immédiatement
                    // Pour éviter de casser le contrôle de caméra, etc.
                    playerObject.SetActive(false);
                }
                else
                {
                    // Détruire le joueur sur les autres clients
                    Destroy(playerObject);
                }

                // Vérifier s'il reste des joueurs actifs
                StartCoroutine(CheckIfGameOver());
            }
        }

        void SpectateOtherPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.activeInHierarchy)
                {
                    Camera.main.transform.SetParent(player.transform);
                    Camera.main.transform.localPosition = new Vector3(0, 5, -5);
                    Camera.main.transform.LookAt(player.transform);
                    return;
                }
            }

            // Aucun joueur actif → retour menu ou fermeture
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOverScene"); // ou Application.Quit();
            }
        }


        IEnumerator CheckIfGameOver()
        {
            yield return new WaitForSeconds(0.5f); // attendre désactivation/destruction

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            bool anyAlive = false;

            foreach (GameObject p in players)
            {
                if (p.activeInHierarchy)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOverScene"); // Ou Application.Quit();
            }
        }


        void CheckActivePlayers()
        {
            // Vérifier s'il y a encore des joueurs actifs dans la partie
            int activePlayersCount = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.IsInactive) continue; // Si le joueur est inactif (spectateur), on ne le compte pas

                activePlayersCount++;
            }

            if (activePlayersCount == 0)
            {
                // Si plus de joueurs actifs, fermer le jeu
                PhotonNetwork.LeaveRoom();
                //Application.Quit();
            }
        }
        public void SetDeatination(Transform t)
        {
            agent.destination = t.position;
            isWaiting = false;
        }
    }
}
