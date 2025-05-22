using UnityEngine;
using Photon.Pun;

public class CollierTrigger : MonoBehaviourPun
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (PhotonNetwork.IsMasterClient)
        {
            triggered = true;
            photonView.RPC("StartDefeatSequence", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartDefeatSequence()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.PlayDefeatCinematic());
    }
}
