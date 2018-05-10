using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Mark Cohen
/// On 2017-02-04
///
/// <summary>
/// Secondary Attack for archer
/// 	This is the Aiming (camera over shoulder+crossair) functionality
/// </summary>
public class ArcherSecondaryAttack : AbilityHold {

	/// <summary>
	/// The shooting point
	/// Where arrow will be spawned
	/// Which direction(angle) arrow will be flying
	/// 	NOTE: This may not be necessary for this parent component
	/// 			this ability itself will not need to shoot/deal damage
	/// </summary>
	[SerializeField] Transform _shootingPoint;
	[SerializeField] PlayerMovementControl _player;
//	//Duke 3/15/2017
//	//Target to aim, used for IK aim animation adjustment
//	[SerializeField] Transform _aimTarget;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		foreach (var child in ChildAbilities) {
			child.SetCombatHandler (_combatHandler);
		}
	}

	/// <summary>
	/// Activate / Use this ability.
	/// This ability simply centers the camera over the shoulder (with crossair)
	/// and perhaps slows movement speed
	/// </summary>
	public override void Activate ()
	{
//		//Duke
//		//Dev for IK setup 3/15/2017
//		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
//		if (aimIK) {
//			aimIK.enabled = true;
//		}

		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;
		_player.IsAiming = true;
		base.Activate ();
	}

//	//Duke
//	//Dev for IK setup 3/15/2017
//	void Update(){
//		if (!_combatHandler.GetComponent<PhotonView> ().isMine)
//			return;
//
//		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
//		if (aimIK) {
//			_aimTarget.position = CameraController.CC.CombatCamera.transform.position + CameraController.CC.CombatCamera.transform.forward * 100f;
//			float angle = Vector3.Angle (_combatHandler.transform.forward, _aimTarget.position);
//			print (angle);
//			if (angle > 120f) {
//				Quaternion targetRot = Quaternion.LookRotation (_aimTarget.transform.position, _combatHandler.transform.position);
//				targetRot = Quaternion.Euler (new Vector3 (0f, targetRot.eulerAngles.y, 0f));
//				_combatHandler.transform.rotation = Quaternion.Lerp (_combatHandler.transform.rotation, targetRot, Time.deltaTime);
//			}
//		}
//	}

	/// <summary>
	/// Deactivate / cancel this ability.
	/// This should simply put the camera back to over the shoulder
	/// AND NOT allow the child ability (charge and shoot) to fire
	/// </summary>
	public override void Deactivate ()
	{
//		//Duke
//		//Dev for IK setup 3/15/2017
//		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
//		if (aimIK) {
//			aimIK.enabled = false;
//		}

		//TODO: Aim animation, perhaps
//		_animator.SetBool (_animHash, false);
		_player.IsAiming = false;
		//The child ability, hold to charge/shoot,
		//should be CANCELED, and no arrow should be fired
		foreach (var child in ChildAbilities)
		{
			//The child ability, hold to charge/shoot,
			//should be CANCELED, and no arrow should be fired
			if (child is ArcherSecondaryAttackChild) {
				((ArcherSecondaryAttackChild)child).SetCancel (true);
				((ArcherSecondaryAttackChild)child).Deactivate ();
			}
		}

		//These commands are included in the base.Deactivate()
//		_status = ABILITY_STATUS.AVAILABLE;	
//		Invoke ("unlockControls", 0.2f);
		base.Deactivate ();
	}
}
