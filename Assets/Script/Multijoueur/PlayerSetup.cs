using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviour
{
    public CharacterInteraction Interaction;
    public PlayerScript playerScript;
    public Camera playerCamera;

    public void IslocalPlayer()
    {
        playerCamera.gameObject.SetActive(true);
        playerScript.enabled = true;
        Interaction.enabled = true;
        StartCoroutine(WaitAndLog());
    }
    
    IEnumerator WaitAndLog()
    {
        yield return new WaitForSeconds(7.3f);
        playerScript.Move(true);
    }
}