using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Written by Duke Im
/// On 2017-03-08
/// 
/// <summary>
/// Cooperative ability where character jump on & ride another character
/// </summary>
public class AbilityCoopRide : AbilityAnimationAttack
{
	/// <summary>
	/// Target to jump over
	/// </summary>
	Transform _target;

	public bool Riding{ get; private set; }

	//	//Original RCB
	//	Collider _RCB;
	Rigidbody _rigidbody;

	float _jumpForce = 30f;

	//	override
	//	void Start(){
	//		_status = ABILITY_STATUS.UNAVAILABLE;
	//	}
	protected override void Start ()
	{
		base.Start ();
		_status = ABILITY_STATUS.UNAVAILABLE;
		Riding = false;
	}

	public void FallDown ()
	{
		//remove this archer from tank
		_target.GetComponent<TankCombatHandler>().SetArcherRiding(null);

		//		Riding = false;
//		StartCoroutine (CoroutineHelper.IEChangeBool ((x) => Riding = x, false, _animTime1));
		Riding = false;

		_rigidbody.isKinematic = false;
		_rigidbody.GetComponent<Collider> ().enabled = true;	
		_rigidbody.velocity = Vector3.zero;
		((ArcherCombatHandler)_combatHandler).SetRide (false);

//		_rigidbody.AddForce (_rigidbody.transform.up * 1f * _jumpForce);
//		_rigidbody.AddForce (-_rigidbody.transform.forward * 1f * _jumpForce);

		_combatHandler.GetComponent<AIMovementHandler> ().SetCheckGrounded (true);
//		coolDown (_coolDown);

		StartCoroutine (MathHelper.IELerpRBPositionOverTime (_rigidbody, _rigidbody.transform.position, _rigidbody.transform.position - _rigidbody.transform.forward, 0.5f));

//		print ("falling down");
		unlockControls ();
		//		_rigidbody.AddForce (-_rigidbody.transform.forward * 3f * _jumpForce);
//		StartCoroutine (MathHelper.IELerpPositionOverTime (_rigidbody, _rigidbody.position, _rigidbody.position - _target.transform.forward * 0.25f, 1f));

		_status = ABILITY_STATUS.UNAVAILABLE;
	}

	void Update ()
	{
		if (Riding) {
			_rigidbody.transform.position = _target.transform.position + _target.transform.up * 1.5f;
			lockControls ();
		}

//		print (_status.ToString ());
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{	
		//save this archer to tank
		_target.GetComponent<TankCombatHandler>().SetArcherRiding(_combatHandler);
		
		if (!_combatHandler.GetComponent<AIMovementHandler> ().IsGrounded)
			return;

		//		base.Activate ();

		if (_aimAssist) {
			assistAim (0.2f);
		}
		if (_positionAssist) {
			assistPosition (0.3f);
		}
		//lock player controls during activation
		//must be unlocked in child(better design?)
		lockControls ();
		if (_weapon)
			_weapon.SetDamage (_damage);
		_animator.SetTrigger (_animHash);
		//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay));
		ActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay);
		checkInUse (_animTime1);

		SetStatus (ABILITY_STATUS.IN_USE);

		//clear any remaining force
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;

		//move to the right position
		Vector3 disposition = transform.position - _target.position;
		//		disposition = disposition.normalized * 1f;
		_animator.transform.position = _target.position + disposition;

		//		Debug.Break ();

		//look at the target before jumping
		//		_animator.transform.LookAt(_target);
		_animator.transform.LookAt (new Vector3 (_target.position.x, _combatHandler.transform.position.y, _target.position.z));

//		_rigidbody.GetComponent<Collider> ().enabled = false;
		((ArcherCombatHandler)_combatHandler).SetRide (true);

		StartCoroutine (IELerpPositionOverTime (_rigidbody, _rigidbody.transform, _target.transform, _target.transform.up * 1.5f, _animTime1));
//		StartCoroutine (MathHelper.IELerpPositionOverTime (_rigidbody, _rigidbody.position, _target.transform + _target.transform * 1.5f, _animTime1));
		//apply force and change direction mid-action
		StartCoroutine (IEMidJump (_animTime1));
		//		StartCoroutine (IEFollowTarget (_animTime1));

		//finetune
		//initial up & forward force towards the target
		//		_rigidbody.AddForce (Vector3.up * 3f * _jumpForce);
		//		_rigidbody.AddForce (Vector3.forward * 6f * _jumpForce);
//		_rigidbody.AddForce (_rigidbody.transform.up * 3f * _jumpForce);
//		_rigidbody.AddForce (_rigidbody.transform.forward * 3f * _jumpForce);

	}

	public static IEnumerator IELerpPositionOverTime (Rigidbody lerpTarget, Transform posA, Transform posB, Vector3 offset, float seconds)
	{
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.position = Vector3.Lerp (posA.position, posB.position + offset, percentage);
			yield return null;
		}
		lerpTarget.position = posB.position;
	}

	//	IEnumerator IEFollowTarget(float seconds){
	//		Vector3 offset = _target.transform.position - _rigidbody.transform.position;
	//		float timer = Time.time + seconds;
	//		while (timer > Time.time) {
	//			_rigidbody.transform.position = _target.transform.position + offset;
	//			yield return null;
	//		}
	//	}

	IEnumerator IEMidJump (float seconds)
	{
		yield return new WaitForSeconds (seconds);
		//snap to riding position
//		_rigidbody.isKinematic = true;

		Riding = true;

//		_animator.SetBool (AnimationHashHelper.PARAM_ON_GROUND, true);

//		//		_animator.transform.localEulerAngles = new Vector3(_animator.transform.localEulerAngles.x, _target.localEulerAngles.y ,_animator.transform.localEulerAngles.z);
//		_animator.transform.localEulerAngles = _target.localEulerAngles;
//		//clear any remaining force
//		_rigidbody.velocity = Vector3.zero;
//		_rigidbody.angularVelocity = Vector3.zero;
//		//
//		//		//this is to solve force added before turning
//		//		yield return new WaitForSeconds (0.1f);
//
//		_rigidbody.AddForce (_target.up * 3.5f * _jumpForce);
//		_rigidbody.AddForce (_target.forward * 3f * _jumpForce);
	}

	public void SetTarget (Transform newTarget)
	{
		_target = newTarget;
	}

	protected override IEnumerator IECheckInUse (float seconds)
	{
		//return base.IECheckInUse (seconds);
		_status = ABILITY_STATUS.IN_USE;

		//network sometimes plays falling animation before jump, so disable falling animation for the duration
		_combatHandler.GetComponent<AIMovementHandler> ().SetCheckGrounded (false);

		//Network
		//set RCB to clip through other bodies
		_combatHandler.ChangeRCBLayer (LayerHelper.COMMON_BODY_GHOST);
		//		_RCB.gameObject.layer = LayerHelper.COMMON_BODY_GHOST;

		//		Debug.Break ();
		yield return new WaitForSeconds (seconds);
//		_status = ABILITY_STATUS.UNAVAILABLE;
		//Network
		//set RCB back to normal
		_combatHandler.ChangeRCBLayer (LayerHelper.COMMON_BODY);
		//		_RCB.gameObject.layer = LayerHelper.COMMON_BODY;
		//		Debug.Break ();
//		unlockControls ();

//		_combatHandler.GetComponent<PlayerMovementControl> ().SetCheckGrounded (true);

		//		Debug.Break ();
	}

	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);
		_rigidbody = ch.GetComponent<Rigidbody> ();
	}

}
