using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCutsceneCamera : MonoBehaviour {
	public Transform RotationTarget;

	void Update(){
		if (Input.GetKey (KeyCode.C)) {
			transform.RotateAround (RotationTarget.position, RotationTarget.forward, 60f * Time.deltaTime);
		}
	}
}
