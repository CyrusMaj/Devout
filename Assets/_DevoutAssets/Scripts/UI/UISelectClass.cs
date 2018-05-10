using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UISelectClass : MonoBehaviour
{
	//	CHARACTER_CLASS _selectedClass;
	[SerializeField] Button _playButton;
	[SerializeField] List<UIClassButton> _ClassButtons = new List<UIClassButton> ();

	void Start ()
	{
//		if (!PhotonNetwork.isMasterClient)
			_playButton.interactable = false;
//		else
//			_playButton.interactable = true;

//		InvokeRepeating ("updateClassesInUse", 0f, 0.1f);

//		Hashtable playerCustomProperties = new Hashtable ();
//		playerCustomProperties.Add (RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS, CHARACTER_CLASS.NULL);
//		playerCustomProperties.Add (RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM, TEAM.NULL);

		setPlayerCustomProperties (CHARACTER_CLASS.NULL, TEAM.NULL);
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE,
				false
			}
		});	
	}

	void OnEnable ()
	{
//		print ("Enable");
		InvokeRepeating ("updateClassesInUse", 0f, 0.1f);
	}

	void OnDisable ()
	{
//		print ("Disable");
		CancelInvoke ();
	}

	CHARACTER_CLASS getCustomPropertiesClass ()
	{
		return (CHARACTER_CLASS)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS];
	}

	TEAM getCustomPropertiesTeam ()
	{
		return (TEAM)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM];
	}

	void setPlayerCustomProperties (CHARACTER_CLASS newClass, TEAM newTeam)
	{
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS,
				newClass
			}, {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM, 
				newTeam
			}, {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH, 
				0
			}
		});	
	}

	/// <summary>
	/// update currently used characters.
	/// I.E. if someone else chose warrior, it disables warrior selection
	/// </summary>
	void updateClassesInUse ()
	{		
//		print (RoomLevelHelper.ROOM_INFO.Name);

		bool warriorAvail = false;
		bool tankAvail = false;
		bool archerAvail = false;
		bool warriorAvail_Team2 = false;
		bool tankAvail_Team2 = false;
		bool archerAvail_Team2 = false;

		bool isMasterInGame = false;

		//null means not connected
		if (PhotonNetwork.room != null) {
			warriorAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE];
			tankAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE];
			archerAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE];
			warriorAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2];
			tankAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2];
			archerAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2];
			isMasterInGame = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME];
//		if (RoomLevelHelper.ROOM_INFO != null) {
//			warriorAvail = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE];
//			tankAvail = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE];
//			archerAvail = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE];
//			warriorAvail_Team2 = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2];
//			tankAvail_Team2 = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2];
//			archerAvail_Team2 = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2];
//			isMasterInGame = (bool)RoomLevelHelper.ROOM_INFO.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME];
		} else
			print ("Room info is null");

//		print ("classes available, w : " + warriorAvail.ToString () + ", t : " + tankAvail.ToString () + ", a : " + archerAvail.ToString ());
//		print ("classes available, w2 : " + warriorAvail_Team2.ToString () + ", t2 : " + tankAvail_Team2.ToString () + ", a2 : " + archerAvail_Team2.ToString ());

		bool wAvail;
		bool tAvail;
		bool aAvail;

		foreach (var b in _ClassButtons) {
			if (b.GetTeam () == TEAM.ONE) {
				wAvail = warriorAvail;
				tAvail = tankAvail;
				aAvail = archerAvail;
			} else if (b.GetTeam () == TEAM.TWO) {
				wAvail = warriorAvail_Team2;
				tAvail = tankAvail_Team2;
				aAvail = archerAvail_Team2;
			} else {
				Debug.LogWarning ("WARNING : this team is not for players");
				break;
			}

			switch (b.GetClass ()) {
			case CHARACTER_CLASS.WARRIOR:
//				if (RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.WARRIOR && !warriorAvail) {
				if (RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.WARRIOR && !wAvail && RoomLevelHelper.PLAYER_TEAM == b.GetTeam () && !(getCustomPropertiesClass () == CHARACTER_CLASS.WARRIOR && getCustomPropertiesTeam () == b.GetTeam ())) {
					//warrior selected but not avialable. deselect and diable it
					DeselectAllClassButtons ();
					b.GetComponent<Button> ().interactable = false;
					_playButton.interactable = false;																																										
				} else if (!wAvail) {
					b.GetComponent<Button> ().interactable = false;
				} else {
					b.GetComponent<Button> ().interactable = true;
				}
				break;
			case CHARACTER_CLASS.TANK:
				if (RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.TANK && !tAvail && RoomLevelHelper.PLAYER_TEAM == b.GetTeam () && !(getCustomPropertiesClass () == CHARACTER_CLASS.TANK && getCustomPropertiesTeam () == b.GetTeam ())) {
					//TANK selected but not avialable. deselect and diable it
					DeselectAllClassButtons ();
					b.GetComponent<Button> ().interactable = false;
					_playButton.interactable = false;
				} else if (!tAvail) {
					b.GetComponent<Button> ().interactable = false;
				} else {
					b.GetComponent<Button> ().interactable = true;					
				}				
				break;
			case CHARACTER_CLASS.ARCHER:
				if (RoomLevelHelper.PLAYER_CLASS == CHARACTER_CLASS.ARCHER && !aAvail && RoomLevelHelper.PLAYER_TEAM == b.GetTeam () && !(getCustomPropertiesClass () == CHARACTER_CLASS.ARCHER && getCustomPropertiesTeam () == b.GetTeam ())) {
					//ARCHER selected but not avialable. deselect and diable it
					DeselectAllClassButtons ();
					b.GetComponent<Button> ().interactable = false;
					_playButton.interactable = false;
				} else if (!aAvail) {
					b.GetComponent<Button> ().interactable = false;
				} else {
					b.GetComponent<Button> ().interactable = true;					
				}			
				break;
			default :
				Debug.LogWarning ("WARNING : CLASS BUTTON WITH UNKNOWN CLASS");
				break;
			}
		}

		if (PhotonNetwork.room != null && (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME]) {
			if (RoomLevelHelper.PLAYER_CLASS != CHARACTER_CLASS.NULL &&
			    RoomLevelHelper.PLAYER_TEAM != TEAM.NULL) {
				//enable loading a scene since class has been selected
				_playButton.interactable = true;
			}				
		} else if (!PhotonNetwork.isMasterClient) {
			//enable loading a scene since class has been selected
			_playButton.interactable = false;
//			print ("not master");
		}

		//
