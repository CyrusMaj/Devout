using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public static CameraController CC;
	public Camera CombatCamera;
	public Camera CutsceneCamera;

	public Transform MovePos;

	// Use this for initialization
	void Awake () {
		if (CC == null) {
			CC = this;
		}
	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_EDITOR

//		if(Input.GetKeyDown(KeyCode.C))
//			DevCutsceneCam();
//		
//		if (_devCamMove) {
//			CutsceneCamera.transform.position = Vector3.MoveTowards (CutsceneCamera.transform.position, MovePos.position, 4f * Time.deltaTime);
//			CutsceneCamera.transform.rotation = Quaternion.Lerp(CutsceneCamera.transform.rotation, MovePos.rotation, 2f * Time.deltaTime);
//		}

		#endif
	}

	public void InitializeCamera(){
		ThirdPersonOrbitCam tpoc = CameraController.CC.CombatCamera.GetComponent<ThirdPersonOrbitCam> ();
		tpoc.Initialize (GameController.GC.CurrentPlayerCharacter);
	}
		
	[ContextMenu ("Switch Cutscene")]
	public void SwitchCutsceneCamera()
	{
		CutsceneCamera.enabled = true;
		CombatCamera.enabled = false;

		GameController.GC.SetIsControlAllowed (false);
		AIController.AIC.SetIsAIMovementAllowed (false);
	}

	public void SwitchCombatCamera()
	{
		GameController.GC.SetIsControlAllowed (true);
		AIController.AIC.SetIsAIMovementAllowed (true);
		CombatCamera.enabled = true;
		CutsceneCamera.enabled = false;
	}

	bool _devCamMove = false;

	public void DevCutsceneCam(){
		_devCamMove = true;
		CutsceneCamera.depth = 2f;
		CutsceneCamera.transform.SetParent (transform.root);
	}
}
