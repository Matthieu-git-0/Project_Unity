using UnityEngine;

public class CameraMultiRaycast : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float detectionDistance = 5f;

    void Update()
    {
        Vector3[] directions = {
            transform.forward,
            transform.forward + transform.right,
            transform.forward - transform.right,
            transform.forward + transform.up,
            transform.forward - transform.up
        };

        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir.normalized, out hit, detectionDistance, detectionLayer))
            {
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Npc"))
                {
                    Debug.DrawRay(transform.position, dir.normalized * detectionDistance, Color.green);
                    door.GetComponent<InteractableDoor>().OpenCamera();
                }
            }
            else
            {
                Debug.DrawRay(transform.position, dir.normalized * detectionDistance, Color.red);
            }
        }
    }
}