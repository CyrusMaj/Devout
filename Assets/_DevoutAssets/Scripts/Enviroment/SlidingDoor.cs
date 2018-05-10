using UnityEngine;
using System.Collections;
using Apex.WorldGeometry;

/// Written by Duke Im
/// On 2016-12-03
/// 
/// <summary>
/// Slide open or close a door
/// IE. Sliding up or down opening entrance to an areana
/// </summary>
public class SlidingDoor : MonoBehaviour {
	/// <summary>
	/// Position of this door when open
	/// </summary>
	[SerializeField] Vector3 _openPos;
	public Vector3 GetOpenPos(){return _openPos;}
	/// <summary>
	/// Position of this door when closed
	/// </summary>
	[SerializeField] Vector3 _closedPos;
	public Vector3 GetClosePos(){return _closedPos;}

	//Move door to pos
	public void Move(Vector3 targetPos){
		StopAllCoroutines ();
		StartCoroutine (MathHelper.IELerpLocalPositionOverTime (this.transform, this.transform.localPosition, targetPos, 1f));
		Invoke ("updateDynamicObstacle", 1f);
	}
	public IEnumerator IEMove(Vector3 targetPos, float delay){
		yield return new WaitForSeconds(delay);
		Move(targetPos);
//		Debug.Log ("Door IEMove after delay");
//		Debug.Break ();
	}
//
//	/// <summary>
//	/// Open this door
//	/// </summary>
//	public void Open(AISpawnerElevator spawner = null){
//		StopAllCoroutines ();
//		StartCoroutine (MathHelper.IELerpLocalPositionOverTime (this.transform, _closedPos, _openPos, 1f));
//		Invoke ("updateDynamicObstacle", 1f);
//		//To be deleted
////		if (spawner != null) {
//////			spawner.SetState (AISpawner.State.OPEN);
//////			spawner.DoNext ();
////			StartCoroutine(spawner.IEDoNextDelayed(spawner, AISpawnerElevator.State.OPEN, 1f));
////		}
//	}
//
//	/// <summary>
//	/// Close this door
//	/// </summary>
//	public void Close(AISpawnerElevator spawner = null){
//		StopAllCoroutines ();
//		StartCoroutine (MathHelper.IELerpLocalPositionOverTime (this.transform, _openPos, _closedPos, 1f));
//		Invoke ("updateDynamicObstacle", 1f);
//		//To be deleted
////		if (spawner != null) {
//////			spawner.SetState (AISpawner.State.CLOSED);
////			//			spawner.DoNext ();
////			StartCoroutine(spawner.IEDoNextDelayed(spawner, AISpawnerElevator.State.CLOSED, 1f));
////		}
//	}

	/// <summary>
	/// Update dynamic obstacle, if this object has one as a component
	/// </summary>
	void updateDynamicObstacle(){
		IDynamicObstacle dynamicObstacle = GetComponent<DynamicObstacle> ();
		if (dynamicObstacle != null) {
			dynamicObstacle.ActivateUpdates (null, false);
//			print ("Updated");
		}
	}

	//	IEnumerator IEDoNextDelayed(AISpawner spawner,AISpawner.State stateToSet,float duration){
	//		yield return new WaitForSeconds (duration);
	//		spawner.SetState (stateToSet);
	//		spawner.DoNext ();
	//	}
}
