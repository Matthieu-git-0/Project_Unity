using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableWardrobe : MonoBehaviourPun
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    public void Interact()
    {
        photonView.RPC("SyncDrawerState", RpcTarget.AllBuffered, !isOpen);
    }

    [PunRPC]
    void SyncDrawerState(bool newState)
    {
        isOpen = newState;
        animator.SetBool("isOpen", isOpen);
    }
}