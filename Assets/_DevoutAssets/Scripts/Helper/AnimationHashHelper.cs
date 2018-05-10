using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Static helper class that gives corresponding hash value for animation strings to improve performance
/// </summary>
public static class AnimationHashHelper {
	public static int PARAM_HANG = Animator.StringToHash("Hang");
	public static int PARAM_CLIMB = Animator.StringToHash("Climb");
	public static int PARAM_JUMP = Animator.StringToHash("Jump");

	public static int PARAM_FORWARD = Animator.StringToHash ("Forward");
	public static int PARAM_ON_GROUND = Animator.StringToHash("OnGround");
	public static int PARAM_TURN = Animator.StringToHash("Turn");
	public static int PARAM_STRAFE= Animator.StringToHash("Strafe");
	public static int PARAM_CROUCH = Animator.StringToHash("Crouch");
	public static int PARAM_JUMP_LEG = Animator.StringToHash("JumpLeg");
	public static int PARAM_IN_COMBAT = Animator.StringToHash("InCombat");
	public static int PARAM_ANIM_SPEED = Animator.StringToHash ("AnimSpeed");

	//abilities
	public static int PARAM_BASIC_ATTACK_1 = Animator.StringToHash ("BasicAttack1");
	public static int STATE_BASIC_ATTACK_1 = Animator.StringToHash("Base.BasicAttack1");
	public static int PARAM_BASIC_ATTACK_2 = Animator.StringToHash ("BasicAttack2");
	public static int PARAM_CHARGE = Animator.StringToHash("Charge");
	public static int PARAM_CLEAVE = Animator.StringToHash("Cleave");
	public static int PARAM_ULTIMATE = Animator.StringToHash("UltimateAbility");
	public static int PARAM_ULTIMATE_TRIGGER = Animator.StringToHash("UltimateAbilityTrigger");
	public static int PARAM_STAGGERED = Animator.StringToHash("Staggered");
	public static int PARAM_BLOCK = Animator.StringToHash("Block");

	//Archer abilities
	public static int PARAM_AIM = Animator.StringToHash("Aim");
	public static int TRIGGER_QUICK_AIM = Animator.StringToHash ("QuickAim");

	//Coop Abilities
	public static int PARAM_COOP_JUMP = Animator.StringToHash("CoopAbilityJump");
	public static int PARAM_COOP_CHARGE = Animator.StringToHash("CoopAbilityCharge");

	public static int STATE_GROUNDED = Animator.StringToHash("Base.Grounded");
	public static int STATE_AIM = Animator.StringToHash("Base.Aim");
	public static int STATE_DRAW = Animator.StringToHash("Base.Draw");

	//for test below
	public static int STATE_HANG_IDLE = Animator.StringToHash("Base.Crouch_Airbourne_Hang.Hang idle");
	public static int STATE_KICK_ROUND_FRONT = Animator.StringToHash("Base.Kick Round Front");

	static List<int> _hashList = new List<int>();

	/// <summary>
	/// Initializes the <see cref="AnimationHashHelper"/> class.
	/// Pre-load all animation hash into a list for iteration
	/// </summary>
	static AnimationHashHelper(){
		_hashList.Add (PARAM_HANG);
		_hashList.Add (PARAM_CLIMB);
		_hashList.Add (PARAM_FORWARD);
		_hashList.Add (PARAM_ON_GROUND);
		_hashList.Add (PARAM_TURN);
		_hashList.Add (PARAM_STRAFE);
		_hashList.Add (PARAM_CROUCH);
		_hashList.Add (PARAM_JUMP);
		_hashList.Add (PARAM_JUMP_LEG);
		_hashList.Add (PARAM_IN_COMBAT);
		_hashList.Add (PARAM_BASIC_ATTACK_1);
		_hashList.Add (STATE_BASIC_ATTACK_1);
		_hashList.Add (PARAM_BASIC_ATTACK_2);
		_hashList.Add (PARAM_CHARGE);
		_hashList.Add (PARAM_CLEAVE);
		_hashList.Add (PARAM_ULTIMATE);
		_hashList.Add (PARAM_ULTIMATE_TRIGGER);
		_hashList.Add (STATE_GROUNDED);
		_hashList.Add (STATE_HANG_IDLE);
		_hashList.Add (STATE_KICK_ROUND_FRONT);
		_hashList.Add (STATE_AIM);
		_hashList.Add (PARAM_BLOCK);
		_hashList.Add (PARAM_AIM);
		_hashList.Add (TRIGGER_QUICK_AIM);
		_hashList.Add (PARAM_COOP_JUMP);
		_hashList.Add (PARAM_COOP_CHARGE);
	}

	/// <summary>
	/// Finds the hash that matches string of the animation parameter
	/// </summary>
	/// <returns>The matching hash.</returns>
	/// <param name="animString">Animation name(parameter).</param>
	public static int FindHash(string animString){
//		Debug.Log ("finding hash : " + animString);
		int hash = Animator.StringToHash (animString);
		foreach (var h in _hashList) {
			if (h == hash)
				return h;
		}
		if(animString != "")
			Debug.Log ("WARNING : ANIM HASH NOT FOUND for " + animString);
		return hash;
	}
}
