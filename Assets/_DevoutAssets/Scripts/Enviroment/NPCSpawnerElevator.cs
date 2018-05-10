using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On 2016-12-03
/// 
/// <summary>
/// Spawns AI as well as handles enabling & disabling AI after instantiation
/// </summary>
public class NPCSpawnerElevator : NPCSpawner
{
	//Where to spawn npcs on this eleavtor
	[SerializeField] List<Transform> _spawnPoints = new List<Transform> ();
	//recently spawned characters
	List<AIMovementHandler> _spawnedAIs = new List<AIMovementHandler> ();
	//Characters currently on this elavator
	[SerializeField]CharactersInArea _charactersOnBoard;

	public List<AIMovementHandler> GetSpawnedAIs ()
	{
		return _spawnedAIs;
	}
	//Where spaned AIs are moving towards before aquiring a target, in order to clear from the initial position(eleavtor)
	[SerializeField] Transform _releaseMoveTarget;
	//Position to stop when going under killzone
	[SerializeField] Vector3 _drawnPos;
	//position to stop for spawning
	[SerializeField] Vector3 _upPos;
	//position to stop for releasing spawned AIs
	[SerializeField] Vector3 _downPos;

	public void SetIsAvailable (bool newIsAvailable)
	{
		this.IsAvailable = newIsAvailable;
	}
	//door for this spawner
	[SerializeField] SlidingDoor _door;
	//to be deleted
	public SlidingDoor GetDoor ()
	{
		return _door;
	}

	/// <summary>
	/// Sequence of action of a spawning elevator from spawning to release and back to spawn-ready state(simplified)
	/// </summary>
	public override void Spawn (int count)
	{
		//abort if not ready
		if (!IsAvailable) {
			return;
		}
		float delay = 0f;
		IsAvailable = false;
		StopAllCoroutines ();
		//spawn AIs
		_spawnedAIs.Clear ();
		InstantiateAIs (count);//todo : rename this to Instantiate or soemthing along that line
		//Move to release pos
		delay += 0.5f;
		StartCoroutine (IEMove (delay, _downPos, 2f));
		delay += 2f;
		//Open door and after a short delay, release AIs
		StartCoroutine (_door.IEMove (_door.GetOpenPos (), delay));
		delay += 1f;
		StartCoroutine (IERelease (delay, _releaseMoveTarget.position));
		delay += 3f;
		//wait until elevator is clear(empty)
		StartCoroutine (IEWaitUntilClear (delay));

		//old
//		//Then close the door
//		StartCoroutine (_door.IEMove (_door.GetClosePos (), delay));
//		delay += 1f;
//		//Move underwater to clear any remaning or new objects on board
//		StartCoroutine (IEMove (delay, _drawnPos, 2f));
//		delay += 2f;
//		//Move up to spawning position
//		StartCoroutine (IEMove (delay, _upPos, 3f));
//		delay += 3f;
//		Debug.Break ();
		//Set availability for next spawn depending on the total delay
//		IsAvailable = false;
//		StartCoroutine (CoroutineHelper.IEChangeBool ((x) => IsAvailable = x, true, delay));
	}

	/// <summary>
	/// Wait until the elevator is empty
	/// </summary>
	IEnumerator IEWaitUntilClear (float delay)
	{
		yield return new WaitForSeconds (delay);
		bool isClear = false;
//		print (_charactersOnBoard.GetCharactersInArea ().Count);
		while (!isClear) {
			if (_charactersOnBoard.GetCharactersInArea ().Count < 1)
				isClear = true;
			yield return null;
		}
//		print (_charactersOnBoard.GetCharactersInArea ().Count);
		EndSpawn ();
	}

	/// <summary>
	///  Close door, return to original position
	/// </summary>
	void EndSpawn ()
	{
		float delay = 0f;
		//Then close the door
		StartCoroutine (_door.IEMove (_door.GetClosePos (), delay));
		delay += 1f;
		//Move underwater to clear any remaning or new objects on board
		StartCoroutine (IEMove (delay, _drawnPos, 2f));
		delay += 2f;
		//Move up to spawning position
		StartCoroutine (IEMove (delay, _upPos, 3f));
		delay += 3f;
		StartCoroutine (CoroutineHelper.IEChangeBool ((x) => IsAvailable = x, true, delay));
	}

	/// <summary>
	/// move the elevator to a position(lerp transform.position over time)
	/// </summary>
	void Move (Vector3 targetPos, float duration = 2f)
	{
		Vector3 fromPos = this.transform.localPosition;
		Vector3 toPos = targetPos;
		StartCoroutine (MathHelper.IELerpLocalPositionOverTime (this.transform, fromPos, toPos, duration));
	}
	//Move with delay
	IEnumerator IEMove (float delay, Vector3 targetPos, float duration = 2f)
	{
		yield return new WaitForSeconds (delay);
		Move (targetPos, duration);
	}

	void Start ()
	{
		SetIsAvailable (false);
		StartCoroutine (IEWaitUntilClear (3f));
	}

	/// <summary>
	/// Spawn an AI
	/// </summary>
	/// <param name="count">Count.</param>
	public void InstantiateAIs (int count)
	{
		_spawnedAIs.Clear ();
		count = Mathf.Clamp (count, 1, _spawnPoints.Count);
		for (int i = 0; i < count; i++) {

			GameObject AIInstance;
			//chance of spawning differnt types of minions
//			if (Random.Range (0, 10) > 2) { 
			//regular minion
			if (Random.Range (0, 2) > 0) {
				AIInstance = PhotonNetwork.Instantiate (NetworkHelper.NPC_PALADIN_01, new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			} else {
				AIInstance = PhotonNetwork.Instantiate (NetworkHelper.NPC_PALADIN_02, new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			}
//			} else {
//				//stronger minion
//				AIInstance = PhotonNetwork.Instantiate (NetworkHelper.NPC_BRUTE_01, new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
//			}

			AIInstance.transform.position = _spawnPoints [i].position;
			AIInstance.transform.rotation = _spawnPoints [i].rotation;
			AIInstance.transform.SetParent (this.transform);
			_spawnedAIs.Add (AIInstance.GetComponent<AIMovementHandler> ());

			//disable steer component until release(otherwise it will moonwalk)
			AIInstance.GetComponent<Apex.Steering.SteerableUnitComponent> ().enabled = false;
		}
//		print ("Spawn finished");
	}

	/// <summary>
	/// Release the AIs to moveToPos
	/// </summary>
	/// <param name="moveToPos">Move to position.</param>
	void release (Vector3 moveToPos)
	{
		foreach (var aimh in _spawnedAIs) {
			if (aimh == null)
				print ("aimh is null");
			aimh.MoveTo (moveToPos);
			aimh.transform.SetParent (null);

			//allow movement
//			aimh.GetComponent<AIBehavior> ().SetIsBehaviorAllowed (true);
			aimh.GetComponent<Apex.Steering.SteerableUnitComponent> ().enabled = true;
		}
	}

	IEnumerator IERelease (float delay, Vector3 moveToPos)
	{
		yield return new WaitForSeconds (delay);
		release (moveToPos);
	}

	public enum State
	{
		OPEN,
		RELEASED,
		CLOSED,
		DRAWN,
		UP,
		SPAWN,
		DOWN,
	}
}
