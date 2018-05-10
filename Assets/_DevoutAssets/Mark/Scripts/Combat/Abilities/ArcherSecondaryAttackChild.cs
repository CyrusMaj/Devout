using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Mark Cohen
/// On 2017-02-24
///
/// <summary>
/// Secondary Attack child for archer
/// 	This is the actual shoot (hold+release) functionality
/// </summary>
public class ArcherSecondaryAttackChild : AbilityHold
{
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

	//to hold bow in-place from IK_aim rotating it
	[SerializeField] Transform _IK_Bow;
	Vector3 _IK_Bow_Pos;
	Quaternion _IK_Bow_Rot;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

		_IK_Bow_Pos = _IK_Bow.localPosition;
		_IK_Bow_Rot = _IK_Bow.localRotation;
	}

	public override void Activate ()
	{
		//Duke
		//Dev for IK setup 3/15/2017
		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK> ();
		if (aimIK) {
			aimIK.enabled = true;
		}

		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;
		SetCancel (false);
		base.Activate ();
	}

	//Duke
	//Dev for IK setup 3/15/2017
	void Update ()
	{
		if (!_combatHandler.GetComponent<PhotonView> ().isMine || _status != ABILITY_STATUS.IN_USE)
			return;

		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK> ();

		if (aimIK) {
			_IK_Bow.localPosition = _IK_Bow_Pos;
			_IK_Bow.localRotation = _IK_Bow_Rot;

			_aimTarget.position = CameraController.CC.CombatCamera.transform.position + CameraController.CC.CombatCamera.transform.forward * 20f;
			//			_aimTarget.position = _shootingPoint.position + _shootingPoint.forward * 20f;

			Vector3 rPos = _aimTarget.transform.position - _combatHandler.transform.position;
			Quaternion tRot = Quaternion.LookRotation (rPos);
			tRot = Quaternion.Euler (new Vector3 (0f, tRot.eulerAngles.y, 0f));

//			float angle = Vector3.Angle (_combatHandler.transform.position, _aimTarget.position);
			float angle = Mathf.Abs((tRot.eulerAngles - _combatHandler.transform.eulerAngles).magnitude);

//			print (angle);

			if (angle > 15f) { 
				Vector3 relPos = _aimTarget.transform.position - _combatHandler.transform.position;
				//				Quaternion targetRot = Quaternion.LookRotation (_aimTarget.transform.position, _combatHandler.transform.position);
				Quaternion targetRot = Quaternion.LookRotation (relPos);
				targetRot = Quaternion.Euler (new Vector3 (0f, targetRot.eulerAngles.y, 0f));
				_combatHandler.transform.rotation = Quaternion.Lerp (_combatHandler.transform.rotation, targetRot, Time.deltaTime * 3.5f);
			}
		} else
			print ("AIM_IK is null");
	}

	public override void Deactivate ()
	{
		//Duke
		//Dev for IK setup 3/15/2017
		RootMotion.FinalIK.AimIK aimIK = _combatHandler.GetComponent<RootMotion.FinalIK.AimIK> ();
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

	public void SetCancel (bool isCancel)
	{
		cancel = isCancel;
	}

	private void ShootArrow ()
	{
		//Calculate the damage of the arrow
		//If the current animation is DRAW, then the arrow is not at full power/speed
		//TODO: 50% threshold (arbitrary) for when the hand is not yet drawn
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_DRAW) {
			aimTime = _animator.GetCurrentAnimatorStateInfo (0).normalizedTime;
		} else if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == AnimationHashHelper.STATE_AIM) {//Drawing is complete, currently 'AIM'ing
			aimTime = 1f;
		}
//		Transform direction = _shootingPoint;
//		direction.LookAt (Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f)));
//		direction.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
//		direction.LookAt(Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0.5f)));
//		direction.LookAt(Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f)).GetPoint(20));
//		direction.position = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f)).GetPoint(2);
//		direction.LookAt (CameraController.CC.CombatCamera.transform.forward * 10000000);

//		Ray target = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f));
		LayerMask lm = LayerHelper.GetLayerMask (_combatHandler.GetTeam ());
		RaycastHit hit;
		if (Physics.Raycast (CameraController.CC.CombatCamera.transform.position, CameraController.CC.CombatCamera.transform.forward, out hit, 100, lm)) {
			_shootingPoint.LookAt (hit.point);
		} else {
			_shootingPoint.LookAt (CameraController.CC.CombatCamera.transform.forward * 100);
		}


		ProjectileController.PC.InstantiateProjectile (_shootingPoint.position, _shootingPoint.rotation, _combatHandler.GetComponent<PhotonView> ().viewID, (int)(aimTime * _damage), ProjectileController.TYPE.ARROW_NORMAL);
//		Debug.Break ();
	}
}
