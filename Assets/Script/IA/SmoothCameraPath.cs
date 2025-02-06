using UnityEngine;

public class CameraLookAtPoints : MonoBehaviour
{
    public Transform[] points;           // Liste des points que la caméra va regarder
    public float rotationSpeed = 2f;     // Vitesse de rotation de la caméra
    public float waitTime = 2f;          // Temps d'attente entre les changements de point
    public GameObject ia;                // Référence à l'objet IA (assurez-vous que l'objet IA a un script avec la variable 'enable')
    public float detectionRange = 10f;   // Distance de détection du joueur
    public float detectionAngle = 90f;   // Angle de détection de la caméra
    public LayerMask detectionLayer;     // Couches à détecter, par exemple "Player"

    private int currentPointIndex = 0;   // Index actuel de la liste des points
    private bool isWaiting = false;      // Pour savoir si la caméra doit attendre avant de passer au point suivant

    void Update()
    {
        if (points.Length == 0 || isWaiting) return;

        // Faire pivoter la caméra vers le point actuel
        Vector3 direction = points[currentPointIndex].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Si la caméra a suffisamment tourné vers le point, passer au point suivant
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            StartCoroutine(WaitAndChangePoint());
        }

        // Vérifier si un joueur est dans la zone de détection de la caméra
        DetectPlayerInRange();
    }

    private System.Collections.IEnumerator WaitAndChangePoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentPointIndex = (currentPointIndex + 1) % points.Length; // Passer au point suivant
        isWaiting = false;
    }

    // Détection du joueur dans la zone de vision de la caméra
    void DetectPlayerInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, detectionLayer);

        foreach (Collider collider in colliders)
        {
            // Si l'objet détecté est un joueur
            if (collider.CompareTag("Player"))  // Assurez-vous que le joueur a le tag "Player"
            {
                Vector3 directionToPlayer = collider.transform.position - transform.position;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                // Si l'angle entre la caméra et le joueur est inférieur à l'angle de détection
                if (angleToPlayer <= detectionAngle / 2)
                {
                    // Activer l'IA si le joueur est détecté
                    if (ia != null && !ia.activeSelf)
                    {
                        ia.SetActive(true);  // Activer l'IA (en supposant que l'IA est un GameObject)
                    }
                    Debug.Log("Joueur détecté ! IA activée.");
                    return; // Le joueur est détecté, pas besoin de continuer à chercher
                }
            }
        }

        // Si aucun joueur n'est détecté, désactiver l'IA
        /*if (ia != null && ia.activeSelf)
        {
            ia.SetActive(false); // Désactiver l'IA si le joueur n'est plus dans la zone
        }*/
    }

    // Dessiner le cône de détection dans l'éditeur
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Position de la caméra
        Vector3 origin = transform.position;

        // Direction de la caméra
        Vector3 forward = transform.forward * detectionRange;

        // Angle de vision en radians
        float angle = detectionAngle * Mathf.Deg2Rad;

        // Nombre de segments pour dessiner la base du cône (cercle)
        int segments = 30; // Plus de segments pour plus de détails

        // Dessiner le cône : relier le sommet aux points de la base
        for (int i = 0; i < segments; i++)
        {
            // Calculer l'angle de chaque segment de la base
            float currentAngle = (i * detectionAngle / segments) - detectionAngle / 2;

            // Convertir l'angle en direction 3D
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;

            // Dessiner une ligne entre la caméra et chaque point de la base
            Gizmos.DrawLine(origin, origin + direction);
        }

        // Dessiner un cercle à la base du cône pour mieux le visualiser
        float baseRadius = detectionRange * Mathf.Tan(angle / 2); // Rayon de la base
        Vector3 lastPoint = origin + forward;
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = (i * detectionAngle / segments) - detectionAngle / 2;
            Vector3 pointOnBase = origin + Quaternion.Euler(0, currentAngle, 0) * forward;

            // Relier les points de la base pour former un cercle
            Gizmos.DrawLine(lastPoint, pointOnBase);
            lastPoint = pointOnBase;
        }
    }
}
