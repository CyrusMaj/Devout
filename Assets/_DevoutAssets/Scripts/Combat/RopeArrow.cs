using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeArrow : Arrow
{
	protected override void Start ()
	{
		PhotonView pv = GetComponent<PhotonView> ();
		if (pv != null) {
			if (pv.isMine) {

			} else {
				GetComponent<Collider> ().enabled = false;
				Destroy (this);
				Destroy (GetComponent<Weapon> ());
				Destroy (GetComponent<Collider> ());
				Destroy (GetComponent<Rigidbody> ());
				return;
			}
		}

		_collider = GetComponent<Collider> ();
//		Destroy (gameObject, _lifeTime);

		initRope ();
		SetIsActive (true);
	}

	protected override void FixedUpdate ()
	{
		if (PhotonView.Get (this) == null || !PhotonView.Get (this).isMine)
			return;
		//		transform.Translate (transform.forward * Speed * Time.fixedDeltaTime);
		if (_isActive)
			transform.position += transform.forward * Time.fixedDeltaTime * _speed;
//		_isActive = true;
	}

	RopeHandler _rh;

	void initRope ()
	{
		CombatHandler ch = _weapon.GetOwner ();
		PlayerCombatHandler mpch = (PlayerCombatHandler)ch;

		if (mpch.RopeSlotStart == null) {
			GameObject go = PhotonNetwork.Instantiate ("Rope_05", ch.transform.position, Quaternion.identity, 0);
			_rh = go.GetComponent<RopeHandler> ();
			ch.MyRope = _rh;
			_rh.Owner = ch;
			_rh.StartTarget = mpch.transform;
//			_rh.StartAttachPoint = mpch.RopeTiePoint;
			_rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.CHARACTER, mpch.GetComponent<PhotonView> ().viewID);

			_rh.EndTarget = transform;
//			_rh.EndAttachPoint = transform;
			_rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.OTHER, GetComponent<PhotonView> ().viewID);
			mpch.RopeSlotStart = _rh;
		} else {
			_rh = mpch.RopeSlotStart;
			_rh.StartTarget = transform;
//			_rh.StartAttachPoint = transform;
			_rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.START, RopeHandler.ATTACH_TYPE.OTHER, GetComponent<PhotonView> ().viewID);

			mpch.RopeSlotStart = null;
			_rh.IsSetup = false;
		}
	}

	void Update ()
	{
		if (!_rh.IsSetup)
			cutIfFar ();
	}

	//	void SetPos(Vector3 pos){
	//		if (PhotonNetwork.offlineMode)
	//			setPos (pos);
	//		else
	//			NetworkManager.SINGLETON.Targeted_RPC (PhotonView.Get (this), "PRCSetPos", pos);
	//	}
	//
	//	[PunRPC]
	//	void PRCSetPos(Vector3 pos){
	//		setPos (pos);
	//	}
	//
	//	void setPos(Vector3 pos){
	//		transform.position = pos;
	//	}

	protected override void stick (Collision collision)
	{
		if (_rh.StartTarget.GetComponent<CombatHandler> () == null) {
			_rh.Owner.SetRopeCount (_rh.Owner.GetRopeCount () - 1);
			//unignore collision
//			Physics.IgnoreCollision (_rh.RC_Collider.GetComponent<Collider> (), _rh.Owner.GetComponent<Collider> (), false);
//			_rh.IgnoreCollision (_rh.Owner.GetComponent<PhotonView> ().viewID, false);
//			if(collision.collider.GetComponent<PhotonView>() != null)
//				_rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.OTHER, collision.collider.GetComponent<PhotonView>().viewID);
//			_rh.IsSetup = true;
//
//			print ("CALLED");
//			if (IsMine) {
//				ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point);
//			}
//			_speed = 0f;
//			SetIsActive (false);

//			if (collision.collider.GetComponent<PhotonView> () != null)
//				_rh.SetAttachPoint (RopeHandler.ATTACH_SIDE.END, RopeHandler.ATTACH_TYPE.OTHER, collision.collider.GetComponent<PhotonView> ().viewID);

			//			SetPos (transform.position);
			//disable roping character for now
			_rh.Destroy ();
			Destroy (gameObject);

			return;
		} else if (collision.collider.GetComponent<CombatHandler> () != null) {
			//disable roping character for now
			_rh.Destroy ();
			Destroy (gameObject);
			return;
		} else {
//			print ("HUT");
//			_speed = 0f;
//			transform.SetParent (collision.transform);
			SetIsActive (false);
//			_collider.enabled = false;

			//link endtarget if it can be pulled(has rigidbody)
			if (collision.gameObject.GetComponent<Rigidbody> () != null) {
				_rh.EndTarget = collision.transform;
			}
			_rh.IsSetup = true;

			return;
		}

		//link endtarget if it can be pulled(has rigidbody)
		if (collision.gameObject.GetComponent<Rigidbody> () != null) {
			_rh.EndTarget = collision.transform;
		}
		
		base.stick (collision);
//		_rh._currentRopeDistance = Vector3.Distance (_rh.StartPoint.position, _rh.EndPoint.position);
//		print ("Rope distance : " + _rh._currentRopeDistance);
		_rh.IsSetup = true;
	}

	void cutIfFar ()
	{
		float distance = Vector3.Distance (_rh.StartPoint.position, _rh.EndPoint.position);
		if (distance > _rh.GetMaxRopeDistance ()) {
			_rh.Destroy ();
			Destroy (gameObject);
//			print ("Too far : " + distance + ", " + _rh.GetMaxRopeDistance());
//			_weapon.GetOwner ().SetRopeCount (_weapon.GetOwner ().GetRopeCount() + 1);
		}
	}
}
