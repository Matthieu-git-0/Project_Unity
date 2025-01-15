using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    public Transform player;
    public float interactionDistance = 3f;
    private bool isInteracting = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (Input.GetKeyDown(KeyCode.E) && !isInteracting && distanceToPlayer <= interactionDistance)
        {
            isInteracting = true;
    
            if (!animator.GetBool("IsOpen"))
            {
                animator.SetTrigger("OpenDoor");
                animator.SetBool("IsOpen", true);
            }
            else
            {
                animator.SetTrigger("CloseDoor");
                animator.SetBool("IsOpen", false);
            }

            Invoke("ResetInteraction", 1f);
        }
    }

    private void ResetInteraction()
    {
        isInteracting = false;
    }
}
