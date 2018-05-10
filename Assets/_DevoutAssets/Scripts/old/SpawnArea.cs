using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class SpawnArea : MonoBehaviour
{
//	float _originalYPos;
//	[SerializeField] Transform _startPos;
//	[SerializeField] Transform _startPos, _endPos;

//	[SerializeField] Transform _prisonBar;

//	[HideInInspector] public bool _isPrisonBarOpened { get; private set; }

	[HideInInspector] public Transform _spawnedTarget { get; private set; }

	void Start ()
	{
//		_originalYPos = _prisonBar.position.y;
		_spawnedTarget = null;
	}

//	public void OpenPrisonBar ()
//	{
//		StartCoroutine (IEOpenPrisonBar ());
//	}
//
//	public void ClosePrisonBar ()
//	{
//		StartCoroutine (IEClosedPrisonBar ());
//	}

//	IEnumerator IEOpenPrisonBar ()
//	{
//		float targetYPos = _originalYPos + 2.2f;
//		while (_prisonBar.position.y < targetYPos) {
////			_prisonBar.Translate (0f, 0.02f, 0f);
//			_prisonBar.position = new Vector3 (_prisonBar.position.x, _prisonBar.position.y + 0.02f, _prisonBar.position.z);
//			yield return new WaitForFixedUpdate ();
//		}
//		_isPrisonBarOpened = true;
//
//		StartCoroutine (IEMoveTarget (_endPos.position, false));
//	}

//	IEnumerator IEClosedPrisonBar ()
//	{
//		while (_prisonBar.position.y > _originalYPos) {
//			//			_prisonBar.Translate (0f, -0.02f, 0f);
//			_prisonBar.position = new Vector3 (_prisonBar.position.x, _prisonBar.position.y - 0.02f, _prisonBar.position.z);
//			yield return new WaitForFixedUpdate ();
//		}	
//		_isPrisonBarOpened = false;
//		_spawnedTarget = null;
//	}

	public void SetSpawnedTarget (Transform newTarget)
	{
		if (PhotonNetwork.isMasterClient) {
			_spawnedTarget = newTarget;
//			StartCoroutine (IEMoveTarget (_startPos.position, true));
		}
	}

	void OnTriggerExit(Collider other) {
		if (_spawnedTarget != null && other.transform == _spawnedTarget) {
//			print ("Exited");
			_spawnedTarget = null;
		}

	}

//	IEnumerator IEMoveTarget (Vector3 pos, bool Open)
//	{
//		CharacterMovementHandler cmh = _spawnedTarget.GetComponent<CharacterMovementHandler> ();
//		while (Vector3.Distance (_spawnedTarget.position, pos) > 0.05f) {
////			cmh.Move (1f, 0f, false, false);
//
//			if (cmh is DevEnemyAIBehavior) {
//				((DevEnemyAIBehavior)cmh).SetInputV (1f);
//			}
//
//			cmh.TurnTowardsPos (pos);
//			yield return new WaitForFixedUpdate ();
//
//			if (_spawnedTarget == null)
//				break;
//		}

//		if (Open)
//			OpenPrisonBar ();
//		else {
//			ClosePrisonBar ();
//			if (cmh is DevEnemyAIBehavior) {
//				((DevEnemyAIBehavior)cmh).SetEnableBehavior (true);
//				((DevEnemyAIBehavior)cmh).SetInputV (0f);
////				print ("Attacking now!");
//			}
//		}
//	}
}
