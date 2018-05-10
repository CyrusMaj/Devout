using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class TempPlayerMovementHandler : MonoBehaviour
{
	//		[SerializeField] float m_MovingTurnSpeed = 360;
	//		[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[SerializeField] float _maxMoveSpeed = 0.1f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	//		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	//		[SerializeField] float m_MoveSpeedMultiplier = 1f;
	//		[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	public bool m_IsGrounded;//private
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	//		float m_TurnAmount;
	//		float m_ForwardAmount;
	//		Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;
	//animator hash
	int _hashFoward;
	int _hashOnGround;
	int _hashStrafe;
	int _hashCrouch;
	int _hashJump;
	int _hashJumpLeg;
	LayerMask _layerMask;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

		_hashFoward = AnimationHashHelper.PARAM_FORWARD;
		_hashOnGround = AnimationHashHelper.PARAM_ON_GROUND;
		_hashStrafe = AnimationHashHelper.PARAM_STRAFE;
		_hashCrouch = AnimationHashHelper.PARAM_CROUCH;
		_hashJump = AnimationHashHelper.PARAM_JUMP;
		//			_hashJumpLeg = AnimationHashHelper.PARAM_JUMP_LEG;
		_layerMask = LayerHelper.TEAM_1_BODY; // ignore collisions with this layer
	}


	public void Move(float v, float h, bool crouch, bool jump)
	{
		CheckGroundStatus ();

		// control and velocity handling is different when grounded and airborne:
		if (m_IsGrounded)
		{
			HandleGroundedMovement(crouch, jump);
		}
		else
		{
			HandleAirborneMovement();
		}

		ScaleCapsuleForCrouching(crouch);
		//			PreventStandingInLowHeadroom();

		// send input and other state parameters to the animator
		UpdateAnimator(v,h);

		//backward movement are slower
		if (v < 0)
			v *= 0.7f;

		//move rigidbody
		m_Rigidbody.transform.Translate(_maxMoveSpeed * Vector3.forward * v);
		m_Rigidbody.transform.Translate(_maxMoveSpeed * Vector3.right * h);
	}

	void UpdateAnimator(float v, float h)
	{
		// update the animator parameters
		m_Animator.SetFloat(_hashFoward, v, 0.1f, Time.deltaTime);
		m_Animator.SetFloat(_hashStrafe, h, 0.1f, Time.deltaTime);
		m_Animator.SetBool(_hashCrouch, m_Crouching);
		m_Animator.SetBool(_hashOnGround, m_IsGrounded);

		if (!m_IsGrounded)
		{
			m_Animator.SetFloat(_hashJump, m_Rigidbody.velocity.y);
		}
		m_Animator.speed = 1;
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y <= 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}
	}

	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			Vector3 v = m_Animator.deltaPosition / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}

	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
		#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + m_Capsule.radius)), m_Capsule.radius/2, Vector3.down, out hitInfo, m_GroundCheckDistance,_layerMask))
		{
			//				m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			//				m_GroundNormal = Vector3.up;
			m_Animator.applyRootMotion = false;
		}
	}

	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
		}
		else
		{
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, _layerMask, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}
	}
}
