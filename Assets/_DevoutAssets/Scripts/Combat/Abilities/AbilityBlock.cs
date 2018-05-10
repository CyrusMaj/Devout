using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Written by Duke Im
/// On 2016-10-04
///
/// <summary>
/// Blocking ability
/// Ex) Blocking ability for tank, while holding a button, the character will stay in the animation and does affects of the block such as bouncing back attacks
/// </summary>
public class AbilityBlock : AbilityHold
{	
	/// <summary>
	/// The blocking point.(ex. Shield)
	/// </summary>
	[SerializeField] BlockingPoint _blockingPoint;
	//dev
	PhotonView _pv;

	protected override void Start ()
	{
		base.Start ();
		if (_blockingPoint == null)
			print ("_blockingPoint = null");
		else if (_blockingPoint.GetCollider() == null)
			print ("GetCollider = null");
		_blockingPoint.GetCollider ().enabled = false;

		//dev
		_pv = PhotonView.Get(this);
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public override void Activate ()
	{
		base.Activate ();
//		_blockingPoint.GetCollider ().enabled = true;
		setEnableBlockingPoint (true);

		//if player
		//turn towards camera direction 
		if (_combatHandler.GetComponent<PlayerCharacterStatusHandler>()){
			_combatHandler.GetComponent<CharacterMovementHandler> ().TurnTowardsMousePos (0.2f);
		}

		//set coop ability for characters to interact with
		if (_combatHandler.GetComponent<PlayerCombatHandler> () != null) {
			_combatHandler.GetComponent<PlayerCombatHandler> ().SetCoopStatus (PLAYER_COOP_STATUS.TANK_BLOCKING);
		}
	}

	/// <summary>
	/// Enable or disable blocking point
	/// </summary>
	/// <param name="isEnable">If set to <c>true</c> is enable.</param>
	void setEnableBlockingPoint(bool isEnabled){
		if (!_pv.isMine)
			return;

		if (PhotonNetwork.offlineMode) {
			_blockingPoint.GetCollider ().enabled = isEnabled;
		} else {
//			_pv.RPC ("RPCSetEnableBlockingPoint", PhotonTargets.All, isEnabled);
			NetworkManager.SINGLETON.Targeted_RPC (_pv, "RPCSetEnableBlockingPoint", isEnabled);
		}
	}
	[PunRPC]
	void RPCSetEnableBlockingPoint(bool isEnabled){
		_blockingPoint.GetCollider ().enabled = isEnabled;
//		print ("called");
	}

	void Update(){
		if (_status != ABILITY_STATUS.IN_USE)
			return;

		//if player character w/ ult(Tank Ultimate Ability)
		if (_combatHandler is PlayerCombatHandler) {			
			foreach (var a in _combatHandler.GetAbilities()) {
				if (a is AbilityUltimateTank) {
					AbilityUltimateTank aut = (AbilityUltimateTank)a;
					//can use ult if ult is 30 or above
					if (_combatHandler.GetUltimatePoint () > 29) {
						if(aut.GetStatus() != ABILITY_STATUS.IN_USE)
							aut.SetStatus (ABILITY_STATUS.AVAILABLE);
					}
				}
			}
		}
	}

	/// <summary>
	/// Deactivate / Stop using this ability.
	/// </summary>
	public override void Deactivate ()
	{
		//dev
		//if player character w/ ult(Tank Ultimate Ability)
		if (_combatHandler is PlayerCombatHandler) {			
			foreach (var a in _combatHandler.GetAbilities()) {
				if (a is AbilityUltimateTank) {
					AbilityUltimateTank aut = (AbilityUltimateTank)a;
//					aut.Deactivate ();
				}
			}
		}

		base.Deactivate ();
//		_blockingPoint.GetCollider ().enabled = false;
		setEnableBlockingPoint (false);

		if (_combatHandler.GetComponent<PlayerCombatHandler> () != null) {
			_combatHandler.GetComponent<PlayerCombatHandler> ().SetCoopStatus (PLAYER_COOP_STATUS.DEFAULT);
		}
	}

	/// <summary>
	/// Sets the combat handler linked to character holding this ability
	/// </summary>
	/// <param name="ch">Combat handler</param>
	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);
		//for blocking, handle it like hitbox, setting status handler
		_blockingPoint.SetOSH (_combatHandler.GetComponent<PlayerCharacterStatusHandler> ());
	}

	/// <summary>
	/// Gets the blocking point.
	/// </summary>
	/// <returns>The blocking point.</returns>
	public BlockingPoint GetBlockingPoint(){
		return _blockingPoint;
	}
}
