using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    public Camera cinematicCamera;
    public float cinematicDuration = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator PlayDefeatCinematic()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Camera[] cameras = Camera.allCameras;
        foreach (Camera cam in cameras)
        {
            if (cam != cinematicCamera)
                cam.enabled = false;
        }

        cinematicCamera.enabled = true;

        yield return new WaitForSeconds(cinematicDuration);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Defaite");
        }
    }
}
