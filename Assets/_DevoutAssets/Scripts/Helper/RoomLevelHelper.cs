using UnityEngine;
using System.Collections;

public static class RoomLevelHelper
{
	//infos saved for selecting/creating room / class needed after switching scene
	public static RoomInfo ROOM_INFO;
	public static CHARACTER_CLASS PLAYER_CLASS = CHARACTER_CLASS.NULL;
	public static TEAM PLAYER_TEAM = TEAM.NULL;
	public static ROOM_TYPE ROOMTYPE;
	public static SCENE CURRENT_SCENE;

//	//to check who's creating(master client) before actually creating a room
//	public static bool isRoomCreator = false;

	//hash / string keys
//	public const int LEVEL_PRISON_HASH = 1;
//	public const int LEVEL_GRAVEYARD_HASH = 2;
//	public const int LEVEL_TOWN_HASH = 3;
//	public const int LEVEL_CAMP_HASH = 4;
//	public const int LEVEL_CAVE_HASH = 5;
//	public const int SCENE_MAIN_MENU = 6;
//	public const int SCENE_SURVEY = 7;
//	public const int LEVEL_PVP_1 = 8;
	public const string SCENE_PRISON_STRING = "PrisonLevel";
	public const string SCENE_MAIN_MENU_STRING = "MainMenu";
	public const string SCENE_SURVEY_STRING = "LinkToSurvey";
	public const string SCENE_PVP_1 = "PvP1";
	public const string SCENE_PVP_2 = "PvP2";
	public const string SCENE_PVP_3 = "PvP3";

	//for CustomRoomProperties(NAME for hashtable)
	public const string CUSTOM_ROOM_PROPERTY_CURRENT_SCENE = "CL";
	//currently selected & In-game classes(characters)
	public const string CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE = "WA";
	public const string CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE = "TA";
	public const string CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE = "AA";
	//for team 2 in pvp
	public const string CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2 = "WA2";
	public const string CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2 = "TA2";
	public const string CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2 = "AA2";
	//type of a room
	public const string CUSTOM_ROOM_PROPERTY_ROOM_TYPE = "RT";

	//Where master client is ingame or not
	public const string CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME = "IMI";

	//Player custom properties
	public const string CUSTOM_PLAYER_PROPERTY_CLASS = "C";
	public const string CUSTOM_PLAYER_PROPERTY_TEAM = "T";
	public const string CUSTOM_PLAYER_PROPERTY_HEALTH = "H";
	public const string CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE = "I";
	public const string CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID = "PID";

	public static string GetSceneName (SCENE levelToLoad)
	{
		string sceneName = "";	
		switch (levelToLoad) {
		case SCENE.PRISON:
			sceneName = SCENE_PRISON_STRING;
			break;
		case SCENE.MAIN_MENU:
			sceneName = SCENE_MAIN_MENU_STRING;
			break;
		case SCENE.SURVEY:
			sceneName = SCENE_SURVEY_STRING;
			break;
		case SCENE.PVP_1:
			sceneName = SCENE_PVP_1;
			break;
		case SCENE.PVP_2:
			sceneName = SCENE_PVP_2;
			break;
		case SCENE.PVP_3:
			sceneName = SCENE_PVP_3;
			break;
		}
		return sceneName;
	}

	public enum SCENE
	{
		PRISON,
		GRAVEYARD,
		TOWN,
		CAMP,
		CAVE,
		MAIN_MENU,
		SURVEY,
		PVP_1,
		PVP_2,
		PVP_3
//		PRISON = LEVEL_PRISON_HASH,
//		GRAVEYARD = LEVEL_GRAVEYARD_HASH,
//		TOWN = LEVEL_TOWN_HASH,
//		CAMP = LEVEL_CAMP_HASH,
//		CAVE = LEVEL_CAVE_HASH,
//		MAIN_MENU = SCENE_MAIN_MENU,
//		SURVEY = SCENE_SURVEY,
//		PVP_1 = LEVEL_PVP_1,
	}

	/// <summary>
	/// Sets custom property that represent usage of certain class
	/// </summary>
	/// <param name="myClass">My class.</param>
	/// <param name="InUse">If set to <c>true</c> in use.</param>
	public static void SetRoomCustomPropertyPlayerClass(CHARACTER_CLASS myClass, bool available, TEAM team = TEAM.ONE){
		//must be connected(in a room)
		if (PhotonNetwork.room == null) {
			Debug.LogWarning ("WARNING : Must be in a room to chagne custom properties");
			return;
		}
		if (team == TEAM.ONE) {
			switch (myClass) {
			case CHARACTER_CLASS.WARRIOR:
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE,
						available
					}
				});	
				break;
			case CHARACTER_CLASS.TANK:	
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE,
						available
					}
				});	
				break;
			case CHARACTER_CLASS.ARCHER:
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE,
						available
					}
				});
				break;
			default :
				Debug.LogWarning ("WARNING : CLASS BUTTON WITH UNKNOWN CLASS");
				break;
			}
		} else if (team == TEAM.TWO) {
			switch (myClass) {
			case CHARACTER_CLASS.WARRIOR:
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2,
						available
					}
				});	
				break;
			case CHARACTER_CLASS.TANK:	
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2,
						available
					}
				});	
				break;
			case CHARACTER_CLASS.ARCHER:
				PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2,
						available
					}
				});
				break;
			default :
				Debug.LogWarning ("WARNING : CLASS BUTTON WITH UNKNOWN CLASS");
				break;
			}
		} else {
			Debug.LogWarning ("WARNING : This team is not used for players");
		}
	}
}
/// <summary>
/// Class / Role of a character
/// </summary>
public enum CHARACTER_CLASS{
	WARRIOR = 0,
	TANK,	
	ARCHER,
	NULL,
}

public enum ROOM_TYPE{
	COOP,
	PVP,
}
