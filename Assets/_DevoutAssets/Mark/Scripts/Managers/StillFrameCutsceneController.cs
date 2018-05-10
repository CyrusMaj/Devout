using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//#if UNITY_EDITOR
//Note from Duke
//UnityEditor namespace is platform dependent(Meaning it shouldn't be used other than debugging perpose)
//using UnityEditor;
//#endif
public class StillFrameCutsceneController : MonoBehaviour {


	[HideInInspector] public static StillFrameCutsceneController SINGLETON;
	private StillFrameCutscene currScene;
	public CutsceneUIHandler cutsceneUI;

	// Use this for initialization
	void Awake () {
		if (SINGLETON == null)
			SINGLETON = this;
		else
			print ("WARNING : Controller loaded twice");

		//Note from Duke
		//Assets shouldn't be loaded using path name
		//Instead, pre-load locally by setting variables in this class, for example, Public Text CutSceneText; and drag and drop asset into the inspector
//		Image img = (Image) AssetDatabase.LoadAssetAtPath ("Assets/_DevoutAssets/Mark/CutscenePrototype/testPic.jpg", typeof(Image));
//		TextAsset text = (TextAsset) AssetDatabase.LoadAssetAtPath ("Assets/_DevoutAssets/Mark/CutscenePrototype/CutscenePrototypeDialogue.txt", typeof(TextAsset));
//		CutsceneFrame testFrame = new CutsceneFrame (img, text.text);

//		List<CutsceneFrame> frames = new List<CutsceneFrame>();
//		frames.Add (testFrame);

//		StillFrameCutscene testCS = new StillFrameCutscene (frames);
		currScene.LoadFrame (cutsceneUI);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) 
		{
			currScene.AdvanceFrame ();
			currScene.LoadFrame (cutsceneUI);
		}
	}
}
