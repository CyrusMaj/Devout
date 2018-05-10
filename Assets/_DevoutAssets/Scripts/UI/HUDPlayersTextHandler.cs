using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Text))]
public class HUDPlayersTextHandler : MonoBehaviour
{
	Text _text;
	// Use this for initialization
	void Start ()
	{
		_text = GetComponent<Text> ();
		InvokeRepeating ("updatePlayersText", 0.5f, 0.2f);
	}

	void updatePlayersText ()
	{
		_text.text = "";
		if (GameController.GC.CurrentPlayerCharacter != null) {
//			foreach (var po in GameObject.FindGameObjectsWithTag(TagHelper.PLAYER)) {
//				if(po != null && po.GetComponent<PhotonView>() != null && po.GetComponent<PhotonView>().owner != null && po.GetComponent<PhotonView>().owner.name != null)
//					_text.text += po.GetComponent<PhotonView> ().owner.name	 + "\n";
//			}
			foreach (var p in PlayerCharacterStatusHandler.Get_PVs()) {
				if (_text == null || _text.text == null)
					return;
				_text.text += p.owner.name	 + "\n";
			}
		}
	}
}
