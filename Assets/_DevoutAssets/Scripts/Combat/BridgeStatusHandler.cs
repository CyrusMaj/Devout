using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeStatusHandler : ObjectStatusHandler
{
	protected override void Start ()
	{
		base.Start ();
		InvokeRepeating ("slowUpdate", 1.0f, 1f);
	}

	void slowUpdate(){
		SetOSH ();
	}

	protected void SetOSH(){
		if (!PhotonNetwork.connected)
			return;
		
		if (PhotonNetwork.offlineMode)
			setOSH ();
		else if(NetworkManager.SINGLETON != null)
			NetworkManager.SINGLETON.Targeted_RPC (_photonView, "RPCSetOSH");
	}

	void setOSH(){
		foreach (var hb in _hitboxes) {
			hb.SetOSH (this);
		}
	}

	[PunRPC]
	protected void RPCSetOSH(){
		setOSH ();
	}

	void Update ()
	{
		#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.C)) {
			destroy ();
		}
		#endif
	}

	protected override void checkAlive ()
	{
		if (_health < 1) {
			destroy ();
		}		
	}

	void destroy ()
	{
		foreach (var hb in _hitboxes) {
			if (hb.transform.parent.GetComponent<HingeJoint> () != null)
				Destroy (hb.transform.parent.GetComponent<HingeJoint> ());
			if (hb.transform.parent.GetComponent<SpringJoint> () != null)
				Destroy (hb.transform.parent.GetComponent<SpringJoint> ());
			if (hb.transform.parent.GetComponent<Rigidbody> () != null) {
				float rand = Random.Range (-1f, 1f);
				hb.transform.parent.GetComponent<Rigidbody> ().velocity = new Vector3 (rand, rand, rand);
				hb.transform.parent.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (rand, rand, rand);
			}
		}
		Destroy (gameObject, 2f);
	}
}
