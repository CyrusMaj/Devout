using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCameraMovement : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Vector3.forward * Time.deltaTime * 2f);
		}
//		if (Input.GetKey (KeyCode.W)) {
//			transform.Translate (Vector3.forward * Time.deltaTime * 2f);
//		}
	}
}
