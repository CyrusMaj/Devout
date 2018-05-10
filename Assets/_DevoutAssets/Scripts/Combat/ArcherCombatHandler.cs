using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Written by Duke Im
/// On 2017-03-08
/// 
/// <summary>
/// Archer specific player combat handler
/// </summary>
public class ArcherCombatHandler : PlayerCombatHandler
{
	/// <summary>
	/// Maximum distance for interaction
	/// </summary>
	float _interactionDisatance = 2f;

	protected override void Start ()
	{
		base.Start ();

		if (_pv.isMine && GameController.GC.CurrentPlayerCharacter == transform && UIController.SINGLETON)
			UIController.SINGLETON.PlayerUIInteractInstance.SetText ("Ride\n[F]");
		else if (!UIController.SINGLETON)
			print ("UIController is null");
//		else
//			print (_pv.isMine.ToString () + ", " + (GameController.GC.CurrentPlayerCharacter == transform).ToString ());
	}

	public override void SetRopeCount (int newCount)
	{
		base.SetRopeCount (newCount);
		foreach (var a in _abilities) {
			foreach (var ca in a.ChildAbilities) {
				if (ca is ArcherSecondaryAttackChildRopeShot) {
					if (newCount > 0 && RopeSlotStart == null) {
						//rope avilable, rope related abilities should be available
						if (ca.GetStatus () == ABILITY_STATUS.UNAVAILABLE) {
							ca.SetStatus (ABILITY_STATUS.AVAILABLE);
						}
					} else {
						//no rope availble
						if (ca.GetStatus () == ABILITY_STATUS.AVAILABLE) {
							ca.SetStatus (ABILITY_STATUS.UNAVAILABLE);
						}
					}
				}
			}
		}
	}

	//	protected override void checkForClassSpecificAbilities ()
	//	{
	//		base.checkForClassSpecificAbilities ();
	//	}

	protected override void checkForCoopInteraction ()
	{
		base.checkForCoopInteraction ();

		//		print ("checking from : " + name);
		bool interactableFound = false;
		PhotonView closestPV = _pv;

		closestPV = GetClosestPVWithinRange (_interactionDisatance, true);

		if (closestPV != _pv)
			interactableFound = true;

		if (interactableFound) {
//			print ("interactable found");
			//			found, setup UI
			if (closestPV.GetComponent<PlayerCombatHandler> ().GetClass () == CHARACTER_CLASS.TANK) {
//				if (UIController.SINGLETON.PlayerUIInteractInstance.Target == null || UIController.SINGLETON.PlayerUIInteractInstance.Target != closestPV.transform) {
					foreach (var a in _abilities) {
						if (a is AbilityCoopRide) {
							if (a.GetStatus () != ABILITY_STATUS.IN_COOLDOWN && a.GetStatus () != ABILITY_STATUS.IN_USE) {
								AbilityCoopRide acj = (AbilityCoopRide)a;
								acj.SetTarget (closestPV.transform);
								a.SetStatus (ABILITY_STATUS.AVAILABLE);
								print ("not use");
							} else {
								print ("in use");
								UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (false);
								UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (null);
								return;
							}
						}
					}
					UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (true);
					UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (closestPV.transform);
					//										print ("Set target : " + closestPV.transform.name);
//				}
			} else {
//				print ("condition failed");
				//interactable target is found, but not in interactable state
				//hide UI
				UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (false);
				UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (null);

//				//no jump is available
//				foreach (var a in _abilities) {
//					if (a is AbilityCoopRide) {
//						a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
//					}
//				}
			}
		} else {
//			print ("interactable NOT found");
			if (UIController.SINGLETON == null)
				return;

			//no interaction found, hide UI
			UIController.SINGLETON.PlayerUIInteractInstance.SetEnabled (false);
			UIController.SINGLETON.PlayerUIInteractInstance.SetTarget (null);

//			//no jump is available
//			foreach (var a in _abilities) {
//				if (a is AbilityCoopRide) {
//					a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
//				}
//			}
		}
	}

	public override bool CheckAbilitiesInUse ()
	{
		bool isInUse = false;
		foreach (var a in _abilities) {
			if (a.GetStatus () == ABILITY_STATUS.IN_USE && !(a is AbilityCoopRide)) {
				isInUse = true;
				break;
			}
		}
		return isInUse;
	}

	public void SetRide(bool riding){
		if (PhotonNetwork.offlineMode)
			setRide (riding);
		else
			NetworkManager.SINGLETON.Targeted_RPC (_pv, "RPCSetRide", riding);
	}
	void setRide(bool riding){
		print (riding.ToString ());
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (riding) {
			rb.isKinematic = true;
			rb.GetComponent<Collider> ().enabled = false;	
		} else {
			rb.isKinematic = false;
			rb.GetComponent<Collider> ().enabled = true;	
		}
	}
	[PunRPC]
	void RPCSetRide(bool riding){
		setRide (riding);
	}
}
