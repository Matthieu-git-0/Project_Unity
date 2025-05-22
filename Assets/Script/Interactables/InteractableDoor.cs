using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableDoor : MonoBehaviourPun
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
    public void Use()
    {
        //isOpen = !isOpen;
        //animator.SetBool("isOpen", isOpen);
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, !isOpen);
    }

    public void Open()
    {
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, true);
        StartCoroutine(CloseAfterDelay(5f));
    }

    public void OpenCamera()
    {
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, true);
        StartCoroutine(CloseAfterDelay(5f));
    }


    public void OpenForIa()
    {
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, true);
        StartCoroutine(CloseAfterDelay(2f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("SyncDoorState", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    void SyncDoorState(bool newState)
    {
        isOpen = newState;
        animator.SetBool("isOpen", isOpen);
    }
}
