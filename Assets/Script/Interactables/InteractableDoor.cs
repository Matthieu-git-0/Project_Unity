using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private bool isOpen = false;

    public void Start()
    {
        animator.SetBool("isOpen", isOpen);
    }
    
    public void Use()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }

    public void Open()
    {
        isOpen = true;
        animator.SetBool("isOpen", isOpen);
    }

    public void close()
    {
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
    }
}
