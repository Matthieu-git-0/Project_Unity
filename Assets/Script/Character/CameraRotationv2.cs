/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationv2 : MonoBehaviour
{
    public Camera head;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    private float rotationX = 0;
    private bool canMove = false;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canMove = true;
    }
    public void Update()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            head.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.localRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void CanMove(bool boolean)
    {
        canMove = boolean;
    }
}*/
/*
using UnityEngine;

public class CameraRotationv2 : MonoBehaviour
{
    public Transform head;
    public Transform body;
    public float rotationSpeed = 5f;
    public float turnThreshold = 45f;
    public bool canMove = false;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //canMove = true;
    }
    
    void Update()
    {
        if (canMove)
            RotateHead();
        else
            return;
    }
    
    public void CanMove(bool boolean)
    {
        canMove = boolean;
    }

    void RotateHead()
    {
        Vector3 lookDirection = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")).normalized;
        
        if (lookDirection.magnitude < 0.1f) return;
        
        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        float headAngle = Mathf.LerpAngle(head.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
        
        head.rotation = Quaternion.Euler(0, headAngle, 0);
        
        float angleDifference = Mathf.DeltaAngle(body.eulerAngles.y, headAngle);

        if (Mathf.Abs(angleDifference) > turnThreshold)
        {
            body.rotation = Quaternion.Lerp(body.rotation, Quaternion.Euler(0, headAngle, 0), Time.deltaTime * rotationSpeed);
        }
    }
}
*/

using UnityEngine;

public class CameraRotationv2 : MonoBehaviour
{
    public Transform head;  // Référence à la tête
    public Transform body;  // Référence au corps
    public float rotationSpeed = 5f;
    public float turnThreshold = 45f;  // Seuil de déclenchement pour tourner le corps
    public bool canMove = false;

    private float rotationX = 180f; // Rotation X du personnage
    private float rotationZ = 0f;   // Rotation Z du personnage

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (canMove)
            RotateHead();
    }
    
    public void CanMove(bool boolean)
    {
        canMove = boolean;
    }

    void RotateHead()
    {
        // Récupérer les mouvements de la souris
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Modifier la rotation X (haut/bas) et la rotation Z (gauche/droite)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, 180f - turnThreshold, 180f + turnThreshold);
        rotationZ += mouseX; 

        // Appliquer la rotation sur la tête
        head.localRotation = Quaternion.Euler(rotationX, 0, rotationZ);

        // Vérifier si la tête dépasse le seuil pour tourner le corps
        if (rotationX <= 180f - turnThreshold || rotationX >= 180f + turnThreshold)
        {
            // Faire tourner le corps en douceur vers la direction de la tête
            float targetBodyZ = body.eulerAngles.z + mouseX;
            body.rotation = Quaternion.Lerp(body.rotation, Quaternion.Euler(rotationX, 0, targetBodyZ), Time.deltaTime * rotationSpeed);
        }
    }
}
