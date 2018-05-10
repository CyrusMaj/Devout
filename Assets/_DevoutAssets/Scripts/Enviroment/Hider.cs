using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-12-03
/// 
/// <summary>
/// Hide / Show objects as it enters/exit
/// Ie. hidden pulley system, spawning objects from hidden places
/// </summary>
[RequireComponent (typeof(Collider))]
public class Hider : MonoBehaviour
{
	void Start(){
		//disable mesh renderer of this object when starting
		MeshRenderer mr = GetComponent<MeshRenderer> ();
		if (mr != null) {
			mr.enabled = false;
		}
	}
	void OnTriggerEnter (Collider other)
	{
		//disable mesh renderer of this object when entering
		MeshRenderer mr = other.GetComponent<MeshRenderer> ();
		if (mr != null) {
			mr.enabled = false;
		}
	}

	void OnTriggerExit (Collider other)
	{
		//enable mesh renderer of this object when entering
		MeshRenderer mr = other.GetComponent<MeshRenderer> ();
		if (mr != null) {
			mr.enabled = true;
		}
	}

//	void OnTriggerStay(Collider other) {
//		print ("Trigger stay");
//	}
}
