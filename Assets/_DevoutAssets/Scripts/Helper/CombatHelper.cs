using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CombatHelper
{
//	public static TEAM getAvailableTeam ()
//	{
//		List<TEAM> availability = new List<TEAM> ();
//		availability.Add (TEAM.ONE);
//		availability.Add (TEAM.TWO);
//		availability.Add (TEAM.THREE);
//		availability.Add (TEAM.FOUR);
//		foreach (var go in GameObject.FindGameObjectsWithTag(TagHelper.PLAYER)) {
//			if (go.GetComponent<PhotonView> ().owner == PhotonNetwork.player)
//				continue;
//			
//			CombatHandler ch = go.GetComponent<CombatHandler> ();
//
//			//dev
//			if (ch == null)
//				Debug.Log ("CH null in " + go.name);
//			Debug.Log ("Comparing " + ch.GetTeam ());
//			if (availability.Contains (ch.GetTeam ())) {
//				Debug.Log (ch.GetTeam ().ToString () + " already exists, finding other teams");
//				availability.Remove (ch.GetTeam ());
//			}
//		}
//		if (availability.Count > 0) {
//			return availability [0];
//		} else {
//			Debug.Log ("ERROR : NO AVAIABLE TEAM LEFT");
//			return TEAM.ONE;
//		}
//	}

	/// <summary>
	/// Gets the relative direction. from subtracting one vector(center) from another(target) than normalizing it
	/// </summary>
	/// <returns>The relative direction.</returns>
	/// <param name="center">Center.</param>
	/// <param name="target">Target.</param>
	/// <param name="dir">Relative direction to return</param>
	public static Vector3 getRelativeDirection (Vector3 center, Vector3 target, PUSHBACK_DIRECTION dir)
	{
		Vector3 relativeDirection;
		if (dir == PUSHBACK_DIRECTION.OUTWARD) {
			relativeDirection = (target - center).normalized;
		} else {
			relativeDirection = -(target - center).normalized;
		}
		//ignore y direction(unless added in the future)
		relativeDirection = new Vector3 (relativeDirection.x, 0f, relativeDirection.z);

		return relativeDirection;
	}
}

public enum PUSHBACK_DIRECTION
{
	OUTWARD,
	INWARD,
}

/// <summary>
/// Team 1 is red, team 2 is blue
/// </summary>
public enum TEAM
{
	ONE,
	TWO,
	THREE,
	FOUR,
	FIVE,
	//Enemy NPCs(common enemies)
	NULL,
}

/// <summary>
/// Status of player character used for coop-ability such as jumping over Tank character when it's blocking
/// </summary>
public enum PLAYER_COOP_STATUS
{
	DEFAULT,
	TANK_BLOCKING,
}

/// <summary>
/// Transform with a timer
/// </summary>
public class OSHTimer
{
	//	private ObjectStatusHandler _target;
	//	public ObjectStatusHandler Target{ get { return _target; } set { _target = value; } }
	//	private float _timer;
	//	public float Timer{ get { return _timer; } set { _timer = value; } }
	public ObjectStatusHandler Target;
	public float Timer;
}