using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Dev classes are for placeholders / prototypes that needs to be replaced with proper classes
public class DevEnemyAIBehavior : CharacterMovementHandler
{
	float coolDown = 1f;
	Transform _target;
	//	float _turnSpeed = 270f;
	float _clostEnoughDistance = 2.0f;
	ObjectStatusHandler _osh;
	CombatHandler _ch;
	float _inputV = 0f;

	//	[SerializeField] bool _enableBehavior = true;
	[SerializeField] float _searchRange = 1000f;

	// Use this for initializationv
	protected override void Start ()
	{
		base.Start ();
		//continuesly update and search for target
		InvokeRepeating ("updateTarget", 0.5f, 1.0f);
		_osh = GetComponent<ObjectStatusHandler> ();
		_ch = GetComponent<CombatHandler> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//dev
		CheckGroundStatus();
		
//		if (_photonView.isMine) {
//			if (AIController.AIC.GetIsAIMovementAllowed ()) {
//				if (_osh.Alive ()) {
////					if (_enableBehavior) {
//					if (_target != null && Vector3.Distance (transform.position, _target.position) < _searchRange) {
//						if (coolDown < 0f) {
////							if (_target != null) {
//							//if not close enough to attack, move towards the target
//							if (Vector3.Distance (_target.position, transform.position) > _clostEnoughDistance) {
//								_inputV = 1f;
//							} else {
//								//Attack if close enough
//								attack ();
//								coolDown += 2f;
//								_inputV = 0f;
//							}
//							//turn towards target
//							TurnTowardsPos (_target.position);
////							}
//						} else {
//							coolDown -= Time.fixedDeltaTime;
//						}
//					} else
//						_inputV = 0f;
//					Move (_inputV, 0f, false);
////					}
//
//					UpdateAnimator (_inputV, 0f);
//				}
//			}
//		}
	}

	public void SetInputV (float newV)
	{
		_inputV = newV;
	}

	void attack ()
	{
		foreach (var a in _ch.GetAbilities()) {
//			print ("abil stat : " + a.GetStatus ().ToString ());
			if (a.GetStatus () == ABILITY_STATUS.AVAILABLE) {
				a.Activate ();//use ability
				coolDown += 2f;
				return;
			}
		}		
	}

	//get lists of csh from player list
	List<CharacterStatusHandler> _playerCSHs = new List<CharacterStatusHandler> ();

	void updateTarget ()
	{
		if (_target != null) {
			if (!_target.GetComponent<ObjectStatusHandler> ().Alive ())
				_target = null;
		}

		//get lists of csh from player list
		_playerCSHs.Clear ();
		foreach (var p in PlayerCharacterStatusHandler.Get_PVs()) {
			PlayerCharacterStatusHandler psh = p.GetComponent<PlayerCharacterStatusHandler> ();
			if (psh != null) {
				_playerCSHs.Add (psh);
			}
		}

//		foreach (var p in AIController.AIC.GetPlayerList()) {
		foreach (var psh in _playerCSHs) {
//			if (transform == null)
//				print ("transform is null");
			if (psh == null)
				continue;
			
			if (_target == null)
				_target = psh.transform;
			else {
				if (Vector3.Distance (_target.position, transform.position) > Vector3.Distance (psh.transform.position, transform.position))
					_target = psh.transform;
			}
		}
	}

	//	public void SetEnableBehavior (bool enableOrDisable)
	//	{
	//		_enableBehavior = enableOrDisable;
	//	}
}