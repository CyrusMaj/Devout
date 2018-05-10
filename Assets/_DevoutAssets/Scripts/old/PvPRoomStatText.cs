using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Text))]
public class PvPRoomStatText : MonoBehaviour {
	Text _txt;

	// Use this for initialization
	void Start () {
		_txt = GetComponent<Text> ();
		InvokeRepeating ("updateTxt", 0.1f, 0.5f);
	}
	void updateTxt(){
		if (PhotonNetwork.connected) {
			List<RoomInfo> roomList = PhotonNetwork.GetRoomList ().ToList ();
			bool roomFound = false;
			foreach (var r in roomList) {
				if (r.name == NetworkHelper.DEV_ROOM_PVP) {
					roomFound = true;
					_txt.text = "Active players : " + r.playerCount + "/" + r.maxPlayers; 
				}
			}
			if (!roomFound) {
				_txt.text =  "No active player";
			}
		} else
			_txt.text = "Not connected";
	}
}
