using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Hitbox that takes damage from damagingpoint on hit
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour {
	/// <summary>
	/// ObjectStatusHandler of linked object
	/// </summary>
	protected ObjectStatusHandler _osh;
	/// <summary>
	/// The collider of this hitbox
	/// </summary>
	protected Collider _collider;
	protected virtual void Start(){
		_collider = GetComponent<Collider> ();
	}

	//dev
	//collider attached to the rig seems to be spinning like hell
	//Thus force-prevent spinning
//	void LateUpdate(){
//		if (_osh is CharacterStatusHandler) {
//			transform.localPosition = Vector3.zero;
//			transform.localRotation = Quaternion.identity;
//		} 
////		else
////			print ("osh : " + _osh.name);
//	}

	//find better sollution if possible
	/// <summary>
	/// Gets the collider of this hitbox
	/// </summary>
	/// <returns>The collider.</returns>
	public Collider GetCollider(){
		if(_collider == null)
			_collider = GetComponent<Collider> ();			
		return _collider;
	}
	/// <summary>
	/// Set ObjectStatusHandler
	/// </summary>
	/// <param name="osh">Osh.</param>
	public void SetOSH(ObjectStatusHandler osh){
		_osh = osh;
	}
	/// <summary>
	/// Gets the ObjectStatusHandler that holds this hitbox
	/// </summary>
	/// <returns>The OS.</returns>
	public ObjectStatusHandler GetOSH(){
		return _osh;
	}
}
