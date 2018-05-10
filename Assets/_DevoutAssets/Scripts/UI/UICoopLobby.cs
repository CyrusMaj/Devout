using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICoopLobby : MonoBehaviour {
	[SerializeField] Text _logText;

	public void JoinSelectedRoom(){
		if (CoopLobbyController.CLC.SelectedRoom == null) {
			_logText.text = "Select a room from the list to join";
		} else {
			if (CoopLobbyController.CLC.SelectedRoom.playerCount >= CoopLobbyController.CLC.SelectedRoom.maxPlayers)
				_logText.text = "Selected room is full";
			else {
				RoomLevelHelper.ROOM_INFO = CoopLobbyController.CLC.SelectedRoom;
				RoomLevelHelper.ROOMTYPE = (ROOM_TYPE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE];
				RoomLevelHelper.CURRENT_SCENE = (RoomLevelHelper.SCENE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE];


//				RoomLevelHelper.SCENE currentLevel = (RoomLevelHelper.SCENE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_LEVEL];
				//after saving valid room / level info, display class selection screen
//				CoopLobbyController.CLC.SetActiveCoopLobbySelectClass(true);
//				MainMenuController.MMC.StartScene (RoomLevelHelper.GetSceneName(currentLevel));	
			}
		}
	}
}
