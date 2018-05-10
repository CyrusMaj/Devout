using UnityEngine;
using System.Collections;

public class PlayerUIHandler : MonoBehaviour {

	//player UI
	[SerializeField] GameObject _playerUIPrefab;
	[SerializeField] Transform _target;
//	PlayerInfo _pi;

//	public void SetPlayerInfo(PlayerInfo pi){
//		_pi = pi;
//	}

	// Use this for initialization
	void Start () {
		if (_playerUIPrefab != null) {
			GameObject _uiGo = Instantiate (_playerUIPrefab) as GameObject;
//			_uiGo.GetComponent<PlayerUIOverhead> ().SetTarget (_pi);
//			_uiGo.GetComponent<PlayerUIOverhead> ().SetTarget (PlayerListController.PLC.GetPlayerInfo(_target));
			_uiGo.GetComponent<PlayerUIOverhead> ().SetTarget (_target);
			//			_uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
		} else
			print ("WARNING : Missing prefab");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
