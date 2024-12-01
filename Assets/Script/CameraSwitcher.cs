using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    void Start()
    {
        // Assurez-vous que seule la caméra 1 est active au début
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {
        // Bascule de caméra lorsqu'on appuie sur "P"
        if (Input.GetKeyDown(KeyCode.P))
        {
            camera1.enabled = !camera1.enabled;
            camera2.enabled = !camera2.enabled;
        }
    }
}