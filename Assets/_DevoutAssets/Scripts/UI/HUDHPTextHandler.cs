using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class HUDHPTextHandler : MonoBehaviour {
	Text _text;
	// Use this for initialization
	void Start () {
		_text = GetComponent<Text> ();
		InvokeRepeating ("updateHPText", 0.5f, 0.1f);
	}
	void updateHPText(){
		if (GameController.GC.CurrentPlayerCharacter != null) {
			int hp = GameController.GC.CurrentPlayerCharacter.GetComponent<ObjectStatusHandler> ().GetHealth ();
			_text.text = "HP : " + hp;
		}
		else
			_text.text = "HP : N/A";
	}
}
