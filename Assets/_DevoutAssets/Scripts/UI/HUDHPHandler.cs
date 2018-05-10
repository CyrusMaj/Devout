using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHPHandler : MonoBehaviour {
	[SerializeField]Text _text;
	[SerializeField]Image _image;

	void Start () {
		//		_text = GetComponent<Text> ();
		//		InvokeRepeating ("updateUltGaugeText", 0.5f, 0.1f);
	}
	public void UpdateHPUI(){
		updateHPGaugeText ();
		updateHPGaugeImage ();
	}
	void updateHPGaugeText(){
		if (GameController.GC.CurrentPlayerCharacter != null) {
//			int ult = GameController.GC.CurrentPlayerCharacter.GetComponent<CombatHandler> ().GetUltimatePoint();
			int HP = GameController.GC.CurrentPlayerCharacter.GetComponent<ObjectStatusHandler> ().GetHealth();
			_text.text = HP.ToString();
		}
		else
			_text.text = "N/A";
	}
	void updateHPGaugeImage(){
		if (GameController.GC.CurrentPlayerCharacter != null) {
			int HP = GameController.GC.CurrentPlayerCharacter.GetComponent<ObjectStatusHandler> ().GetHealth();
			int maxHP = GameController.GC.CurrentPlayerCharacter.GetComponent<ObjectStatusHandler> ().GetMaxHealth();
			//			print ("fillamount : " + ((float)ult / (float)CombatHandler.MAX_ULTIMATE_GUAGE));
			_image.fillAmount = ((float)HP / (float)maxHP);
		}
	}
}
