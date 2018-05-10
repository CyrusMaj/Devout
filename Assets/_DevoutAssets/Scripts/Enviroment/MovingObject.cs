using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PhotonView))]
public class MovingObject : Photon.PunBehaviour
{
	public float Speed = 3f;

	void Start ()
	{
////		Destroy (gameObject, 30f);
//		if (!photonView.isMine)
//			Destroy (this);

		Invoke ("destroy", 30f);
	}

	void destroy ()
	{
		if (!photonView.isMine)
			return;
		PhotonNetwork.Destroy (gameObject);
	}

	void Update ()
	{
		if (!photonView.isMine)
			return;
		transform.Translate (-transform.forward * Time.deltaTime * Speed);
	}
}
