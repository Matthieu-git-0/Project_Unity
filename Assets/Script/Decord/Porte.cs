using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Porte : MonoBehaviourPun
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isOpen = false;

    [SerializeField] private AudioSource doorOpenSound;
    [SerializeField] private AudioSource doorCloseSound;

    public void playOpenDoorSound()
    {
        doorOpenSound.Play();
    }

    public void playCloseDoorSound()
    {
        doorCloseSound.Play();
    }

    public void Useforever()
    {
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, !isOpen);
    }

    

    [PunRPC]
    void SyncDoorState(bool newState)
    {
        isOpen = newState;
        animator.SetBool("isOpen", isOpen);
    }
}
