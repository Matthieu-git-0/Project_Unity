using UnityEngine;
using Photon.Pun;

public class InitialisationCanvas : MonoBehaviour
{
    public Canvas[] worldCanvas;

    public void SetCanvasCamera(Camera playerCamera)
    {
        if (playerCamera != null)
        {
            foreach (Canvas canvas in worldCanvas)
            {
                canvas.worldCamera = playerCamera;
            }
        }
    }
}
