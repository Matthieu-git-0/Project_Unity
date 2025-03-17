using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviour
{
    public PlayerMoovement movement;
    public CharacterInteraction Interaction;
    public CameraRotation cameraRotation;
    public Camera playerCamera;
    /*public Canvas[] worldCanvas;

    public void SetCanvasCamera()
    {
        if (playerCamera != null)
        {
            foreach (Canvas canvas in worldCanvas)
            {
                canvas.worldCamera = playerCamera;
            }
        }
    }*/

    public void IslocalPlayer()
    {
        //SetCanvasCamera();
        cameraRotation.enabled = true;
        movement.enabled = true;
        Interaction.enabled = true;
        playerCamera.gameObject.SetActive(true);
    }
}