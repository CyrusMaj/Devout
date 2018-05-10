using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Enemy AI handler.
/// Handler class provides non-control methods to specified category(AI for this example)
/// </summary>
public class EnemyAIHandler : MonoBehaviour {
	/// <summary>
	/// List of enemies used, for example, players to find and lock on
	/// </summary>
//	public static List<EnemyAIHandler> EnemyList = new List<EnemyAIHandler>();

	//on scene loaded deligate
//	void OnEnable(){
//		SceneManager.sceneLoaded += onSceneLoaded;
//	}
//	void OnDisable(){
//		SceneManager.sceneLoaded -= onSceneLoaded;
//	}

//	void onSceneLoaded(Scene scene, LoadSceneMode mode){
//		EnemyList.Clear ();
//	}

	// Use this for initialization
//	void Start () {
//		if (EnemyList.Contains (this))
//			print ("ERROR : Already contains this Enemy");
//		else
//			EnemyList.Add (this);

//		//dev
//		if (PhotonView.Get (this).isMine) {
//			print ("This AI is mine");
//		}
//	}

	/// <summary>
	/// Gets the closest enemyAI from EnemyList
	/// </summary>
	/// <returns>The closest enemy.</returns>
	/// <param name="searchPos">Search position.</param>
//	public static EnemyAIHandler GetClosestEnemy(Vector3 searchPos){
//		EnemyAIHandler closestEAIH = null;
//		foreach (var p in AIStatusHandler.Get_PVs(true)) {
//			CombatHandler ch = p.GetComponent<CombatHandler> ();
//			if (ch != null) {
//				//check if enemy
//
//			}
//			else{
//				Debug.LogWarning ("WARNING : this photonview must have combathandler as its component");
//			}
//		}
//		foreach (var e in EnemyList) {
//			if (closestEAIH == null)
//				closestEAIH = e;
//			else if (Vector3.Distance (e.transform.position, searchPos) < Vector3.Distance (closestEAIH.transform.position, searchPos)) {
//				closestEAIH = e;
//			}
//		}
//		return closestEAIH;
//	}

	/// <summary>
	/// Gets the closest enemyAI from EnemyList
	/// </summary>
	/// <returns>The closest enemy distance.</returns>
	/// <param name="searchPos">Search position.</param>
//	public static float GetClosestEnemyDistance(Vector3 searchPos){
//		float distance = 9999f;
////		List<EnemyAIHandler> emptyLst = new List<EnemyAIHandler> ();
//		EnemyList.RemoveAll(x=>x == null);
//		foreach (var e in EnemyList) {
//			if(Vector3.Distance (e.transform.position, searchPos) < distance) {
//				distance = Vector3.Distance (e.transform.position, searchPos);
//			}
//		}
//		return distance;
//	}
}
