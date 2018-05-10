using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevWarriorAIBehavior : MonoBehaviour {
	
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
}
