using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-27
/// 
/// <summary>
/// Provides access to all changable HUD elements
/// </summary>
public class HUDManager : MonoBehaviour {
	public static HUDManager HUD_M;

	void Start(){
		if (HUD_M == null)
			HUD_M = this;
		else
			Debug.LogWarning ("WARNING : More than one manager");
	}

	void OnApplicationFocus(bool hasFocus){
		//dev
//		if (hasFocus) {
//			Cursor.lockState = CursorLockMode.Locked;
//		}
	}

	public HUDULTHandler UltimateUI;
	public HUDHPHandler HPUI;
	public HUDHPTextHandler HPText;
	public GameObject Warrior, Tank, Archer;
}
