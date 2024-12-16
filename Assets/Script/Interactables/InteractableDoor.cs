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
        animator.SetBool("isOpen", true);
        StartCoroutine(CloseAfterDelay(5f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isOpen", false);
    }
}
