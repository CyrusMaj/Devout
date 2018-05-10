using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Written by Mark Cohen
/// On 2017-02-04
///
/// <summary>
/// Ability basic attack ranged bow.
/// </summary>
public class ArcherQuickAttack : Ability {

	/// <summary>
	/// The shooting point
	/// Where arrow will be spawned
	/// Which direction(angle) arrow will be flying
	/// </summary>
	[SerializeField] Transform _shootingPoint;

	//Using animTrigger instead
	[SerializeField] string _animTrigger;
	protected int _animHash;
	Animator _animator;

	// Use this for initialization
	protected override void Start () {
		_animHash = AnimationHashHelper.FindHash (_animTrigger);
		base.Start ();
	}
	
	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;

		//HARD CODED VALUE from actual animation length
		//(QuickDraw - QuickLoose transition time
		IEnumerator shootRoutine = IEShootArrow(0.443f, _damage);
		_animator.SetTrigger (_animHash);
		StartCoroutine (shootRoutine);
		SetStatus (ABILITY_STATUS.IN_USE);

		base.Activate ();
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

	IEnumerator IEShootArrow(float waitTime, int dmg)
	{
		yield return new WaitForSeconds (waitTime);

//		ProjectileController.PC.InstantiateProjectile (_shootingPoint.position, Quaternion.identity, _combatHandler.GetComponent<PhotonView>().viewID, dmg);
		//Duke
		//arrow wasn't rotating correctly
//		print("ArcherQuickAttack : " + _shootingPoint.position.ToString() + ", " + _shootingPoint.name + ", " + _shootingPoint.TransformPoint(0f,0f,0f).ToString());
//		GameObject test = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//		test.transform.position = _shootingPoint.position;
//		test.GetComponent<Collider> ().enabled = false;

		ProjectileController.PC.InstantiateProjectile (_shootingPoint.position, _combatHandler.transform.rotation, _combatHandler.GetComponent<PhotonView>().viewID, dmg, ProjectileController.TYPE.ARROW_NORMAL);
//		_animator.SetBool ("QuickAim", false);
		SetStatus (ABILITY_STATUS.AVAILABLE);
		unlockControls ();
	}
}
