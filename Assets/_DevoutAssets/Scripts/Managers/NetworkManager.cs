using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PhotonView))]
public class NetworkManager : Photon.PunBehaviour
{
	public static NetworkManager SINGLETON;
	PhotonView _photonView;

	[SerializeField] Transform _spawnAreaTank;
	[SerializeField] Transform _spawnAreaWarrior;
	[SerializeField] Transform _spawnAreaArcher;
	[SerializeField] Transform _respawnArea;
	[SerializeField] Transform _spawnAreaTank2;
	[SerializeField] Transform _spawnAreaWarrior2;
	[SerializeField] Transform _spawnAreaArcher2;
	[SerializeField] Transform _respawnArea2;

	[SerializeField]float _spawnDelay = 4f;

	void Start ()
	{		
		if (SINGLETON == null)
			SINGLETON = this;
		else
			Debug.LogWarning ("There are more than one controller");

		_photonView = PhotonView.Get (this);

		if (PhotonNetwork.connected) {
			if (!PhotonNetwork.offlineMode) {
				//ready to receieve RPC calls
				PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
						RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE,
						true
					}
				});	

				if (!PhotonNetwork.isMasterClient) {
					//if not master, join room after done loading
//					PhotonNetwork.JoinRoom (RoomLevelHelper.ROOM_INFO.name);
					updatePVs ();
				} else {
					PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
							RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME,
							true
						}
					});	
//					RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, false, RoomLevelHelper.PLAYER_TEAM);
//					setPlayerNames ();
					//if master, spawn player when scene loaded
//					SpawnPlayer (_spawnDelay);
					//also make the room visible for other players to join from lobby
//					PhotonNetwork.room.visible = true;
				}
			} else {
				//singleplayer mode
				PhotonNetwork.JoinRandomRoom ();
			}
		} else {
			//for dev
			//offline mode
//			PhotonNetwork.offlineMode = true;

			PhotonNetwork.ConnectUsingSettings ("DEV");
		}
	}

	//dev controlls
	void Update ()
	{
		#if UNITY_EDITOR
		if (PhotonNetwork.isMasterClient) {
			if (Input.GetKeyUp (KeyCode.P)) {
				if (AIController.AIC.GetIsAIMovementAllowed ()) {
					AIController.AIC.SetIsAIMovementAllowed (false);
				} else {
					AIController.AIC.SetIsAIMovementAllowed (true);
				}
			}
		}
		if (GameController.GC.CurrentPlayerCharacter == null) {
			if (GameController.GC.IsGameOver) {
				if (Input.GetKeyUp (KeyCode.Q)) {
					Cursor.lockState = CursorLockMode.None;
					StartCoroutine (CoroutineHelper.IELoadAsyncScene (RoomLevelHelper.GetSceneName (RoomLevelHelper.SCENE.SURVEY)));
				}				
			} else {
//				if (Input.GetKeyUp (KeyCode.R)) {
//					UIController.SINGLETON.SetDevText ("");
//					NetworkManager.SINGLETON.spawnPlayer ();
//
//					if (RoomLevelHelper.ROOMTYPE == ROOM_TYPE.COOP) {
//						GameController.GC.SetLives (GameController.GC.Lives - 1);
//					} else {
////						if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
////							PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life - 1, PvpManager.SINGLETON.Team_2_Life);
////						} else if (RoomLevelHelper.PLAYER_TEAM == TEAM.TWO) {
////							PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life, PvpManager.SINGLETON.Team_2_Life - 1);
////						}
//					}
//				}
			}
		}
		#endif
	}


	#region for dev only

	public override void OnJoinedLobby ()
	{
		base.OnJoinedLobby ();

		RoomOptions ro = new RoomOptions ();
		ro.MaxPlayers = 6;

		ro.IsVisible = true;

		RoomLevelHelper.CURRENT_SCENE = RoomLevelHelper.SCENE.PVP_3;
		RoomLevelHelper.ROOMTYPE = ROOM_TYPE.PVP;

		PhotonNetwork.JoinOrCreateRoom (NetworkHelper.DEV_ROOM_PVP, ro, TypedLobby.Default);
	}

	public override void OnJoinedRoom ()
	{
		base.OnJoinedRoom ();

		//ready to receieve RPC calls
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE,
				true
			}
		});	

		if (!PhotonNetwork.isMasterClient) {
			updatePVs ();
			//find randome available team & class
			bool warriorAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE];
			bool tankAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE];
			bool archerAvail = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE];
			bool warriorAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2];
			bool tankAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2];
			bool archerAvail_Team2 = (bool)PhotonNetwork.room.CustomProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2];

			if (warriorAvail_Team2) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.WARRIOR;
				RoomLevelHelper.PLAYER_TEAM = TEAM.TWO;
			} else if (warriorAvail) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.WARRIOR;
				RoomLevelHelper.PLAYER_TEAM = TEAM.ONE;
			}  else if (tankAvail_Team2) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.TANK;
				RoomLevelHelper.PLAYER_TEAM = TEAM.TWO;
			} else if (tankAvail) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.TANK;
				RoomLevelHelper.PLAYER_TEAM = TEAM.ONE;
			} else if (archerAvail) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.ARCHER;
				RoomLevelHelper.PLAYER_TEAM = TEAM.ONE;
			}else if (archerAvail_Team2) {
				RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.ARCHER;
				RoomLevelHelper.PLAYER_TEAM = TEAM.TWO;
			} else
				print ("NO AVAILABLE CLASS and team");
		} else {

			RoomLevelHelper.PLAYER_CLASS = CHARACTER_CLASS.WARRIOR;
			RoomLevelHelper.PLAYER_TEAM = TEAM.ONE;

			PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { 
				//			{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_LEVEL, RoomLevelHelper.LEVEL_PRISON_HASH }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_CURRENT_SCENE, RoomLevelHelper.CURRENT_SCENE }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE, false }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE, true }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE, true},
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2, true }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2, true }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2, true },
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE, RoomLevelHelper.ROOMTYPE },
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME, true }
			});

