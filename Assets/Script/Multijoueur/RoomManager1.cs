using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomCodeInput;
    public TMP_Text statusText;

    public GameObject playerPrefab;
    
    public void Start()
    {
        statusText.text = "Connecting...";
        Debug.Log("Connecting...");

        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Connexion à Photon en cours...";
            //Debug.LogWarning("Le client n'est pas encore prêt. Veuillez attendre la connexion au Master Server.");
            PhotonNetwork.ConnectUsingSettings();
            //return;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        statusText.text = "You are connected to the Server";
        Debug.Log("Connected");
        PhotonNetwork.JoinLobby();
    }

    public void JoinCreateRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Connecting to server...";
            Debug.LogWarning("The client is not ready yet. Please wait for the connection to the Master Server.");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        string roomCode = roomCodeInput.text;

        if (!string.IsNullOrEmpty(roomCode))
        {
            statusText.text = "Trying to connect to the room...";
            Debug.Log("We are in the Lobby");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;

            TypedLobby typedLobby = new TypedLobby("DefaultLobby", LobbyType.Default);

            PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, typedLobby);
        }
        else
        {
            statusText.text = "The room code is empty!";
            return;
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map_Multi");
    }
}
