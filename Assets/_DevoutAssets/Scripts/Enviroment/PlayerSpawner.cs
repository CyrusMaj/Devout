using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-1-9
/// 
/// <summary>
/// Handles spawning of player characters
/// </summary>
public class PlayerSpawner : MonoBehaviour {

	//TBD
	[SerializeField] Transform _spawnAreaTank;
	[SerializeField] Transform _spawnAreaWarrior;
	[SerializeField] Transform _spawnAreaArcher;
	[SerializeField] Transform _respawnArea;

	public void SpawnPlayer ()
	{
		Transform spawnArea = _spawnAreaWarrior;
		//spawn player character and set it to the GameController
		GameObject playerInstance = null;
		switch (RoomLevelHelper.PLAYER_CLASS) {
		case CHARACTER_CLASS.WARRIOR:
			playerInstance = PhotonNetwork.Instantiate ("Warrior", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			spawnArea = _spawnAreaWarrior;
			break;
		case CHARACTER_CLASS.TANK:
			playerInstance = PhotonNetwork.Instantiate ("Tank", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			spawnArea = _spawnAreaTank;
			break;
		case CHARACTER_CLASS.ARCHER:
			playerInstance = PhotonNetwork.Instantiate ("Archer", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			spawnArea = _spawnAreaArcher;
			break;
		default:
			Debug.LogWarning ("WARNING : Character not specified set when spawning");
			break;
		}
		if (playerInstance == null) {
			Debug.LogWarning ("WARNING : Character not instantiated, aborting");
			return;
		}
		GameController.GC.CurrentPlayerCharacter = playerInstance.transform;
		//set and enable camera
		CameraController.CC.InitializeCamera ();

		//if this is a revive
		playerInstance.transform.rotation = spawnArea.rotation;
		if (GameController.GC.DeathCount > 0) {
			if (PlayerCharacterStatusHandler.Get_PVs (true).Count > 0) {
				//there are alive chr to respawn to(not tested)
				int rnd = Random.Range (0, PlayerCharacterStatusHandler.Get_PVs (true).Count);
				Vector3? pos = getNearAlivePlayerPos (PlayerCharacterStatusHandler.Get_PVs (true) [rnd].GetComponent<AIMovementHandler> ());
				if(pos != null)
					spawnArea.position = (Vector3)pos;
				else
					spawnArea.position = _respawnArea.position;					
			} else {
				//respawn but no chr to respawn to
				spawnArea.position = _respawnArea.position;
			}
		} else {
			//spawn to original position if this is spawning for the first time when the game starts
			if ((PlayerCharacterStatusHandler.Get_PVs (true).Count > 0)) {
				//there are alive chr to respawn to
				int rnd = Random.Range (0, PlayerCharacterStatusHandler.Get_PVs (true).Count);
				Vector3? pos = getNearAlivePlayerPos (PlayerCharacterStatusHandler.Get_PVs (true) [rnd].GetComponent<AIMovementHandler> ());
				if(pos != null)
					spawnArea.position = (Vector3)pos;
			}
		}
		playerInstance.transform.position = spawnArea.position;

		//		dev
		//		GameController.GC.SetIsControlAllowed (false);
		GameController.GC.SetIsControlAllowed (true);
	}

	/// <summary>
	/// Gets the near alive player position to spawn a revived / connecting player
	/// </summary>
	/// <returns>The near alive player position.</returns>
	Vector3? getNearAlivePlayerPos (AIMovementHandler alivePlayer)
	{
		Vector3? pos = null;
		int rand = Random.Range (0, alivePlayer.MeleeSlots.Count);
		foreach (var slot in alivePlayer.GetComponent<AIMovementHandler>().MeleeSlots) {
			//check if position is movable
			if (alivePlayer.IsCellWalkable (slot.POS)) {
				pos = slot.POS;
			}	
		}

		return pos;
	}
}
