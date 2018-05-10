using UnityEngine;
using System.Collections;
using Apex.Steering.Components;

/// Written by Duke Im
/// On 2016-12-02
/// 	
/// <summary>
/// AI behavior class for basic melee AI
/// Scan for enemies, moves and attack. Repeats until death.
/// </summary>
public class AIBehaviorMeleeSimple : AIBehavior
{
	//	/// <summary>
	//	/// The reaction time of this AI
	//	/// How long this AI takes before moving after detecting enemies
	//	/// </summary>
	//	[SerializeField] float _reactionTime = 0.6f;
	/// <summary>
	/// How often does this behavior checks for changes?
	/// </summary>
	[SerializeField] float _updateRate = 0.2f;

	//move if further than this amount from a slot position
	//	float _nearDistSlotPos = 0.5f;

	float _coolDownTimer;

	/// <summary>
	/// Current move target position of this AI
	/// </summary>
	Vector3? _moveTarget;

	protected override void Start ()
	{
		base.Start ();
		//mine or scene controlled object
		//		if (_pv.isMine || (_pv.ownerId == 0 && (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode))) {
		if (_pv.isMine || _pv.ownerId == 0) {
			InvokeRepeating ("slowUpdate", 0.1f, _updateRate);
		}
	}

	/// <summary>
	/// Slower version of Unity's Update
	/// </summary>
	void slowUpdate ()
	{
		//only behave when allowed and alive
//		if (!AIController.AIC.GetIsAIMovementAllowed () || !_aish.Alive () || !IsBehaviorAllowed)
		if (!AIController.AIC.GetIsAIMovementAllowed () || !IsBehaviorAllowed || !_aimh.IsGrounded)
			return;

		if (!_aish.Alive ()) {
			//dev for chr moving after death
			_aimh.StopFacade ();
			return;
		}

		//decrease cooldown
		_coolDownTimer = Mathf.Clamp (_coolDownTimer - _updateRate, 0f, 2f);

		//if in cooldown, dont do anything
		if (_coolDownTimer > 0)
			return;

		//if current target is null or dead, scan again for better target
		if (_ais.Target == null || !_ais.Target.Alive ()) {
			_ais.Scan ();
		}

		//if no target, don't do anything
		if (_ais.Target == null || !_ais.Target.Alive ()) {
			return;
		}
			
		//if already in attack range, just attack
		if (Vector3.Distance (this.transform.position, _ais.Target.transform.position) < _ais.Target.GetComponent<AIMovementHandler> ().GetMeleeSlotDistance () + 0.1f) {
			_aimh.StopFacade ();
			
			//don't do if not at halt
			if (_aimh.IsMoving) {
				return;
			}
			//once arrived, look at the target
			if (Vector3.Angle (transform.forward, _ais.Target.transform.position - transform.position) > 5f) {
				_aimh.TurnTowardsPos (_ais.Target.transform.position);
			} else {
				attack ();
			}
			return;
		}

		_moveTarget = _ais.GetAttackPosition (null);
		if (_moveTarget == null)
			return;
		
		//move to attack position if not already in
//		if (Vector3.Distance (this.transform.position, (Vector3)_moveTarget) > _nearDistSlotPos) {
		_aimh.MoveTo ((Vector3)_moveTarget);
		AddCooldown (1f);
//		}
	}

	/// <summary>
	/// Use ability(basic attack)
	/// </summary>
	void attack ()
	{
		if (_ch.GetAbilities () [0].GetStatus () == ABILITY_STATUS.AVAILABLE) {
			_ch.GetAbilities () [0].Activate ();//use ability
			AddCooldown (1.5f);
			return;
		}
	}

	/// <summary>
	/// Called after switching to a new MasterClient when the current one leaves.
	/// </summary>
	/// <remarks>This is not called when this client enters a room.
	/// The former MasterClient is still in the player list when this method get called.</remarks>
	/// <param name="newMasterClient">New master client.</param>
	public override void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		base.OnMasterClientSwitched (newMasterClient);
		//behave in the new master, if this is the new master
		CancelInvoke ();
		if (PhotonNetwork.player == newMasterClient) {
			InvokeRepeating ("slowUpdate", 0.1f, _updateRate);
		}
	}

	/// <summary>
	/// Adds the cooldown.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void AddCooldown (float amount)
	{
		_coolDownTimer = Mathf.Clamp (_coolDownTimer + amount, 0f, 2f);
	}
}
