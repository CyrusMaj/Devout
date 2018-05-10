using UnityEngine;
using System.Collections;

public static class DialogueParser{
	static char dialogueSep = '"';
	static char nameSepStartLeft = '[';//seperators for speaker name	
	static char nameSepEndLeft = ']';
	static char nameSepStartRight = '{';//seperators for speaker on the right
	static char nameSepEndRight = '}';
	static char cmdSepStart = '<';//command seperater. Command works as any other functionality that is triggerd via reading dialogue text files
	static char cmdSepEnd = '>';

	public static DialogueLineParsed GetDialogueLineParsed(string dialogueLine){
		DialogueLineParsed dlp;
		dlp.Dialogue = GetDialogue (dialogueLine);
		dlp.Name = GetName (dialogueLine);
		dlp.SpeakerPosition = GetSpeakerPosition (dialogueLine);
		dlp.Cmd = GetCmd (dialogueLine);
		return dlp;
	}

	static string GetDialogue(string dialogueLine){
		if(dialogueLine.Split(dialogueSep)[1] != null)
			return dialogueLine.Split(dialogueSep)[1];
		else
			return "NO_DIALOGUE_FOUND";		
	}

	static string GetName(string dialogueLine){
		if (dialogueLine.Split (new char[] { nameSepStartLeft, nameSepEndLeft }).Length > 1)//check if this is a speaker on the left
		{
			return dialogueLine.Split (new char[] { nameSepStartLeft, nameSepEndLeft }) [1];
		}
		else if (dialogueLine.Split (new char[] { nameSepStartRight, nameSepEndRight }).Length > 1)//check if this is a speaker on the right side
		{
			return dialogueLine.Split (new char[] { nameSepStartRight, nameSepEndRight }) [1];
		}
		else
			return "NO_NAME_FOUND";
	}
	static DIALOGUE_SPEAKER_POSITION GetSpeakerPosition(string dialogueLine){
		if (dialogueLine.Split (new char[] { nameSepStartLeft, nameSepEndLeft }).Length > 1)//check if this is a speaker on the left
		{
			return DIALOGUE_SPEAKER_POSITION.LEFT;
		}
		else if (dialogueLine.Split (new char[] { nameSepStartRight, nameSepEndRight }).Length > 1)//check if this is a speaker on the right side
		{
			return DIALOGUE_SPEAKER_POSITION.RIGHT;
		}
		else
			return DIALOGUE_SPEAKER_POSITION.HIDE;
	}
	static DIALOGUE_COMMAND GetCmd(string dialogueLine){
		if (dialogueLine.Split (new char[] { cmdSepStart, cmdSepEnd }).Length > 1) {//read comand
			if (dialogueLine.Split (new char[] { cmdSepStart, cmdSepEnd }) [1] == "END") {
				return DIALOGUE_COMMAND.END;
			} else {
				Debug.Log ("ERROR : PARSING CMD LINE");
				return DIALOGUE_COMMAND.NO_COMMAND;
			}
		} else
			return DIALOGUE_COMMAND.NO_COMMAND;
	}
}
public struct DialogueLineParsed{
	public string Name;
	public string Dialogue;
	public DIALOGUE_SPEAKER_POSITION SpeakerPosition;
	public DIALOGUE_COMMAND Cmd;
}
public enum DIALOGUE_SPEAKER_POSITION{
	LEFT,
	RIGHT,
	HIDE,
}
public enum DIALOGUE_TYPE{
	POP_UP,
	NORMAL,
}
public enum DIALOGUE_COMMAND{
	END,
	NO_COMMAND,
}