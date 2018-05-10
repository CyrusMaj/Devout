using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2016-12-03
	/// 
	/// <summary>
	/// Handles overall execution of a level including victory condition check in order to move onto the next section / level
	/// </summary>
	public class EventSpawnNPC : Event
	{
		/// <summary>
		/// Should this event start when scene starts?
		/// </summary>
		[SerializeField] bool _triggerOnStart = false;

		/// <summary>
		/// List of AI spawners used for spawning AIs
		/// </summary>
		[SerializeField] List<NPCSpawner> _NPCSpawners = new List<NPCSpawner> ();
		//Enemies left to spawn
		public int TotalEnemiesPool = 40;
		[SerializeField] int _enemiesAtOnce = 6;

		public override void StartEvent ()
		{
			base.StartEvent ();
			
			if (PhotonNetwork.isMasterClient)
				InvokeRepeating ("updateSection", 2f, 1f);
			else
				print ("Not master");
		}

		/// <summary>
		/// When pool is empty(Note there may still be alive NPCs just spawned)
		/// </summary>
		public override void EndEvent ()
		{
			base.EndEvent ();
			CancelInvoke ();
		}

		void updateSection ()
		{
			if (!PhotonNetwork.isMasterClient) {
				print ("not master");
				return;
			}
			
			if (TotalEnemiesPool > 0) {
				//if there's any enemy left, spawn
				if (AIStatusHandler.Get_PVs (true).Count < _enemiesAtOnce) {
					int aiToSpawn = Mathf.Clamp (_enemiesAtOnce - AIStatusHandler.Get_PVs (true).Count, 0, Mathf.Min (TotalEnemiesPool, 4));
					_NPCSpawners.Shuffle ();
					foreach (var s in _NPCSpawners) {
						//Spawn if ready
						if (s.IsAvailable && aiToSpawn > 0) {
//							print ("current alive AIs : " + AIStatusHandler.Get_PVs (true).Count + ", spawning : " + aiToSpawn);
							s.Spawn (aiToSpawn);
							//deduct once spawned
							TotalEnemiesPool -= aiToSpawn;
							return;
						}
					}
				}
			} else {
//				print ("pool empty");
				EndEvent ();
			}
		}

		void Start ()
		{
			if (_triggerOnStart) {
				StartEvent ();
			}
		}
	}
}