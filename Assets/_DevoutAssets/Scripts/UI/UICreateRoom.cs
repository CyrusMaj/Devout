using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// User interface flow control for creating a room
/// Takes needed information to create a room and if sufficient, create & join the room
/// </summary>
public class UICreateRoom : Photon.PunBehaviour
{
	public InputField RoomNameInput;
	public Text TextLog;
	//todo : scene may be selected when creating a room
	public RoomLevelHelper.SCENE LevelToLoad;
	[SerializeField] ROOM_TYPE _roomType;
	//	[SerializeField]

	public void CreateRoom ()
	{
		bool isValid = true;
		string invalidInfo = "";

		if (RoomNameInput.text.Length < 1) {
			isValid = false;
			invalidInfo += " [RoomName too short]";
		}

		foreach (var room in PhotonNetwork.GetRoomList()) {
			if (room.name == RoomNameInput.text) {
				isValid = false;
				invalidInfo += " [RoomName already exists]";
				break;
			}
		}

		if (isValid) {
			createRoom ();
			TextLog.text = "[Room created]";
		} else {
			TextLog.text = invalidInfo;
		}
	}

	public void ExitRoom ()
	{
//		PhotonNetwork.LeaveRoom ();
//		TextLog.text = "Room creation canceled";
	}

	void createRoom ()
	{
		RoomOptions ro = new RoomOptions ();
		if (_roomType == ROOM_TYPE.COOP)
			ro.MaxPlayers = 3;
		else
			ro.MaxPlayers = 6;

		ro.IsVisible = true;

		ro.CustomRoomPropertiesForLobby = new string[] {
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE,
			RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME
		};

		//level may be selected when creating a room
		ro.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable () { 
//			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_LEVEL, RoomLevelHelper.LEVEL_PRISON_HASH }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE, LevelToLoad }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE, true }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE, true }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE, true },
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2, true }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2, true }, 
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2, true },
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE, _roomType },
			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME, false }
		};

		RoomLevelHelper.ROOMTYPE = _roomType;
		RoomLevelHelper.CURRENT_SCENE = LevelToLoad;

		PhotonNetwork.CreateRoom (RoomNameInput.text, ro, TypedLobby.Default);

//		//after creating a valid room, display class selction UI
//		CoopLobbyController.CLC.SetActiveCoopLobbySelectClass (true);
	}

	public override void OnCreatedRoom ()
	{
		RoomLevelHelper.ROOM_INFO = PhotonNetwork.room;
		if ((ROOM_TYPE)PhotonNetwork.room.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE] == ROOM_TYPE.COOP)
			CoopLobbyController.CLC.SetActiveCoopLobbySelectClass (true);
		else
			PvPLobbyManager.SINGLETON.SetActivePvpLobbySelectClass (true);
	}
}