//			PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
//					RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_MASTER_IN_GAME,
//					true
//				}
//			});	
		}

		RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, false, RoomLevelHelper.PLAYER_TEAM);

		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS,
				RoomLevelHelper.PLAYER_CLASS
			},{
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM,
				RoomLevelHelper.PLAYER_TEAM
			},{
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH,
				0
			}
		});	

		//get random available class & team
	}

	//for dev, when creating room in this scene
	public override void OnCreatedRoom ()
	{
		base.OnCreatedRoom ();
	}
	//	//for dev, when creating room in this scene
	//	void joinCOOPRoom ()
	//	{
	//		RoomOptions ro = new RoomOptions ();
	//		ro.MaxPlayers = 3;
	//		PhotonNetwork.JoinOrCreateRoom (NetworkHelper.DEV_ROOM_COOP, ro, TypedLobby.Default);
	//	}
	//	//for dev, when creating room in this scene
	//	public override void OnJoinedLobby ()
	//	{
	//		joinCOOPRoom ();
	//	}

	#endregion

	[PunRPC]
	void RPC_Remove_Photon_View_List_Player (int viewID)
	{
		PhotonView pv = PhotonView.Find (viewID);
		remove_Photon_View_List_Player (pv);
	}

	public void Remove_Photon_View_List_Player (PhotonView pv)
	{
		if (PhotonNetwork.offlineMode) {
			remove_Photon_View_List_Player (pv);
		} else {
//			_photonView.RPC ("RPC_Remove_Photon_View_List_Player", PhotonTargets.All, pv.viewID);
			targeted_RPC ("RPC_Remove_Photon_View_List_Player", pv.viewID);
		}
	}

	void remove_Photon_View_List_Player (PhotonView pv)
	{
		if (PlayerCharacterStatusHandler.Get_PVs ().Contains (pv))
			PlayerCharacterStatusHandler.Get_PVs ().Remove (pv);
	}

	/// <summary>
	/// Update Photonview list that contains all AIStatusHandler(including playercharacters)
	/// </summary>
	public void UpdatePVs ()
	{
		//Update EnemyAIHandler List
		if (PhotonNetwork.offlineMode) {
			updatePVs ();
		} else {
//			_photonView.RPC ("RPCUpdatePVs", PhotonTargets.All);
			targeted_RPC ("RPCUpdatePVs");
		}
	}

	//send rpc to players who are in same room and same scene
	void targeted_RPC (string RPC, params object[] parameters)
	{
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				_photonView.RPC (RPC, p, parameters);
			}
		}
	}

	//send rpc to players who are in same room and same scene
	public void Targeted_RPC (PhotonView pv, string RPC, params object[] parameters)
	{
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				pv.RPC (RPC, p, parameters);
			}
		}
	}

	[PunRPC]
	void RPCUpdatePVs ()
	{
		updatePVs ();
	}

	void updatePVs ()
	{
		AIStatusHandler.PHOTON_VIEW_LIST.Clear ();
		foreach (AIStatusHandler aish in GameObject.FindObjectsOfType(typeof(AIStatusHandler)) as AIStatusHandler[]) {
			PhotonView pv = PhotonView.Get (aish.gameObject);
//			print (pv.name);
			if (!AIStatusHandler.PHOTON_VIEW_LIST.Contains (pv)) {
				AIStatusHandler.PHOTON_VIEW_LIST.Add (pv);
			}
		}
//		print ("LEngth : " + (GameObject.FindObjectsOfType (typeof(AIStatusHandler)) as AIStatusHandler[]).Length);
	}

	/// <summary>
	/// Removew a photonview from the list of units(both AI and playercharacter)
	/// </summary>
	/// <param name="pv">Pv.</param>
	public void RemovePV (PhotonView pv)
	{
		if (PhotonNetwork.offlineMode) {
			removePV (pv);
		} else {
//			_photonView.RPC ("RPCRemovePV", PhotonTargets.All, pv.viewID);
			targeted_RPC ("RPCRemovePV", pv.viewID);
		}
	}
	//note : Maybe handle this & update of PV list locally to reduce overhead?
	[PunRPC]
	void RPCRemovePV (int viewID)
	{
		PhotonView pv = PhotonView.Find (viewID);
		removePV (pv);
	}

	void removePV (PhotonView pv)
	{
		if (AIStatusHandler.PHOTON_VIEW_LIST.Contains (pv))
			AIStatusHandler.PHOTON_VIEW_LIST.Remove (pv);
	}

	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected (newPlayer);

		//Tell the newly joined player to spawn player character
