using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaInteraction : MonoBehaviour
{
	[SerializeField] private GameObject body = null;
	[SerializeField] private float viewDiastance = 5f;
	[SerializeField] private LayerMask interactionLayer;

    void Start()
    {
        if (interactionLayer.value == 0)
        {
            interactionLayer = LayerMask.GetMask("Interactable");
        }
    }
    
    void Update()
    {
        Vector3 origin = body.transform.position;
        Vector3 direction = body.transform.forward;
		//direction.y = 0;
		//direction.Normalize();

        Debug.DrawRay(origin, direction * viewDiastance, Color.green);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, viewDiastance, interactionLayer) && hit.collider.tag == "Interaction/Slice")
        {
			hit.collider.GetComponent<InteractableDoor>().Open();
		}
    }
}	