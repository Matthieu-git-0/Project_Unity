using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class IAscript : MonoBehaviourPunCallbacks
{
    public float speed = 3f;
    public float detectionRadius = 10f;

    private Transform target;
    private bool isChasing = false;

    private NavMeshAgent agent;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (isChasing && target != null)
        {
            agent.SetDestination(target.position);
        }
        if (!isChasing) 
        { 
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (target == null && other.CompareTag("Player"))
        {
            target = other.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (target != null && other.transform == target)
        {
            target = null;
            isChasing = false;
        }
    }   
}