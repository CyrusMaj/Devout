using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-06
/// 
/// <summary>
/// 
/// DEPRECATED
/// 
/// Character controller for level testers
/// This is for Development purpose only
/// Level testers use character with this scrip attached to test levels off-line
/// </summary>
public class LevelTesterMovementControl : CharacterMovementHandler
{
//	//	private PlayerMovementHandlerNetwork _PMHN;
//	// A reference to the ThirdPersonCharacter on the object
//	Transform _Cam;
//	// A reference to the main camera in the scenes transform
//	Vector3 _camForward;
//	// The current forward direction of the camera
//	Vector3 _move;
//	//	bool _jump;
//	float _inputH;
//	float _inputV;
//	bool _inputCrouch;
//	// the world-relative desired move direction, calculated from the camForward and user input.
//
//	bool _isTurnAllowed;
//	bool _isMovmentAllowed;
//	bool _isSprintAllowed;
//
//	float _turnSpeed = 270f;
//
//	public bool IsFlying{ get; private set; }
//
//	public bool IsSprinting{ get; private set; }
//
//	public bool IsAiming{ get; private set; }
//
//	protected override void Start ()
//	{
//		base.Start ();
//
//		// get the transform of the main camera
//		if (Camera.main != null) {
//			_Cam = Camera.main.transform;
//		} else {
//			Debug.LogWarning (
//				"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
//			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
//		}
//
//		_isTurnAllowed = true;
//		_isMovmentAllowed = true;
//		_isSprintAllowed = true;
//
//		IsFlying = false;
//		IsSprinting = false;
//		IsAiming = false;
//		//		_jump = false;
//		_inputCrouch = false;
//
//		SetLayerMask (LayerHelper.GetLayerMask (TEAM.ONE));
//	}
//
//	private void Update ()
//	{
//		//get movement direction input
//		_inputH = Input.GetAxis ("Horizontal");
//		_inputV = Input.GetAxis ("Vertical");
//
//		//restrict diagonal movement from being too fast
//		if (Mathf.Abs (_inputV) + Mathf.Abs (_inputH) > 1) {
//			_inputH = _inputH / (Mathf.Abs (_inputH) + Mathf.Abs (_inputV));
//			_inputV = _inputV / (Mathf.Abs (_inputH) + Mathf.Abs (_inputV));
//		}
//
//		//if hold leftshit, sprint
//		if (!Input.GetKey (KeyCode.LeftShift) || _inputCrouch || !_isSprintAllowed) {
//			_inputH *= 0.3f;
//			_inputV *= 0.3f;
//		}
//
//
//		//backward movement are slower
//		if (_inputV < 0)
//			_inputV *= 0.7f;
//
//		UpdateAnimator (_inputV, _inputH);
//
//		//			_jump = false;
//	}
//
//	void FixedUpdate ()
//	{
//		//			Move (_inputV, _inputH, _inputCrouch, _jump);
//		Move (_inputV, _inputH, _inputCrouch);
//
//		if (_inputV != 0 || _inputH != 0)
//			turnTowardsCamera ();
//	}
//
//	/// <summary>
//	/// allow/disallow turning
//	/// </summary>
//	/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
//	public void SetIsTurnAllowed (bool isAllowed)
//	{
//		_isTurnAllowed = isAllowed;
//	}
//
//	/// <summary>
//	/// allow/disallow movement
//	/// </summary>
//	/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
//	public void SetIsMovementAllowed (bool isAllowed)
//	{
//		_isMovmentAllowed = isAllowed;
//	}
//
//	/// <summary>
//	/// allow/disallow sprint movement
//	/// </summary>
//	/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
//	public void SetIsSprintAllowed (bool isAllowed)
//	{
//		_isSprintAllowed = isAllowed;
//	}
//
//	/// <summary>
//	/// Rotate towards camera using turnSpeed
//	/// </summary>
//	void turnTowardsCamera ()
//	{
//		if (_isTurnAllowed && _animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_GROUNDED) {
//			if (_Cam != null) {
//				_camForward = Vector3.Scale (_Cam.forward, new Vector3 (1, 0, 1)).normalized;
//			}
//			Quaternion toRotation = Quaternion.FromToRotation (transform.forward, _camForward);
//			if (Mathf.Abs (toRotation.y) > 0.05f) {
//				if (toRotation.y > 0) {
//					transform.Rotate (0f, _turnSpeed * Time.fixedDeltaTime, 0f);
//				} else {
//					transform.Rotate (0f, -_turnSpeed * Time.fixedDeltaTime, 0f);
//				}
//			} else
//				transform.LookAt (transform.position + _camForward);
//		}
//	}
}
