using Photon.Pun;
using UnityEngine;
public class PhotonLuncher : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connexion à Photon en cours...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au Master Server !");
        //PhotonNetwork.JoinLobby(); // Rejoint le lobby (facultatif)
    }
}
