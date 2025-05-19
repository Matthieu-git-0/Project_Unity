using UnityEngine;
using Photon.Pun;

public class CoupureElectrique : MonoBehaviourPun
{
    public GameObject lightGroup;
    public GameObject porte;
    private Porte p;

    void Start()
    {
        p = porte.GetComponent<Porte>();
    }

    public void DisableAllLightsAndNotify()
    {
        lightGroup.SetActive(false);
        p.Useforever();
        photonView.RPC("RPC_DisableAllLights", RpcTarget.OthersBuffered);
        this.enabled = false;
    }

    [PunRPC]
    void RPC_DisableAllLights()
    {
        lightGroup.SetActive(false);
        p.Useforever();
        this.enabled = false;
    }
}
