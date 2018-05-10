using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Deprecated, TBD
/// </summary>
public class MultiplayerPvPController : Photon.PunBehaviour {
////	public GameObject ZuhraPrefab;
//	[SerializeField] List<SpawnArea_old> _playerSpawnAreas = new List<SpawnArea_old>();
//
//	void Start () {
//		if (PhotonNetwork.connected) {
//			joinPvPRoom ();
//		} else {
//			PhotonNetwork.player.name = System.Environment.UserName;
//			PhotonNetwork.ConnectUsingSettings(NetworkHelper.VERSION);
//		}
//	}
//
//	public override void OnJoinedLobby ()
//	{
//		joinPvPRoom ();		
//	}
//
//	public override void OnConnectedToMaster ()
//	{
//		joinPvPRoom ();
//	}
//		
//	public override void OnCreatedRoom ()
//	{
////		PhotonNetwork.autoCleanUpPlayerObjects = false;
//		print ("Created a room, room count : " + PhotonNetwork.GetRoomList ().Length);
//	}
//	public override void OnPhotonCreateRoomFailed (object[] codeAndMsg)
//	{
//		print ("Creating room failed");
//	}
//	public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
//	{
//		print ("joining room failed");
//	}
//	public override void OnJoinedRoom ()
//	{
//		print ("Room joined in MultiplayerPvP Scene");
////		startPvP ();
//		Invoke("startPvP", 2f);//Todo : make this handled beside just putting time. Make sure other onplayerjoin is executed before instantiating player
//		//
//		PhotonNetwork.room.visible = true;
//	}
//	void joinPvPRoom(){
////		PhotonNetwork.autoCleanUpPlayerObjects = false;
//		RoomOptions ro = new RoomOptions ();
//		ro.MaxPlayers = 4;
//		PhotonNetwork.JoinOrCreateRoom (NetworkHelper.DEV_ROOM_PVP, ro, TypedLobby.Default);
//	}
//	void startPvP(){
//		if (!PhotonNetwork.offlineMode) {
//			//spawn player character and set it to the GameController
//			GameObject playerInstance = PhotonNetwork.Instantiate ("Zuhra_Network", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
//			GameController.GC.CurrentPlayerCharacter = playerInstance.transform;
//			//set and enable camera
////			GameController.GC.InitializeCamera();
//			CameraController.CC.InitializeCamera ();
////			ThirdPersonCameraCore tpcc = GameController.GC.mainCamera.GetComponent<ThirdPersonCameraCore> ();
////			tpcc.target = playerInstance.transform;
////			tpcc.enabled = true;
////			tpcc.GetComponent<ThirdPersonCameraFreeForm> ().enabled = true;
//
//			//Assigning player team
//			TEAM team = TEAM.NULL;
//			List<TEAM> lstTeam = new List<TEAM> ();
//			print ("go count : " + GameObject.FindGameObjectsWithTag (TagHelper.PLAYER).Length);
//			foreach (var go in GameObject.FindGameObjectsWithTag(TagHelper.PLAYER)) {
//				print ("go : " + go.name);
//				if (go == playerInstance)
//					continue;
//				CombatHandler ch = go.GetComponent<CombatHandler> ();
//				print ("go has team : " + ch.GetTeam ());
//				lstTeam.Add (ch.GetTeam ());
//			}
//			if (!lstTeam.Contains (TEAM.ONE))
//				team = TEAM.ONE;
//			else if (!lstTeam.Contains (TEAM.TWO))
//				team = TEAM.TWO;
//			else if (!lstTeam.Contains (TEAM.THREE))
//				team = TEAM.THREE;
//			else if (!lstTeam.Contains (TEAM.FOUR))
//				team = TEAM.FOUR;
//			else
//				print ("ERROR, NO TEAM AVAILABLE");
//			playerInstance.GetComponent<CombatHandler> ().SetTeam (team);		
//			print ("Team set to " + team.ToString ());
//
//			//Assigning player spawn position
//			_playerSpawnAreas.Shuffle ();
//			foreach (var psa in _playerSpawnAreas) {
//				if (!psa.IsInUse ()) {
//					psa.SetSpawnArea (playerInstance, team);
//					playerInstance.transform.position = psa.transform.position;
//					break;
//				}
//			}
//		} else
//			print ("ERROR : PhotonNetwork.offlineMode in multiplayer");
//
//		GameController.GC.SetIsControlAllowed (true);
//	}
//
//	public override void OnPhotonPlayerConnected (PhotonPlayer player)
//	{
//		Debug.Log ("Player Connected " + player.name);
//
//		//update team stat for the newly joined player
////		CombatHandler ch = GameController.GC.CurrentPlayerCharacter.GetComponent<CombatHandler> ();
////		ch.SetTeam (ch.GetTeam ());
//
////		//update health for the newly joined player
////		ObjectStatusHandler osh = ch.GetComponent<ObjectStatusHandler> ();
////		osh.AddHealth (0);
//	}
//
//	public override void OnPhotonPlayerDisconnected (PhotonPlayer player)
//	{   
//		Debug.Log ("Player Disconnected " + player.name);
//		if (PhotonNetwork.isMasterClient) {
//			PhotonNetwork.RemoveRPCs (player);
//			PhotonNetwork.DestroyPlayerObjects (player);
//		}
//	}
//
//	void setTeam (GameObject playerInstance)
//	{
//		//		_playerInstance.GetComponent<CombatHandler>().SetTeam(CombatHelper.getAvailableTeam ());//dev
//		TEAM team = new TEAM ();
//		List<TEAM> lstTeam = new List<TEAM> ();
//		print ("go count : " + GameObject.FindGameObjectsWithTag (TagHelper.PLAYER).Length);
//		foreach (var go in GameObject.FindGameObjectsWithTag(TagHelper.PLAYER)) {
//			print ("go : " + go.name);
//			if (go == playerInstance)
//				continue;
//
//			CombatHandler ch = go.GetComponent<CombatHandler> ();
//			print ("go has team : " + ch.GetTeam ());
//			lstTeam.Add (ch.GetTeam ());
//		}
//		//
//		if (!lstTeam.Contains (TEAM.ONE))
//			team = TEAM.ONE;
//		else if (!lstTeam.Contains (TEAM.TWO))
//			team = TEAM.TWO;
//		else if (!lstTeam.Contains (TEAM.THREE))
//			team = TEAM.THREE;
//		else if (!lstTeam.Contains (TEAM.FOUR))
//			team = TEAM.FOUR;
//		else
//			print ("ERROR, NO TEAM AVAILABLE");
//
//		playerInstance.GetComponent<CombatHandler> ().SetTeam (team);
//	}
}
