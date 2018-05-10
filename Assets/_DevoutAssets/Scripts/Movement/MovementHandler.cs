using UnityEngine;
using Apex.Steering.Components;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Character movement handler.
/// Provides methods for character movements
/// </summary>
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(HumanoidSpeedComponent))]
public class CharacterMovementHandler : Photon.PunBehaviour
{
	[SerializeField] float m_JumpPower = 12f;

	//dev
	//	[SerializeField] protected float _maxMoveSpeed = 6f;
	//	[Range(1f, 4f)] float m_GravityMultiplier = 2f;
	float _groundCheckDistance = 1.0f;

	protected Rigidbody _rigidbody;
	protected PhotonView _photonView;
	protected Animator _animator;

	[HideInInspector] public bool IsGrounded;

	float _origGroundCheckDistance;
	const float k_Half = 0.5f;
	float _capsuleHeight;
	Vector3 _capsuleCenter;
	CapsuleCollider _capsule;
	bool _crouching;

	//animator hash
	int _hashFoward;
	int _hashOnGround;
	int _hashStrafe;
	int _hashCrouch;
	int _hashJump;
	int _hashJumpLeg;
	LayerMask _layerMask;

	//hanging
	int _hashStateHangIdle;
	//	[HideInInspector]public bool Hanging;
	[HideInInspector]public bool Hanging{ get; private set; }

	protected HangHandler _hangHandler;
	[Range (1f, 4f)]float gravityMultiplier = 2.0f;

	//Rope wip
	[HideInInspector]public bool Roped = false;

	//Apex component. Gets speed from this.
	protected HumanoidSpeedComponent _hsc;
	protected bool _isRunning = false;


	/// <summary>
	/// Does this character check grounded status?
	/// Disables while using jumping abilities
	/// </summary>
	protected bool _checkGrounded = true;

