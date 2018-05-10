using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueEventController : MonoBehaviour {
	[SerializeField] Transform _dialogueCanvas;
//	[SerializeField] Text _dialogueTextLeft;			//dialogue guitext asset
//	[SerializeField] Text _dialogueTextRight;
//	[SerializeField] Text _dialogueNameTextLeft;		//dialogue name guitext asset
//	[SerializeField] Text _dialogueNameTextRight;		//dialogue name guitext asset
//	[SerializeField] Image _backgroundLeft;
//	[SerializeField] Image _backgroundRight;
//	[SerializeField] Image _speakerLeft;
//	[SerializeField] Image _speakerRight;
//	[SerializeField] Sprite _zuhraLeft;
//	[SerializeField] Sprite _zuhraRight;
	[SerializeField] DialoguePaneHandler _normalDialogueGroup;
	[SerializeField] DialoguePaneHandler _popupDialogueGroup;

	DialoguePaneHandler _currPane;
	float dialogueTime = 3.0f;
	List<DialogueEvent> _events = new List<DialogueEvent>();

	[HideInInspector] public static DialogueEventController SINGLETON;

	void Update()
	{
		if (isEventPopup ()) {
			//count time from when current dialogue time started
			dialogueTime -= Time.deltaTime;
			if (dialogueTime < 0) {
				DialogueEventController.SINGLETON.Populate ();
			}
		} else {//Event is NORMAL
			if (Input.GetMouseButtonDown (0)) { //check left click
				DialogueEventController.SINGLETON.Populate ();
			}
		}

	}

	void Awake(){
		
		if (SINGLETON == null)
			SINGLETON = this;
		else
			print ("WARNING : Controller loaded twice");


//		_speakerLeft.enabled = false;
//		_speakerRight.enabled = false;
//		_backgroundLeft.enabled = false;
//		_backgroundRight.enabled = false;
		_dialogueCanvas.gameObject.SetActive (false);
//		displayDialoguePane (false, _normalDialogueGroup);
//		displayDialoguePane (false, _popupDialogueGroup);
	}
	public void AddEvent(DialogueEvent eventToAdd){
		if (!_events.Contains (eventToAdd))
			_events.Add (eventToAdd);
		else
			print ("WARNING : Trying to add duplicate event");
//		_currPane = isEventPopup() ? _popupDialogueGroup : _normalDialogueGroup;
		if (isEventPopup()) {
			_popupDialogueGroup.setEnabled (true);
			_normalDialogueGroup.setEnabled (false);
		} else {
			_popupDialogueGroup.setEnabled (false);
			_normalDialogueGroup.setEnabled (true);
		}
	}
	public void Populate(){
		_events.RemoveAll (e => e.getIsEnded ());	//remove all events that has ended
		if (_events.Count < 1) {//check if event is over(empty)
			_dialogueCanvas.gameObject.SetActive (false);
//			displayDialoguePane (false, _currPane);
			GameController.GC.SetIsControlAllowed (true);
			AIController.AIC.SetIsAIMovementAllowed (true);
			return;
		}
		//load & populate GUI
		DialogueLineParsed dlp = _events [0].LoadNextDialogueLine ();
//		_normalDialogueGroup.loadPortrait (dlp.Name, dlp.SpeakerPosition);
//		_normalDialogueGroup.loadDialogue (dlp);
		if (isEventPopup ()) {//Pop-up dialogue, don't pause gameplay
			dialogueTime = 2.0f; //reset timer until next line
			_popupDialogueGroup.loadPortrait (dlp.Name, dlp.SpeakerPosition);
			_popupDialogueGroup.loadDialogue (dlp);
			//TO DO: Load separate canvas group for Normal vs. Pop-up
		} else {//Normal dialogue, pause gameplay
			GameController.GC.SetIsControlAllowed (false);
			AIController.AIC.SetIsAIMovementAllowed (false);
			_normalDialogueGroup.loadPortrait (dlp.Name, dlp.SpeakerPosition);
			_normalDialogueGroup.loadDialogue (dlp);
		}
//		_currPane.loadPortrait (dlp.Name, dlp.SpeakerPosition);
//		_currPane.loadDialogue (dlp);
//		displayDialoguePane (true, _currPane);


//		_dialogueTextLeft.text = dlp.Dialogue;
//		loadPortrait (dlp.Name, dlp.SpeakerPosition);
//		loadDialogue (dlp);
		_dialogueCanvas.gameObject.SetActive (true);
	}

//	void loadDialogue(DialogueLineParsed dlp)
//	{
//		if (dlp.SpeakerPosition == DIALOGUE_SPEAKER_POSITION.LEFT) 
//		{
//			_dialogueNameTextLeft.text = dlp.Name;
//			_dialogueTextLeft.text = dlp.Dialogue;//set left side
//			_dialogueNameTextRight.text = "";
//			_dialogueTextRight.text = "";
//
//			_speakerLeft.enabled = true;
//			_backgroundLeft.enabled = true;
//			_backgroundRight.enabled = false;
//			_speakerLeft.color = new Color32 (255, 255, 255, 255);
//			_speakerRight.color = new Color32 (100, 100, 100, 255);
//		} else if (dlp.SpeakerPosition == DIALOGUE_SPEAKER_POSITION.RIGHT) {
//			_dialogueNameTextLeft.text = "";
//			_dialogueTextLeft.text = "";
//			_dialogueNameTextRight.text = dlp.Name;
//			_dialogueTextRight.text = dlp.Dialogue;//set right side
//
//			_speakerRight.enabled = true;
//			_backgroundRight.enabled = true;
//			_backgroundLeft.enabled = false;
//			_speakerLeft.color = new Color32(100,100,100,255);
//			_speakerRight.color = new Color32(255,255,255,255);
//		} else {
//			_speakerLeft.enabled = false;
//			_speakerRight.enabled = false;
//		}
//
//	}
//	//todo : group left & right image and handle conditional efficiently
//	void loadPortrait(string speakerName, DIALOGUE_SPEAKER_POSITION pos){
//		if (speakerName.ToLower () == "zuhra") {
//			if (pos == DIALOGUE_SPEAKER_POSITION.LEFT) {
//				_speakerLeft.sprite = _zuhraLeft;
//			} else if (pos == DIALOGUE_SPEAKER_POSITION.RIGHT) {
//				_speakerRight.sprite = _zuhraRight;
//			}			
//		}
//	}

	private bool isEventPopup(){
		if (_events.Count > 0) {
			return _events [0].getDialogueType () == DIALOGUE_TYPE.POP_UP;
		}

		return false;
	}

	private void displayDialoguePane(bool isEnabled, DialoguePaneHandler currPane)
	{
		_dialogueCanvas.gameObject.SetActive (isEnabled);
		currPane.setEnabled (isEnabled);
	}
}
