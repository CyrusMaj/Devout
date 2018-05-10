using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerCoopController : Photon.PunBehaviour
{
	//	public GameObject ZuhraPrefab;
	//	[SerializeField] List<SpawnArea> _playerSpawnAreas = new List<SpawnArea>();
//	[SerializeField] List<Transform> _playerSpawnAreas = new List<Transform> ();

	void Start ()
	{

//		if (PhotonNetwork.connected) {
////			joinCOOPRoom ();
//		} else {
//			PhotonNetwork.autoCleanUpPlayerObjects = false;
////			PhotonNetwork.player.name = System.Environment.UserName;
//			//this is for dev purpose only(when connected starting from this scene
//			PhotonNetwork.ConnectUsingSettings (NetworkHelper.VERSION);
//		}
	}

	public override void OnJoinedRoom ()
	{
//		print ("Room joined in MultiplayerPvP Scene");
		//		startPvP ();
//		Invoke ("spawnPlayer", 2f);//Todo : make this handled beside just putting time. Make sure other onplayerjoin is executed before instantiating player
		//
//		//set player names
//		if (PhotonNetwork.isMasterClient)
//			PhotonNetwork.player.name = "(Master)" + System.Environment.UserName;
//		else
//			PhotonNetwork.player.name = System.Environment.UserName;
			
//		PhotonNetwork.room.visible = true;

//		print ("autoclean : " + PhotonNetwork.autoCleanUpPlayerObjects.ToString ());
//		print ("autoclean_Room : " + PhotonNetwork.room.autoCleanUp);
	}

//	void joinCOOPRoom ()
//	{
//		RoomOptions ro = new RoomOptions ();
//		ro.MaxPlayers = 3;
//		PhotonNetwork.JoinOrCreateRoom (NetworkHelper.DEV_ROOM_COOP, ro, TypedLobby.Default);
//	}

//	void spawnPlayer ()
//	{
//		if (!PhotonNetwork.offlineMode) {
//			//spawn player character and set it to the GameController
//			GameObject playerInstance = PhotonNetwork.Instantiate ("Zuhra_Network", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
//			GameController.GC.CurrentPlayerCharacter = playerInstance.transform;
//			//set and enable camera
//			ThirdPersonCameraCore tpcc = GameController.GC.mainCamera.GetComponent<ThirdPersonCameraCore> ();
//			tpcc.target = playerInstance.transform;
//			tpcc.enabled = true;
//			tpcc.GetComponent<ThirdPersonCameraFreeForm> ().enabled = true;
//
//			//Assigning player team
////			playerInstance.GetComponent<CombatHandler> ().SetTeam (TEAM.ONE);		
////			print ("Team set to " + team.ToString ());
//
//			//Assigning player spawn position
//			_playerSpawnAreas.Shuffle ();
//			foreach (var psa in _playerSpawnAreas) {
////				if (!psa.IsInUse ()) {
////					psa.SetSpawnArea (playerInstance, TEAM.ONE);
////					playerInstance.transform.position = psa.transform.position;
////					break;
////				}
//				playerInstance.transform.position = psa.transform.position;
//				break;
//			}
//		} else
//			print ("ERROR : PhotonNetwork.offlineMode in multiplayer");
//
//		GameController.GC.SetIsControlAllowed (true);
//	}

	public override void OnJoinedLobby ()
	{
//		joinCOOPRoom ();		
	}
//
//	public override void OnConnectedToMaster ()
//	{
//		joinCOOPRoom ();
//	}

	public override void OnPhotonCreateRoomFailed (object[] codeAndMsg)
	{
		print ("Creating room failed");
	}

	public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
	{
		print ("joining room failed");
	}
}
