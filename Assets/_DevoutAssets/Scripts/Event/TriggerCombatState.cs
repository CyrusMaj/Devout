using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-18
	/// 
	/// <summary>
	/// Trigger that checks combat status changes
	/// SINGLETON
	/// </summary>
//	public class TriggerCombatState : Trigger
//	{
//		/// <summary>
//		/// Checks continuously if set true
//		/// </summary>
//		[SerializeField] bool _isLoop = false;
//
//		//		float _lastStateChangedTime;
//
//		public static TriggerCombatState SINGLETON;
//
//		float _lastStateChanged;
//
//		//		/// <summary>
//		//		/// Current combat state of this player character
//		//		/// </summary>
//		//		[SerializeField]COMBAT_STATUS _state;
//
//		protected override void Start ()
//		{
//			base.Start ();
//
//			if (!SINGLETON)
//				SINGLETON = this;
//			else if (SINGLETON != this)
//				Destroy (gameObject);
//		}
//
//		#region implemented abstract members of Trigger
//
//		protected override void checkCondition ()
//		{
//			//if the game is not ready, don't check for conditions
//			if (GameController.GC.CurrentPlayerCharacter == null ||
//			    MusicManager.SINGLETON == null)
//				return;
//
//			MusicManager mm = MusicManager.SINGLETON;
//
//			CharacterStatusHandler myCSH = GameController.GC.CurrentPlayerCharacter.GetComponent<CharacterStatusHandler> ();
//			TEAM myTeam = myCSH.GetComponent<CombatHandler> ().GetTeam ();
//
//			//get all AIs
//			foreach (CombatHandler ch in AIStatusHandler.Get_PVs(true).Select(x=>x.GetComponent<CombatHandler>())) {
////				print (AIStatusHandler.Get_PVs (true).Select (x => x.GetComponent<CombatHandler> ()).Count ());
//				//check if enemy AI
//				if (ch.GetTeam () != myTeam) {
//					//check if I'm targeted by enemies
//					AIScanner aic = ch.GetComponent<AIScanner> ();
//					if (aic.Target != null && aic.Target == myCSH) {
////						print ("I'm targgeted");
//
//						//if one of the enemies is targetting me
//						if (mm.CombatStatus != COMBAT_STATUS.IN_COMBAT && 
//							Time.time > _lastStateChanged + mm.GetMinPlayTime()) {
//							mm.CombatStatus = COMBAT_STATUS.IN_COMBAT;
//							_lastStateChanged = Time.time;
//
//							if (!_isLoop)
//								triggerNext ();
//							else
//								triggerNext (false);
//						}
//							
//						return;
//					}
//				}
//			}
//
////			print ("I'm not targgeted");
//
//			//no enemy is targetting me
//			if (mm.CombatStatus != COMBAT_STATUS.MAIN_MENU && 
//				Time.time > _lastStateChanged + mm.GetMinPlayTime()) {
//
//				mm.CombatStatus = COMBAT_STATUS.MAIN_MENU;
//				_lastStateChanged = Time.time;
//
//				if (!_isLoop)
//					triggerNext ();
//				else
//					triggerNext (false);
//			}
//		}
//
//		#endregion
//	}
//
//	/// <summary>
//	/// Status of combat that current character is in
//	/// </summary>
//	public enum COMBAT_STATUS
//	{
//		IN_COMBAT,
//		MAIN_MENU,
//	}
}