	/// <summary>
	/// Sets _checkGrounded
	/// </summary>
	/// <param name="newBool">If set to <c>true</c> new bool.</param>
	public void SetCheckGrounded (bool newBool)
	{
		_checkGrounded = newBool;
	}

	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected (newPlayer);
		SetHanging (Hanging);
	}

	[PunRPC]
	protected void RPCSetHanging (bool newHanging)
	{
		setHanging (newHanging);
	}

	public void SetHanging (bool newHanging)
	{
		if (PhotonNetwork.offlineMode)
			setHanging (newHanging);
		else {
			photonView.RPC ("RPCSetHanging", PhotonTargets.All, newHanging);
		}
	}

	void setHanging (bool newHanging)
	{
		Hanging = newHanging;
	}

	[PunRPC]
	protected void RPCHangToDrop ()
	{
		hangToDrop ();
	}

	public void HangToDrop ()
	{
		if (PhotonNetwork.offlineMode)
			hangToDrop ();
		else
			photonView.RPC ("RPCHangToDrop", photonView.owner);		
	}

	void hangToDrop ()
	{
		_hangHandler.HangToDrop ();
	}

	/// <summary>
	/// Sets the layer mask.
	/// </summary>
	/// <param name="newMask">New mask.</param>
	public void SetLayerMask (LayerMask newMask)
	{
		_layerMask = newMask;		
	}

	protected virtual void Start ()
	{
		_photonView = PhotonView.Get (this);
		_animator = GetComponent<Animator> ();
		_rigidbody = GetComponent<Rigidbody> ();
		_capsule = GetComponent<CapsuleCollider> ();
		_hsc = GetComponent<HumanoidSpeedComponent> ();
		_hangHandler = GetComponentInChildren<HangHandler> ();

		if (!_hangHandler) {
//			print ("HangHandler not found");
		}
		_capsuleHeight = _capsule.height;
		_capsuleCenter = _capsule.center;

		_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		_origGroundCheckDistance = _groundCheckDistance;

		_hashFoward = AnimationHashHelper.PARAM_FORWARD;
		_hashOnGround = AnimationHashHelper.PARAM_ON_GROUND;
		_hashStrafe = AnimationHashHelper.PARAM_STRAFE;
		_hashCrouch = AnimationHashHelper.PARAM_CROUCH;
		_hashJump = AnimationHashHelper.PARAM_JUMP;
		_hashStateHangIdle = AnimationHashHelper.STATE_HANG_IDLE;
	}

	public void Move (float v, float h, bool crouch, bool jump, bool checkGrounded = true)
	{
		if (checkGrounded)
			CheckGroundStatus ();

		// control and velocity handling is different when grounded and airborne:
		if (!IsGrounded) {
			HandleAirborneMovement (v, h);
		} else {
			HandleGroundedMovement (v, h, crouch, jump);
		}
		ScaleCapsuleForCrouching (crouch);

		//don't move when handing
		if (Hanging && _hangHandler) {
			if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == _hashStateHangIdle) {
//				print ("State check passed");
				if (jump) {//drop from hanging
					_hangHandler.HangToDrop ();
//					print ("dropping");
				} else if (v > 0) {//climb from hanging
					_hangHandler.HangToClimb ();
//					print ("climbing");
				}
			} else {
//				print ("State check failed");
			}
		} else {
//			print (Hanging.ToString () + ", " + _hangHandler.ToString ());

			//slow or stop air movement
			if (!IsGrounded) {
				//adjust velocity to move around while in air
				//todo : clamp & moveposition
//				Debug.Log ("Not grounded");
			} else {
//				Debug.Log ("moving, amount : " + (_rigidbody.position + _hsc.GetPreferredSpeed (transform.forward) * transform.forward * v * Time.fixedDeltaTime).magnitude.ToString ());
				//move rigidbody
				transform.position += _hsc.GetPreferredSpeed (transform.forward) * transform.forward * v * Time.fixedDeltaTime;
				transform.position += _hsc.GetPreferredSpeed (transform.right) * transform.right * h * Time.fixedDeltaTime;
//				_rigidbody.MovePosition (_rigidbody.position + _hsc.GetPreferredSpeed (transform.forward) * transform.forward * v * Time.fixedDeltaTime);
//				_rigidbody.MovePosition (_rigidbody.position + _hsc.GetPreferredSpeed (transform.right) * transform.right * h * Time.fixedDeltaTime);
			}
//			_rigidbody.transform.Translate (_hsc.GetPreferredSpeed (transform.forward) * Vector3.forward * v * Time.fixedDeltaTime);
//			_rigidbody.transform.Translate (_hsc.GetPreferredSpeed (transform.right) * Vector3.right * h * Time.fixedDeltaTime);
		}
	}

	public void UpdateAnimator (float v, float h)
	{
		// update the animator parameters
		_animator.SetFloat (_hashFoward, v * _hsc.GetPreferredSpeed (transform.forward) / _hsc.runSpeed, 0.1f, Time.deltaTime);

//		if (!_isRunning)
//			_animator.SetFloat (_hashStrafe, h * _hsc.walkSpeed / _hsc.strafeMaxSpeed, 0.1f, Time.deltaTime);
//		else
		_animator.SetFloat (_hashStrafe, h * _hsc.GetPreferredSpeed (transform.right) / _hsc.strafeMaxSpeed, 0.1f, Time.deltaTime);
			
//		_animator.SetFloat(_hashFoward, v, 0.1f, Time.deltaTime);
//		_animator.SetFloat(_hashStrafe, h, 0.1f, Time.deltaTime);
		_animator.SetBool (_hashCrouch, _crouching);
		_animator.SetBool (_hashOnGround, IsGrounded);

		if (!IsGrounded) {
			_animator.SetFloat (_hashJump, _rigidbody.velocity.y);
//			_animator.SetFloat (_hashJump, 3f);
		}
		_animator.speed = 1;
	}

	protected void HandleAirborneMovement (float v, float h)
	{
		// apply extra gravity from multiplier:
		if (!Hanging && !Roped) {
			_rigidbody.AddForce (Physics.gravity * _rigidbody.mass * 2f);
//			print ("HERE_2");
//			Vector3 localVel = transform.InverseTransformDirection (_rigidbody.velocity);
//			localVel.x = h;
//			localVel.z = v;
//			localVel = transform.TransformDirection (localVel);
//			if(_rigidbody.velocity.magnitude < 1f)
//				_rigidbody.velocity += new Vector3 (localVel.x, 0f, localVel.z);
		} else {
			_rigidbody.AddForce (Physics.gravity * _rigidbody.mass * 2f);
		}
		_groundCheckDistance = _rigidbody.velocity.y <= 0 ? _origGroundCheckDistance : 0.01f;
	}

	protected void HandleAirborneMovement (Vector3 vel)
	{
		// apply extra gravity from multiplier:
		if (!Hanging && !Roped) {
//			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier * _rigidbody.mass);
//			_rigidbody.AddForce (extraGravityForce);
//			if(_rigidbody.velocity.y > -20f)
//				_rigidbody.velocity -= new Vector3 (0f, 0.3f, 0f);
//			print ("applying gravity");
			_rigidbody.AddForce (Physics.gravity * _rigidbody.mass * 2f);
//			print ("HERE");
//			Vector3 localVel = transform.InverseTransformDirection (_rigidbody.velocity);
//			localVel.x = vel.x;
//			localVel.z = vel.z;
//			localVel = transform.TransformDirection (localVel);
//			_rigidbody.velocity += new Vector3 (localVel.x, 0f, localVel.z);
		} else {
//			print ("THERE");
//						_rigidbody.AddForce (Physics.gravity);
			//
//			if (Roped) {
//				Vector3 extraGravityForce = (Physics.gravity * 3.0f * _rigidbody.mass);
//				_rigidbody.AddForce (extraGravityForce);
			//			} else {
			_rigidbody.AddForce (Physics.gravity * _rigidbody.mass * 2f);
//			}
			//			print ("not applying gravity : " + _rigidbody.velocity.ToString ());
		}
		_groundCheckDistance = _rigidbody.velocity.y <= 0 ? _origGroundCheckDistance : 0.01f;
	}

	void HandleGroundedMovement (float v, float h, bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && _animator.GetCurrentAnimatorStateInfo (0).IsName ("Grounded")) {
			//apply movement speed into velocity when jump
			//local velocity conversion
			Vector3 localVel = transform.InverseTransformDirection (_rigidbody.velocity);
			localVel.x = h * _hsc.GetPreferredSpeed (transform.right);
			localVel.z = v * _hsc.GetPreferredSpeed (transform.forward);
			localVel = transform.TransformDirection (localVel);

			// jump!
			_rigidbody.velocity = new Vector3 (
				localVel.x
				, m_JumpPower
				, localVel.z);
			
			IsGrounded = false;
			_animator.applyRootMotion = false;
			_groundCheckDistance = 0.1f;
		} 
