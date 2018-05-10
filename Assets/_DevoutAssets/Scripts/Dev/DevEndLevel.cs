using UnityEngine;
using System.Collections;

public class DevEndLevel : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerCharacterStatusHandler> () && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().isMine) {		
			PhotonNetwork.Disconnect ();
			StartCoroutine (CoroutineHelper.IELoadAsyncScene ("MainMenu"));
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
