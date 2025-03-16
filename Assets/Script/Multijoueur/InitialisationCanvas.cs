using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Canvas[] worldCanvas;

    void Start()
    {
        if (photonView.IsMine)
        {
            Camera playerCam = Camera.main;
            if (playerCam != null)
            {
                foreach (Canvas canvas in worldCanvas)
                {
                    canvas.worldCamera = playerCam;
                }
            }
        }
    }
}