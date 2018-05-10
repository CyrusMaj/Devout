using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof(CharacterMovementHandler))]
public class TempPlayerMovementControl : MonoBehaviour
{
	private CharacterMovementHandler _PMH;
	// A reference to the ThirdPersonCharacter on the object
	private Transform _Cam;
	// A reference to the main camera in the scenes transform
	private Vector3 _camForward;
	// The current forward direction of the camera
	private Vector3 _move;

	//Jump
	private bool _jump;
	[SerializeField] float m_JumpPower = 12f;

	// the world-relative desired move direction, calculated from the camForward and user input.
	bool _isTurnAllowed;
	bool _isMovmentAllowed;

	Animator _animator;

	private void Start ()
	{
		if (GetComponent<Animator> () != null)
			_animator = GetComponent<Animator> ();
		else
			print ("WARNING : Animator not loaded");

		// get the transform of the main camera
		if (Camera.main != null) {
			_Cam = Camera.main.transform;
		} else {
			Debug.LogWarning (
				"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
		}
		// get the third person character ( this should never be null due to require component )
		_PMH = GetComponent<CharacterMovementHandler> ();

		_isTurnAllowed = true;
	}

	private void Update ()
	{
		if (!_jump) {
			_jump = CrossPlatformInputManager.GetButtonDown ("Jump");
		}
	}

	// Fixed update is called in sync with physics
	private void FixedUpdate ()
	{
		float h = CrossPlatformInputManager.GetAxis ("Horizontal");
		float v = CrossPlatformInputManager.GetAxis ("Vertical");
		bool crouch = CrossPlatformInputManager.GetButton ("Crouch");

		if (_Cam != null) {
			_camForward = Vector3.Scale (_Cam.forward, new Vector3 (1, 0, 1)).normalized;
		}

		if (_isTurnAllowed && _animator.GetCurrentAnimatorStateInfo(0).fullPathHash == AnimationHashHelper.STATE_GROUNDED) {
			//rotate character to where camera is looking
			_PMH.GetComponent<Transform> ().LookAt (_PMH.transform.position + _camForward);
		}

		//restrict diagonal movement from being too fast
		if (Mathf.Abs (v) + Mathf.Abs (h) > 1) {
			h = h / (Mathf.Abs (h) + Mathf.Abs (v));
			v = v / (Mathf.Abs (h) + Mathf.Abs (v));
		}

		if (Input.GetKey (KeyCode.LeftShift) || crouch) {
			h *= 0.3f;
			v *= 0.3f;
		}

		//			print ("h : " + h + ", v : " + v);

		if (!GameController.GC.GetIsControlAllowed () 
			|| (_animator.GetCurrentAnimatorStateInfo(0).fullPathHash != AnimationHashHelper.STATE_GROUNDED && _PMH.IsGrounded)
		) {
			v = 0f;
			h = 0f;
			crouch = false;
			_jump = false;
		}

		_PMH.Move (v, h, crouch, _jump);

		_jump = false;
	}
	public void SetIsTurnAllowed(bool isAllowed){
		_isTurnAllowed = isAllowed;
	}
}
