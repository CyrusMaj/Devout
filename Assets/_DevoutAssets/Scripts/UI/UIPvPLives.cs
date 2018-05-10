using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPvPLives : MonoBehaviour {
	public Image RedTeam;
	public Text BlueText, RedText;

	// Use this for initialization
	void Start () {
		BlueText.text = "Blue Team : ??";
		RedText.text = "Red Team : ??";
//		InvokeRepeating ("slowUpdate", 1f, 1f);
	}

//	void slowUpdate(){
//		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode || RoomLevelHelper.ROOMTYPE != ROOM_TYPE.PVP)
//			return;
//		
//		int r = PvpManager.SINGLETON.Team_1_Life;
//		int b = PvpManager.SINGLETON.Team_2_Life;
//		RedTeam.fillAmount = ((float)r) / (float)(r + b);
//
//		BlueText.text = "Blue Team : " + b;
//		RedText.text = "Red Team : " + r;
//	}
}
