using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DukeIm;

/// Created by Duke Im
/// On 1/27/2017
/// 
/// <summary>
/// Top level management of Player vs Player part of the game
/// EX) keep score and end the game when one side win and the other lose
/// Singleton
/// </summary>
public class PvpManager : Photon.PunBehaviour
{
	public static PvpManager SINGLETON;

	GameCapturePoints _game = new GameCapturePoints ();

	public GameObject _bridge_1, _bridge_2;
	Vector3 _bridgePos_1, _bridgePos_2;
	Quaternion _bridgeRot_1, _bridgeRot_2;

	public Collider LavaCollider;

	[SerializeField]
	bool _dev = false;

	[PunRPC]
	void RPCEnableLavaCollider (bool enable, float delay)
	{
		if (enable) {
			Invoke ("enableLavaCol", delay);
		} else
			Invoke ("disableLavaCol", delay);
	}

	void enableLavaCol ()
	{
		LavaCollider.enabled = true;
	}

	void disableLavaCol ()
	{
		LavaCollider.enabled = false;
	}

	public GameCapturePoints GetGameInfo ()
	{
		return _game;
	}

	public override void OnJoinedRoom ()
	{
		base.OnJoinedLobby ();
		Start ();
	}

	void Start ()
	{		
		if (PhotonNetwork.room == null)
			return;
		
		if (SINGLETON == null)
			SINGLETON = this;
		else
			Debug.LogWarning ("There are more than one controller");

		if (!PhotonNetwork.isMasterClient)
			requestUpdateGameStatus ();

		_bridgePos_1 = _bridge_1.transform.position;
		_bridgePos_2 = _bridge_2.transform.position;
		_bridgeRot_1 = _bridge_1.transform.rotation;
		_bridgeRot_2 = _bridge_2.transform.rotation;

		InvokeRepeating ("slowUpdate", 0.2f, 0.2f);
		InvokeRepeating ("timer", 1f, 1f);


//		MusicManager.SINGLETON.SetMusic (MusicManager.COMBAT_STATUS.IN_COMBAT);
	}

	void timer ()
	{
		if (PhotonNetwork.isMasterClient) {
			if (_game == null || _game.GetCurrentRound () == null) {
				return;
			}
			if (_game.State == GameCapturePoints.STATE.IN_PROGRESS) {
				_game.GetCurrentRound ().Timer -= 1f;
//				_game.SetCurrentRoundTimer(_game.GetCurrentRound ().Timer - 1f);
//				print("Current round : " + _game.CurrentRound);
				UIController.SINGLETON.UpdateTimer (_game.GetCurrentRound ().Timer);
			}
		}
	}

	void slowUpdate ()
	{
		if (PhotonNetwork.isMasterClient) {
//			#if UNITY_EDITOR
//
//			#else
			if (_game.State == GameCapturePoints.STATE.IN_PROGRESS) {
				//Round over check
				TEAM winner = roundWinnerCheck ();
				if (winner != TEAM.NULL) {
					roundOver (winner);
				} else if (_game.GetCurrentRound ().Timer < 0f) {
					roundOver (TEAM.NULL);
				}
			} else if (_game.State == GameCapturePoints.STATE.ROUND_READY_WAITING) {
				if (!isTeamEmpty () && isTeamBalanced ()) {
					PrepRound ();
				}
			}
			//update availability in lobby
			bool warriorAvail = true;
			bool tankAvail = true;
			bool archerAvail = true;
			bool warriorAvail_Team2 = true;
			bool tankAvail_Team2 = true;
			bool archerAvail_Team2 = true;
			foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
				CHARACTER_CLASS c = (CHARACTER_CLASS)pp.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CLASS];
				TEAM t = (TEAM)pp.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM];
				switch (c) {
				case CHARACTER_CLASS.ARCHER:
					if (t == TEAM.ONE)
						archerAvail = false;
					else
						archerAvail_Team2 = false;
					break;
				case CHARACTER_CLASS.WARRIOR:
					if (t == TEAM.ONE)
						warriorAvail = false;
					else
						warriorAvail_Team2 = false;
					break;
				case CHARACTER_CLASS.TANK:
					if (t == TEAM.ONE)
						tankAvail = false;
					else
						tankAvail_Team2 = false;
					break;
				default:
					break;
				}
			}
			PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE, warriorAvail }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE, tankAvail }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE, archerAvail },
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_WARRIOR_AVAILABLE_TEAM_2, warriorAvail_Team2 }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_TANK_AVAILABLE_TEAM_2, tankAvail_Team2 }, 
				{ RoomLevelHelper.CUSTOM_ROOM_PROPERTY_IS_ARCHER_AVAILABLE_TEAM_2, archerAvail_Team2 },
			});
