using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
/// <summary>
/// Protects spawning player(unit) from enemy units getting too close, by blocking with collider of the spawning unit's attack(RC#A)
/// </summary>
public class SpawnAreaProtector : MonoBehaviour {
	Collider _collider;

	void Start(){
		_collider = GetComponent<Collider> ();
		gameObject.layer = LayerHelper.COMMON_BODY;
		_collider.enabled = true;
	}
	public void EnableProtector(TEAM spawningTeam){
		gameObject.layer = LayerHelper.GetAttackBoxLayer (spawningTeam);
	}
	public void DisableProtector(){
		gameObject.layer = LayerHelper.COMMON_BODY;
	}
}
