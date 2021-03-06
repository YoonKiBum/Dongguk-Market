using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {

    public PhotonView PV;
    public Rigidbody rb;
    private Transform tr;
    public float Velocity;
    [Space]

	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;
	public bool isGrounded;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    public float verticalVel;
    private Vector3 moveVector;
	public string nickName;
	public GameObject shop; // 구매 UI
    public GameObject register; // 판매 UI

	public NetworkManager manager;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();
		tr = GetComponent<Transform>();
        if (PV.IsMine)
            Camera.main.GetComponent<CamController>().player = tr.Find("CamPivot").transform;

		manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		register = GameObject.FindWithTag("Canvas").transform.GetChild(0).gameObject;
		shop = GameObject.FindWithTag("Canvas").transform.GetChild(1).gameObject;
	}

	// Update is called once per frame
	void Update () {
		if(PV.IsMine){
			InputMagnitude ();

			isGrounded = controller.isGrounded;
			if (isGrounded)
			{
				verticalVel -= 0;
			}
			else
			{
				verticalVel -= 1;
			}
			moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
			controller.Move(moveVector);
		}

    }

    void PlayerMoveAndRotation() {
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
		}
	}

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

	void InputMagnitude() {
		//Calculate Input Vectors
		InputX = Input.GetAxis ("Horizontal");
		InputZ = Input.GetAxis ("Vertical");

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player

		if (Speed > allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation ();
		} else if (Speed < allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
		}
	}
	void OnTriggerStay(Collider other){
		nickName=NetworkManager.t;
		
		if (other.tag==nickName){//shop 주인
			Debug.Log(nickName+" enter");
			if(Input.GetButtonDown("registers")){
				Debug.Log("e clicked");
				Debug.Log(register.name);
				register.SetActive(true);		
				
			}
			if(Input.GetKeyDown(KeyCode.Escape)){
				register.SetActive(false);
			}
		}
		Debug.Log("you can see this shop");
		if(Input.GetButtonDown("shops")){
			Debug.Log("r clicked");
			manager.setOwner(other.tag);
			shop.SetActive(true);
		}
		if(Input.GetKeyDown(KeyCode.Escape)){
			manager.setOwner(other.tag);
				shop.SetActive(false);
			}
		
	}
	void OnTriggerExit(Collider other){
		if(other.tag==nickName){
			manager.setOwner("");
			Debug.Log("shopping finished");
		}
	}
}