//			#endif
		} else {
			if (_game.State == GameCapturePoints.STATE.NOT_STARTED) {
				UIController.SINGLETON.MasterGameControlText.text = "Waiting for host to start...";
				UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
			} else if (_game.State == GameCapturePoints.STATE.ROUND_READY_WAITING) {
				UIController.SINGLETON.MasterGameControlText.text = "Waiting for players...";
				UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
			} else {
				UIController.SINGLETON.MasterGameControlText.text = "";
				UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (false);
			}

		}
	}

	TEAM roundWinnerCheck ()
	{
		TEAM winner = TEAM.NULL;

		//survivor check
		int TEAM_1_Survivor = 0;
		int TEAM_2_Survivor = 0;
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				if ((int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH] > 0) {
					if ((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM] == TEAM.ONE) {
						TEAM_1_Survivor++;
					} else if ((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM] == TEAM.TWO) {
						TEAM_2_Survivor++;
					}
				}
			}
		}

		//for dev
		if (_dev) {
			if (TEAM_1_Survivor == 0 && TEAM_2_Survivor == 0)
				winner = TEAM.ONE;
		} else {		
			if (TEAM_1_Survivor > 0 && TEAM_2_Survivor < 1) {
				winner = TEAM.ONE;
			} else if (TEAM_2_Survivor > 0 && TEAM_1_Survivor < 1) {
				winner = TEAM.TWO;
			}
		}

		return winner;
	}

	void Update ()
	{
		if (PhotonNetwork.isMasterClient) {
			if (_game.State == GameCapturePoints.STATE.NOT_STARTED) {
				#if UNITY_EDITOR
				UIController.SINGLETON.MasterGameControlText.text = "Press F1 to Start Game";
				UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
				#else
				if (isTeamBalanced ()) {
					UIController.SINGLETON.MasterGameControlText.text = "Press F1 to Start Game";
					UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
				} else {
					UIController.SINGLETON.MasterGameControlText.text = "Waiting for players...";
					UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
				}
				#endif
			} else if (_game.State == GameCapturePoints.STATE.ROUND_READY_WAITING) {
				UIController.SINGLETON.MasterGameControlText.text = "Waiting for players...";
				UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
			}

			if (Input.GetKeyDown (KeyCode.F1)) {
				#if UNITY_EDITOR
				clearRound ();
				PrepRound ();
				#else
				if (_game.State == GameCapturePoints.STATE.NOT_STARTED && isTeamBalanced ()) {
				clearRound();
				PrepRound ();
				} else{
					UIController.SINGLETON.MasterGameControlText.text = "Waiting for players...";
					UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (true);
				}
				#endif
			}
		}
	}

	void PrepRound ()
	{
		//check if one of the teams is empty
//		int redTeam = 0;
//		int blueTeam = 0;
//		foreach (var p in PhotonNetwork.playerList) {
//			if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.ONE && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
//				redTeam++;
//			else if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.TWO && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
//				blueTeam++;
//		}
//		if ((redTeam < 1 || blueTeam < 0)) {
//			_game.State = GameCapturePoints.STATE.ROUND_READY_WAITING;
//			updateGameStatus ();
//			return;
//		}
		#if UNITY_EDITOR

		#else
		if (isTeamEmpty ()) {
		_game.State = GameCapturePoints.STATE.ROUND_READY_WAITING;
		updateGameStatus ();
		return;
		}
		#endif

//		print ("Timer : " + _game.GetCurrentRound ().Timer);
		_game.SetUpNewRound ();

		UIController.SINGLETON.MasterGameControlText.text = "";
		UIController.SINGLETON.MasterGameControlText.gameObject.SetActive (false);

//		print ("Preping round");
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				NetworkManager.SINGLETON.SpawnPlayer (p);
			}
		}

		UIController.SINGLETON.CountTo (GameCapturePoints.TIMER_COUNTER_ROUND, 0);
		Invoke ("StartRound", GameCapturePoints.TIMER_COUNTER_ROUND);

		MusicManager.SINGLETON.SetMusic (MusicManager.COMBAT_STATUS.ROUND_COUNTDOWN);

		_game.State = GameCapturePoints.STATE.PREPARED;

		updateGameStatus ();

		UIController.SINGLETON.UpdateTimer (_game.GetCurrentRound ().Timer);

		//dev
