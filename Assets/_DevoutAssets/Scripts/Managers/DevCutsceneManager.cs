using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCutsceneManager : MonoBehaviour {
	public Animator Anim1, Anim2, Anim3;

	void Update(){
		if (Input.GetKeyDown (KeyCode.V)) {
			Anim1.SetTrigger ("DevCutscene");
		}
		else if(Input.GetKeyDown (KeyCode.B)) {
			Anim2.SetTrigger ("DevCutscene");
		}
		else if(Input.GetKeyDown (KeyCode.N)) {
			Anim3.SetTrigger ("DevCutscene");
		}
	}
}
