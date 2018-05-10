using System.Collections;
using System.Collections.Generic;
using Apex;
using Apex.Steering;
using Apex.Units;
using UnityEngine;

public class DevArcherAIBehavior : MonoBehaviour{

	CombatHandler _combatHandler;

	public Transform movePoint;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.O)) {
			if (movePoint) {
				GetComponent<AIMovementHandler> ().MoveTo (movePoint.position);
				//				GetComponent<AIMovementHandler>().
				print ("Moving");
			}
		}
	}

	// Use this for initialization
	void Start () {
//		PhotonView pv = PhotonView.Get (this);
//		_combatHandler = GetComponent<CombatHandler> ();
//
//		Invoke ("initialOrder", 2f);
//
//		IUnitFacade _unitFacade = this.GetUnitFacade ();
//		_unitFacade.Stop ();
	}

	void initialOrder(){
		foreach (var a in _combatHandler.GetAbilities()) {
			if (a is AbilityCoopRide) {
//				print ("Ability found");
				if (a.GetStatus () == ABILITY_STATUS.AVAILABLE)
					a.Activate ();
//				else
//					print ("not avilable : " + a.GetStatus ().ToString ());
			}
		}
	}
}
