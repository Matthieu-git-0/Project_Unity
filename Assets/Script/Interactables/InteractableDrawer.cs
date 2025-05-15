/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDrawer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isOpen = false;

    /*public void Start()
    {
        animator.SetBool("isOpen", isOpen);
    }

    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableDrawer : MonoBehaviourPun
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