using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-06
///
/// <summary>
/// Ability basic attack range bow.
/// </summary>
public class AbilityBasicAttackRangeBow : AbilityHold {

//	/// <summary>
//	/// Name of the release animation
//	/// </summary>
//	[SerializeField] string _releaseAnimParam;
//	protected int _releaseAnimHash;

	/// <summary>
	/// The shooting point
	/// Where arrow will be spawned
	/// Which direction(angle) arrow will be flying
	/// </summary>
	[SerializeField] Transform _shootingPoint;

	protected override void Start ()
	{
		base.Start ();
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;
		
		base.Activate ();
	}

	/// <summary>
	/// Deactivate / Stop using this ability.
	/// </summary>
	public override void Deactivate ()
	{
		//base.Deactivate ();
		_animator.SetBool (_animHash, false);	
		_status = ABILITY_STATUS.AVAILABLE;	
		Invoke ("unlockControls", 0.2f);

		//Don't fire arrow unless it's fully drawn / loaded
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_AIM)
			return;
		
		ProjectileController.PC.InstantiateProjectile (_shootingPoint.position, Quaternion.identity, _combatHandler.GetComponent<PhotonView>().viewID, _damage, ProjectileController.TYPE.ARROW_NORMAL);
//		instance.transform.position = _shootingPoint.position;
//		instance.transform.eulerAngles = new Vector3(_shootingPoint.eulerAngles.x - 90f, _shootingPoint.eulerAngles.y, _shootingPoint.eulerAngles.z);
//		instance.transform.eulerAngles = _combatHandler.transform.eulerAngles;

//		Weapon weapon = instance.GetComponent<Weapon> ();
//		weapon.SetOwner (_combatHandler);
//		weapon.SetDamage (_damage);
//		Arrow arrow = weapon.GetDamagingPoints()[0].GetComponent<Arrow>();
//		//only deals damage if set to mine
//		arrow.SetIsMine (true);

//		arrow.gameObject.layer = LayerHelper.GetAttackBoxLayer (_combatHandler.GetTeam ());
//		arrow.SetDamagableLayers ();
//		arrow.GetComponent<Collider> ().enabled = true;

//		weapon.GetComponent<Rigidbody> ().velocity = _combatHandler.transform.forward * arrow.Speed;

//		Debug.Break ();
	}
}