//		else
//			print ("WHERE?");

		//apply normal gravity
		_rigidbody.AddForce (Physics.gravity);

		//apply airborne momentum
//		v * _hsc.GetPreferredSpeed (transform.forward)
//		h * _hsc.GetPreferredSpeed (transform.right)
//		_rigidbody.velocity = new Vector3(v * _hsc.GetPreferredSpeed (transform.forward),);
	}

	//	public void OnAnimatorMove ()
	//	{
	//		// we implement this function to override the default root motion.
	//		// this allows us to modify the positional speed before it's applied.
	//		if (IsGrounded && Time.deltaTime > 0) {
	//			Vector3 v = _animator.deltaPosition / Time.deltaTime;
	//
	//			// we preserve the existing y part of the current velocity.
	//			v.y = _rigidbody.velocity.y;
	//			_rigidbody.velocity = v;
	//		}
	//	}

	protected void CheckGroundStatus ()
	{
//		print ("ground check is called");
//		Debug.Break ();

		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine (transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * _groundCheckDistance));
		#endif

//		print(_layerMask.value + ", " + LayerHelper.TEAM_5_BODY);

		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character

		//dev layermask
		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL | 1 << LayerHelper.COMMON_BODY | 1 << LayerHelper.NEAUTRAL | 1 << LayerHelper.ROPE;

//		if (Physics.SphereCast (transform.position + (Vector3.up * (0.1f + _capsule.radius)), _capsule.radius / 2, Vector3.down, out hitInfo, _groundCheckDistance, _layerMask)) {
		if (Physics.SphereCast (transform.position + (Vector3.up * (0.1f + _capsule.radius)), _capsule.radius / 2, Vector3.down, out hitInfo, _groundCheckDistance, lm)) {
//		if (Physics.SphereCast (transform.position + (Vector3.up * 0.5f), 0.3f, Vector3.down, out hitInfo, _groundCheckDistance, _layerMask)) {
			if (hitInfo.collider.gameObject != gameObject)
				IsGrounded = true;
			else {
				IsGrounded = false;
			}
//				_animator.applyRootMotion = true;

//			if (this.transform == GameController.GC.CurrentPlayerCharacter)
//				print ("hit something : " + hitInfo.collider.name);
		} else {
//			print ("hit nothing");
			IsGrounded = false;
//			if (this.transform == GameController.GC.CurrentPlayerCharacter)
//				print ("hit nothing");
//				m_GroundNormal = Vector3.up;
//			_animator.applyRootMotion = false;
		}

