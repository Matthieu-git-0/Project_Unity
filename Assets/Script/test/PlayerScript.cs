using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(CharacterController))]

public class PlayerScript : MonoBehaviour, IPunObservable
{
	[Header("NUMERICAL PARAMETERS")]
	public float walkSpeed = 5f;
	public float runSpeed = 10f;
	public float jumpSpeed = 5.0f;
	public float gravity = 20.0f;

	[Header("STAMINA VALORS")]
    public float playerStamina = 100.0f;
    private float _maxStamina = 100.0f;

    [Header("STAMINA MODIFIERS")]
	private bool canRun = true;
    public float _staminaDrain = 30f;
    public float _staminaRegen = 20f;

    [Header("LOOKING PARAMETERS")]
	public float lookSensitivityX;
	public float lookSensitivityY;
    public float lookXLimit = 45f;
	private float rotationX = 0;

	[Header("ELEMENTS")]
	public Camera playerCamera;
	public GameObject character;
	public Animator animator;

	[Header("MENUS")]
	//public GameObject pauseObject;
    private float animatorSides;
    private float animatorFrontBack;
    private bool animatorIsRunning;
    private bool animatorIsJumping;

	private PhotonView view;

	private bool canMove = false;

	//public GameObject guiDoorMenu;

	CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    void Start()
    {
		// Verification des sensibilités de rotation camera
		lookSensitivityX = 2f * PlayerPrefs.GetFloat("sensitivityX", 2f);
		lookSensitivityY = 2f * PlayerPrefs.GetFloat("sensitivityY", 2f);

		// Initialisation des composants réseaux et physiques
        characterController = GetComponent<CharacterController>();
        view = GetComponent<PhotonView>();

		// Blocage et disparition du curseur
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //pauseObject.SetActive(false);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
		// Test s'appliquant uniquement pour le joueur local
        if (view.IsMine)
        {
            //string interactKey = PlayerPrefs.GetString("Interact", "None");
            //guiDoorMenu.GetComponent<TMP_Text>().text = $"Press \"{interactKey}\" to interact";

            // Vérification si le joueur est autorisé à bouger
            if (canMove && Cursor.lockState == CursorLockMode.Locked)
	        {
				MovePlayer();
				MoveCamera();
	        }
	        else // Sinon Update des paramètres de pause
	        {
		        UdpateSensitivityCamera();
	        }

	        CheckPauseActivation(); // Activation ou désactivation du menu pause

        }
		else
		{
			// Application des paramètres synchronisés pour les autres joueurs
            animator.SetFloat("Sides", animatorSides);
            animator.SetFloat("Front/Back", animatorFrontBack);
			animator.SetBool("isRunning", animatorIsRunning);
			animator.SetBool("isJumping", animatorIsJumping);
		}

    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envoie des paramètres de l'Animator locaux aux autres joueurs
            stream.SendNext(animatorSides);
            stream.SendNext(animatorFrontBack);
            stream.SendNext(animatorIsRunning);
            stream.SendNext(animatorIsJumping);
        }
        else
        {
            // Reception des paramètres de l'Animator depuis le serveur
            animatorSides = (float)stream.ReceiveNext();
            animatorFrontBack = (float)stream.ReceiveNext();
            animatorIsRunning = (bool)stream.ReceiveNext();
			animatorIsJumping = (bool)stream.ReceiveNext();

			// Appliquer ces paramètres à l'Animator local
	        animator.SetFloat("Sides", animatorSides);
	        animator.SetFloat("Front/Back", animatorFrontBack);
			animator.SetBool("isRunning", animatorIsRunning);
			animator.SetBool("isJumping", animatorIsJumping);
        }
    }

	private void CheckPauseActivation()
	{
		/*if (Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Pause", "None"))))
	    {
		    //pauseObject.SetActive(true);
		    Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
	    }

		if (Cursor.lockState == CursorLockMode.None && Cursor.visible)
	    {
		    //pauseObject.SetActive(true);
	    }
	    else
	    {
		    //pauseObject.SetActive(false);
	    }*/
	}
	
	// Update des sensibilités de rotation camera
	private void UdpateSensitivityCamera()
	{
		lookSensitivityX = 2f * PlayerPrefs.GetFloat("sensitivityX", 2f);
		lookSensitivityY = 2f * PlayerPrefs.GetFloat("sensitivityY", 2f);
	}

	// Déplacements de camera
	private void MoveCamera()
	{
		rotationX += -Input.GetAxis("Mouse Y") * lookSensitivityX;												// Detection du mouvement Y de souris
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);											// Blocage du mouvement Y selon les paramètres prédéfinis
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);								// Déplacement de la camera selon les mouvements de la souris
        character.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSensitivityY, 0);	// Rotation du joueur pour suivre les mouvements camera
		
		/*Rotation verticale (haut/bas) sur la caméra
		rotationX += -Input.GetAxis("Mouse Y") * lookSensitivityY;
		rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
		playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

		// Rotation horizontale (gauche/droite) sur le personnage
		float mouseX = Input.GetAxis("Mouse X") * lookSensitivityX;
		Vector3 currentRotation = character.transform.rotation.eulerAngles;
		character.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + mouseX, currentRotation.z);

		// Debug pour vérifier les valeurs
		Debug.Log($"MouseX: {mouseX}, Rotation Y: {character.transform.rotation.eulerAngles.y}");*/
	}

	// Déplacements latéraux
    private void MovePlayer()
    {
        // Initialisation du droites vectorielles
        Vector3 direction = Vector3.zero;
		
		// Touches de déplacement
		string forwardKey = PlayerPrefs.GetString("Forward", "None");
    	string backwardKey = PlayerPrefs.GetString("Backward", "None");
    	string leftKey = PlayerPrefs.GetString("Left", "None");
    	string rightKey = PlayerPrefs.GetString("Right", "None");
    	string sprintKey = PlayerPrefs.GetString("Sprint", "None");
    	string jumpKey = PlayerPrefs.GetString("Jump", "None");

		// Detection du changement d'état
		if (Input.GetKey(GetKeyCodeFromString(forwardKey))) 
			direction += transform.forward;
    	if (Input.GetKey(GetKeyCodeFromString(backwardKey))) 
			direction -= transform.forward;
    	if (Input.GetKey(GetKeyCodeFromString(leftKey))) 
			direction -= transform.right;
    	if (Input.GetKey(GetKeyCodeFromString(rightKey))) 
			direction += transform.right;

		// Normalisation du déplacement diagonal
		direction = direction.normalized;
        //Debug.Log($"Direction Calculée: {direction}");

        // Gestion du sprint avec stamina
        bool isRunning = Input.GetKey(GetKeyCodeFromString(sprintKey));
        
        // Gestion de la stamina
        if (!isRunning && playerStamina < _maxStamina)
        {
	        if (playerStamina > 5f) 
		        canRun = true;
	        
	        playerStamina += _staminaRegen * Time.deltaTime;
	        
	        if (playerStamina > _maxStamina) 
		        playerStamina = _maxStamina;
        }
        else if (isRunning && canRun)
        {
            playerStamina -= _staminaDrain * Time.deltaTime;
            if (playerStamina <= 0f)
            {
	            canRun = false;
	            playerStamina = 0f;
            }
        }

        float currentSpeed = canRun && isRunning ? runSpeed : walkSpeed;
        
		// Gestion du saut
        if (characterController.isGrounded)
        {
            moveDirection.y = -0.1f; // Garde le joueur au sol
            if (Input.GetKeyDown(GetKeyCodeFromString(jumpKey)))
            {
                moveDirection.y = jumpSpeed;
                animator.SetBool("isJumping", true);
            }
            else
            {
                animator.SetBool("isJumping", false);
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime; // Application de la gravité
            animator.SetBool("isJumping", true);
        }
        
        // Application du mouvement horizontal
        Vector3 horizontalMovement = direction * currentSpeed;

		// Fusion avec le mouvement vertical
        Vector3 finalMovement = horizontalMovement + Vector3.up * moveDirection.y;
        //Debug.Log($"Final Movement Applied: {finalMovement}");
        // Déplacement via CharacterController
        characterController.Move(finalMovement * Time.deltaTime);
        //Debug.Log("CharacterController Move called!");

        //transform.position += new Vector3(0, 0, 1) * Time.deltaTime;

        //Update des animations
        UpdateAnimatorParameters();
        //Debug.Log($"MovePlayer called: {canMove}, Direction: {moveDirection}");
    }

	// Fonction d'extraction de touche
	private KeyCode GetKeyCodeFromString(string key)
	{
    	return (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
	}

	private void UpdateAnimatorParameters()
	{
		// Initialisation
		int moveX = 0;
		int moveY = 0;

		// Vérification des touches directionnelles
		bool isMovingForward = Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Forward", "None")));
		bool isMovingBackward = Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Backward", "None")));
		bool isMovingLeft = Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Left", "None")));
		bool isMovingRight = Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Right", "None")));

		// Sprint et état de marche
		bool isRunning = Input.GetKey(GetKeyCodeFromString(PlayerPrefs.GetString("Sprint", "None"))) && canRun;

		// Attribution des valeurs pour l'Animator (int)
		moveY = isMovingForward ? 1 : (isMovingBackward ? -1 : 0);
		moveX = isMovingRight ? 1 : (isMovingLeft ? -1 : 0);

		// Mise à jour de l'Animator avec des entiers
		animator.SetFloat("Sides", moveX);
		animator.SetFloat("Front/Back", moveY);
		animator.SetBool("isRunning", isRunning);

		// Enregistrement pour la synchronisation réseau
		animatorSides = moveX;
		animatorFrontBack = moveY;
		animatorIsRunning = isRunning;
	}

	public void Move(bool move)
	{
		canMove = move;
	}
}

/*
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPun, IPunObservable
{
    [Header("NUMERICAL PARAMETERS")]
    public float walkSpeed = 1.2f;
    public float runSpeed = 2.0f;
    public float jumpSpeed = 5.0f;
    public float gravity = 20.0f;

    [Header("STAMINA PARAMETERS")]
    public float playerStamina = 100.0f;
    private float _maxStamina = 100.0f;
    private bool canRun = true;
    public float staminaDrain = 30f;
    public float staminaRegen = 20f;

    [Header("LOOKING PARAMETERS")]
    public float lookSensitivityX = 2.0f;
    public float lookSensitivityY = 2.0f;
    public float lookXLimit = 45f;
    private float rotationX = 0;

    [Header("ELEMENTS")]
    public Camera playerCamera;
    public GameObject character;
    public Animator animator;

    private Rigidbody rb;
    private PhotonView view;
    public bool canMove = false;
    private bool isGrounded = true;
    private Vector3 moveDirection;

    void Start()
    {
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        // Vérifie si c'est bien le joueur local
        if (!view.IsMine) return;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!view.IsMine || !canMove) return;

        MovePlayer();
        MoveCamera();
    }

    void MovePlayer()
    {
        // Détection des touches
        string forwardKey = PlayerPrefs.GetString("Forward", "W");
        string backwardKey = PlayerPrefs.GetString("Backward", "S");
        string leftKey = PlayerPrefs.GetString("Left", "A");
        string rightKey = PlayerPrefs.GetString("Right", "D");
        string sprintKey = PlayerPrefs.GetString("Sprint", "LeftShift");
        string jumpKey = PlayerPrefs.GetString("Jump", "Space");

        Vector3 direction = Vector3.zero;

        // Détection du déplacement
        if (Input.GetKey(GetKeyCodeFromString(forwardKey))) direction += transform.forward;
        if (Input.GetKey(GetKeyCodeFromString(backwardKey))) direction -= transform.forward;
        if (Input.GetKey(GetKeyCodeFromString(leftKey))) direction -= transform.right;
        if (Input.GetKey(GetKeyCodeFromString(rightKey))) direction += transform.right;

        direction = direction.normalized;

        // Gestion du sprint
        bool isRunning = Input.GetKey(GetKeyCodeFromString(sprintKey)) && canRun;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Gestion du saut
        if (Input.GetKeyDown(GetKeyCodeFromString(jumpKey)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("isJumping", true);
        }

        // Appliquer le mouvement
        rb.velocity = new Vector3(direction.x * currentSpeed, rb.velocity.y, direction.z * currentSpeed);

        UpdateAnimator(direction, isRunning);
    }

    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivityX;
        rotationX += -Input.GetAxis("Mouse Y") * lookSensitivityY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        character.transform.Rotate(Vector3.up * mouseX);
    }

    void UpdateAnimator(Vector3 direction, bool isRunning)
    {
        animator.SetFloat("Sides", direction.x);
        animator.SetFloat("Front/Back", direction.z);
        animator.SetBool("isRunning", isRunning);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(animator.GetFloat("Sides"));
            stream.SendNext(animator.GetFloat("Front/Back"));
            stream.SendNext(animator.GetBool("isRunning"));
            stream.SendNext(animator.GetBool("isJumping"));
        }
        else
        {
            animator.SetFloat("Sides", (float)stream.ReceiveNext());
            animator.SetFloat("Front/Back", (float)stream.ReceiveNext());
            animator.SetBool("isRunning", (bool)stream.ReceiveNext());
            animator.SetBool("isJumping", (bool)stream.ReceiveNext());
        }
    }

    private KeyCode GetKeyCodeFromString(string key)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
    }

    public void Move(bool move)
    {
        canMove = move;
    }
}*/