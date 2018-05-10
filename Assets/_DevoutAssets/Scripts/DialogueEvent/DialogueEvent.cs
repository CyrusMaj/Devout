using UnityEngine;
using System.Collections;

public abstract class DialogueEvent : MonoBehaviour {
	[SerializeField] TextAsset _dialogueTextFile;		//dialogue text file
	[SerializeField] DIALOGUE_TYPE _dialogueType; //determines type of event [POP_UP or NORMAL]
	string[] _dialogueLines;				//stores each line from the text file
	int _index;								//index of current dialogue
	protected bool _isEnded;							//is the dialogue reached the end?
	DialogueLineParsed _currentDialogueLineParsed;

	protected virtual void Start(){
		_index = -1;

		readDialogueLines();//read lines of dialogue and store them
		if (_dialogueLines.Length < 1)
			print ("WARNING : Dialogue lines too short");
//		else
//			print ("dialogue loaded");
	}
	void readDialogueLines(){
		_dialogueLines = _dialogueTextFile.text.Split ("\n" [0]);		
	}
	protected void reset(){
		readDialogueLines ();
		_index = -1;
		_isEnded = false;
	}

	public DialogueLineParsed LoadNextDialogueLine(){
		_index++;
		_currentDialogueLineParsed = DialogueParser.GetDialogueLineParsed (_dialogueLines [_index]);
		checkCmd ();
		return _currentDialogueLineParsed;
	}
	public bool getIsEnded(){
		return _isEnded;
	}
	protected void register(){
		DialogueEventController.SINGLETON.AddEvent (this);
		DialogueEventController.SINGLETON.Populate ();
	}
	protected virtual void checkCmd(){
		if (_currentDialogueLineParsed.Cmd == DIALOGUE_COMMAND.END)
			_isEnded = true;	
	}

	public DIALOGUE_TYPE getDialogueType()
	{
		return _dialogueType;
	}
}
