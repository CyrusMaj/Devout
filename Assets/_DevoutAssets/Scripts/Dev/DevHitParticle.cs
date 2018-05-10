using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class DevHitParticle : MonoBehaviour {
	ParticleSystem _ps;
	// Use this for initialization
	void Start () {
		_ps = GetComponent<ParticleSystem> ();
//		DestroyObject (gameObject, _ps.duration);
		Destroy (gameObject, _ps.duration);
	}

//	void Update () {
//		if (!_ps.IsAlive ())
//			Destroy (gameObject);
//	}
}
