using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatus : MonoBehaviour {
	[SerializeField]GameObject _classIcon;
	[SerializeField]GameObject _whiteBackGround;
	[SerializeField]GameObject _dead;

	public TEAM Team;
	public CHARACTER_CLASS Class;

	public void SetState(STATE state){
		switch(state){
		case STATE.ALIVE:
			_classIcon.SetActive (true);
			_whiteBackGround.SetActive (true);
			_dead.SetActive (false);
			break;
		case STATE.DEAD:
			_classIcon.SetActive (true);
			_whiteBackGround.SetActive (true);
			_dead.SetActive (true);
			break;
		case STATE.NOT_CONNECTED:
			_classIcon.SetActive (false);
			_whiteBackGround.SetActive (false);
			_dead.SetActive (false);
			break;
		}
	}
	public enum STATE{
		ALIVE,
		DEAD,
		NOT_CONNECTED
	}
}
