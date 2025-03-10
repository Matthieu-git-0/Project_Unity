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

    public void IslocalPlayer()
    {
        cameraRotation.enabled = true;
        movement.enabled = true;
        Interaction.enabled = true;
        playerCamera.gameObject.SetActive(true);
    }
}