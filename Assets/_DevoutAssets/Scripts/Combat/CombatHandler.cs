using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Combat handler handles abilities and combat in general
/// </summary>
public class CombatHandler : Photon.PunBehaviour
{
	[HideInInspector]public RopeHandler MyRope;

	/// <summary>
	/// There are two slots for rope per character
	/// One is linked to start point of a rope and another is linked to end of a different rope
	/// One character cannot have more than one start or end
	/// </summary>
	[HideInInspector]public RopeHandler RopeSlotStart, RopeSlotEnd;

	/// <summary>
	/// Ultimate point that is used for ultimate ability.
	/// Can use Ultimate ability when this gauge is 100
	/// Range : 0 ~ 100
	/// </summary>
	int _ultimatePoint = 0;
	/// <summary>
	/// Max value of ultimate point
	/// </summary>
	public const int MAX_ULTIMATE_POINT = 100;
	protected TEAM _team = TEAM.NULL;
	//	List<Hitbox> _hitboxes = new List<Hitbox> ();
	/// <summary>
	/// List of abilities this character can use
	/// </summary>
	[SerializeField] protected List<Ability> _abilities = new List<Ability> ();
	/// <summary>
	/// Ultimate ability of this charcter
	/// </summary>
	[SerializeField] protected Ability _ultimateAbility;

	/// <summary>
	/// The photon view.
	/// </summary>
	protected PhotonView _pv;

	/// <summary>
	/// RCB collider of this character
	/// </summary>
	Collider _RCB;

	/// <summary>
	/// How many rope is in inventory of this character?
	/// </summary>
	protected int _ropeCount = 1;

	public int GetRopeCount ()
	{
		return _ropeCount;
	}

	public virtual void SetRopeCount (int newCount)
	{
		_ropeCount = newCount;
		if (_ropeCount < 0)
			Debug.LogWarning ("This shouldn't be less than 0, check where this is called");

//		print ("current rope count : " + _ropeCount);

		foreach (var a in _abilities) {
			if (a is AbilityRopePlacementMode) {
				if ((newCount > 0 && RopeSlotStart == null)) {
					a.SetStatus (ABILITY_STATUS.AVAILABLE);
				} else {
					a.SetStatus (ABILITY_STATUS.UNAVAILABLE);
				}
			}
		}
	}

	/// <summary>
	/// The starting team of this character
	/// Team determines how you can deal damage or not and others
	/// </summary>
	[SerializeField] protected TEAM _startingTeam = TEAM.NULL;

	protected virtual void Awake ()
	{
		if (_ultimateAbility != null) {
			_abilities.Add (_ultimateAbility);

//			//dev
			_ultimateAbility.SetStatus (ABILITY_STATUS.AVAILABLE);
			_ultimatePoint = 0;
		}
		foreach (var a in _abilities) {
			if (a != null) {
				a.SetCombatHandler (this);
				foreach (var ca in a.ChildAbilities) {
					ca.SetCombatHandler (this);
				}
			}
		}
	}

	protected virtual void Start ()
	{
		if (_pv == null)
			_pv = PhotonView.Get (this);

		if (_startingTeam != TEAM.NULL) {
			SetTeam (_startingTeam);
		} 
//		else
//			print ("WARNING : Character's TEAM is null");

		_RCB = GetComponent<Collider> ();
		InvokeRepeating ("slowUpdate", 1f, 1f);
	}

	void slowUpdate ()
	{
		//charge ultimate point slowely over time
		AddUltimatePoint (1);
	}

	public virtual bool CheckAbilitiesInUse ()
	{
		bool isInUse = false;
		foreach (var a in _abilities) {
			if (a.GetStatus () == ABILITY_STATUS.IN_USE) {
				isInUse = true;
				break;
			}
		}
		return isInUse;
	}

	/// <summary>
	/// Gets list of abilities
	/// </summary>
	/// <returns>The abilities.</returns>
	public List<Ability> GetAbilities ()
	{
		return _abilities;
	}

