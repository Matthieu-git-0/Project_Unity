using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerMoovement movement;
    public CharacterInteraction Interaction;
    public CameraRotation cameraRotation;
    public new GameObject camera;

    public void IslocalPlayer()
    {
        cameraRotation.enabled = true;
        movement.enabled = true;
        Interaction.enabled = true;
        camera.SetActive(true);
    }
}
