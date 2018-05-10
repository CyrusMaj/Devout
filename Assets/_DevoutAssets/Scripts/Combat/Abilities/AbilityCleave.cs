using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
///
//Dev
//Note : maybe not needed ; replaced by abilityanimationattack
//Replaced by abilityanimationattack
//To be deleted
public class AbilityCleave : AbilityAnimationAttack
{
//	/// <summary>
//	/// Activate / Use this instance.
//	/// </summary>
//	public override void Activate ()
//	{
//		if (_status == ABILITY_STATUS.AVAILABLE) {
//			base.Activate ();
//			StartCoroutine (IEMoveUpWeapon (_animTime1));
//		}
//		//		if (_status == ABILITY_STATUS.AVAILABLE) {
//		//			_weapon.SetDamage (_damage);
//		//			_animator.SetTrigger (_animHash);
//		//			StartCoroutine (IEMoveUpWeapon (_animTime1));
//		//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _animTime1 / 2.75f));
//		//			StartCoroutine (IECheckInUse (_animTime1));
//		//			StartCoroutine (IECooldown (_coolDown));
//		//		}
//	}
//	//	protected override void activateDamagingPoint (float seconds)
//	//	{
//	////		StartCoroutine(IEActivateDamagingPoint (weapon, seconds, seconds / 5f));
//	//	}
//	//	protected override void checkInUse (float seconds)
//	//	{
//	////		print ("checkInUse(Empty) called");
//	////		StartCoroutine (IEMoveUpWeapon (_animTime1));
//	//	}
//	//	protected override void coolDown (float seconds)
//	//	{
//	////		base.coolDown (seconds);
//	//	}
//
//	//dev
//	IEnumerator IEMoveUpWeapon (float seconds)
//	{
//		Vector3 originalPos = _weapon.transform.localPosition;
////		_weapon.transform.position = new Vector3 (_weapon.transform.position.x - 1.3f, _weapon.transform.position.y, _weapon.transform.position.z);
//		_weapon.transform.Translate (Vector3.up * 0.3f);
//		yield return new WaitForSeconds (seconds);
//		_weapon.transform.localPosition = originalPos;
//	}
}
