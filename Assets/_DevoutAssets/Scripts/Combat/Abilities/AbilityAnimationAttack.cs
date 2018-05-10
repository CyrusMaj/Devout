using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Animated attacks that uses animation to attack(for example, sliding attack)
/// </summary>
public class AbilityAnimationAttack : Ability
{
	/// <summary>
	/// Cooldown of this ability ; how long does it take to use this ability again?(Starting from activation of ability)
	/// </summary>
	[SerializeField] protected float _coolDown;

	/// <summary>
	/// How long is the animation?
	/// </summary>
	[SerializeField] protected float _animTime1;

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

	protected override void Start ()
	{
		base.Start ();
		_animHash = AnimationHashHelper.FindHash (_animParam);
		if (_animTime1 > _coolDown) {
			Debug.LogWarning ("Animation time shouldn't be longer than cooldown");
		}
	}

	/// <summary>
	/// Activate / Use this instance.
	/// </summary>
	public override void Activate ()
	{
		if (_status == ABILITY_STATUS.AVAILABLE) {
			base.Activate ();
			if (_weapon)
				_weapon.SetDamage (_damage);
			_animator.SetTrigger (_animHash);
//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay));
			ActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay);
			checkInUse (_animTime1);
			coolDown (_coolDown);//maybe cooldown should start after checkInUse??
		}
	}

	/// <summary>
	/// Set that this ability is in use
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	protected virtual void checkInUse (float seconds)
	{
//		print ("checkInUse called");
		StartCoroutine (IECheckInUse (seconds));		
	}

	protected virtual IEnumerator IECheckInUse (float seconds)
	{
		_status = ABILITY_STATUS.IN_USE;
		yield return new WaitForSeconds (seconds);
		_status = ABILITY_STATUS.UNAVAILABLE;
		unlockControls ();
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

		if (_weapon)
			_weapon.SetOwner (_combatHandler);
	}
}
