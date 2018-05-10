using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-03-27
/// 
/// <summary>
/// Place rope on an object(not to another character)
/// </summary>
public class AbilityPlaceRope : Ability
{
	[SerializeField]AbilityRopePlacementMode _arpm;

	protected override void Start ()
	{
		if (_arpm == null)
			Debug.LogWarning ("WARNING : placement ability should be parented & assigned with AbilityRopePlacementMode");
	}

	public override void Activate ()
	{
//		base.Activate ();
//		print ("Rope Spike Placed");

		placeSpikeAndRope ();

		_arpm.Deactivate ();

		SetStatus (ABILITY_STATUS.UNAVAILABLE);
	}

	void placeSpikeAndRope ()
	{
		GameObject spike = PhotonNetwork.Instantiate ("RopeSpike", _arpm.RopeSpikeHologram.position, _arpm.RopeSpikeHologram.rotation, 0);
		spike.transform.SetParent (_arpm.RopeSpikeParent);

		//when first time, rope ties to player character & spike
		if (_combatHandler.RopeSlotStart == null) {
			GameObject rope = PhotonNetwork.Instantiate ("Rope_05", _combatHandler.transform.position, Quaternion.identity, 0);
			RopeHandler rh = rope.GetComponent<RopeHandler> ();
			if (rh.EndTarget != null && rh.EndTarget.GetComponent<CombatHandler>()) {
				rh.StartTarget = rh.EndTarget;

//				rh.StartAttachPoint = rh.EndAttachPoint;
				rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.CHARACTER, rh.EndTarget.GetComponent<PhotonView> ().viewID);

				rh.EndTarget = _arpm.RopeSpikeParent;

//				rh.EndAttachPoint = spike.transform.FindChild ("RopeAttachPoint");
				rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.SPIKE, spike.GetComponent<PhotonView> ().viewID);

			} else {
				rh.StartTarget = _combatHandler.transform;

//				rh.StartAttachPoint = ((PlayerCombatHandler)_combatHandler).RopeTiePoint;
				rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.CHARACTER, _combatHandler.GetComponent<PhotonView> ().viewID);

				rh.EndTarget = _arpm.RopeSpikeParent;
//				rh.EndAttachPoint = spike.transform.FindChild ("RopeAttachPoint");
				rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.SPIKE, spike.GetComponent<PhotonView> ().viewID);
			}
//			rh.EndTarget = spike.transform.FindChild ("RopeAttachPoint");
//			rh.EndAttachPoint = rh.EndTarget;
			rh.IsSetup = true;
			rh.Owner = _combatHandler;
		} else {
			RopeHandler rh = _combatHandler.RopeSlotStart;
			rh.StartTarget = _arpm.RopeSpikeParent;

//			rh.StartAttachPoint = spike.transform.FindChild ("RopeAttachPoint");
			rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.SPIKE, spike.GetComponent<PhotonView> ().viewID);

			_combatHandler.RopeSlotStart = null;
			_combatHandler.SetRopeCount (_combatHandler.GetRopeCount () - 1);

			//unignore collision
//			Physics.IgnoreCollision (rh.RC_Collider.GetComponent<Collider> (), _combatHandler.GetComponent<Collider> (), false);
			rh.IgnoreCollision(_combatHandler.GetComponent<PhotonView>().viewID, false);
		}
	}
}
