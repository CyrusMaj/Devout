using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apex.Steering.Components;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// AI status handler
/// Status for AI only
/// </summary>
public class AIStatusHandler : CharacterStatusHandler
{
	/// <summary>
	/// List of photonviews of AIs
	/// </summary>
	public static List<PhotonView> PHOTON_VIEW_LIST = new List<PhotonView> ();

	/// <summary>
	/// Gets the list of photonviews of bith player and non-player characters
	/// </summary>
	/// <returns>The P vs.</returns>
	/// <param name="checkAlive">If set to <c>true</c> only returns alive character</param>
	public static List<PhotonView> Get_ALL_PVs (bool checkAlive = false)
	{
		List<PhotonView> AICharacterPVs = new List<PhotonView> ();
		PHOTON_VIEW_LIST.RemoveAll (x => x == null);
		foreach (var p in PHOTON_VIEW_LIST) {
			CharacterStatusHandler csh = p.GetComponent<CharacterStatusHandler> ();
			if (csh != null) {
				if (checkAlive && !csh.Alive ()) {
					//if checkalive and dead, don't add this one
					continue;
				} else {
					AICharacterPVs.Add (p);
				}
			} else {
				Debug.Log ("WARNING : Element of this list must have CharacterStatusHandler as its component");
			}
		}
		return AICharacterPVs;
	}

	/// <summary>
	/// Gets the list of photonviews of non-player AI characters
	/// </summary>
	/// <returns>The P vs.</returns>
	/// <param name="checkAlive">If set to <c>true</c> only returns alive character</param>
	public static List<PhotonView> Get_PVs (bool checkAlive = false)
	{
		List<PhotonView> AICharacterPVs = new List<PhotonView> ();
		PHOTON_VIEW_LIST.RemoveAll (x => x == null);
		foreach (var p in PHOTON_VIEW_LIST) {
			CharacterStatusHandler csh = p.GetComponent<CharacterStatusHandler> ();
			if (csh != null) {
//				if (csh is AIStatusHandler) {
				if(csh.Type == TYPE.NON_PLAYER_AI){
					if (checkAlive && !csh.Alive ()) {
						//if checkalive and dead, don't add this one
						continue;
					} else {
						AICharacterPVs.Add (p);
					}
				}
			} else {
				Debug.Log ("WARNING : Element of this list must have CharacterStatusHandler as its component");
			}
		}
		return AICharacterPVs;
	}

	protected override void Start ()
	{
		base.Start ();
	}

	/// <summary>
	/// Called when this character dies
	/// </summary>
	protected override void die ()
	{
		base.die ();

		//do below only for AIStatusHandlers(Non-player units)
//		if (!(this is AIStatusHandler))
//			return;

		if (Type != TYPE.NON_PLAYER_AI)
			return;

//		//dev
//		DevAISpawner.DAIS.DisplayRemainingEnemyCount ();

		//Network
		//destroy object after duration
		destroyObject (20f);
//		Invoke ("destroyObject", 20f);

		//remove this character from dynamic obstacle
		AIMovementHandler.UpdateDynamicAINumbers();
	}

	protected void destroyObject (float delay)
	{
		Invoke ("destroyObject", delay);
	}

	/// <summary>
	/// Destroy this object
	/// For example, this is called when ragdoll is timed out
	/// </summary> 
	void destroyObject ()
	{
		if (PhotonNetwork.offlineMode) {
			Destroy (gameObject);
		} else {
			if (_photonView.isMine) {
				NetworkManager.SINGLETON.RemovePV (_photonView);
				PhotonNetwork.Destroy (_photonView);
				PhotonNetwork.RemoveRPCs (_photonView);	
			}
		}
	}

	public override void OnPhotonInstantiate (PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate (info);

		//Update list of views
		NetworkManager.SINGLETON.UpdatePVs ();
	}
}
