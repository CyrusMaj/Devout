using UnityEngine;
using System.Collections;

/// <summary>
/// Controls singleplayer scene specific values & methods
/// </summary>
public class SingleplayerController : MonoBehaviour {
	public GameObject ZuhraPrefab;
	void Start(){
		startSingleplayer ();
	}

	/// <summary>
	/// Starts the singleplayer by instantiating player character and setting character
	/// </summary>
	void startSingleplayer ()
	{
		if(PhotonNetwork.connected)
			PhotonNetwork.Disconnect ();
		PhotonNetwork.offlineMode = true;
		GameController.GC.SetIsControlAllowed (true);
		GameObject playerInstance = (GameObject)GameObject.Instantiate (ZuhraPrefab, new Vector3 (0f, 5f, 0f), Quaternion.identity);
		GameController.GC.CurrentPlayerCharacter = playerInstance.transform;
//		GameController.GC.mainCamera.GetComponent<ThirdPersonCameraCore> ().target = playerInstance.transform;
//		GameController.GC.mainCamera.GetComponent<ThirdPersonCameraCore> ().enabled = true;
//		GameController.GC.mainCamera.GetComponent<ThirdPersonCameraFreeForm> ().enabled = true;
//		GameController.GC.InitializeCamera();
		CameraController.CC.InitializeCamera ();

//		playerInstance.GetComponent<CombatHandler> ().SetTeam (TEAM.ONE);

		print ("Staring singleplayer");
	}
}
