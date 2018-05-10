using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
///
/// <summary>
/// Basic melee attacks
/// Combo attacks that can be active during time window after previous basic attack
/// </summary>
public class AbilityBasicAttackMelee : Ability
{
	[SerializeField] float _animTime1;
	[SerializeField] float _animTime2;
	Animator _animator;
	IEnumerator coroutineWaitForCombo;
	IEnumerator coroutineStatusUpdate;

	bool _comboReady1 = false;
	//how long after first attack can player use this again to use second attack(combo)
	float _comboWait1 = 0.5f;

	protected override void Start ()
	{
		base.Start ();
//		print (_combatHandler.name.ToString () + " called");
	}

	/// <summary>
	/// Activate / Use this instance.
	/// </summary>
	public override void Activate ()
	{

		//combo
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_BASIC_ATTACK_1 &&
			_comboReady1) {
			base.Activate ();
			_weapon.SetDamage (_damage);

			//2nd attack
			if (coroutineStatusUpdate != null)
				StopCoroutine (coroutineStatusUpdate);
			_status = ABILITY_STATUS.IN_USE;
			coroutineStatusUpdate = IESetStatus (ABILITY_STATUS.AVAILABLE, _animTime2);
			StartCoroutine (coroutineStatusUpdate);
			_animator.SetTrigger (AnimationHashHelper.PARAM_BASIC_ATTACK_2);
//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime2, _animTime2 / 5f));
			ActivateDamagingPoint(_weapon, _animTime2, _animTime2 / 5f);
		} else if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_GROUNDED) {
			base.Activate ();
			_weapon.SetDamage (_damage);

			//reset trigger of second attack in case of remaining from unaccomplished attacks
			_animator.ResetTrigger(AnimationHashHelper.PARAM_BASIC_ATTACK_2);

			//1st attack
			if (coroutineStatusUpdate != null)
				StopCoroutine (coroutineStatusUpdate);
			_status = ABILITY_STATUS.IN_USE;
			coroutineStatusUpdate = IESetStatus (ABILITY_STATUS.AVAILABLE, _animTime1 / 2f);
			StartCoroutine (coroutineStatusUpdate);
			_animator.SetTrigger (AnimationHashHelper.PARAM_BASIC_ATTACK_1);
//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _animTime1 / 4f));
			ActivateDamagingPoint(_weapon, _animTime1, _animTime1 / 4f);
			if (coroutineWaitForCombo != null)
				StopCoroutine (coroutineWaitForCombo);
			_comboReady1 = false;
			coroutineWaitForCombo = CoroutineHelper.IEChangeBool ((x) => _comboReady1 = x, true, _comboWait1);
			StartCoroutine (coroutineWaitForCombo);
		}
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

	/// <summary>
	/// Set ability status after waittime
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="status">Status.</param>
	/// <param name="waitTime">Wait time.</param>
	IEnumerator IESetStatus (ABILITY_STATUS status, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		_status = status;

//		if (_status == ABILITY_STATUS.AVAILABLE) {
//			unlockControls ();
//		}
		unlockControls ();
	}
}