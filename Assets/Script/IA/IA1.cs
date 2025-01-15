using UnityEngine;
using UnityEngine.AI;

public class IAennemi : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float triggerRadius = 10f; // Rayon pour déclencher le mouvement
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= triggerRadius)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath(); // Arrête l'agent si la cible sort du rayon
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dessine une sphère pour visualiser le rayon dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
