using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollHelper : MonoBehaviour
{
	public bool IsCollidersEnabledOnStart = false;
	public bool IsUseGravityOnStart = false;
	public bool IsKinematicOnStart = false;
	public bool FreezePositionOnStart = false;
	public bool FreezeRotationOnStart = false;
	//	public bool ProjectionEnabledOnStart = false;
	public RigidbodyInterpolation RI;
	public CollisionDetectionMode CDM;
	List<Rigidbody> _lstRigidbody;
	List<CharacterJoint> _lstCharacterJoint;
	// Use this for initialization
	void Start ()
	{
		_lstRigidbody = new List<Rigidbody> ();
		foreach (var r in GetComponentsInChildren<Rigidbody>()) {
//			if (r.gameObject.layer == LayerHelper.NEAUTRAL) {
			if (r.gameObject.layer == LayerHelper.RAGDOLL) {
				if (!_lstRigidbody.Contains (r) &&
				    r.CompareTag (TagHelper.RAGDOLL)) {
					_lstRigidbody.Add (r);
				}
			}
		}
		_lstCharacterJoint = new List<CharacterJoint> ();
		foreach (var r in GetComponentsInChildren<CharacterJoint>()) {
			if (!_lstCharacterJoint.Contains (r)) {
				_lstCharacterJoint.Add (r);
			}
		}
		EnableColliders (IsCollidersEnabledOnStart);
		SetKinematic (IsKinematicOnStart);
		UseGravity (IsUseGravityOnStart);
		FreezePosition (FreezePositionOnStart);
		FreezeRotation (FreezeRotationOnStart);
		SetInterpolation (RI);
		SetCollisionDetectionMode (CDM);
//		SetProjectionEnabled (ProjectionEnabledOnStart);

//		print ("ragdoll count : " + _lstRigidbody.Count);
	}

	public void SetProjectionEnabled (bool isEnabled)
	{
		foreach (var cj in _lstCharacterJoint) {
			cj.enableProjection = isEnabled;
		}
	}

	public void SetProjectionEnabled ()
	{
		foreach (var cj in _lstCharacterJoint) {
			cj.enableProjection = true;
		}
	}

	public void SetKinematic (bool isKinematic)
	{
		foreach (var r in _lstRigidbody) {
			r.isKinematic = isKinematic;
		}
	}

	public void SetInterpolation (RigidbodyInterpolation ri)
	{
		foreach (var r in _lstRigidbody) {
			r.interpolation = ri;
		}
	}

	public void SetCollisionDetectionMode (CollisionDetectionMode cdm)
	{
		foreach (var r in _lstRigidbody) {
			r.collisionDetectionMode = cdm;
		}
	}

	public void FreezePosition (bool freezeOrNot)
	{
		foreach (var r in _lstRigidbody) {
			if (freezeOrNot)
				r.constraints = RigidbodyConstraints.FreezePosition;
		}
	}

	public void FreezeRotation (bool freezeOrNot)
	{
		foreach (var r in _lstRigidbody) {
			if (freezeOrNot)
				r.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}

	public void EnableColliders (bool enableOrDisable)
	{
		foreach (var r in _lstRigidbody) {
			if (r.GetComponent<Collider> () != null)
				r.GetComponent<Collider> ().enabled = enableOrDisable;
		}
	}

	public void UseGravity (bool UseOrNot)
	{
		foreach (var r in _lstRigidbody) {
			r.useGravity = UseOrNot;
		}
	}
}
