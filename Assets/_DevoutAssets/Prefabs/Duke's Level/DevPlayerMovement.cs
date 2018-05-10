
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Player movement control.
/// How player controls this character's movement
/// </summary>
[RequireComponent (typeof(CombatHandler))]
public class DevPlayerMovement : CharacterMovementHandler
{
	//	private PlayerMovementHandlerNetwork _PMHN;
	// A reference to the ThirdPersonCharacter on the object
	Transform _Cam;
	// A reference to the main camera in the scenes transform
	Vector3 _camForward;
	// The current forward direction of the camera
	Vector3 _move;
	bool _inputJump;
	float _inputH;
	float _inputV;
	bool _inputCrouch;
	// the world-relative desired move direction, calculated from the camForward and user input.

	bool _isTurnAllowed;
	bool _isMovmentAllowed;
	bool _isSprintAllowed;

	float _turnSpeed = 270f;

	public bool IsFlying{ get; private set; }

	public bool IsSprinting{ get; private set; }

	//	public bool IsAiming{ get; private set; }
	public bool IsAiming{ get; set; }
	//For Archer Ultimate ability
	public bool IsOverheadAiming{ get; set; }

	CombatHandler _ch;

	[SerializeField]
	bool _dev = false;

	//	public HangHandler HangHandler;

	protected override void Start ()
	{
		base.Start ();

		//		if (HangHandler)
		//			HangHandler = HangHandler;

		//default is walk for players
		_hsc.Walk ();

		_photonView = PhotonView.Get (this);

		if (_photonView.isMine || _dev) {
			// get the transform of the main camera
			if (Camera.main != null) {
				_Cam = Camera.main.transform;
			} else {
				Debug.LogWarning (
					"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
				// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
			}

			_isTurnAllowed = true;
			_isMovmentAllowed = true;
			_isSprintAllowed = true;
		}

		IsFlying = false;
		IsSprinting = false;
		IsAiming = false;
		IsOverheadAiming = false;
		_inputJump = false;
		_inputCrouch = false;

		_ch = GetComponent<CombatHandler> ();

		//dev
		//Disable apex behavior while player is in control
		//Enable when AI takes over
//		SteerableUnitComponent suc = GetComponent<SteerableUnitComponent> ();
//		if (suc != null) {
//			suc.enabled = false;
//		} else {
//			Debug.LogWarning ("WARNING : Steerable Unit Component not found");
//		}
		//		_rigidbody.useGravity = true;
		toggleWalkRun ();
	}

	private void Update ()
	{
		//dev
		//		print("free slot count : " + MeleeSlots.Where(x=>x.CHS == null).Count());

		//dev
		//		if (GetComponent<DevPlayerAIBehavior> () != null)
		//			return;

		if (_photonView.isMine || _dev) {
			//get movement direction input
			_inputH = Input.GetAxis ("Horizontal");
			_inputV = Input.GetAxis ("Vertical");
			//			Debug.Log ("U : " + _inputH + ", " + _inputV);

			//restrict diagonal movement from being too fast
			if (Mathf.Abs (_inputV) + Mathf.Abs (_inputH) > 1) {
				_inputH = _inputH / (Mathf.Abs (_inputH) + Mathf.Abs (_inputV));
				_inputV = _inputV / (Mathf.Abs (_inputH) + Mathf.Abs (_inputV));
			}

			//if hold leftshit, sprint, otherwise, walk
			if (Input.GetKey (KeyCode.LeftShift)) {
				if (_isRunning) {
					if (_hsc.GetPreferredSpeed (transform.forward) != _hsc.walkSpeed) {
						_hsc.Walk ();
					}
				} else {
					if (_hsc.GetPreferredSpeed (transform.forward) != _hsc.runSpeed) {
						_hsc.Run ();
					}
				}
			} else if (Input.GetKeyUp (KeyCode.LeftShift)) {
				if (_isRunning) {
					_hsc.Run ();
				} else {
					_hsc.Walk ();
				}
			}

			//toggle sprint
			if (Input.GetButtonUp (InputHelper.WALK_RUN_TOGGLE)) {
				toggleWalkRun ();
			}

			if (!_inputJump) {
				_inputJump = Input.GetButtonDown ("Jump");
			}

			//stop movement if restricted for reasons
			if (!_dev && (GameController.GC != null && !GameController.GC.GetIsControlAllowed () || !_isMovmentAllowed) /*
//				if (!GameController.GC.GetIsControlAllowed () || !_isMovmentAllowed
				//				|| !IsGrounded*/) {
				_inputV = 0f;
				_inputH = 0f;
				_inputCrouch = false;
				_inputJump = false;
				}


				//backward movement are slower
				if (_inputV < 0)
				_inputV *= 0.7f;

				UpdateAnimator (_inputV, _inputH);
				} 
				}

				void FixedUpdate ()
				{
					if (_photonView.isMine || _dev) {
						//			Move (_inputV, _inputH, _inputCrouch, _jump);
						Move (_inputV, _inputH, _inputCrouch, _inputJump, _checkGrounded);
						//			Debug.Log ("FU : " + _inputV + ", " + _inputH);
						_inputJump = false;

						if (_isTurnAllowed && !Hanging) {
							if (_inputV != 0 || _inputH != 0) {
								//allow turning while using some abilities
								turnTowardsCamera ();
							} else if (_ch.CheckAbilitiesInUse ()) {
								turnTowardsCamera ();
							}
						}
					}

					//limit rigidbody velocity
					if (_rigidbody.velocity.magnitude > 15f)
						_rigidbody.velocity = _rigidbody.velocity.normalized * 15f;
				}

				/// <summary>
				/// allow/disallow turning
				/// </summary>
				/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
				public void SetIsTurnAllowed (bool isAllowed)
				{
					_isTurnAllowed = isAllowed;
				}

				/// <summary>
				/// allow/disallow movement
				/// </summary>
				/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
				public void SetIsMovementAllowed (bool isAllowed)
				{
					_isMovmentAllowed = isAllowed;
				}

				public bool GetIsMovementAllowed ()
				{
					return _isMovmentAllowed;
				}

				/// <summary>
				/// allow/disallow sprint movement
				/// </summary>
				/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
				public void SetIsSprintAllowed (bool isAllowed)
				{
					_isSprintAllowed = isAllowed;
				}

				/// <summary>
				/// Rotate towards camera using turnSpeed
				/// </summary>
				void turnTowardsCamera ()
				{
					//		if (_isTurnAllowed 	&& _animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_GROUNDED) {
					//		if (_isTurnAllowed && !Hanging) {
					if (_Cam != null) {
						_camForward = Vector3.Scale (_Cam.forward, new Vector3 (1, 0, 1)).normalized;
					}
					Quaternion toRotation = Quaternion.FromToRotation (transform.forward, _camForward);
					//		transform.LookAt (transform.position + _camForward);
					//		Debug.Log (_rigidbody.velocity.ToString ());

					if (Mathf.Abs (toRotation.y) > 0.05f) {
						if (toRotation.y > 0) {
							transform.Rotate (0f, _turnSpeed * Time.fixedDeltaTime, 0f);
						} else {
							transform.Rotate (0f, -_turnSpeed * Time.fixedDeltaTime, 0f);
						}
					} else {
						transform.LookAt (transform.position + _camForward);
					}

					//		}
				}
				}
