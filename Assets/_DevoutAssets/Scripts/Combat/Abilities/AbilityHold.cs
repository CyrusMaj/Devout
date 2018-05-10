using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-04
///
/// <summary>
/// Ability that is active while holding(pressing) an input(button)
/// </summary>
public class AbilityHold : Ability
{

	/// <summary>
	/// Cooldown of this ability ; how long does it take to use this ability again?(Starting from activation of ability)
	/// </summary>
	[SerializeField] protected float _coolDown = 0f;

	/// <summary>
	/// Name of the animation
	/// </summary>
	[SerializeField] string _animParam;

	/// <summary>
	/// Animator component of character object holding this ability
	/// </summary>
	protected Animator _animator;
	protected int _animHash;

	protected override void Start ()
	{
		base.Start ();
		_animHash = AnimationHashHelper.FindHash (_animParam);
	}

	/// <summary>
	/// Starts the cooldown
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	protected virtual void coolDown (float seconds)
	{
		StartCoroutine (IECooldown (seconds));
	}

	protected virtual IEnumerator IECooldown (float seconds)
	{ 
		//		_status = ABILITY_STATUS.UNAVAILABLE;
		_status = ABILITY_STATUS.IN_COOLDOWN;
		yield return new WaitForSeconds (seconds);
		_status = ABILITY_STATUS.AVAILABLE;
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{
		base.Activate ();
		if (_animHash != 0)
			_animator.SetBool (_animHash, true);
		_status = ABILITY_STATUS.IN_USE;
	}

	/// <summary>
	/// Deactivate / Stop using this ability.
	/// </summary>
	public virtual void Deactivate ()
	{
		if (_animHash != 0)
			_animator.SetBool (_animHash, false);	
		_status = ABILITY_STATUS.AVAILABLE;	
		unlockControls ();
		coolDown (_coolDown);//maybe cooldown should start after checkInUse??
	}

	/// <summary>
	/// Sets the combat handler linked to character holding this ability
	/// </summary>
	/// <param name="ch">Combat handler</param>
	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);

		//set animator linked to combathandler
		if (_combatHandler.GetComponent<Animator> () != null) {
			_animator = _combatHandler.GetComponent<Animator> ();
		} else
			print ("WARNING : Failed to load animator");
	}
}
