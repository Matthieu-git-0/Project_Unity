using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableKey : MonoBehaviour
{
    [SerializeField] private GameObject key;

    public void Take()
    {
        key.SetActive(false);
        PlayerPrefs.SetString("Key", "true");
        PlayerPrefs.Save();
    }
}
