using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager2 : MonoBehaviour
{
    public GameObject playerPrefab;
    [Space]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public void Start()
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
        _player.GetComponentInChildren<PlayerSetup>()?.IslocalPlayer();
        Debug.Log("Player instantiated at: " + spawnPoint.position);
    }
}
