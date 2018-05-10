using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Player character status handler.
/// Status for player character only
/// </summary>
public class PlayerCharacterStatusHandler : AIStatusHandler
{
	/// <summary>
	/// Gets the list of photonviews of player characters
	/// </summary>
	/// <returns>The P vs.</returns>
	/// <param name="checkAlive">If set to <c>true</c> only returns alive character</param>
	public static List<PhotonView> Get_PVs (bool checkAlive = false)
	{
		//remove if destroyed(null)
		PHOTON_VIEW_LIST.RemoveAll (x => x == null);

		List<PhotonView> playersCharacterPVs = new List<PhotonView> ();
		foreach (var p in PHOTON_VIEW_LIST) {
			if (p == null)
				continue;
			
			CharacterStatusHandler csh = p.GetComponent<CharacterStatusHandler> ();
			if (csh != null) {
//				if (csh is PlayerCharacterStatusHandler) {
				if (csh.Type == TYPE.PLAYER) {
					if (checkAlive && !csh.Alive ()) {
						//if checkalive and dead, don't add this one
						continue;
					} else {
						playersCharacterPVs.Add (p);
					}
				}
			} else {
				Debug.Log ("WARNING : Element of this list must have CharacterStatusHandler as its component");
			}
		}
		return playersCharacterPVs;
	}

	protected override void Start ()
	{
		base.Start ();

		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH,
				_health
			},
		});	
	}

	/// <summary>
	/// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
	/// </summary>
	/// <remarks>PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).</remarks>
	/// <param name="info">Info.</param>
	public override void OnPhotonInstantiate (PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate (info);

		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
				RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_CHARACTER_PV_ID,
				photonView.viewID
			}
		});	

//		//player character has been instantiated, update list for all players
//		NetworkController.NC.Update_Photon_View_List_Players();

//		//Update list of views
//		NetworkController.NC.Update_PVs ();
	}

	protected override void setHealth (int newHealth)
	{		
		if (_photonView.isMine) {
			PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {
					RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_HEALTH,
					newHealth
				},
			});	
		}

//		print ("Archer hit");
		if (_health > newHealth) {
//			print ("Archer damaged");
			//Archer falls off when attacked
			PlayerCombatHandler pch = GetComponent<PlayerCombatHandler> ();

			if (pch.GetClass () == CHARACTER_CLASS.ARCHER) {
				foreach (var a in pch.GetAbilities()) {
					if (a is AbilityCoopRide) {
						AbilityCoopRide acr = (AbilityCoopRide)a;
						if (acr.Riding) {
							acr.FallDown ();
//							print ("Archer falling down");
						} 
						break; 
					} 
				}
			} 
		}

		base.setHealth (newHealth);
	}

	/// <summary>
	/// Kill this instance and play ragdoll
	/// </summary>
	protected override void die ()
	{
		base.die ();

//		Invoke ("destroyObject", 3f);
		destroyObject (3f);

		if (_photonView.isMine) {
			if (GameController.GC.IsGameOver) {
				return;
			}

			if (RoomLevelHelper.ROOMTYPE == ROOM_TYPE.COOP) {
//				if ((PlayerCharacterStatusHandler.Get_PVs (true).Count < 1) && GameController.GC.Lives < 1) {
				if (GameController.GC.Lives < 1) {
					UIController.SINGLETON.SetDevText ("Game Over\nPress Q to return");
					GameController.GC.GameOver ();
				} else {
					UIController.SINGLETON.SetDevText ("Press R to revive(" + GameController.GC.Lives + " lives left)...");
					GameController.GC.AddDeathCount (1);
				}
			} else {
				if (GetComponent<CombatHandler> ().GetTeam () == TEAM.ONE) {
//					PvpManager.SINGLETON.SetTeamAlive (TEAM.ONE, PvpManager.SINGLETON.Team_1_alive - 1);
//					PvpManager.SINGLETON.SetTeamLife (TEAM.ONE, PvpManager.SINGLETON.Team_1_Life, PvpManager.SINGLETON.Team_1_alive - 1);
				} else if (GetComponent<CombatHandler> ().GetTeam () == TEAM.TWO) {
//					PvpManager.SINGLETON.SetTeamAlive (TEAM.TWO, PvpManager.SINGLETON.Team_2_alive - 1);
//					PvpManager.SINGLETON.SetTeamLife (TEAM.TWO, PvpManager.SINGLETON.Team_2_Life, PvpManager.SINGLETON.Team_2_alive - 1);
				}

				//for pvp, wait for the team when dead
//				if (PvpManager.SINGLETON.GetTeamLife (GetComponent<CombatHandler> ().GetTeam ()) < 1) {
//					UIController.SINGLETON.SetDevText ("No life left\nWaiting for your team to carry you...\nPress Q to return to main menu");
//				} else {
//					if (GetComponent<CombatHandler> ().GetTeam () == TEAM.ONE) {
////						PvpManager.SINGLETON.SetTeamAlive (TEAM.ONE, PvpManager.SINGLETON.Team_1_alive - 1);
//						UIController.SINGLETON.SetDevText ("Press R to revive(" + (PvpManager.SINGLETON.Team_1_Life) + " lives left)...");
//					} else if (GetComponent<CombatHandler> ().GetTeam () == TEAM.TWO) {
////						PvpManager.SINGLETON.SetTeamAlive (TEAM.TWO, PvpManager.SINGLETON.Team_2_alive - 1);
//						UIController.SINGLETON.SetDevText ("Press R to revive(" + (PvpManager.SINGLETON.Team_2_Life) + " lives left)...");
//					}
//				}
			}
		}
	}
}
