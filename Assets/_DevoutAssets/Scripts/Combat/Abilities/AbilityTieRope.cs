using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-03-05
/// 
/// <summary>
/// Tie rope to nearby ally
/// </summary>
public class AbilityTieRope : Ability
{
	[HideInInspector] public Transform Target;

	[HideInInspector] public float TieDistance = 2f;

	protected override void Start ()
	{
		base.Start ();
		//not available until target in range
		SetStatus (ABILITY_STATUS.UNAVAILABLE);
	}

	public override void Activate ()
	{
		if (!Target || _combatHandler.MyRope || _combatHandler.RopeSlotStart != null)
			return;

		CombatHandler tch = Target.GetComponent<CombatHandler> ();
		GameObject go;
		go = PhotonNetwork.Instantiate ("Rope_05", _combatHandler.transform.position, Quaternion.identity, 0);
		RopeHandler rh = go.GetComponent<RopeHandler> ();
		_combatHandler.MyRope = rh;
		rh.Owner = _combatHandler;

//		print ("Rope spawned");

//		_combatHandler.SetRopeCount (_combatHandler.GetRopeCount () - 1);

		StartCoroutine (CoroutineHelper.IEChangeBool ((x) => rh.IsSetup = x, true, 0.5f));

		if (_combatHandler is PlayerCombatHandler && tch is PlayerCombatHandler) {
			PlayerCombatHandler mpch, tpch;
			mpch = (PlayerCombatHandler)_combatHandler;
			tpch = (PlayerCombatHandler)tch;

			rh.StartTarget = mpch.transform;

//			rh.StartAttachPoint = mpch.RopeTiePoint;
			rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.CHARACTER, mpch.GetComponent<PhotonView> ().viewID);
			rh.IgnoreCollision(mpch.GetComponent<PhotonView>().viewID, true);

			rh.EndTarget = tpch.transform;

//			rh.EndAttachPoint = tpch.RopeTiePoint;
			rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.CHARACTER, tpch.GetComponent<PhotonView> ().viewID);
			rh.IgnoreCollision(tpch.GetComponent<PhotonView>().viewID, true);

			mpch.RopeSlotStart = rh;
			tpch.RopeSlotEnd = rh;
		} else {
			print ("Not player or character (shouldn't be called)");
		}
	}
}
