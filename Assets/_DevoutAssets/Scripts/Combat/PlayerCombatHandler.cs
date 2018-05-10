using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player combat handler
/// Handles player specific combat that is non-control related
/// </summary>
public class PlayerCombatHandler : CombatHandler
{
	/// <summary>
	/// Status of player character used for coop-ability such as jumping over Tank character when it's blocking
	/// </summary>
	PLAYER_COOP_STATUS _coopStatus = PLAYER_COOP_STATUS.DEFAULT;

	public Transform RopeTiePoint;

	/// <summary>
	/// List of cooperative abilities this player character can use
	/// </summary>
	//	[SerializeField] protected List<Ability> _coopAbilities = new List<Ability> ();

	/// <summary>
	/// The class of this character
	/// </summary>
	[SerializeField]CHARACTER_CLASS _class;

	public CHARACTER_CLASS GetClass ()
	{
		return _class;
	}

	protected override void Start ()
	{
		base.Start ();

		_pv = PhotonView.Get (this);

//		if (_pv.isMine
//			&& transform == GameController.GC.CurrentPlayerCharacter) {//also check for dev purpose(so below doesn't get called for all player characters in testing)
		if (_pv.isMine && GameController.GC.CurrentPlayerCharacter == transform) {
			InvokeRepeating ("checkForCoopInteraction", 1f, 0.25f);
//			InvokeRepeating ("checkForClassSpecificAbilities", 1f, 0.25f);

			//initialize UI
			if (HUDManager.HUD_M != null)
				HUDManager.HUD_M.UltimateUI.UpdateUltUI ();
		}
	}

	/// <summary>
	/// Sets the coop status of this character
	/// Network
	/// </summary>
	/// <param name="newCoopStatus">New coop status.</param>
	public void SetCoopStatus (PLAYER_COOP_STATUS newCoopStatus)
	{
//		print ("Called");

		if (PhotonNetwork.offlineMode)
			setCoopStatus (newCoopStatus);
		else
			_pv.RPC ("RPCSetCoopStatus", PhotonTargets.All, newCoopStatus);
	}

	[PunRPC]
	protected void RPCSetCoopStatus (PLAYER_COOP_STATUS newCoopStatus)
	{
		setCoopStatus (newCoopStatus);
	}

	void setCoopStatus (PLAYER_COOP_STATUS newCoopStatus)
	{
		_coopStatus = newCoopStatus;
//		print ("coop status : " + newCoopStatus.ToString ());
	}

	public PLAYER_COOP_STATUS GetCoopStatus ()
	{
		return _coopStatus;
	}

	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected (newPlayer);

		//Update coopstatus since new player joined
		if (_pv.isMine)
			_pv.RPC ("RPCSetCoopStatus", PhotonTargets.All, _coopStatus);
	}

	void checkForRopeTieAvailability ()
	{
		if (_ropeCount < 1)
			return;
		
		bool interactableFound = false;
		PhotonView closestPV = _pv;
		float ropeTieDistance = 2f;
		closestPV = GetClosestPVWithinRange (ropeTieDistance);

		if (closestPV != _pv)
			interactableFound = true;
		
		foreach (var a in _abilities) {
			if (a is AbilityTieRope) {			
				if (interactableFound) {					
					if (a.GetStatus () != ABILITY_STATUS.IN_COOLDOWN) {
						AbilityTieRope acj = (AbilityTieRope)a;
						acj.Target = closestPV.transform;
						a.SetStatus (ABILITY_STATUS.AVAILABLE);
//						print ("rope target found");
					}
				} else {
					a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
				}

			}
		}
	}

	/// <summary>
	/// Checks for coop interaction availability
	/// If avilable, set and display Interaction
	/// </summary>
	protected virtual void checkForCoopInteraction ()
	{
		checkForRopeTieAvailability ();
	}

	/// <summary>
	/// Gets the closest enemyAI from EnemyList
	/// </summary>
	/// <returns>The closest enemy.</returns>
	/// <param name="searchPos">Search position.</param>
	public CombatHandler GetClosestEnemy (Vector3 searchPos)
	{
		CombatHandler closestCH = null;
		foreach (var ch in CombatHandler.GET_ENEMIES(_team, true)) {
			if (ch != null) {
				if (closestCH == null) {
					closestCH = ch;
				} else if (Vector3.Distance (ch.transform.position, searchPos) < Vector3.Distance (closestCH.transform.position, searchPos)) {
					closestCH = ch;				
				}
			} else {
				Debug.LogWarning ("WARNING : this photonview must have combathandler as its component");
			}
		}
		return closestCH;
	}

	/// <summary>
	/// Gets the closest enemyAI from EnemyList
	/// </summary>
	/// <returns>The closest enemy distance.</returns>
	/// <param name="searchPos">Search position.</param>
	public float GetClosestEnemyDistance (Vector3 searchPos)
	{
		float distance = 9999f;
		foreach (var ch in CombatHandler.GET_ENEMIES(_team, true)) {
			if (Vector3.Distance (ch.transform.position, searchPos) < distance) {
				distance = Vector3.Distance (ch.transform.position, searchPos);
			}
		}
		return distance;
	}

	/// <summary>
	/// Gets the closest photonview(character) within range.
	/// </summary>
	/// <returns>The closest within range.</returns>
	public PhotonView GetClosestPVWithinRange (float range, bool OnlyAlly = false)
	{
		PhotonView closestPV = _pv;

		foreach (PhotonView pv in PlayerCharacterStatusHandler.Get_PVs()) {
			//skip my own character
			//or differnt team
			if (pv == _pv)
				continue;

			if (OnlyAlly && pv.GetComponent<CombatHandler> ().GetTeam () != _team)
				continue;

			if (Vector3.Distance (transform.position, pv.transform.position) < range) {
				if (closestPV == _pv)
					closestPV = pv;
				else if (Vector3.Distance (transform.position, pv.transform.position) < Vector3.Distance (transform.position, closestPV.transform.position)) {
					closestPV = pv;
				}
			}
		}

		return closestPV;
	}
}
