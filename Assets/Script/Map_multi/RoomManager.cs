using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject camera;
    
    [Space]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    void Start()
    {
        Debug.Log("Connecting ...");

        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu"; // Exemple : Europe
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("We are in the Lobby");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        TypedLobby typedLobby = new TypedLobby("DefaultLobby", LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, typedLobby);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

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
            
            if (camera != null)
            {
                camera.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Camera is not assigned!");
            }
        }
        else
        {
            Debug.LogError("Player prefab is not assigned!");
        }
    }
}