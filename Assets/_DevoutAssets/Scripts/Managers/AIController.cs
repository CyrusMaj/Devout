using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : Photon.PunBehaviour
{
	public static AIController AIC;

	[SerializeField] bool _isAIMovementAllowed = true;

//	List<CharacterStatusHandler> _playerList = new List<CharacterStatusHandler> ();

	void Start ()
	{
		if (AIC == null)
			AIC = this;
	
		//update playerlists once in a while
//		InvokeRepeating ("updatePlayerList", 0.5f, 0.5f);

//		//dev : start with paused state
//		AIController.AIC.SetIsAIMovementAllowed (false);
	}

	//	void initialize ()
	//	{
	//		if (AIC == null)
	//			AIC = this;
	//
	//		//update playerlists once in a while
	//		InvokeRepeating ("updatePlayerList", 0.5f, 0.5f);
	//	}
	//
	//	public override void OnJoinedRoom ()
	//	{
	//		base.OnJoinedRoom ();
	//
	//		print ("Joined room");
	//		initialize ();
	//	}

	public bool GetIsAIMovementAllowed ()
	{
		return _isAIMovementAllowed;
	}

	public void SetIsAIMovementAllowed (bool isAllowed)
	{
		_isAIMovementAllowed = isAllowed;
	}

	void updatePlayerList ()
	{
//		_playerList.Clear ();
		foreach (var go in GameObject.FindGameObjectsWithTag(TagHelper.PLAYER)) {
			ObjectStatusHandler osh = go.GetComponent<ObjectStatusHandler> ();
//			if (osh.Alive ())
//				_playerList.Add (go.GetComponent<CharacterStatusHandler>());
		}
	}

//	public List<CharacterStatusHandler> GetPlayerList ()
//	{
//		return _playerList;
//	}
}
