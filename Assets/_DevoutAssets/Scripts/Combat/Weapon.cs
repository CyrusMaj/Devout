using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Weapon holds damaging points
/// Note that weapon does not have to be a visible physical object(i.e. character's arm can be a weapon)
/// </summary>
public class Weapon : MonoBehaviour
{
	/// <summary>
	/// Type of this weapon
	/// </summary>
	public TYPE Type;
	
	/// <summary>
	/// The damage.
	/// weapon damage can be changed by ability
	/// </summary>
	int _damage = 0;
	/// <summary>
	/// List of damaging points this weapon holds
	/// </summary>
	[SerializeField] List<DamagingPoint> _damagingPoints = new List<DamagingPoint> ();
	/// <summary>
	/// The owner's combatHandler
	/// </summary>
	CombatHandler _ownerCH;
	/// <summary>
	/// Force of pushback
	/// </summary>
	[SerializeField] float _pushbackForce = 0f;
	Vector3 _pushbackDirection = Vector3.forward;
	/// <summary>
	/// Coroutine for activating damaging points
	/// </summary>
	IEnumerator _IEADP;

	//if true, this ability disable's enemy/target's damaging points so it doesn't accidentaly damage attacker
	[SerializeField] bool _disablesTargetDamagingPointOnHit = false;

	public bool GetDisablesTargetDamagingPointOnhit ()
	{
		return _disablesTargetDamagingPointOnHit;
	}

	void Start ()
	{
		foreach (var dp in _damagingPoints) {
			dp.SetWeapon (this);
		}
	}

	/// <summary>
	/// Gets the owner CombatHandler
	/// </summary>
	/// <returns>The owner.</returns>
	public CombatHandler GetOwner ()
	{
		if (_ownerCH == null)
			print ("returning null");
		return _ownerCH;
	}

	/// <summary>
	/// Sets the owner CobmatHandler
	/// </summary>
	/// <param name="owner">Owner.</param>
	public void SetOwner (CombatHandler owner)
	{
		_ownerCH = owner;
	}

	/// <summary>
	/// Sets the damage.
	/// </summary>
	/// <param name="newDamage">New damage.</param>
	public void SetDamage (int newDamage)
	{
		_damage = newDamage;
	}

	/// <summary>
	/// Gets the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	public int GetDamage ()
	{
		return _damage;
	}

	/// <summary>
	/// Gets the pushback force.
	/// </summary>
	/// <returns>The pushback force.</returns>
	public float GetPushbackForce ()
	{
		return _pushbackForce;
	}

	/// <summary>
	/// Activates all DamagingPoints this weapon holds
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	public void ActivateDamagingPoint (float seconds)
	{
//		print ("activated : " + seconds + " seconds");
		if (_IEADP != null)
			StopCoroutine (_IEADP);

		foreach (var dp in _damagingPoints) {
			dp.SetIsActive (false);
			dp.ClearOSHList ();
		}
		
		_IEADP = IEActivateDamagingPoint (seconds);
		StartCoroutine (_IEADP);
	}

	public void DeactivateDamagingPoint(){
		if (_IEADP != null)
			StopCoroutine (_IEADP);

		foreach (var dp in _damagingPoints) {
			dp.SetIsActive (false);
			dp.ClearOSHList ();
		}
		foreach (var dp in _damagingPoints)
			dp.SetIsActive (false);
	}

	IEnumerator IEActivateDamagingPoint (float seconds)
	{
		foreach (var dp in _damagingPoints)
			dp.SetIsActive (true);
		yield return new WaitForSeconds (seconds);
		foreach (var dp in _damagingPoints)
			dp.SetIsActive (false);
	}

	/// <summary>
	/// Gets the list of damaging points.
	/// </summary>
	/// <returns>The damaging points.</returns>
	public List<DamagingPoint> GetDamagingPoints ()
	{
		return _damagingPoints;
	}

	/// <summary>
	/// Type of a weapon
	/// </summary>
	public enum TYPE{
		SWORD,
		AXE,
		ARROW,
		MACE,
	}
}