//		print (IsGrounded.ToString ());

//		if (!IsGrounded && Roped) {
//			_capsule.height = _capsule.height / 2f;
//			_capsule.center = _capsule.center / 2f;
//		} else {
//			_capsule.height = _capsuleHeight;
//			_capsule.center = _capsuleCenter;
//		}
	}

	void OnCollisionStay (Collision col)
	{
		if (IsGrounded)
			return;
		
		if (
//			col.contacts.ToList ().Where (x => x.point.y <= transform.position.y).Count () > 1
//			&&
			(col.collider.gameObject.layer == LayerHelper.WALL || col.collider.gameObject.layer == LayerHelper.FLOOR
			)) {
			Bounds b = col.collider.bounds;
//			print ("collided : " + col.collider.name);
			//			if (b.max.y >= transform.position.y && b.min.x < transform.position.x && b.max.x > transform.position.x && b.min.z < transform.position.z && b.max.z > transform.position.z) {
//			if (this.transform == GameController.GC.CurrentPlayerCharacter)
//				print ("minz : " + b.min.z + ", maxz : " + b.max.z + ", " + transform.position.z);
			if (b.max.y >= transform.position.y && b.min.x - 0.1f <= transform.position.x && b.max.x + 0.1f >= transform.position.x && b.min.z - 0.1f <= transform.position.z && b.max.z + 0.1f >= transform.position.z) {
				IsGrounded = true;
//				if (this.transform == GameController.GC.CurrentPlayerCharacter)
//					print (col.collider.name);
			}
		}
	}

	protected void ScaleCapsuleForCrouching (bool crouch = false)
	{
		if (IsGrounded && crouch) {
			if (_crouching)
				return;
			_capsule.height = _capsule.height / 2f;
			_capsule.center = _capsule.center / 2f;
			_crouching = true;
		} else {
			_capsule.height = _capsuleHeight;
			_capsule.center = _capsuleCenter;
			_crouching = false;
		}
	}

	void PreventStandingInLowHeadroom ()
	{
		// prevent standing up in crouch-only zones
		if (!_crouching) {
			Ray crouchRay = new Ray (_rigidbody.position + Vector3.up * _capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = _capsuleHeight - _capsule.radius * k_Half;
			if (Physics.SphereCast (crouchRay, _capsule.radius * k_Half, crouchRayLength, _layerMask, QueryTriggerInteraction.Ignore)) {
				_crouching = true;
			}
		}
	}

	/// <summary>
	/// Rotate towards position
	/// </summary>
	public void TurnTowardsPos (Vector3 targetPos)
	{
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_GROUNDED) {
			Vector3 direction = targetPos - transform.position;
			direction = new Vector3 (direction.x, 0f, direction.z);
			transform.LookAt (transform.position + direction);
		}
	}

	/// <summary>
	/// Turns the towards mouse position.
	/// </summary>
	/// <param name="seconds">speed; seconds take to rotate</param>
	public void TurnTowardsMousePos (float seconds)
	{
		Vector3	camForward = Vector3.Scale (CameraController.CC.CombatCamera.transform.forward, new Vector3 (1, 0, 1)).normalized;
		Vector3 lookPosition = transform.position + camForward;
		Vector3 direction = lookPosition - transform.position;
		direction = new Vector3 (direction.x, 0f, direction.z);
		Quaternion toRotation = Quaternion.LookRotation (direction);
		StartCoroutine (MathHelper.IELerpRotationOverTime (transform, transform.rotation, toRotation, seconds)); 
	}

	/// <summary>
	/// if running set to walking and vice versa
	/// </summary>
	protected void toggleWalkRun ()
	{
		if (_hsc.GetPreferredSpeed (transform.forward) > _hsc.walkSpeed) {
			_hsc.Walk ();
			_isRunning = false;
		} else {
			_hsc.Run ();
			_isRunning = true;
		}
//		print ("WALK/RUN Toggled, current speed : " + _hsc.GetPreferredSpeed (Vector3.forward));
	}
}
