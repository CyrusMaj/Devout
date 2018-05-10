using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevOfflineController : MonoBehaviour {

	void Awake(){
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.JoinRandomRoom ();
	}

	void Start(){
		Invoke ("init", 1f);
		//set and enable camera
		CameraController.CC.InitializeCamera ();
		GameController.GC.SetIsControlAllowed (true);
		GameController.GC.CurrentPlayerCharacter.GetComponent<PlayerMovementControl> ().SetIsMovementAllowed (true);

		updatePVs ();

		RoomLevelHelper.PLAYER_CLASS = GameController.GC.CurrentPlayerCharacter.GetComponent<PlayerCombatHandler> ().GetClass ();
	}

	void init(){
		GameController.GC.CurrentPlayerCharacter.GetComponent<PhotonView> ().TransferOwnership (PhotonNetwork.player.ID);
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
}
