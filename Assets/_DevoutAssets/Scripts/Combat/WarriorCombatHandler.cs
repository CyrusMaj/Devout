using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-02-24
/// 
/// <summary>
/// Warrior specific player combat handler
/// </summary>
public class WarriorCombatHandler : PlayerCombatHandler
{
	/// <summary>
	/// Maximum distance for interaction
	/// </summary>
	float _interactionDisatance = 2f;


	protected override void Start ()
	{
		base.Start ();
		//		Invoke ("devDelayedInit", 1f);
		//		UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Charge\n[F]");

		if (_pv.isMine && GameController.GC.CurrentPlayerCharacter == transform && UIController.SINGLETON) {
//			UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Jump\n[F]");
			UIController.SINGLETON.PlayerUIInteractInstance.SetText ("");
		}
	}

	protected override void checkForCoopInteraction ()
	{
		base.checkForCoopInteraction ();

		//		print ("checking from : " + name);
		bool interactableFound = false;
		PhotonView closestPV = _pv;

		closestPV = GetClosestPVWithinRange (_interactionDisatance);

		if (closestPV != _pv)
			interactableFound = true;

		if (interactableFound) {
			//			found, setup UI
			if (closestPV.GetComponent<PlayerCombatHandler> ().GetCoopStatus () == PLAYER_COOP_STATUS.TANK_BLOCKING && RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.WARRIOR) {
				if (UIController.SINGLETON.PlayerUIInteractInstance.Target == null || UIController.SINGLETON.PlayerUIInteractInstance.Target != closestPV.transform) {
					UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (true);
					UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (closestPV.transform);
//										print ("Set target : " + closestPV.transform.name);
					foreach (var a in _abilities) {
						if (a is AbilityCoopJump) {
							if (a.GetStatus () != ABILITY_STATUS.IN_COOLDOWN && a.GetStatus() != ABILITY_STATUS.IN_USE) {
								AbilityCoopJump acj = (AbilityCoopJump)a;
								acj.SetTarget (closestPV.transform);
								a.SetStatus (ABILITY_STATUS.AVAILABLE);
							}
						}
					}
				}
			} else {
				//interactable target is found, but not in interactable state
				//hide UI
				UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (false);
				UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (null);

				//no jump is available
				foreach (var a in _abilities) {
					if (a is AbilityCoopJump) {
						if (a.GetStatus () == ABILITY_STATUS.AVAILABLE)
							a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
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
					if (a.GetStatus () == ABILITY_STATUS.AVAILABLE)
						a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
				}
			}
		}
	}
}
