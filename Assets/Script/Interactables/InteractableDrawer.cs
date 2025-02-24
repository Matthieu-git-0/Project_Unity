using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDrawer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isOpen = false;

    /*public void Start()
    {
        animator.SetBool("isOpen", isOpen);
    }*/

    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}
