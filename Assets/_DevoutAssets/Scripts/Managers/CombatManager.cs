using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2017-03-05
///
/// <summary>
/// Manages top-level combat stuff including stuff like rope
/// </summary>
public class CombatManager : MonoBehaviour {
	public static CombatManager SINGLETON;
//	public GameObject RopePrefab;

	void Start(){
		if (!SINGLETON)
			SINGLETON = this;
	}
}
