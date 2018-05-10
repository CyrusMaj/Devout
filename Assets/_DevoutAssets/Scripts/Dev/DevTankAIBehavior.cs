using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTankAIBehavior : MonoBehaviour
{
	CombatHandler _combatHandler;

	public Transform movePoint;

	// Use this for initialization
	void Start ()
	{
		PhotonView pv = PhotonView.Get (this);
		_combatHandler = GetComponent<CombatHandler> ();

		Invoke ("initialOrder", 0.1f);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.O)) {
			if (movePoint) {
				GetComponent<AIMovementHandler> ().MoveTo (movePoint.position);
//				print ("Moving");
			}
		}
	}

	void initialOrder ()
	{
//		foreach (var a in _combatHandler.GetAbilities()) {
//			if (a.GetType () == typeof(AbilityBlock)) {
//				if (a.GetStatus () == ABILITY_STATUS.AVAILABLE)
//					a.Activate ();
//			}
//		}
	}
}