//		int dev = 0;
//		foreach (var p in PhotonNetwork.playerList) {
//			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
//				dev++;
//			}
//		}
//		print ("mainmenu total player : " + PhotonNetwork.playerList.Length + ", in-game : " + dev);
	}


	public void BackButton ()
	{
//		CancelInvoke ();

//		if (getCustomPropertiesClass() != CHARACTER_CLASS.NULL || getCustomPropertiesTeam() != TEAM.NULL) {
//			RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, true, RoomLevelHelper.PLAYER_TEAM);
////			print ("setting true : " + RoomLevelHelper.PLAYER_CLASS.ToString () + ", " + RoomLevelHelper.PLAYER_TEAM.ToString ());
//		} else
//			print ("NULL NOW");

		if (RoomLevelHelper.PLAYER_CLASS != CHARACTER_CLASS.NULL || RoomLevelHelper.PLAYER_TEAM != TEAM.NULL) {
			RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, true, RoomLevelHelper.PLAYER_TEAM);
		}

		_playButton.interactable = false;
		DeselectAllClassButtons ();

		Invoke ("leaveRoom", 0.2f);
//		PhotonNetwork.LeaveRoom ();

		foreach (var btn in _ClassButtons) {
			btn.GetComponent<Button> ().interactable = true;
		}
	}

	void leaveRoom ()
	{
		PhotonNetwork.LeaveRoom ();
	}

	void OnApplicationQuit ()
	{
		BackButton ();
	}

	/// <summary>
	/// Select a class by interacting with UI(button for instance)
	/// </summary>
	/// <param name="newClass">New class.</param>
	public void SetSelectedClass (CHARACTER_CLASS newClass, TEAM team)
	{
		//todo: maybe not here, but add a feature that continuesly check if the certain character is not taken(by another player joining and selecting simultaniously).
//		_selectedClass = newClass;
		//Save the selected class
		RoomLevelHelper.PLAYER_CLASS = newClass;
		RoomLevelHelper.PLAYER_TEAM = team;

		setPlayerCustomProperties (newClass, team);

		RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, false, RoomLevelHelper.PLAYER_TEAM);


		if ((bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME] || PhotonNetwork.isMasterClient) {
			//enable loading a scene since class has been selected
			_playButton.interactable = true;
		}
	}

	/// <summary>
	/// Loads the scene which is determined by when creating or joining existing room
	/// </summary>
	public void LoadScene ()
	{
//		RoomLevelHelper.SCENE currentLevel = (RoomLevelHelper.SCENE)RoomLevelHelper.ROOM_INFO.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE];
		RoomLevelHelper.SCENE currentLevel = RoomLevelHelper.CURRENT_SCENE;
		MainMenuController.MMC.StartScene (RoomLevelHelper.GetSceneName (currentLevel));
	}

	public void DeselectAllClassButtons ()
	{		
//		print ("DeselectAll called");
		RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.NULL;
		RoomLevelHelper.PLAYER_TEAM = TEAM.NULL;

		foreach (var btn in _ClassButtons) {
			//if manually disalbe(set ineractable to false) ignore it
			if (btn.GetComponent<Button> ().interactable) {
				btn.GetComponent<Button> ().image.color = btn.GetComponent<Button> ().colors.normalColor;
			}
		}
	}
}
