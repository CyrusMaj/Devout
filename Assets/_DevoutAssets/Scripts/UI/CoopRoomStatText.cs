using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Text))]
public class CoopRoomStatText : MonoBehaviour {
	Text _txt;

	// Use this for initialization
	void Start () {
		_txt = GetComponent<Text> ();
		InvokeRepeating ("updateTxt", 0.1f, 0.5f);
	}
	void updateTxt(){
		if (PhotonNetwork.connected) {
			_txt.text = "Connected"; 
		} else
			_txt.text = "Not connected";
	}
}