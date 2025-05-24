using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class desactivationduperso : MonoBehaviourPun
{
    public List<GameObject> Corp;
    private PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            foreach (var a  in Corp)
            {
                a.SetActive(false);
            }
        }
    }
}
