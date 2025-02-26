using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_debut : MonoBehaviour
{
	public GameObject corp;
	public GameObject mouvement;
	public Animator animation;
	private CameraRotationv2 characterRotation;

	public void Start()
    {
        StartCoroutine(WaitAndLog());
    }

    IEnumerator WaitAndLog()
    {
        yield return new WaitForSeconds(7.3f);
        characterRotation = mouvement.GetComponent<CameraRotationv2>();
		animation.enabled = false;
		//characterMouvement = GetComponent<CharacterController>();
		characterRotation.CanMove(true);
    }
}
