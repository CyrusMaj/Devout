using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialoguePaneHandler : MonoBehaviour
{
	[SerializeField] string _name;
	[SerializeField] Transform _group;
	[SerializeField] Text _dialogueTextLeft;			//dialogue guitext asset
	[SerializeField] Text _dialogueTextRight;
	[SerializeField] Text _nameTextLeft;		//dialogue name guitext asset
	[SerializeField] Text _nameTextRight;		//dialogue name guitext asset
	[SerializeField] Image _backgroundLeft;
	[SerializeField] Image _backgroundRight;
	[SerializeField] Image _speakerLeft;
	[SerializeField] Image _speakerRight;
	[SerializeField] Sprite _zuhraLeft;
	[SerializeField] Sprite _zuhraRight;

	public DialoguePaneHandler ()
	{
		
	}

	public void setEnabled(bool isEnabled)
	{
		_group.gameObject.SetActive (isEnabled);
		_speakerLeft.enabled = isEnabled;
		_speakerRight.enabled = isEnabled;
		_backgroundLeft.enabled = isEnabled;
		_backgroundRight.enabled = isEnabled;
	}

	public void loadDialogue(DialogueLineParsed dlp)
	{
		if (dlp.SpeakerPosition == DIALOGUE_SPEAKER_POSITION.LEFT) 
		{
			_nameTextLeft.text = dlp.Name;
			_dialogueTextLeft.text = dlp.Dialogue;//set left side
			_nameTextRight.text = "";
			_dialogueTextRight.text = "";

			_speakerLeft.enabled = true;
//			_speakerRight.enabled = false;
			_backgroundLeft.enabled = true;
			_backgroundRight.enabled = false;
			_speakerLeft.color = new Color32 (255, 255, 255, 255);
			greyImage (_speakerRight);
//			_speakerRight.color = new Color32 (100, 100, 100, 255);
		} else if (dlp.SpeakerPosition == DIALOGUE_SPEAKER_POSITION.RIGHT) {
			_nameTextLeft.text = "";
			_dialogueTextLeft.text = "";
			_nameTextRight.text = dlp.Name;
			_dialogueTextRight.text = dlp.Dialogue;//set right side

			_speakerRight.enabled = true;
//			_speakerLeft.enabled = false;
			_backgroundRight.enabled = true;
			_backgroundLeft.enabled = false;
			greyImage (_speakerLeft);
//			_speakerLeft.color = new Color32(100,100,100,255);
			_speakerRight.color = new Color32(255,255,255,255);
		} else {
			_speakerLeft.enabled = false;
			_speakerRight.enabled = false;
		}
	}

	public void loadPortrait(string speakerName, DIALOGUE_SPEAKER_POSITION pos){
		if (speakerName.ToLower () == "zuhra") {
			if (pos == DIALOGUE_SPEAKER_POSITION.LEFT) {
				_speakerLeft.sprite = _zuhraLeft;
			} else if (pos == DIALOGUE_SPEAKER_POSITION.RIGHT) {
				_speakerRight.sprite = _zuhraRight;
			}			
		}
	}

	private void greyImage(Image img)
	{//Grey image, or disable if no sprite to show
		if (img.sprite == null) {
			img.enabled = false;
		} else {
			img.color = new Color32 (100, 100, 100, 255);
		}
	}
}