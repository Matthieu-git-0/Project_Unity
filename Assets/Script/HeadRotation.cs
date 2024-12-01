using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRotation : MonoBehaviour
{
    public Transform Head;           // Transform de la tête, assigné dans l'inspecteur
    public Camera playerCamera;      // Référence à la caméra du joueur
    public float lookSpeed = 2f;     // Vitesse de la rotation de la tête
    public float lookXLimit = 45f;   // Limite de l'inclinaison de la tête (haut/bas)
    public float lookYLimit = 45f;   // Limite de la rotation de la tête (gauche/droite)

    private float rotationX = 0;     // Rotation sur l'axe X (haut/bas)
    private float rotationY = 0;     // Rotation sur l'axe Y (gauche/droite)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleHeadRotation();
    }

    private void HandleHeadRotation()
    {
        // Rotation haut/bas de la tête (axe X)
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;  // Inverser la direction du mouvement de la souris (haut/bas)
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);  // Limiter l'inclinaison de la tête
        Head.localRotation = Quaternion.Euler(rotationX, 0, 0);  // Appliquer la rotation de la tête sur l'axe X

        // Rotation gauche/droite de la tête (axe Y)
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;  // Rotation gauche/droite
        rotationY = Mathf.Clamp(rotationY, -lookYLimit, lookYLimit);  // Limiter la rotation
        Head.localRotation *= Quaternion.Euler(0, rotationY, 0);  // Appliquer la rotation de la tête sur l'axe Y

        // La caméra est enfant de la tête, donc elle suivra la tête
        //playerCamera.transform.localRotation = Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}