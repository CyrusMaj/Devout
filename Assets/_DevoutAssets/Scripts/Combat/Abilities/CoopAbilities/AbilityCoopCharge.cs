using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-02-24
/// 
/// <summary>
/// Cooperative ability where character charge towards friendly character
/// </summary>
public class AbilityCoopCharge : Ability
{
	#region coop part

	[SerializeField] float _chargeSpeed = 1f;

	/// <summary>
	/// Target to charge to
	/// </summary>
	Transform _target;

	public void SetTarget(Transform newTarget){
		_target = newTarget;
	}

	IEnumerator move(float chargeDuration){
		float timer = Time.time + chargeDuration;
		while (timer > Time.time) {
			_combatHandler.transform.Translate (_chargeSpeed * Vector3.forward * Time.deltaTime);

			//abort if close enough to target
			if (Vector3.Distance (_combatHandler.transform.position, _target.position) < 2f) {
				print ("Close enough, Aborting");
//				StopAllCoroutines ();
				if (_IE_ActivateDamagingPoint != null)
					StopCoroutine (_IE_ActivateDamagingPoint);
				_weapon.DeactivateDamagingPoint ();
				if (_IE_CIU != null)
					StopCoroutine (_IE_CIU);
				
				_status = ABILITY_STATUS.UNAVAILABLE;
				unlockControls ();
				_animator.SetBool (_animHash, false);
				timer = Time.time;
//				yield break;
			}

			yield return null;
		}
	}

	#endregion
	//below is pretty much AbilityAnimationAttack.cs with boolean animation param instead of trigger
	//so separate and inherit from it if there's need to reuse below code

	/// <summary>
	/// Cooldown of this ability ; how long does it take to use this ability again?(Starting from activation of ability)
	/// </summary>
	[SerializeField] protected float _coolDown;

	/// <summary>
	/// How long is the animation loop?
	/// </summary>
	[SerializeField] protected float _animTime;

	/// <summary>
	/// Name of the animation
	/// </summary>
	[SerializeField] string _animParam;

	[SerializeField] protected float _activateDamagingPointDelay;

	/// <summary>
	/// Animator of character holding this ability
	/// </summary>
	protected Animator _animator;
	protected int _animHash;

	IEnumerator _IE_CIU;

	protected override void Start ()
	{
		base.Start ();
		_animHash = AnimationHashHelper.FindHash (_animParam);
		if (_animTime > _coolDown) {
			Debug.LogWarning ("Animation time shouldn't be longer than cooldown");
		}

		_status = ABILITY_STATUS.UNAVAILABLE;
	}

	/// <summary>
	/// Activate / Use this instance.
	/// </summary>
	public override void Activate ()
	{
		if (_status == ABILITY_STATUS.AVAILABLE) {
			base.Activate ();
			_weapon.SetDamage (_damage);
			_animator.SetBool (_animHash, true);
			//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay));
			ActivateDamagingPoint (_weapon, _animTime, _activateDamagingPointDelay);

			_combatHandler.transform.LookAt(new Vector3(_target.position.x, _combatHandler.transform.position.y, _target.position.z));
			StartCoroutine (move (_animTime));

			if (_IE_CIU != null)
				StopCoroutine (_IE_CIU);
			_IE_CIU = IECheckInUse(_animTime);
			coolDown (_coolDown);//maybe cooldown should start after checkInUse??
		}
	}

//	/// <summary>
//	/// Set that this ability is in use
//	/// </summary>
//	/// <param name="seconds">Seconds.</param>
//	protected virtual void checkInUse (float seconds)
//	{
//		//		print ("checkInUse called");
//		StartCoroutine (IECheckInUse (seconds));		
//	}

	protected virtual IEnumerator IECheckInUse (float seconds)
	{
		_status = ABILITY_STATUS.IN_USE;
		yield return new WaitForSeconds (seconds);
		_status = ABILITY_STATUS.UNAVAILABLE;
		unlockControls ();
		_animator.SetBool (_animHash, false);
	}

	/// <summary>
	/// Starts the cooldown
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	protected virtual void coolDown (float seconds)
	{
		StartCoroutine (IECooldown (seconds));
	}

	protected IEnumerator IECooldown (float seconds)
	{ 
		//		_status = ABILITY_STATUS.UNAVAILABLE;
		_status = ABILITY_STATUS.IN_COOLDOWN;
		yield return new WaitForSeconds (seconds);
		_status = ABILITY_STATUS.AVAILABLE;
	}

	/// <summary>
	/// Sets the combat handler linked to character holding this ability
	/// </summary>
	/// <param name="ch">Combat handler</param>
	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);

		if (_combatHandler.GetComponent<Animator> () != null) {
			_animator = _combatHandler.GetComponent<Animator> ();
		} else
			print ("WARNING : Failed to load animator");

		_weapon.SetOwner (_combatHandler);
	}
}