//		print(_game.GetCurrentRound().Number + " : " + _game.GetCurrentRound().Timer);
	}

	void newGame ()
	{
		_game = new GameCapturePoints ();
		updateGameStatus ();
	}

	void roundOver (TEAM winner)
	{
//		if (winner == TEAM.ONE) {
//			_game.Team_1_Score++;
//		} else if (winner == TEAM.TWO) {
//			_game.Team_2_Score++;
//		}

		_game.State = GameCapturePoints.STATE.ROUND_OVER;
		_game.GetCurrentRound ().Winner = winner;
//		_game.SetCurrentRoundWinner(winner);
		updateGameStatus ();

		if (_game.CurrentRound >= _game.MaxRound ||
		    _game.GetScore (winner) > (_game.MaxRound / 2)) {
			UIController.SINGLETON.GameWinner (winner);
			Invoke ("clearRound", 1f);
			Invoke ("newGame", GameCapturePoints.TIMER_COUNTER_GAME);
		} else {
			UIController.SINGLETON.RoundWinner (winner);
			Invoke ("clearRound", 1f);
			Invoke ("PrepRound", GameCapturePoints.TIMER_COUNTER_ROUND);
		}

		NetworkManager.SINGLETON.AllowControl (false);

		NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCEnableLavaCollider", false, 0f);
	}

	void clearRound ()
	{
		//destroy all views that this client owns
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				PhotonNetwork.DestroyPlayerObjects (p);
			}
		}
	}

	void StartRound ()
	{
//		print ("StartRound");
		NetworkManager.SINGLETON.AllowControl (true);

//		_game.State = GameCapturePoints.STATE.IN_PROGRESS;
		StartCoroutine (IESetState (GameCapturePoints.STATE.IN_PROGRESS, 3f));

		updateGameStatus ();

		MusicManager.SINGLETON.SetMusic (MusicManager.COMBAT_STATUS.IN_COMBAT);

		if (_bridge_1 != null) {
			PhotonNetwork.Destroy (_bridge_1.GetComponent<PhotonView> ());
		}
		if (_bridge_1 != null) {
			PhotonNetwork.Destroy (_bridge_1.GetComponent<PhotonView> ());
		}

		_bridge_1 = PhotonNetwork.Instantiate ("Bridge", _bridgePos_1, _bridgeRot_1, 0);
		_bridge_2 = PhotonNetwork.Instantiate ("Bridge", _bridgePos_2, _bridgeRot_2, 0);

		//		targeted_RPC ("RPCEnablePVs", false);
		NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCEnableLavaCollider", true, 2f);
	}

	IEnumerator IESetState (GameCapturePoints.STATE state, float delay)
	{
		yield return new WaitForSecondsRealtime (delay);
		_game.State = state;
	}

	//	[PunRPC]
	//	void RPCEnablePVs(){
	//		//enable other views
	//		foreach (var p in PhotonNetwork.playerList) {
	//			if (p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID] == null) {
	//				print ("EMPTY");
	//				continue;
	//			}
	//			print ((int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID]);
	//			PhotonView pv = PhotonView.Find ((int)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID]);
	//			if (pv != null) {
	//				pv.gameObject.SetActive (true);
	//				print ("Enabling");
	//			}
	//			else
	//				print ("WARNING : PV null");
	//		}
	//	}

	/// <summary>
	/// check if one of the teams is empty
	/// </summary>
	/// <returns><c>true</c>, if team empty was ised, <c>false</c> otherwise.</returns>
	bool isTeamEmpty ()
	{
		int redTeam = 0;
		int blueTeam = 0;
		foreach (var p in PhotonNetwork.playerList) {
			if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.ONE && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
				redTeam++;
			else if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.TWO && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
				blueTeam++;
		}
		if ((redTeam < 1 || blueTeam < 0)) {
			return true;
		}

		return false;
	}

	bool isTeamBalanced ()
	{
		int redTeam = 0;
		int blueTeam = 0;
		foreach (var p in PhotonNetwork.playerList) {
			if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.ONE && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
				redTeam++;
			else if (((TEAM)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) == TEAM.TWO && (bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE])
				blueTeam++;
		}

		if ((redTeam > 0 && blueTeam > 0) && (Mathf.Abs (redTeam - blueTeam) < 2)) {
			return true;
		} else
			return false;
	}

	void requestUpdateGameStatus ()
	{
		photonView.RPC ("RPC_GameStatusUpdate_Request", PhotonTargets.MasterClient);
	}

	[PunRPC]
	void RPC_GameStatusUpdate_Request ()
	{
		updateGameStatus ();
	}

	void updateGameStatus ()
	{
//		print ("T1 : " + _game.GetScore (TEAM.ONE) + ", T2 : " + _game.GetScore (TEAM.TWO));
		targeted_RPC ("RPCupdateGameStatus", false, _game.CurrentRound, _game.State, _game.GetScore (TEAM.ONE), _game.GetScore (TEAM.TWO));
	}

	//send rpc to players who are in same room and same scene
	void targeted_RPC (string RPC, bool excludeMyself, params object[] parameters)
	{
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				if (excludeMyself && p == PhotonNetwork.player) {
				} else {
					photonView.RPC (RPC, p, parameters);
				}
			}
		}
	}

	[PunRPC]
	void RPCupdateGameStatus (int currentRound, GameCapturePoints.STATE state, int team_1_score, int team_2_score)
	{
		_game.CurrentRound = currentRound;
		_game.State = state;
		_game.Team_1_Score = team_1_score;
		_game.Team_2_Score = team_2_score;

//		print ("T1 : " + team_1_score.ToString() + ", T2 : " + team_2_score.ToString());

		UIController.SINGLETON.Score_Team_1.text = team_1_score.ToString ();
		UIController.SINGLETON.Score_Team_2.text = team_2_score.ToString ();
	}

	//	public int Team_1_Life = 10;
	//	public int Team_2_Life = 10;
	//
	//	public int GetTeamLife (TEAM team)
	//	{
	//		int life = 0;
	//		if (team == TEAM.ONE) {
	//			life = Team_1_Life;
	//		} else if (team == TEAM.TWO) {
	//			life = Team_2_Life;
	//		} else
	//			Debug.LogWarning ("WANRING : this team is not used for pvp");
	//		return life;
	//	}
	//
	//	// Use this for initialization
	//	void Start ()
	//	{
	//		if (SINGLETON == null)
	//			SINGLETON = this;
	//		else
	//			Debug.LogWarning ("There are more than one controller");
	//
	//		InvokeRepeating ("slowUpdate", 0.25f, 0.25f);
	//
	//
	//	}
	//
	//	public int Team_1_alive = 0;
	//	public int Team_2_alive = 0;
	//
	//	void slowUpdate ()
	//	{
	//		if (!GameController.GC.IsGameOver) {
	//			//		if ((Team_1_Life < 1 || Team_2_Life < 1) && !GameController.GC.IsGameOver && (Team_1_alive < 1 || Team_2_alive < 1)) {
	//			//			print("t1 : " + Team_1_Life + ", " + Team_1_alive + ", t2 : " + Team_2_Life + ", " + Team_2_alive);
	//			if (Team_1_alive < 1 && Team_1_Life < 1) {
	//				if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
	//					UIController.SINGLETON.SetDevText ("Defeat!\nPress Q to return");
	//				} else {
	//					UIController.SINGLETON.SetDevText ("Victory!\nPress Q to return");
	//				}
	//				GameController.GC.GameOver ();
	//			} else if(Team_2_alive < 1 && Team_2_Life < 1) {
	//				if (RoomLevelHelper.PLAYER_TEAM == TEAM.ONE) {
	//					UIController.SINGLETON.SetDevText ("Victory!\nPress Q to return");
	//				} else {
	//					UIController.SINGLETON.SetDevText ("Defeat!\nPress Q to return");
	//				}
	//				GameController.GC.GameOver ();
	//			}
	//		}
	//	}
	//
	////	[PunRPC]
	////	void setTeamAlive(TEAM team, int aliveCount){
	////		if (team == TEAM.ONE)
	////			Team_1_alive = aliveCount;
	////		else if (team == TEAM.TWO)
	////			Team_2_alive = aliveCount;
	////		else
	////			print ("only team one and two are used in pvp mode");
	////	}
	////	public void SetTeamAlive(TEAM team, int aliveCount){
	////		photonView.RPC ("setTeamAlive", PhotonTargets.All, team, aliveCount);
	////	}
	//
	//	[PunRPC]
	//	void setTeamLife (int team, int lifeLeft, int aliveCount)
	//	{
	////		this.Team_1_Life = team_1_life;
	////		this.Team_2_Life = team_2_life;
	////		print ("setting life team 1 : " + this.Team_1_Life + ", team 2 : " + this.Team_2_Life);
	//
	//		if ((TEAM)team == TEAM.ONE) {
	////			Team_1_alive = aliveCount;
	//			this.Team_1_Life = lifeLeft;
	//			this.Team_1_alive = aliveCount;
	//		} else if ((TEAM)team == TEAM.TWO) {
	//			//			Team_2_alive = aliveCount;
	//			this.Team_2_Life = lifeLeft;
	//			this.Team_2_alive = aliveCount;
	//		}
	//		else
	//			print ("only team one and two are used in pvp mode");
	//	}
	//
	////	public void SetTeamLife (int t1l, int t2l)
	//	public void SetTeamLife (TEAM team, int lifeLeft, int aliveCount)
	//	{
	//		photonView.RPC ("setTeamLife", PhotonTargets.All, (int)team, lifeLeft, aliveCount);
	//	}
	//
	//	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	//	{
	//		if (!PhotonNetwork.isMasterClient)
	//			return;
	//
	//		base.OnPhotonPlayerConnected (newPlayer);
	////		SetTeamLife (TEAM.ONE, Team_1_Life, Team_1_alive);
	////		SetTeamLife (TEAM.TWO, Team_2_Life, Team_2_alive);
	//	}
}
