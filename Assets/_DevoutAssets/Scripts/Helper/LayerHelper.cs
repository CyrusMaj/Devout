using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LayerHelper {
	public static int DEFAULT = LayerMask.NameToLayer("Default");
	public static int NEAUTRAL = LayerMask.NameToLayer("RCN");
	public static int TEAM_1_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC1H") | 1 << LayerMask.NameToLayer ("RC1A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	public static int TEAM_2_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC2H") | 1 << LayerMask.NameToLayer ("RC2A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	public static int TEAM_3_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC3H") | 1 << LayerMask.NameToLayer ("RC3A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	public static int TEAM_4_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC4H") | 1 << LayerMask.NameToLayer ("RC4A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	public static int TEAM_5_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC5H") | 1 << LayerMask.NameToLayer ("RC5A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	public static int ATTACKBOX_TEAM_1 = LayerMask.NameToLayer("RC1A");
	public static int ATTACKBOX_TEAM_2 = LayerMask.NameToLayer("RC2A");
	public static int ATTACKBOX_TEAM_3 = LayerMask.NameToLayer("RC3A");
	public static int ATTACKBOX_TEAM_4 = LayerMask.NameToLayer("RC4A");
	public static int ATTACKBOX_TEAM_5 = LayerMask.NameToLayer("RC5A");
	public static int HITBOX_TEAM_1 = LayerMask.NameToLayer("RC1H");
	public static int HITBOX_TEAM_2 = LayerMask.NameToLayer("RC2H");
	public static int HITBOX_TEAM_3 = LayerMask.NameToLayer("RC3H");
	public static int HITBOX_TEAM_4 = LayerMask.NameToLayer("RC4H");
	public static int HITBOX_TEAM_5 = LayerMask.NameToLayer("RC5H");
	public static int COMMON_BODY = LayerMask.NameToLayer("RCB");
	public static int COMMON_BODY_GHOST = LayerMask.NameToLayer("RCBG");
	public static int WALL = LayerMask.NameToLayer("WALL");
	public static int FLOOR = LayerMask.NameToLayer("Floor");
	public static int ROPE = LayerMask.NameToLayer("Rope");
	public static int RAGDOLL = LayerMask.NameToLayer("Ragdoll");

	public static int GetHitboxLayer(TEAM team){
		if (team == TEAM.ONE)
			return HITBOX_TEAM_1;
		else if (team == TEAM.TWO)
			return HITBOX_TEAM_2;
		else if (team == TEAM.THREE)
			return HITBOX_TEAM_3;
		else if (team == TEAM.FOUR)
			return HITBOX_TEAM_4;
		else if (team == TEAM.FIVE)
			return HITBOX_TEAM_5;
		else {
			Debug.Log ("Warning : Wrong team number");
			return HITBOX_TEAM_1;
		}
	}

	public static int GetAttackBoxLayer(TEAM team){
		if (team == TEAM.ONE)
			return ATTACKBOX_TEAM_1;
		else if (team == TEAM.TWO)
			return ATTACKBOX_TEAM_2;
		else if (team == TEAM.THREE)
			return ATTACKBOX_TEAM_3;
		else if (team == TEAM.FOUR)
			return ATTACKBOX_TEAM_4;
		else if (team == TEAM.FIVE)
			return ATTACKBOX_TEAM_5;
		else {
			Debug.Log ("Warning : Wrong team number");
			return ATTACKBOX_TEAM_1;
		}
	}

	public static int GetLayerMask(TEAM team){
//		Debug.Log (team.ToString ());
		if (team == TEAM.ONE)
			return TEAM_1_BODY;
		else if (team == TEAM.TWO)
			return TEAM_2_BODY;
		else if (team == TEAM.THREE)
			return TEAM_3_BODY;
		else if (team == TEAM.FOUR)
			return TEAM_4_BODY;
		else if (team == TEAM.FIVE)
			return TEAM_5_BODY;
		else {
			Debug.Log ("Warning : Wrong team number");
			return TEAM_1_BODY;
		}
	}

	/// <summary>
	/// Gets a list of layers this damaging point can damage
	/// </summary>
	/// <returns>The damagable layers.</returns>
	/// <param name="myLayer">Layer of the character looking for damagable layers.</param>
	public static List<int> GetDamagableLayers (int myLayer)
	{
		List<int> layers = new List<int> ();
		layers.Add (LayerHelper.NEAUTRAL);
		if (myLayer == LayerHelper.ATTACKBOX_TEAM_1) {
			layers.Add (LayerHelper.HITBOX_TEAM_2);
			layers.Add (LayerHelper.HITBOX_TEAM_3);
			layers.Add (LayerHelper.HITBOX_TEAM_4);
			layers.Add (LayerHelper.HITBOX_TEAM_5);
		} else if (myLayer == LayerHelper.ATTACKBOX_TEAM_2) {
			layers.Add (LayerHelper.HITBOX_TEAM_1);
			layers.Add (LayerHelper.HITBOX_TEAM_3);
			layers.Add (LayerHelper.HITBOX_TEAM_4);
			layers.Add (LayerHelper.HITBOX_TEAM_5);
		} else if (myLayer == LayerHelper.ATTACKBOX_TEAM_3) {
			layers.Add (LayerHelper.HITBOX_TEAM_1);
			layers.Add (LayerHelper.HITBOX_TEAM_2);
			layers.Add (LayerHelper.HITBOX_TEAM_4);
			layers.Add (LayerHelper.HITBOX_TEAM_5);
		} else if (myLayer == LayerHelper.ATTACKBOX_TEAM_4) {
			layers.Add (LayerHelper.HITBOX_TEAM_1);
			layers.Add (LayerHelper.HITBOX_TEAM_2);
			layers.Add (LayerHelper.HITBOX_TEAM_3);
			layers.Add (LayerHelper.HITBOX_TEAM_5);
		} else if (myLayer == LayerHelper.ATTACKBOX_TEAM_5) {
			layers.Add (LayerHelper.HITBOX_TEAM_1);
			layers.Add (LayerHelper.HITBOX_TEAM_2);
			layers.Add (LayerHelper.HITBOX_TEAM_3);
			layers.Add (LayerHelper.HITBOX_TEAM_4);
		}
		return layers;
	}
}
