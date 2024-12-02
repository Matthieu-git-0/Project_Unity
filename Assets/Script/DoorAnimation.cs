using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    public Transform player; // Référence au joueur (à glisser dans l'Inspector)
    public float interactionDistance = 3f; // Distance à laquelle l'interaction est possible
    private bool isInteracting = false; // Variable pour éviter des entrées répétées

    void Start()
    {
        // Récupère l'Animator attaché au GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Calculer la distance entre la porte et le joueur
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si la touche "E" est pressée et si le joueur est à une distance suffisante
        if (Input.GetKeyDown(KeyCode.E) && !isInteracting && distanceToPlayer <= interactionDistance)
        {
            isInteracting = true; // Empêche d'autres interactions pendant l'animation

            // Si la porte n'est pas ouverte, on l'ouvre
            if (!animator.GetBool("IsOpen"))
            {
                animator.SetTrigger("OpenDoor");
                animator.SetBool("IsOpen", true);
            }
            // Si la porte est ouverte, on la ferme
            else
            {
                animator.SetTrigger("CloseDoor");
                animator.SetBool("IsOpen", false);
            }

            // Réactive l'interaction après un délai
            Invoke("ResetInteraction", 1f); // Délai de 1 seconde, ajustez si nécessaire
        }
    }

    // Cette fonction réactive l'interaction après un délai
    private void ResetInteraction()
    {
        isInteracting = false;
    }
}