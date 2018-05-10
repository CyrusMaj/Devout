using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTimerController : MonoBehaviour {

	public float TimeScale = 1f;
	
	// Update is called once per frame
	void Update () {
		Time.timeScale = TimeScale;	
	}
}
