using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// Written by Duke Im
/// On 2016-10-27
/// 
/// <summary>
/// Head-up display for ultimate handler
/// Handles applying changes of ultimate gauge status to UI elements
/// </summary>
public class HUDULTHandler : MonoBehaviour {
	[SerializeField]Text _text;
	[SerializeField]Image _image;
	[SerializeField]GameObject _key;

	void Start () {
//		_text = GetComponent<Text> ();
//		InvokeRepeating ("updateUltGaugeText", 0.5f, 0.1f);
	}
	public void UpdateUltUI(){
		updateUltGaugeText ();
		updateUltGaugeImage ();
	}
	void updateUltGaugeText(){
		if (GameController.GC.CurrentPlayerCharacter != null) {
			int ult = GameController.GC.CurrentPlayerCharacter.GetComponent<CombatHandler> ().GetUltimatePoint();
			_text.text = ult.ToString() + "%";
		}
		else
			_text.text = "N/A";
	}
	void updateUltGaugeImage(){
		if (GameController.GC.CurrentPlayerCharacter != null) {
			int ult = GameController.GC.CurrentPlayerCharacter.GetComponent<CombatHandler> ().GetUltimatePoint();
//			print ("fillamount : " + ((float)ult / (float)CombatHandler.MAX_ULTIMATE_GUAGE));
			_image.fillAmount = ((float)ult / (float)CombatHandler.MAX_ULTIMATE_POINT);
			if (_image.fillAmount == 1f)
				_key.SetActive (true);
			else
				_key.SetActive (false);				
		}
	}
}
