using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRocksManager : Photon.PunBehaviour
{
	public List<Transform> SpawnPoints;
	public List<GameObject> RockPrefabs;

	float _timer = 0f;

	void Start ()
	{
		_timer = Time.time;	

		InvokeRepeating ("slowUpdate", 0f, 1f);
	}

	void slowUpdate ()
	{
		if (!PhotonNetwork.isMasterClient)
			return;
		
		if (_timer < Time.time) {
			int rnd = Random.Range (0, RockPrefabs.Count);
			int rnd_2 = Random.Range (0, SpawnPoints.Count);
//			GameObject go = Instantiate (RockPrefabs [rnd]);
//			string rockPrefab = "FloatingRock_"+rnd;
			GameObject go = PhotonNetwork.Instantiate (RockPrefabs [rnd].name, SpawnPoints [rnd_2].position, SpawnPoints [rnd_2].rotation, 0);
//			go.transform.position = SpawnPoints [rnd].position;
//			go.transform.rotation = SpawnPoints [rnd].rotation;
			go.transform.rotation = Quaternion.Euler (go.transform.rotation.eulerAngles + new Vector3 (-90f, 0f, 0f));
//			go.transform.SetParent (transform);
			NetworkManager.SINGLETON.SetParent (go.GetComponent<PhotonView> ().viewID, photonView.viewID);
			_timer = Time.time + Random.Range (3, 5);
		}
	}
}
