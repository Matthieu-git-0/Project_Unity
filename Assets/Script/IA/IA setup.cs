using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IAsetup : MonoBehaviourPun
{
    public IAscript Iascript;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Iascript.enabled = true;
        }
    }
}