//		if (PhotonNetwork.isMasterClient)
//			_photonView.RPC ("RPCSpawnPlayer", newPlayer);
	}

	/// <summary>
	/// Called after switching to a new MasterClient when the current one leaves.
	/// </summary>
	/// <remarks>This is not called when this client enters a room.
	/// The former MasterClient is still in the player list when this method get called.</remarks>
	/// <param name="newMasterClient">New master client.</param>
	public override void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		base.OnMasterClientSwitched (newMasterClient);

//		print ("Master switched, otherplayercount : " + PhotonNetwork.otherPlayers.Length);

		//transfer ownerhsip of this AI to the new master
		foreach (var pv in AIStatusHandler.Get_PVs()) {
			pv.TransferOwnership (newMasterClient);
			print ("Ownership transferred");
		}
//		}
	}

	/// <summary>
	/// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
	/// </summary>
	/// <remarks>When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
	/// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
	/// of several seconds.</remarks>
	/// <param name="player">Player.</param>
	public override void OnPhotonPlayerDisconnected (PhotonPlayer player)
	{   
		base.OnPhotonPlayerDisconnected (player);
		Debug.Log ("Player Disconnected " + player.name + ", PlayerCharacterList : " + PlayerCharacterStatusHandler.Get_PVs ().Count);
		//Destroy player character when leaving a room
		List<PhotonView> pvlist = new List<PhotonView> ();
		foreach (var pv in PlayerCharacterStatusHandler.Get_PVs()) {
			//seems like owner is null when the player is disconnected
			//or transfered ownership to sceneview
			//or still has old owner info
			if (pv.owner == null || pv.isSceneView || pv.owner == player) {
				pvlist.Add (pv);
//				print ("Owner found, deleting owener objects");
			}
//			//dev
//			print("owner id : " + pv.ownerId +", owner name : " + pv.owner.name + ", disconnected player id : " + player.ID);
		}

		//new master takes control of destroying disconnect player character
		if (PhotonNetwork.isMasterClient) {
			foreach (var pp in pvlist) {
				if (pp.GetComponent<PlayerCombatHandler> () != null)
					RoomLevelHelper.SetRoomCustomPropertyPlayerClass (pp.GetComponent<PlayerCombatHandler> ().GetClass (), true, pp.GetComponent<PlayerCombatHandler> ().GetTeam ());
				PhotonNetwork.Destroy (pp);
				PhotonNetwork.RemoveRPCs (pp);				
//				_photonView.RPC ("RPC_Remove_Photon_View_List_Player", PhotonTargets.All, pp.viewID);
				Remove_Photon_View_List_Player (pp);
			}
		}
	}

	[PunRPC]
	void RPCSpawnPlayer ()
	{
//		SpawnPlayer (_spawnDelay);
		spawnPlayer ();
	}

	public void SpawnPlayer (PhotonPlayer owner)
	{
		_photonView.RPC ("RPCSpawnPlayer", owner);
	}

	public void AllowControl (bool allow)
	{
		targeted_RPC ("RPCAllowControl", allow);
	}

	[PunRPC]
	void RPCAllowControl (bool allow)
	{
		if (allow)
			GameController.GC.SetIsControlAllowed (true, false);
		else
			GameController.GC.SetIsControlAllowed (false, false);
	}

	//	public void SpawnPlayer (float delay = 0f)
	//	{
	//		//dev
	//		Invoke ("spawnPlayer", delay);
	//	}

	void spawnPlayer ()
	{
		//pvp only
		if (RoomLevelHelper.ROOMTYPE == ROOM_TYPE.PVP) {
//			if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
////				PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life - 1, PvpManager.SINGLETON.Team_2_Life);
////				PvpManager.SINGLETON.Team_1_alive++;
////				PvpManager.SINGLETON.SetTeamAlive (TEAM.ONE, PvpManager.SINGLETON.Team_1_alive + 1);
//				PvpManager.SINGLETON.SetTeamLife(TEAM.ONE, PvpManager.SINGLETON.Team_1_Life - 1, PvpManager.SINGLETON.Team_1_alive + 1);
//			} else if (RoomLevelHelper.PLAYER_TEAM == TEAM.TWO) {
////				PvpManager.SINGLETON.Team_1_alive++;
////				PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life, PvpManager.SINGLETON.Team_2_Life - 1);
//				//				PvpManager.SINGLETON.SetTeamAlive (TEAM.ONE, PvpManager.SINGLETON.Team_2_alive + 1);
//				PvpManager.SINGLETON.SetTeamLife(TEAM.TWO, PvpManager.SINGLETON.Team_2_Life - 1, PvpManager.SINGLETON.Team_2_alive + 1);
//			}
		}
//		if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
//			PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life - 1, PvpManager.SINGLETON.Team_2_Life);
//		} else if (RoomLevelHelper.PLAYER_TEAM == TEAM.TWO) {
//			PvpManager.SINGLETON.SetTeamLife (PvpManager.SINGLETON.Team_1_Life, PvpManager.SINGLETON.Team_2_Life - 1);
//		}
//
		Transform spawnArea = _spawnAreaWarrior;
		//spawn player character and set it to the GameController
		GameObject playerInstance = null;
		switch (RoomLevelHelper.PLAYER_CLASS) {
		case CHARACTER_CLASS.WARRIOR:
//			playerInstance = PhotonNetwork.Instantiate ("Warrior", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
				playerInstance = PhotonNetwork.Instantiate ("Warrior_R", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaWarrior;
			} else {
				playerInstance = PhotonNetwork.Instantiate ("Warrior_B", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaWarrior2;
			}
			HUDManager.HUD_M.Warrior.SetActive (true);
			HUDManager.HUD_M.UltimateUI.gameObject.SetActive (true);
			break;
		case CHARACTER_CLASS.TANK:
//			playerInstance = PhotonNetwork.Instantiate ("Tank", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
				playerInstance = PhotonNetwork.Instantiate ("Tank_R", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaTank;
			} else {
				playerInstance = PhotonNetwork.Instantiate ("Tank_B", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaTank2;
			}
			HUDManager.HUD_M.Tank.SetActive (true);
			HUDManager.HUD_M.UltimateUI.gameObject.SetActive (true);
			break;
		case CHARACTER_CLASS.ARCHER:
//			playerInstance = PhotonNetwork.Instantiate ("Archer", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
				playerInstance = PhotonNetwork.Instantiate ("Archer_R", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaArcher;
			} else {
				playerInstance = PhotonNetwork.Instantiate ("Archer_B", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
				spawnArea = _spawnAreaArcher2;
			}
			HUDManager.HUD_M.Archer.SetActive (true);
			HUDManager.HUD_M.UltimateUI.gameObject.SetActive (false);
			break;
		default:
			Debug.LogWarning ("WARNING : Character not specified set when spawning");
			break;
		}

		HUDManager.HUD_M.HPUI.gameObject.SetActive (true);

		if (playerInstance == null) {
			Debug.LogWarning ("WARNING : Character not instantiated, aborting");
			return;
		}
		GameController.GC.CurrentPlayerCharacter = playerInstance.transform;
		HUDManager.HUD_M.HPUI.UpdateHPUI ();

		//set and enable camera
		CameraController.CC.InitializeCamera ();
		CameraController.CC.CombatCamera.orthographic = false;


		playerInstance.transform.rotation = spawnArea.rotation;

//		spawnArea = getSpawnPos (spawnArea);

		playerInstance.transform.position = spawnArea.position;

//		dev
//		GameController.GC.SetIsControlAllowed (false);
//		GameController.GC.SetIsControlAllowed (true);

		//set player team
		playerInstance.GetComponent<CombatHandler> ().SetTeam (RoomLevelHelper.PLAYER_TEAM);

		//
		playerInstance.SetActive (true);

		GameController.GC.SetIsControlAllowed (false, false);
	}

	Transform getSpawnPos (Transform defaultTrans)
	{
		Transform spawnArea = defaultTrans;
		if (GameController.GC.DeathCount > 0 || !PhotonNetwork.isMasterClient) {
//			if (PlayerCharacterStatusHandler.Get_PVs (true).Count > 0) {
//				//there are alive chr to respawn to(not tested)
//				//WIP
//				int rnd = Random.Range (0, PlayerCharacterStatusHandler.Get_PVs (true).Count);
//				Vector3? pos = getNearAlivePlayerPos (PlayerCharacterStatusHandler.Get_PVs (true) [rnd].GetComponent<AIMovementHandler> ());
//				if (pos != null)
//					spawnArea.position = (Vector3)pos;
//				else
//					spawnArea.position = _respawnArea.position;					
//			} else {
			//respawn but no chr to respawn to
			if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE)
				spawnArea.position = _respawnArea.position;
			else
				spawnArea.position = _respawnArea2.position;
//			}
		} else {
			//spawn to original position if this is spawning for the first time when the game starts
//			if ((PlayerCharacterStatusHandler.Get_PVs (true).Count > 0)) {
//				//there are alive chr to respawn to
//				int rnd = Random.Range (0, PlayerCharacterStatusHandler.Get_PVs (true).Count);
//				Vector3? pos = getNearAlivePlayerPos (PlayerCharacterStatusHandler.Get_PVs (true) [rnd].GetComponent<AIMovementHandler> ());
//				if (pos != null)
//					spawnArea.position = (Vector3)pos;
//			}
		}
		return spawnArea;
	}

	/// <summary>
	/// Gets the near alive player position to spawn a revived / connecting player
	/// </summary>
	/// <returns>The near alive player position.</returns>
	Vector3? getNearAlivePlayerPos (AIMovementHandler alivePlayer)
	{
		Vector3? pos = null;
		int rand = Random.Range (0, alivePlayer.MeleeSlots.Count);
		foreach (var slot in alivePlayer.GetComponent<AIMovementHandler>().MeleeSlots) {
			//check if position is movable
			if (alivePlayer.IsCellWalkable (slot.POS)) {
				pos = slot.POS;
			}	
		}

		return pos;
	}

	List<Vector3> createGrid (Vector3 center, int lengthX = 10, int lengthY = 10, float areaSize = 1f)
	{
		List<Vector3> grid = new List<Vector3> ();
		//grid is disregarding up position(Y)
		Vector3 startingPos = new Vector3 (center.x - (lengthX * areaSize), center.y, center.z - (lengthY * areaSize));
		for (int i = 0; i < lengthX; i++) {
			for (int j = 0; j < lengthY; j++) {
				grid.Add (new Vector3 (startingPos.x + (i * areaSize), startingPos.y, startingPos.z + (j * areaSize)));
			}
		}
		return grid;
	}

	/// <summary>
	/// Set player names
	/// </summary>
	//	void setPlayerNames ()
	//	{
	//		if (PhotonNetwork.isMasterClient)
	//			PhotonNetwork.player.name = "(Master)" + System.Environment.UserName;
	//		else
	//			PhotonNetwork.player.name = System.Environment.UserName;
	//	}

	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	/// <remarks>In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
	/// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().</remarks>
	public override void OnDisconnectedFromPhoton ()
	{
		base.OnDisconnectedFromPhoton ();
		Cursor.lockState = CursorLockMode.None;

//		StartCoroutine (CoroutineHelper.IELoadAsyncScene (RoomLevelHelper.GetSceneName (RoomLevelHelper.SCENE.MAIN_MENU)));
	}

	public override void OnConnectionFail (DisconnectCause cause)
	{
		base.OnConnectionFail (cause);

		print ("[DEV_BUILD_LOG][Duke Im] - Disconnection cause : " + cause.ToString ());
	}

	public void SetParent(int pvChild_ID, int pvParent_ID){
		targeted_RPC ("RPCSetParent", pvChild_ID, pvParent_ID);
	}
	[PunRPC]
	void RPCSetParent(int pvChild_ID, int pvParent_ID){
		Transform c = PhotonView.Find (pvChild_ID).transform;
		Transform p = PhotonView.Find (pvParent_ID).transform;
		c.SetParent (p);
	}
}
