using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On 2016-10-04
///
/// <summary>
/// Particle controller.
/// Handles instantiation of particles.
/// Handlers RPC call of instantiation if connected on network.
/// </summary>
[RequireComponent (typeof(PhotonView))]
public class ParticleController : MonoBehaviour
{
	public static ParticleController PC;

	[SerializeField] GameObject _particlePrefab;

	[SerializeField] List<ParticleController.Particle> _particles;

	PhotonView _pv;

	void Start ()
	{
		if (PC == null)
			PC = this;
		else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTROLLER");

		_pv = PhotonView.Get (this);
	}

//	public void InstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f)
//	{
//		if (PhotonNetwork.offlineMode)
//			StartCoroutine (IEInstantiateParticle (type, position, waitTime));
//		else {
//			_pv.RPC ("RPCInstantiateParticle", PhotonTargets.All, type, position, waitTime);
//		}
//	}
//
//	[PunRPC]
//	void RPCInstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f)
//	{
//		StartCoroutine (IEInstantiateParticle (type, position, waitTime));
//	}
//
//	IEnumerator IEInstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f)
//	{
//		yield return new WaitForSeconds (waitTime);
//
////		if (_particlePrefab) {
//		foreach (var p in _particles) {
//			if (p.TYPE == type) {
//				GameObject particleInstance = (GameObject)Instantiate (p.Prefab);
//				particleInstance.transform.position = position;
//				yield break;
//			}
//		}
//
//		Debug.Log ("Particle type not found");
////		}
//	}

	public void InstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f, float duration = 0f, Transform followTarget = null)
	{
		if (PhotonNetwork.offlineMode)
			StartCoroutine (IEInstantiateParticle (type, position, waitTime, duration, followTarget));
		else {
			_pv.RPC ("RPCInstantiateParticle", PhotonTargets.All, type, position, waitTime, duration, followTarget);
		}
	}

	[PunRPC]
	void RPCInstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f, float duration = 0f, Transform followTarget = null)
	{
		StartCoroutine (IEInstantiateParticle (type, position, waitTime, duration, followTarget));
	}

	IEnumerator IEInstantiateParticle (PARTICLE_TYPE type, Vector3 position, float waitTime = 0f, float duration = 0f, Transform followTarget = null)
	{
		yield return new WaitForSeconds (waitTime);

		//		if (_particlePrefab) {
		GameObject particleInstance = null;
		foreach (var p in _particles) {
			if (p.TYPE == type) {
				particleInstance = (GameObject)Instantiate (p.Prefab);
				particleInstance.transform.position = position;
//				if (followTarget != null) {
//					particleInstance.transform.SetParent (followTarget);
//				}
				break;
			}
		}

		if (particleInstance == null) {
			Debug.Log ("Particle type not found");
		} else {
			float timer = Time.time + duration;
			while (timer > Time.time) {
				particleInstance.transform.position = followTarget.position;
				Destroy (particleInstance, duration);
				yield return null;
			}
		}
		//		}
	}

	[System.Serializable]
	public class Particle
	{
		public PARTICLE_TYPE TYPE;
		public GameObject Prefab;
	}

	public enum PARTICLE_TYPE
	{
		WARRIOR_ULT_TORNADO,
		HIT,
		BLOCK,
		DEATH_LAVA,
	}
}
