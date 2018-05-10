using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On 2016-12-03
/// 
/// <summary>
/// Handles overall execution of a level including victory condition check in order to move onto the next section / level
/// </summary>
public class SectionHandler : Photon.PunBehaviour
{
	/// <summary>
	/// List of AI spawners used for spawning AIs
	/// </summary>
	[SerializeField] List<NPCSpawner> _NPCSpawners = new List<NPCSpawner> ();
	//Enemies left to spawn
	int TotalEnemiesPool = 40;

	[SerializeField] int _enemiesAtOnce = 6;
	//	[SerializeField] int _minAISpawnCount = 3;//how many enemies lacking

	void Start ()
	{
		if (PhotonNetwork.isMasterClient)
			InvokeRepeating ("UpdateSection", 5f, 1f);
	}

	public void UpdateSection ()
	{
		if (!PhotonNetwork.isMasterClient)
			return;
		if (TotalEnemiesPool > 0) {
			//if there's any enemy left, spawn
			if (AIStatusHandler.Get_PVs (true).Count < _enemiesAtOnce) {
				int aiToSpawn = Mathf.Clamp (_enemiesAtOnce - AIStatusHandler.Get_PVs (true).Count, 0, Mathf.Min (TotalEnemiesPool, 4));
				_NPCSpawners.Shuffle ();
				foreach (var s in _NPCSpawners) {
					//Spawn if ready
					if (s.IsAvailable && aiToSpawn > 0) {
//						print ("current alive AIs : " + AIStatusHandler.Get_PVs (true).Count + ", spawning : " + aiToSpawn);
						s.Spawn (aiToSpawn);
						//deduct once spawned
						TotalEnemiesPool -= aiToSpawn;
						return;
					}
				}
			}
		}
	}
}
