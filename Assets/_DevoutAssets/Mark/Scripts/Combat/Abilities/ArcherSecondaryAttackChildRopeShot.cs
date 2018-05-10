using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im, copied version of ArcherSecondaryAttackChild
/// On 2017-03-21
///
/// <summary>
/// Secondary Attack child for archer
/// 	This is the actual shoot (hold+release) functionality
/// 	Shoots rope shot instead of regular shots which ties rope to whatever it hits
/// </summary>
public class ArcherSecondaryAttackChildRopeShot : AbilityHold {
	//Duke 3/15/2017
	//Target to aim, used for IK aim animation adjustment
	[SerializeField] Transform _aimTarget;

	/// <summary>
	/// The shooting point
	/// Where arrow will be spawned
	/// Which direction(angle) arrow will be flying
	/// </summary>
	[SerializeField] Transform _shootingPoint;
	private float aimTime = 0f;
	private bool cancel = false;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	protected override IEnumerator IECooldown (float seconds)
	{
//		print ("Overridden IEnumerator called");
		//		return base.IECooldown (seconds);
		_status = ABILITY_STATUS.IN_COOLDOWN;
		yield return new WaitForSeconds (seconds);

		if (_combatHandler.GetRopeCount() > 0)
			_status = ABILITY_STATUS.AVAILABLE;
		else
			_status = ABILITY_STATUS.UNAVAILABLE;
	}

	public override void Activate()
	{
		//Duke
		//Dev for IK setup 3/15/2017
		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
		if (aimIK) {
			aimIK.enabled = true;
		}

		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;
		
		SetCancel (false);
		base.Activate ();

		if (_combatHandler.GetRopeCount () < 1)
			Debug.LogWarning ("WARNING : Ropecount shouldn't be less than 1 here, check and fix");
		
//		_combatHandler.SetRopeCount (_combatHandler.GetRopeCount () - 1);
	}

	//Duke
	//Dev for IK setup 3/15/2017
	void Update(){
//		print (GetStatus ().ToString ());

		if (!_combatHandler.GetComponent<PhotonView> ().isMine || _status != ABILITY_STATUS.IN_USE)
			return;

		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
		if (aimIK) {
			_aimTarget.position = CameraController.CC.CombatCamera.transform.position + CameraController.CC.CombatCamera.transform.forward * 100f;
			float angle = Vector3.Angle (_combatHandler.transform.forward, _aimTarget.position);
			//			print (angle);
			if (angle > 95f) {
				Quaternion targetRot = Quaternion.LookRotation (_aimTarget.transform.position, _combatHandler.transform.position);
				targetRot = Quaternion.Euler (new Vector3 (0f, targetRot.eulerAngles.y, 0f));
				_combatHandler.transform.rotation = Quaternion.Lerp (_combatHandler.transform.rotation, targetRot, Time.deltaTime * 1.5f);
			}
		}
	}

	public override void Deactivate()
	{
		//Duke
		//Dev for IK setup 3/15/2017
		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK>();
		if (aimIK) {
			aimIK.enabled = false;
		}

		//If this Deactivate was NOT called by parent 
		if (!cancel) {
			ShootArrow ();
		}

		//Normal Deactivate
		//stop animating (set trigger to false)
		//set ability status to be used again
		//unlock controls
		//		_animator.SetBool (_animHash, false);	
		//		_status = ABILITY_STATUS.AVAILABLE;	
		//		Invoke ("unlockControls", 0.2f);
		base.Deactivate ();
	}

	public void SetCancel(bool isCancel)
	{
		cancel = isCancel;
	}

	private void ShootArrow()
	{
		//Calculate the damage of the arrow
		//If the current animation is DRAW, then the arrow is not at full power/speed
		//TODO: 50% threshold (arbitrary) for when the hand is not yet drawn
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_DRAW) {
			aimTime = _animator.GetCurrentAnimatorStateInfo (0).normalizedTime;
		} else if(_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_AIM){//Drawing is complete, currently 'AIM'ing
			aimTime = 1f;
		}
		Transform direction = _shootingPoint;

		LayerMask lm = LayerHelper.GetLayerMask (_combatHandler.GetTeam ());
		RaycastHit hit;
		if (Physics.Raycast (CameraController.CC.CombatCamera.transform.position, CameraController.CC.CombatCamera.transform.forward, out hit, 100, lm)) {
			direction.LookAt (hit.point);
		} else {
			direction.LookAt (CameraController.CC.CombatCamera.transform.forward * 100);
		}

//		ProjectileController.PC.InstantiateProjectile (direction.position, direction.rotation, _combatHandler.GetComponent<PhotonView>().viewID, (int)(aimTime * _damage), ProjectileController.TYPE.ARROW_ROPE);

		//
//		go = PhotonNetwork.Instantiate ("Rope_05", _combatHandler.transform.position, Quaternion.identity, 0);
		GameObject projectileInstance = PhotonNetwork.Instantiate("RopeArrow", direction.position, direction.rotation, 0);

//		projectileInstance.transform.position = direction.position;
//		projectileInstance.transform.rotation = direction.rotation;

//		PhotonView pv = PhotonView.Find (photonViewID);
		CombatHandler ch = _combatHandler;

		Weapon weapon = projectileInstance.GetComponent<Weapon> ();
		weapon.SetOwner (ch);

		Arrow arrow = weapon.GetDamagingPoints()[0].GetComponent<Arrow>();

		//
		arrow.gameObject.layer = LayerHelper.GetAttackBoxLayer (ch.GetTeam ());

		arrow.SetDamagableLayers ();
		arrow.GetComponent<Collider> ().enabled = true;
		arrow.SetIsMine (true);
		weapon.SetDamage (25);
	}
}
