using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPvPPlayerStatus : MonoBehaviour {
	public List<UIPlayerStatus> UIPlayerList = new List<UIPlayerStatus>();

	void Start(){
		InvokeRepeating ("slowUpdate", 0.5f, 0.5f);
	}

	void slowUpdate(){
		//dev
		if (PhotonNetwork.room == null)
			return;

		foreach (var uip in UIPlayerList) {
			uip.SetState (UIPlayerStatus.STATE.NOT_CONNECTED);
		}

//		//
//		int dev = 0;
//		foreach (var p in PhotonNetwork.playerList) {
//			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
//				dev++;
//			}
//		}
//		print ("uip total player : " + PhotonNetwork.playerList.Length + ", in-game : " + dev);

//		print (((CHARACTER_CLASS)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS]).ToString () + ", " + ((TEAM)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]).ToString ());

		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				UIPlayerStatus ui = UIPlayerList.Find (x => x.Class == (CHARACTER_CLASS)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS] && x.Team == (TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]);
				if ((int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH] > 0) {
					ui.SetState (UIPlayerStatus.STATE.ALIVE);
//					if(p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID] != null)
//						print ("ALIVE : " + (int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID] +", " +(int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH]);
				} else {
					ui.SetState (UIPlayerStatus.STATE.DEAD);
//					if(p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID] != null)
//						print ("DEAD : " + (int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID] +", "  +(int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH]);					
				}
			}
		}
	}
}
