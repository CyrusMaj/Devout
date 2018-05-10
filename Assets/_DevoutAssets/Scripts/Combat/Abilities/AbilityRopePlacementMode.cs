using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-03-27
/// 
/// <summary>
/// Placement mode where hologram of rope is displayed on where the camera is facing & hit by its raycast
/// </summary>
public class AbilityRopePlacementMode : Ability
{
	public Transform RopeSpikeHologram;
	/// <summary>
	/// Parent Transform of where spike will be placed
	/// </summary>
	[SerializeField] public Transform RopeSpikeParent;
	//	public Transform RopeSpikeTiePoint;

	AbilityPlaceRope _apr;

	//to prevent activation when canceled with the same key binding
	bool _canceledWithSameKey = false;

	protected override void Start ()
	{
		foreach (var ca in ChildAbilities) {
			if (ca is AbilityPlaceRope) {
				_apr = (AbilityPlaceRope)ca;
			}
		}
		if (_apr == null)
			Debug.LogWarning ("WARNING : placement mode ability should have place child ability");
	}

	public override void Activate ()
	{
//		base.Activate ();
		foreach (var a in _combatHandler.GetAbilities()) {
			if (a is AbilityTieRope) {
				if (a.GetStatus () == ABILITY_STATUS.AVAILABLE) {
//					print ("Skipped_2");
					return;
				}
			}
		}

		if (_canceledWithSameKey) {
			_canceledWithSameKey = false;
//			print ("Skipped");
			return;
		}
		SetStatus (ABILITY_STATUS.IN_USE);
		_apr.SetStatus (ABILITY_STATUS.IN_USE);

//		print ("Placement MODE");
	}

	void Update ()
	{
		if (GetStatus () == ABILITY_STATUS.IN_USE) {
			//Using other ability while in use cancels placement mode
			if (Input.GetButtonDown (InputHelper.BASIC_ATTACK)
			    || Input.GetButtonDown (InputHelper.SECONDARY_ATTACK)
			    || Input.GetButtonDown (InputHelper.UNROPE)
			    || Input.GetButtonDown (InputHelper.JUMP)
			    || Input.GetButtonDown (InputHelper.ULTIMATE)
				//Cancel when placement is invalid & Rope button is pressed,
				//so player can look away and press rope button to cancel
			    || (_apr.GetStatus () == ABILITY_STATUS.UNAVAILABLE && Input.GetButtonDown (InputHelper.ROPE))) {
//				if (_apr.GetStatus () == ABILITY_STATUS.UNAVAILABLE && Input.GetButtonDown (InputHelper.ROPE))
//					Deactivate (true);
//				else
				Deactivate ();
			} else {
				//draw hologram
				placeHologram ();
			}
		}
	}

	void placeHologram ()
	{
		RaycastHit hit;
		float distance = 9f;
		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL | 1 << LayerHelper.NEAUTRAL;
		Vector3 origin = CameraController.CC.CombatCamera.transform.position - CameraController.CC.CombatCamera.transform.forward;// + CameraController.CC.CombatCamera.transform.forward * 2f;
		if (Physics.Raycast (origin, CameraController.CC.CombatCamera.transform.forward, out hit, distance, lm)) { 
			RopeSpikeHologram.gameObject.SetActive (true);
			RopeSpikeHologram.position = hit.point;
			RopeSpikeHologram.rotation = Quaternion.LookRotation (hit.normal);
			RopeSpikeHologram.Rotate (new Vector3 (90f, 0f, 0f));
			RopeSpikeParent = hit.transform;
			//set placement available
			_apr.SetStatus (ABILITY_STATUS.AVAILABLE);
		} else {
			RopeSpikeHologram.gameObject.SetActive (false);
			_apr.SetStatus (ABILITY_STATUS.UNAVAILABLE);
		}
	}

	public void Deactivate (bool skipNextActivate = false)
	{
//		print ("Deactivated");

		RopeSpikeHologram.gameObject.SetActive (false);

		if (_combatHandler.GetRopeCount () > 0)
			SetStatus (ABILITY_STATUS.AVAILABLE);
		else
			SetStatus (ABILITY_STATUS.UNAVAILABLE);
		
		_canceledWithSameKey = skipNextActivate;
	}

	public override void SetCombatHandler (CombatHandler ch)
	{
		base.SetCombatHandler (ch);

		if (ch.GetRopeCount () > 0)
			SetStatus (ABILITY_STATUS.AVAILABLE);
	}
}
