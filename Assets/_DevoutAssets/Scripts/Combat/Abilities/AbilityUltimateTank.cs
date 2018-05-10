using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-27
///
/// <summary>
/// Ability ultimate Tank.
/// "ShieldPress Mothafukka"
/// </summary>
//public class AbilityUltimateTank : Ability
public class AbilityUltimateTank : AbilityAnimationAttack
{
	protected override void Start ()
	{
		base.Start ();
		//unlike other abilities, ultimates are only available when ultimate guage is full
		_status = ABILITY_STATUS.UNAVAILABLE;
	}
	/// <summary>
	/// Activate / Use this instance.
	/// </summary>
	public override void Activate ()
	{
		if (_status == ABILITY_STATUS.AVAILABLE) {
			if (_aimAssist) {
				assistAim (0.2f);
			}
			if (_positionAssist) {
				assistPosition (0.3f);
			}
			//lock player controls during activation
			//must be unlocked in child(better design?)
			lockControls ();
			_weapon.SetDamage (_damage);
			_animator.SetTrigger (_animHash);
			//			StartCoroutine (IEActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay));
			ActivateDamagingPoint (_weapon, _animTime1, _activateDamagingPointDelay);
			checkInUse (_animTime1);
			_combatHandler.SetUltimatePoint(0);

//			Debug.Break ();
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(0f, 0.6f, 0f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(1f, 0.6f, 0f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(-1f, 0.6f, 0f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(0f, 0.6f, 1f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(0f, 0.6f, -1f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(0.707f, 0.6f, -0.707f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(-0.707f, 0.6f, -0.707f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(0.707f, 0.6f, 0.707f), _activateDamagingPointDelay);
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, _weapon.transform.position + new Vector3(-0.707f, 0.6f, 0.707f), _activateDamagingPointDelay);
		}
	}

	//	/// <summary>
	//	/// The expanded blocking point.(ex. Shield)
	//	/// </summary>
	//	[SerializeField] BlockingPoint _ultimateBlockingPoint;
	//	[SerializeField] PressurePoint _ultimatePressurePoint;
	//	PhotonView _pv;
	//
	//	protected override void Start ()
	//	{
	//		base.Start ();
	//		//unlike other abilities, ultimates are only available when ultimate guage is full
	////		_status = ABILITY_STATUS.UNAVAILABLE;
	//
	////		//dev
	////		_status = ABILITY_STATUS.AVAILABLE;
	//
	//		//deduct ultimate point(s) per duration
	//		InvokeRepeating("updateUltimatePoint", 0.5f, 0.25f);
	////		print("called");
	//
	//		_pv = PhotonView.Get(this);
	//	}
	//
	//	void updateUltimatePoint(){
	////		print ("called");
	//
	//		if (_status != ABILITY_STATUS.IN_USE || !_combatHandler.GetComponent<PhotonView> ().isMine)
	//			return;
	//
	////		print ("deducting");
	//		_combatHandler.SetUltimatePoint (_combatHandler.GetUltimatePoint () - 1);
	//
	//		//deactivate when lower than 1
	//		if (_combatHandler.GetUltimatePoint () < 1) {
	//			Deactivate ();
	//		}
	//	}
	//
	//	/// <summary>
	//	/// Sets the active state of pressure point
	//	/// </summary>
	//	/// <param name="isActive">If set to <c>true</c> is active.</param>
	//	void setActivePP(bool isActive){
	//		if (!_pv.isMine)
	//			return;
	//
	//		if (PhotonNetwork.offlineMode) {
	//			_ultimatePressurePoint.gameObject.SetActive (isActive);
	//		} else
	//			_pv.RPC ("RPCSetActivePP", PhotonTargets.All, isActive);
	//	}
	//
	//	[PunRPC]
	//	void RPCSetActivePP(bool isActive){
	//		_ultimatePressurePoint.gameObject.SetActive (isActive);
	//	}
	//
	//	/// <summary>
	//	/// Activate / Use this ability.
	//	/// </summary>
	//	public override void Activate ()
	//	{
	//		if (_status == ABILITY_STATUS.AVAILABLE) {
	////			print ("Acitvate");
	////			base.Activate ();
	//
	//			_status = ABILITY_STATUS.IN_USE;
	//
	////			_ultimatePressurePoint.gameObject.SetActive (true);
	//			setActivePP(true);
	//			setControls (_isTurnAllowed, _isMovmentAllowed, _isSprintAllowed);
	//		} else if (_status == ABILITY_STATUS.IN_USE) {
	//			//if in-use, deactivate it
	//			Deactivate ();
	//		} else {
	////			print (_status.ToString ());
	//		}
	//	}
	//
	//	/// <summary>
	//	/// Deactivate this ability.
	//	/// </summary>
	//	public void Deactivate ()
	//	{
	////		print ("Deactivate");
	////		_ultimatePressurePoint.gameObject.SetActive (false);
	//		setActivePP(false);
	//		_status = ABILITY_STATUS.UNAVAILABLE;
	//		//dev
	//		setControls (false, false, false);
	//
	////		unlockControls ();
	//	}
	//
	//	public override void SetCombatHandler (CombatHandler ch)
	//	{
	//		base.SetCombatHandler (ch);
	//
	//		//for ultimate shield, handle it like hitbox, setting status handler
	//		//		_ultimatePressurePoint.SetOSH (_combatHandler.GetComponent<PlayerCharacterStatusHandler> ());
	//		_ultimateBlockingPoint.SetOSH (_combatHandler.GetComponent<PlayerCharacterStatusHandler> ());
	//		_ultimatePressurePoint.SetCombatHandler (_combatHandler);
	//	}
}
