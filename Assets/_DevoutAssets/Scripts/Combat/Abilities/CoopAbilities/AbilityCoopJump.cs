using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-23
/// 
/// <summary>
/// Cooperative ability where character jump over another character
/// </summary>
public class AbilityCoopJump : AbilityAnimationAttack{
	/// <summary>
	/// Target to jump over
	/// </summary>
	Transform _target;
//	//Original RCB
//	Collider _RCB;

	Rigidbody _rigidbody;

	float _jumpForce = 60f;

//	override
//	void Start(){
//		_status = ABILITY_STATUS.UNAVAILABLE;
//	}
	protected override void Start ()
	{
		base.Start ();
		_status = ABILITY_STATUS.UNAVAILABLE;
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{		
		base.Activate ();

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
		_animator.transform.LookAt(new Vector3(_target.position.x, _combatHandler.transform.position.y, _target.position.z));

		//apply force and change direction mid-action
		StartCoroutine (IEMidJump (_animTime1 / 2.5f));

		//finetune
		//initial up & forward force towards the target
//		_rigidbody.AddForce (Vector3.up * 3f * _jumpForce);
//		_rigidbody.AddForce (Vector3.forward * 6f * _jumpForce);
		_rigidbody.AddForce (_rigidbody.transform.up * 3.5f * _jumpForce);
		_rigidbody.AddForce (_rigidbody.transform.forward * 3f * _jumpForce);
	}

	IEnumerator IEMidJump(float seconds){
		yield return new WaitForSeconds (seconds);
		//		_animator.transform.localEulerAngles = new Vector3(_animator.transform.localEulerAngles.x, _target.localEulerAngles.y ,_animator.transform.localEulerAngles.z);
		_animator.transform.localEulerAngles = _target.localEulerAngles;
		//clear any remaining force
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;
//
//		//this is to solve force added before turning
//		yield return new WaitForSeconds (0.1f);

		_rigidbody.AddForce (_target.up * 4.0f * _jumpForce);
		_rigidbody.AddForce (_target.forward * 4.5f * _jumpForce);
	}

	public void SetTarget(Transform newTarget){
		_target = newTarget;
	}
		
	protected override IEnumerator IECheckInUse (float seconds)
	{
		//return base.IECheckInUse (seconds);
		_status = ABILITY_STATUS.IN_USE;

		//network sometimes plays falling animation before jump, so disable falling animation for the duration
		_combatHandler.GetComponent<PlayerMovementControl> ().SetCheckGrounded (false);

		//Network
		//set RCB to clip through other bodies
		_combatHandler.ChangeRCBLayer(LayerHelper.COMMON_BODY_GHOST);
//		_RCB.gameObject.layer = LayerHelper.COMMON_BODY_GHOST;

//		Debug.Break ();
		yield return new WaitForSeconds (seconds);
		_status = ABILITY_STATUS.UNAVAILABLE;
		//Network
		//set RCB back to normal
		_combatHandler.ChangeRCBLayer(LayerHelper.COMMON_BODY);
//		_RCB.gameObject.layer = LayerHelper.COMMON_BODY;
//		Debug.Break ();
		unlockControls ();

		_combatHandler.GetComponent<PlayerMovementControl> ().SetCheckGrounded (true);

//		Debug.Break ();
	}

	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);
		_rigidbody = ch.GetComponent<Rigidbody> ();
	}
}
