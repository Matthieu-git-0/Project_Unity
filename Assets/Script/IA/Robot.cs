using UnityEngine;

public class IArobot : MonoBehaviour
{
    [SerializeField]  float speed = 1;

    Ray rayon; 
    RaycastHit hit;
    
    
    void Update()
    {
        rayon = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        if (Physics.Raycast(rayon, out hit, Mathf.Infinity))
        {
            //Debug.Log("Objet; " + hit.collider.name + "Distance: " + hit.distance );
            if (hit.distance < 1)
            {
                float angle = Random.Range(100, 300);
                transform.Rotate(Vector3.up * angle);
            }
        }
        
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10f, Color.red);
        // deplacment vers l'avant:    
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    


}

