using UnityEngine;
using UnityEngine.AI;

public class Annimationmove : MonoBehaviour
{
    [SerializeField] Animator animator;
    NavMeshAgent agent;
   
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (agent.velocity.magnitude > 0.01f)
        {
            animator.SetBool("IsWalk", true);
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }
    }
}