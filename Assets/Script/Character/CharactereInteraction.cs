using UnityEngine;

public class CharacterInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private float interactionDistance = 5.0f;
    [SerializeField] private LayerMask interactionLayer;

    private void Start()
    {
        if (interactionLayer.value == 0)
        {
            interactionLayer = LayerMask.GetMask("Interactable");
        }
        
        /*for (int i = 0; i < 32; i++)
        {
            if ((interactionLayer.value & (1 << i)) != 0)
            {
                Debug.Log($"Couche active dans le LayerMask : {LayerMask.LayerToName(i)}");
            }
        }*/
    }

    private void Update()
    {
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        Debug.DrawRay(origin, direction * interactionDistance, Color.green);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            switch (hit.collider.tag)
            {
                case "Interaction/Door":
                    InteractionDoor(hit);
                    break;

                case "Interaction/Slice":
                    InteractionDoor_Slice(hit);
                    break;
                case "Interaction/Drawer":
                    InteractionDrawer(hit);
                    break;
                case "Interaction/Key":
                    break;
                    //InteractionKey(hit);

                default:
                    //Debug.Log("Objet non interactif touché.");
                    break;
            }
        }
    }
    
    private void InteractionDrawer(RaycastHit hit)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractableDrawer drawer = hit.collider.GetComponent<InteractableDrawer>();

            if (drawer != null)
            {
                drawer.Interact();
            }
            else
            {
                Debug.LogError("Le script InteractableDrawer est manquant.", hit.collider);
            }
        }
    }

    private void InteractionDoor_Slice(RaycastHit hit)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractableDoor door = hit.collider.GetComponent<InteractableDoor>();

            if (door != null)
            {
                door.Open();
            }
            else
            {
                Debug.LogError("Le script InteractableDoor est manquant.", hit.collider);
            }
        }
    }

    private void InteractionDoor(RaycastHit hit)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractableDoor door = hit.collider.GetComponent<InteractableDoor>();

            if (door != null)
            {
                door.Use();
            }
            else
            {
                Debug.LogError("Le script InteractableDoor est manquant.", hit.collider);
            }
        }
    }
}
