using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueHandler : MonoBehaviour {
	public TextAsset dialogueTextFile;		//dialogue text file
	public Text DialogueText;			//dialogue guitext asset
	public Text DialogueNameText;		//dialogue name guitext asset

	string[] dialogueLines;					//stores each line from the text file
	int index;								//index of current dialogue
	char dialogueSep, 
	nameSepStart, nameSepEnd,			//seperators for speaker name				
	nameSepStartLis, nameSepEndLis,			//seperators for listener name
	cmdSepStart, cmdSepEnd;					//command seperater
	bool isSpeaker;							//is current person talking the speaker(face located left side of the screen) or the listener(right side of the screen)
	bool isEnd;							//is the dialogue reached the end?

	// Use this for initialization
	void Start () {
		//init / assign
		isSpeaker = true;
		isEnd = false;
		index = 0;
		dialogueSep = '"';
		nameSepStart = '[';
		nameSepEnd = ']';
		nameSepStartLis = '{';
		nameSepEndLis = '}';
		cmdSepStart = '<';
		cmdSepEnd = '>';

		//read lines of dialogue and store them
		dialogueLines = dialogueTextFile.text.Split ("\n" [0]);

		//get the first dialogue and name into the gui
		PopulateCurrentDialogueGUIText ();
	}
	
	// Update is called once per frame
	void Update () {	
	}

	//when dialogueguitext is clicked, update with the next dialogue
	public void PopulateNextDialogueGUIText()
	{
		index++;
		PopulateCurrentDialogueGUIText ();
	}

	//pupulate with current dialogue and name
	void PopulateCurrentDialogueGUIText()
	{
		DialogueText.text = getCurrentDialogue ();
		DialogueNameText.text = getCurrentDialogueName ();
		//set name to be speaker or listener location(left or right)
		if(isSpeaker)
			DialogueNameText.alignment = TextAnchor.MiddleLeft;
		else
			DialogueNameText.alignment = TextAnchor.MiddleRight;
	}

	//returns current dialogue
	string getCurrentDialogue()
	{
		if (isEnd)
		{
			return "";
		}
		else if(dialogueLines [index].Split (new char[] { cmdSepStart, cmdSepEnd }).Length > 1)//read comand
		{
			if(dialogueLines [index].Split (new char[] { cmdSepStart, cmdSepEnd })[1] == "END")
			{
				//disable parent gameobject which is a dialogue gui
				transform.parent.gameObject.SetActive(false);
				isEnd = true;
				return "";
			}
			else
				return "ERROR READING CMD LINE";
		}
		if(dialogueLines[index].Split(dialogueSep)[1] != null)
			return dialogueLines[index].Split(dialogueSep)[1];
		else
			return "NO_DIALOGUE_FOUND";
	}

	//returns current speaker / listener
	string getCurrentDialogueName()
	{		
		if (isEnd)
		{
			return "";
		}
		else if(dialogueLines [index].Split (new char[] { cmdSepStart, cmdSepEnd }).Length > 1)//read comand
		{
			if(dialogueLines [index].Split (new char[] { cmdSepStart, cmdSepEnd })[1] == "END")//check if end of the file
			{
				//disable parent gameobject which is a dialogue gui
				transform.parent.gameObject.SetActive(false);
				isEnd = true;
				return "";
			}
			else
				return "ERROR READING CMD LINE";
		}
		else if (dialogueLines [index].Split (new char[] { nameSepStart, nameSepEnd }).Length > 1)//check if this is a speaker
		{
			isSpeaker = true;
			return dialogueLines [index].Split (new char[] { nameSepStart, nameSepEnd }) [1];
		}
		else if (dialogueLines [index].Split (new char[] { nameSepStartLis, nameSepEndLis }).Length > 1)//check if this is a listener
		{
			isSpeaker = false;
			return dialogueLines [index].Split (new char[] { nameSepStartLis, nameSepEndLis }) [1];
		}
		else
			return "NO_NAME_FOUND";
	}
}
