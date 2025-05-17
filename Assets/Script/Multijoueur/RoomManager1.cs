using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomCodeInput;
    public TMP_Text statusText;
    public GameObject playerPrefab;
    
    public void Start()
    {
        statusText.text = "Connecting...";
        //Debug.Log("Connecting...");

        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Connexion à Photon en cours...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 100000; // 10 secondes
            //PhotonNetwork.NetworkingClient.LoadBalancingPeer.KeepAliveInBackground = 10;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        statusText.text = "You are connected to the Server";
        //Debug.Log("Connected");
        PhotonNetwork.JoinLobby();
    }
    
    public void JoinCreateRoomSolo()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
            return;
        }
        
        Debug.Log("We are in the Lobby");

        string roomCode = Random.Range(100000, 999999).ToString();

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 1,
            IsVisible = true,
            IsOpen = true
        };

        TypedLobby typedLobby = new TypedLobby("DefaultLobby", LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, typedLobby);
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room: {message}");
        JoinCreateRoomSolo();
    }

    public void JoinCreateRoomMulti()
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
        PhotonNetwork.LoadLevel("Map");
    }
}
