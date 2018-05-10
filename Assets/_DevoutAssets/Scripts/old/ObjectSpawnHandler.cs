using UnityEngine;
using System.Collections;

/// <summary>
/// Handles instantiating objects. Used only by room master
/// </summary>
public class ObjectSpawnHandler : Photon.PunBehaviour
{
	[SerializeField] string prefabName;
	//if already positioned and needs to be replaced with instantiation
	[SerializeField] Transform Preset;

	public override void OnJoinedRoom ()
	{
		if (Preset != null) {
			if (PhotonNetwork.isMasterClient) {
				print ("called");
				Preset.gameObject.SetActive (false);
				GameObject playerInstance 	= PhotonNetwork.Instantiate (prefabName, Preset.transform.position, Preset.transform.rotation, 0);
				playerInstance.transform.SetParent (this.transform);
			}
			GameObject.Destroy (Preset.gameObject);
		}
	}
}