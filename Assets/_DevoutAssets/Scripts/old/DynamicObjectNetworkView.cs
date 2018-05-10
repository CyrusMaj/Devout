using UnityEngine;
using System.Collections;

public class DynamicObjectNetworkView : Photon.PunBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;
	private Quaternion correctPlayerRot = Quaternion.identity;
	// We lerp towards this

	void Start ()
	{
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;
	}

	public override void OnJoinedRoom ()
	{

	}
	// Update is called once per frame
	void Update ()
	{
		if (!photonView.isMine) {
			transform.position = Vector3.Lerp (transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		}
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			//			 We own this player: send the others our data
			//			if ((tag == TagHelper.PLAYER || PhotonNetwork.isMasterClient)) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			//			stream.SendNext(
			//			}
			//			myThirdPersonController myC = GetComponent<myThirdPersonController>();
			//			stream.SendNext((int)myC._characterState);
		} else {
			// Network player, receive data
			this.correctPlayerPos = (Vector3)stream.ReceiveNext ();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext ();

			//			myThirdPersonController myC = GetComponent<myThirdPersonController>();
			//			myC._characterState = (CharacterState)stream.ReceiveNext();
		}
	}
}
