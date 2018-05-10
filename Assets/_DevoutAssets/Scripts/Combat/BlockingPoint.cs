using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-04
///
/// <summary>
/// Blocking point when active(enabled collision), block abilities
/// </summary>
public class BlockingPoint: Hitbox {	
	/// <summary>
	/// The duration of the stagger.
	/// Attacker's will stagger when its attack hits this BlockingPoint for this duration.
	/// Attackers will not stagger if this is set to 0.
	/// </summary>
	[SerializeField]float _staggerDuration = 0f;

	/// <summary>
	/// Gets the duration of the stagger.
	/// </summary>
	/// <returns>The stagger duration.</returns>
	public float GetStaggerDuration(){
		return _staggerDuration;
	}

	protected override void Start ()
	{
		base.Start ();
//		_pos = transform.position;
//		_rot = transform.rotation;
	}

//	Vector3 _pos;
//	Quaternion _rot;

//	void Update(){
//		transform.position = _pos;
//		transform.rotation = _rot;
//	}
	//dev
//	void OnCollisionEnter(Collision collision) {
//		print (collision.gameObject.name);
//	}
}
