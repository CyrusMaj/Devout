using UnityEngine;
using System.Collections;
using System.Linq;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Player combat control.
/// How player control character for combat
/// </summary>
[RequireComponent (typeof(PlayerCombatHandler))]
public class PlayerCombatControl : Photon.MonoBehaviour
{
	/// <summary>
	/// The combat handler of this character
	/// </summary>
	PlayerCombatHandler _playerCombatHandler;
	// Use this for initialization
	void Start ()
	{
		//		if (photonView == null || photonView.isMine) {
		if (photonView == null || photonView.isMine) {
			_playerCombatHandler = GetComponent<PlayerCombatHandler> ();
		}

		//dev
		if (GetComponent<DevPlayerAIBehavior> () != null || PhotonNetwork.offlineMode) {
			_playerCombatHandler = GetComponent<PlayerCombatHandler> ();			
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//dev
		if (GetComponent<DevPlayerAIBehavior> () != null)
			return;

//		if (Input.GetButtonDown ("Interact")) {
//			GetComponent<Rigidbody> ().AddForce (Vector3.up * 300f);
//		}

		if (photonView == null || photonView.isMine) {
			if (!_playerCombatHandler.CheckAbilitiesInUse ()//if no abilities are in use, player can use ability
			    && GameController.GC.GetIsControlAllowed ()) {
				foreach (var a in _playerCombatHandler.GetAbilities()) {					
					if (Input.GetButtonDown (a.GetInputName ())) {
						Debug.Log (a.GetInputName () + " Pressed");
						if (a.GetStatus () == ABILITY_STATUS.AVAILABLE)
							a.Activate ();//use ability
					}
				}
			}

			//Deactivate hold abilities when letting go of a button from pressing
			foreach (var a in _playerCombatHandler.GetAbilities()) {
				if (a is AbilityHold) {
					AbilityHold ah = a as AbilityHold;
					if (ah.GetStatus () == ABILITY_STATUS.IN_USE) {
						if (Input.GetButtonUp (ah.GetInputName ())) {
							ah.Deactivate ();//use ability
						}
						//Check child ability functionality
						foreach (var cah in ah.ChildAbilities) {
							if (cah.GetStatus () == ABILITY_STATUS.IN_USE) {
								if (Input.GetButtonUp (cah.GetInputName ())) {
									((AbilityHold)cah).Deactivate ();//use ability
								}
							}
						}
					}
				}
			}

			//abilities that can be used while another is active
			foreach (var a in _playerCombatHandler.GetAbilities().Where(x => x.GetStatus() == ABILITY_STATUS.IN_USE)) {
//				print (a.name + " is in use!");
				//get all child ability of used abilities
				foreach (var c in a.ChildAbilities) {
//					print (c.name + " found!");
					//if non of the child abilities are in use, ues it
//					if (a.ChildAbilities.Where (x => x.GetStatus () == ABILITY_STATUS.IN_USE).Count () < 1) {
					if (Input.GetButtonDown (c.GetInputName ())) {
						if (c.GetStatus () == ABILITY_STATUS.AVAILABLE)
							c.Activate ();//use ability
					}
//					}
				}
			}

			//archer fall down from tank
			if (GameController.GC.GetIsControlAllowed ()) {
				foreach (var a in _playerCombatHandler.GetAbilities()) {
					if (a is AbilityCoopRide) {
						if (Input.GetButtonDown (a.GetInputName ())) {
							AbilityCoopRide acr = (AbilityCoopRide)a;
							if (acr.Riding)
								acr.FallDown ();
						}
					}
				}
			}
		}
	}


}