	/// <summary>
	/// Sets the team.
	/// </summary>
	/// <param name="team">Team.</param>
	public void SetTeam (TEAM team)
	{
		if (_pv == null)
			_pv = PhotonView.Get (this);
		if (PhotonNetwork.offlineMode || !PhotonNetwork.inRoom) {
			setTeam (team);
		} else {
//			_pv.RPC ("RPCSetTeam", PhotonTargets.All, team);
			targeted_RPC("RPCSetTeam", team);
		}
	}

	//send rpc to players who are in same room and same scene
	void targeted_RPC (string RPC, params object[] parameters)
	{
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				photonView.RPC (RPC, p, parameters);
			}
		}
	}

	[PunRPC]
	public void RPCSetTeam (TEAM team)
	{
		setTeam (team);
		if (_pv == null)
			_pv = PhotonView.Get (this);
	}

	void setTeam (TEAM team)
	{
		if (team == TEAM.NULL) {
			Debug.LogWarning ("WARNING : TEAM cannot be set to null");
			return;
		}
		
		_team = team;
		foreach (Transform c in GetComponentsInChildren<Transform>()) {
			if (c.GetComponent<DamagingPoint> () != null) {
				c.gameObject.layer = LayerHelper.GetAttackBoxLayer (team);
				c.GetComponent<DamagingPoint> ().SetDamagableLayers ();
			}
			if (c.GetComponent<Hitbox> () != null) {
				c.gameObject.layer = LayerHelper.GetHitboxLayer (team);
			}
		}
		if (GetComponent<CharacterMovementHandler> () != null) {
//			print ("team : " + team);
			GetComponent<CharacterMovementHandler> ().SetLayerMask (LayerHelper.GetLayerMask (team));
		}
	}

	/// <summary>
	/// Gets this character's team
	/// </summary>
	/// <returns>The team.</returns>
	public TEAM GetTeam ()
	{
		return _team;
	}

	/// <summary>
	/// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
	/// </summary>
	/// <remarks>If your game starts with a certain number of players, this callback can be useful to check the
	/// Room.playerCount and find out if you can start.</remarks>
	/// <param name="newPlayer">New player.</param>
	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		if (_pv.isMine)
			_pv.RPC ("RPCSetTeam", PhotonTargets.All, _team);
	}

	/// <summary>
	/// Adds amount to the ultimate guage.
	/// </summary>
	/// <param name="addAmount">Amount to add</param>
	public void AddUltimatePoint (int addAmount)
	{
//		_ultimatePoint += addAmount;
		SetUltimatePoint (_ultimatePoint + addAmount);
		if (_ultimatePoint >= MAX_ULTIMATE_POINT) {
			_ultimatePoint = MAX_ULTIMATE_POINT;
			if (_ultimateAbility != null) {
				if (_ultimateAbility.GetStatus () == ABILITY_STATUS.UNAVAILABLE) {
					_ultimateAbility.SetStatus (ABILITY_STATUS.AVAILABLE);
//					print ("setting ult to available");
				}
			}
		}
	}

	//	/// <summary>
	//	/// Resets the ultimate guage to 0
	//	/// </summary>
	//	public void ResetUltimateGuage ()
	//	{
	//		_ultimateGuage = 0;
	//	}

	/// <summary>
	/// Gets the ultimate point value(0~100)
	/// </summary>
	/// <returns>The ultimate point.</returns>
	public int GetUltimatePoint ()
	{
		return _ultimatePoint;
	}

	/// <summary>
	/// Sets the ultimate point.
	/// </summary>
	/// <param name="newGauge">New point.</param>
	public void SetUltimatePoint (int newPoint)
	{
		_ultimatePoint = Mathf.Clamp (newPoint, 0, 100);

		//update UI
		if (HUDManager.HUD_M)
			HUDManager.HUD_M.UltimateUI.UpdateUltUI ();
	}

	/// <summary>
	/// Network
	/// Disables the damaging points.
	/// </summary>
	public void DisableDamagingPoints ()
	{
//		disableDamagingPoints ();
		//below may not be needed?
//		print ("DDP");

		if (PhotonNetwork.offlineMode) {
			disableDamagingPoints ();
			return;
		}

		//if not mine, send it to owner to disable
		if (!_pv.isMine) {
			_pv.RPC ("RPCDisableDamagingPoints", _pv.owner);			
			return;
		} else {//if mine, call it
			disableDamagingPoints ();
		}
		
//		if (PhotonNetwork.offlineMode) {
//			disableDamagingPoints ();
//			print ("OLM");
//		} else {
//			_pv.RPC ("RPCDisableDamagingPoints", _pv.owner);
//			print ("RPC sent");
//		}
	}

	void disableDamagingPoints ()
	{
//		print ("abil cnt : " + _abilities.Count);
		foreach (var abil in _abilities) {
//			print ("dp cnt : " + abil.GetWeapon ().GetDamagingPoints ().Count);

			if (abil.GetWeapon () == null)
				continue;
			
			//stop IEnum of activating damaging point
			abil.StopIEADP ();
			
			foreach (var dp in abil.GetWeapon().GetDamagingPoints()) {
				dp.SetIsActive (false);
//				print ("disabled : " + dp.name);
			}
		}
//		print ("called");
	}

	[PunRPC]
	protected void RPCDisableDamagingPoints ()
	{
		disableDamagingPoints ();
	}

	/// <summary>
	/// Network
	/// Position Ultiamte weapons(local position) for duration
	/// Todo : Move this to better place(Player/Class specific RPC calls)
	/// </summary>
	/// <param name="seconds">Duration.</param>
	public void PositionUltimateWeapon (float seconds)
	{
		if (PhotonNetwork.offlineMode) {
			StartCoroutine (IEPositionUltimateWeapon (seconds));
		} else {
			_pv.RPC ("RPCPositionUltimateWeapon", PhotonTargets.All, seconds);
		}		
	}

	[PunRPC]
	public void RPCPositionUltimateWeapon (float seconds)
	{
		StartCoroutine (IEPositionUltimateWeapon (seconds));
	}

	IEnumerator IEPositionUltimateWeapon (float seconds)
	{
		if (_ultimateAbility == null)
			yield return null;
		Weapon weapon = _ultimateAbility.GetWeapon ();
		Vector3 originalPos = weapon.transform.localPosition;
		Vector3 originalRot = weapon.transform.localEulerAngles;
//		weapon.transform.localPosition = new Vector3 (-0.295f, 0.531f, 0.122f);
//		weapon.transform.localEulerAngles = new Vector3 (155.175f, 47.104f, 152.086f);
		weapon.transform.localPosition = new Vector3 (-0.497f, -0.033f, 0.369f);
		weapon.transform.localEulerAngles = new Vector3 (132.88f, -129.795f, -77.177f);
		yield return new WaitForSeconds (seconds);
		weapon.transform.localPosition = originalPos;
		weapon.transform.localEulerAngles = originalRot;
	}

	/// <summary>
	/// Network
	/// Changes the RCB layer.
	/// </summary>
	/// <param name="newLayer">New layer.</param>
	public void ChangeRCBLayer (int newLayer)
	{
		if (PhotonNetwork.offlineMode) {
			RPCChangeRCBLayer (newLayer);
		} else {
			_pv.RPC ("RPCChangeRCBLayer", PhotonTargets.All, newLayer);
		}
	}

	[PunRPC]
	public void RPCChangeRCBLayer (int newLayer)
	{
		changeRCBLayer (newLayer);
	}

	void changeRCBLayer (int newLayer)
	{
		_RCB.gameObject.layer = newLayer;
	}

	///
	/// <summary>
	/// Gets the list of enemies searched from photonview list
	/// </summary>
	/// <returns>The enemies.</returns>
	/// <param name="ofTeam">Of team.</param>
	/// <param name="checkAlive">If set to <c>true</c> check alive.</param>
	public static List<CombatHandler> GET_ENEMIES (TEAM ofTeam, bool checkAlive = false)
	{
		List<CombatHandler> enemies = new List<CombatHandler> ();
		foreach (var p in AIStatusHandler.Get_ALL_PVs(checkAlive)) {
			CombatHandler ch = p.GetComponent<CombatHandler> ();
			if (ch != null) {
				//check if enemy
				if (ofTeam != ch.GetTeam ()) {
					enemies.Add (ch);
				}
			} else {
				Debug.LogWarning ("WARNING : this photonview must have combathandler as its component");
			}
		}
		return enemies;
	}
}
