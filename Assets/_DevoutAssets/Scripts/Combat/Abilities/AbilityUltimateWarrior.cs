using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
///
/// <summary>
/// Ability ultimate warrior.
/// "Spin to win"
/// </summary>
public class AbilityUltimateWarrior : AbilityAnimationAttack
{
	/// <summary>
	/// The rotation speed.
	/// </summary>
	[SerializeField] float _rotationSpeed = 1f;
	/// <summary>
	/// The movement speed.
	/// </summary>
	[SerializeField] float _movementSpeed = 5f;

	//input values for movement while active
	float _inputH;
	float _inputV;

	Rigidbody _rigidBody;

	//dev only, not networked
//	public GameObject ParticleEffect;

	protected override void Start ()
	{
		
		base.Start ();
		//unlike other abilities, ultimates are only available when ultimate guage is full
		_status = ABILITY_STATUS.UNAVAILABLE;

		//dev
//		_status = ABILITY_STATUS.AVAILABLE;
	}

	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);

		_rigidBody = ch.GetComponent<Rigidbody> ();
		_rigidBody.maxAngularVelocity = 15f;
	}

	void Update ()
	{
		if (!_combatHandler.GetComponent<PhotonView> ().isMine)
			return;

		if (_status != ABILITY_STATUS.IN_USE)
			return;

		//Move while spinning
		_inputH = Input.GetAxis ("Horizontal");
		_inputV = Input.GetAxis ("Vertical");
	}

	void FixedUpdate ()
	{
		if (!_combatHandler.GetComponent<PhotonView> ().isMine)
			return;

		if (_status != ABILITY_STATUS.IN_USE)
			return;

		//Move while spinning
		Rigidbody rigidBody = _combatHandler.GetComponent<Rigidbody> ();
		rigidBody.transform.Translate (_movementSpeed * Vector3.forward * _inputV * Time.fixedDeltaTime, CameraController.CC.CombatCamera.transform);
		rigidBody.transform.Translate (_movementSpeed * Vector3.right * _inputH * Time.fixedDeltaTime, CameraController.CC.CombatCamera.transform);
	}

	/// <summary>
	/// Activate / Use this instance.
	/// </summary>
	public override void Activate ()
	{
		if (_status == ABILITY_STATUS.AVAILABLE) {
			//
			//Ability base code
			//
			//stop player from turning during attack
			if (_weapon.GetOwner ().GetComponent<PlayerMovementControl> () != null) {
				PlayerMovementControl pmc = _weapon.GetOwner ().GetComponent<PlayerMovementControl> ();
				if (!_isTurnAllowed)
					pmc.SetIsTurnAllowed (false);
				if (!_isMovmentAllowed)
					pmc.SetIsMovementAllowed (false);
				if (!_isSprintAllowed)
					pmc.SetIsSprintAllowed (false);
			}

			//
			//AbilityAnimationAttack base code with changes
			//
			_weapon.SetDamage (_damage);
			_animator.SetBool (_animHash, true);
			StartCoroutine (CoroutineHelper.IEChangeAnimBool (_animator, _animHash, false, _animTime1));

//			Vector3 targetPos;
//			if (_aimAssist && _targetFound) {
//				targetPos = _animator.transform.position + ((_targetPos - _animator.transform.position).normalized * _movementForward);
//			} else {
//				targetPos = _animator.transform.position + (_animator.transform.forward * _movementForward);
//			}
//			StartCoroutine (MathHelper.IELerpPositionOverTime (_animator.transform, _animator.transform.position, targetPos, 0.5f));

			//
			//AbilityUltimateWarrior code
			//
//			_combatHandler.ResetUltimateGuage ();
			_combatHandler.SetUltimatePoint(0);
//			StartCoroutine (IEPositionWeapon (_animTime1));
			_combatHandler.PositionUltimateWeapon (_animTime1);
//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, 0.25f));
			ActivateDamagingPoint(_weapon, _animTime1, 0.25f);
			StartCoroutine (IECheckInUse (_animTime1));
			StartCoroutine (IEUnlockControls (_coolDown));
			StartCoroutine (IERotate (_animTime1, _rotationSpeed));
			StartCoroutine (IESetRigidbodyRotYConstraint (_animTime1));
			foreach (var dp in _weapon.GetDamagingPoints())
				StartCoroutine (DamagingPoint.IESetOneTimeDamage (dp, _animTime1, false));

			//particle
//			ParticleController.PC.InstantiateParticle(ParticleController.PARTICLE_TYPE.WARRIOR_ULT_TORNADO, _combatHandler.transform.position, 0f, _animTime1, _combatHandler.transform);

		} else {
		}
	}

	/// <summary>
	/// Set rigidbody constrainst over duration, then turns it back to normal
	/// </summary>
	/// <returns>The set rigidbody rot Y constraint.</returns>
	/// <param name="seconds">Seconds.</param>
	IEnumerator IESetRigidbodyRotYConstraint (float seconds)
	{
		RigidbodyConstraints originalConsts = _weapon.GetOwner ().GetComponent<Rigidbody> ().constraints;

		//dev
		//network sync fix
//		_weapon.GetOwner().GetComponent<PhotonTransformView>()

		//freeze X and Z rotations
		_weapon.GetOwner ().GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		yield return new WaitForSeconds (seconds);

		//return to original
		_weapon.GetOwner ().GetComponent<Rigidbody> ().constraints = originalConsts;
	}

	/// <summary>
	/// Rotate character
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="seconds">Seconds.</param>
	/// <param name="rotationSpeed">Rotation speed.</param>
	IEnumerator IERotate (float seconds, float rotationSpeed)
	{
		//
		
		float timer = Time.time + seconds;
//		print ((new Vector3 (0f, -3f, 0f) * _rotationSpeed).magnitude.ToString ());
		while (Time.time < timer) {
			_rigidBody.angularVelocity = new Vector3 (0f, -3f, 0f) * _rotationSpeed;
//			print ("angularVel : " + rigidBody.angularVelocity.ToString ());
			yield return new WaitForFixedUpdate ();
//			_rigidBody.angularVelocity = Vector3.zero;
//			print ("angularVel : " + _rigidBody.angularVelocity.ToString () + ", " + new Vector3 (0f, -3f, 0f) * _rotationSpeed);
		}

		//

		_rigidBody.angularVelocity = Vector3.zero;
	}
}

