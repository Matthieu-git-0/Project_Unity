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

    /*void Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= triggerRadius)
        {
            animator.SetBool("IsWalk", true);
            if (Vector3.Distance(transform.position, target.position) <= 2f)
            {
                animator.SetBool("IsWalk", false);
                jeu.SetActive(false);
                menu.SetActive(true);
                textgameover.SetActive(true);
                //agent.ResetPath();
                return;
            }
            agent.SetDestination(target.position);
        }
        else
        {
            animator.SetBool("IsWalk", false);
            agent.ResetPath(); // Arrête l'agent si la cible sort du rayon
            
        }
    }*/
    
    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= triggerRadius)
        {
            agent.SetDestination(target.position);

            // Active l'animation si l'agent se déplace
            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("IsWalk", true);
            }
            else
            {
                animator.SetBool("IsWalk", false);
            }

            // Vérifie si l'ennemi est proche du joueur (Game Over)
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
            animator.SetBool("IsWalk", false);
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
