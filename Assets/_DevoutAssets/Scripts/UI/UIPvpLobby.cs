using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPvpLobby : MonoBehaviour {
	[SerializeField] Text _logText;

	public void JoinSelectedRoom(){
		if (PvPLobbyManager.SINGLETON.SelectedRoom == null) {
			_logText.text = "Select a room from the list to join";
		} else {
			if (PvPLobbyManager.SINGLETON.SelectedRoom.playerCount >= PvPLobbyManager.SINGLETON.SelectedRoom.maxPlayers)
				_logText.text = "Selected room is full";
			else {
				RoomLevelHelper.ROOM_INFO = PvPLobbyManager.SINGLETON.SelectedRoom;
				RoomLevelHelper.ROOMTYPE = (ROOM_TYPE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE];
				RoomLevelHelper.CURRENT_SCENE = (RoomLevelHelper.SCENE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE];

				PhotonNetwork.JoinRoom (RoomLevelHelper.ROOM_INFO.name);

				//after saving valid room / level info, display class selection screen
//				PvPLobbyManager.SINGLETON.SetActivePvpLobbySelectClass(true);
			}
		}
	}
}
