using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-02-24
/// 
/// <summary>
/// Tank specific player combat handler
/// </summary>
public class TankCombatHandler : PlayerCombatHandler
{
	/// <summary>
	/// Maximum distance for interaction
	/// </summary>
	float _interactionDisatance = 3f;

	/// <summary>
	/// Distance away from camera to start searching for coop ability
	/// </summary>
	float _searchPosOffset = 10f;

	/// <summary>
	/// Archer using coop ability(Ride) on this character.
	/// Null if not using the ability
	/// </summary>
	CombatHandler _archerRiding;

	public void SetArcherRiding (CombatHandler ch)
	{
		_archerRiding = ch;
	}

	protected override void Start ()
	{
		base.Start ();
//		Invoke ("devDelayedInit", 1f);
		//		UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Charge\n[F]");
		if (_pv.isMine && GameController.GC.CurrentPlayerCharacter == transform && UIController.SINGLETON) {
//			UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Charge\n[F]");
			UIController.SINGLETON.PlayerUIInteractInstance.SetText ("");
		}
	}

	void devDelayedInit ()
	{
//		UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Charge\n[F]");
	}

	protected override void checkForCoopInteraction ()
	{
		base.checkForCoopInteraction ();

		bool interactableFound = false;
		PhotonView closestPV = _pv;

		//set search start position which depends on camera direction
		Vector3 searchPos = CameraController.CC.CombatCamera.transform.position + CameraController.CC.CombatCamera.transform.forward * _searchPosOffset;

		foreach (PhotonView pv in PlayerCharacterStatusHandler.Get_PVs()) {
			//skip my own character
			//or differnt team
			if (pv == _pv ||
			    pv.GetComponent<CombatHandler> ().GetTeam () != _team)
				continue;

			//			if (pv.GetComponent<PlayerCombatHandler> () == null)
			//				continue;

			if (Vector3.Distance (searchPos, pv.transform.position) < _interactionDisatance) {
				interactableFound = true;
				if (closestPV == _pv)
					closestPV = pv;
				else if (Vector3.Distance (searchPos, pv.transform.position) < Vector3.Distance (searchPos, closestPV.transform.position)) {
					closestPV = pv;
				}
//				print ("Found");
			}
		}

		if (interactableFound) {
			//			found, setup UI
//			if (closestPV.GetComponent<PlayerCombatHandler> ().GetCoopStatus () == PLAYER_COOP_STATUS.TANK_BLOCKING && RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.WARRIOR) {
			if (UIController.SINGLETON.PlayerUIInteractInstance.Target == null || UIController.SINGLETON.PlayerUIInteractInstance.Target != closestPV.transform) {
				UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (true);
				UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (closestPV.transform);
				//										print ("Set target : " + closestPV.transform.name);
				foreach (var a in _abilities) {
					if (a is AbilityCoopCharge) {
						if (a.GetStatus () != ABILITY_STATUS.IN_COOLDOWN) {
							AbilityCoopCharge acc = (AbilityCoopCharge)a;
							acc.SetTarget (closestPV.transform);
							acc.SetStatus (ABILITY_STATUS.AVAILABLE);
						}
					}
				}
			}
		} else {
			if (UIController.SINGLETON == null)
				return;

			//no interaction found, hide UI
			UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (false);
			UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (null);

			//no jump is available
			foreach (var a in _abilities) {
				if (a is AbilityCoopJump) {
					a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
				}
			}
		}
	}
}
