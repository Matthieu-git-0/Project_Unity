using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject Mainmenu;
    
    [Space]
    public TMP_InputField roomCodeInput;
    public TMP_Text statusText;
    
    [Space]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    void Start()
    {
        statusText.text = "Connecting...";
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        statusText.text = "Connected to server";
        Debug.Log("Connected to server");

        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            statusText.text = "Connecting to server...";
            Debug.LogWarning("The client is not ready yet. Please wait for the connection to the Master Server.");
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
        base.OnJoinedRoom();
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        statusText.text = "Joined Room: " + PhotonNetwork.CurrentRoom.Name;
        
        Mainmenu.SetActive(false);

        if (playerPrefab != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log("Player Count: " + playerCount);
            
            Transform spawnPoint = playerCount == 1 ? spawnPoint1 : spawnPoint2;

            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is not assigned!");
                return;
            }
            
            GameObject _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
            _player.GetComponent<PlayerSetup>()?.IslocalPlayer();
            Debug.Log("Player instantiated at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("Player prefab is not assigned!");
        }
    }
}