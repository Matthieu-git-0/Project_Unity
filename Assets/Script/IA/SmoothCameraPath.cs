using UnityEngine;
using Worq;

public class CameraLookAtPoints : MonoBehaviour
{
    public Transform[] points;
    public float rotationSpeed = 2f;
    public float waitTime = 2f;
    public IA ia;
    public float detectionRange = 10f;
    public float detectionAngle = 90f;
    public LayerMask detectionLayer;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    
    void Update()
    {
        if (points.Length == 0 || isWaiting)
            return;
            
        Vector3 direction = points[currentPointIndex].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            StartCoroutine(WaitAndChangePoint());
        }
        
        DetectPlayerInRange();
    }

    private System.Collections.IEnumerator WaitAndChangePoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentPointIndex = (currentPointIndex + 1) % points.Length;
        isWaiting = false;
    }
    
    void DetectPlayerInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, detectionLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                
                if (angleToPlayer <= detectionAngle / 2)
                {
                    if (ia != null && !ia.enabled)
                    {
                        Debug.Log("dfghj");
                        ia.enabled = true;
                    }
                    Debug.Log("Joueur détecté ! IA activée.");
                    return;
                }
            }
        }

        // Si aucun joueur n'est détecté, désactiver l'IA
        /*if (ia != null && ia.activeSelf)
        {
            ia.SetActive(false); // Désactiver l'IA si le joueur n'est plus dans la zone
        }*/
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Vector3 origin = transform.position;
        
        Vector3 forward = transform.forward * detectionRange;

        float angle = detectionAngle * Mathf.Deg2Rad;

        int segments = 30;
        
        for (int i = 0; i < segments; i++)
        {
            float currentAngle = (i * detectionAngle / segments) - detectionAngle / 2;
            
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;
            
            Gizmos.DrawLine(origin, origin + direction);
        }
        
        float baseRadius = detectionRange * Mathf.Tan(angle / 2);
        Vector3 lastPoint = origin + forward;
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = (i * detectionAngle / segments) - detectionAngle / 2;
            Vector3 pointOnBase = origin + Quaternion.Euler(0, currentAngle, 0) * forward;
            
            Gizmos.DrawLine(lastPoint, pointOnBase);
            lastPoint = pointOnBase;
        }
    }
}
