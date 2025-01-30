using UnityEngine;
using UnityEngine.AI;

public class IAennemi : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float triggerRadius = 10f;
    [Space]
    
    [SerializeField] GameObject jeu;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject textgameover;
    [SerializeField] Animator animator;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
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
                jeu.SetActive(false);
                menu.SetActive(true);
                textgameover.SetActive(true);
                return;
            }
        }
        else
        {
            agent.ResetPath();
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
