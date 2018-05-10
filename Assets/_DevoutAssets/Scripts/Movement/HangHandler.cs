using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class HangHandler : MonoBehaviour
{
	public CharacterMovementHandler CMH;
	//	public Transform RootTransform;
	public Rigidbody RigidBody;
	public Animator Animator;
	int _hashHang;
	int _hashClimb;
	//	bool _hangCheck;
	//this is to set animation only once("Hanging")
	bool _climbCheck;
	Collider _hangTarget;

	//Y offset for aligning hang position
	public float YOffSet = -1.69f;

	void Start ()
	{
//		RigidBody = CMH.GetComponent<Rigidbody> ();
//		Animator = CMH.GetComponent<Animator> ();
		_hashHang = AnimationHashHelper.PARAM_HANG;
		_hashClimb = AnimationHashHelper.PARAM_CLIMB;
		_climbCheck = true;
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag (TagHelper.CLIMBABLE) && _hangTarget == null
		    && !CMH.IsGrounded) {

//			float angle = Quaternion.Angle (other.transform.rotation, CMH.transform.rotation);
//			if (angle < 45f) {
			_hangTarget = other;
//			print ("Hanging to " + _hangTarget.name);
//			CMH.Hanging = true;
			CMH.SetHanging (true);
			RigidBody.useGravity = false;
//			_hangCheck = true;
			AlignToHang (0.1f, 0.2f, 0.2f, _hangTarget);
			RigidBody.isKinematic = true;
			Animator.SetBool (_hashHang, true);

//			}
//			print (other.name + "angle : " + Quaternion.Angle (other.transform.rotation, CMH.transform.rotation).ToString ());
		}
	}

	//	void OnTriggerStay (Collider other)
	//	{
	//		if (other == _hangTarget && CMH.Hanging) {
	//			if (RigidBody.velocity == Vector3.zero && _hangCheck) {
	//				Animator.SetBool (_hashHang, true);
	//				_hangCheck = false;
	//			}
	//		}
	//	}

	void OnTriggerExit (Collider other)
	{
		if (other == _hangTarget) {
			HangToDrop ();
//			print ("Exiting collider");
//			Debug.Break ();
		}
	}

	public void HangToDrop ()
	{
		_hangTarget = null;
		Animator.SetBool (_hashHang, false);
//		RigidBody.useGravity = true;
//		CMH.Hanging = false;
		CMH.SetHanging (false);
		RigidBody.isKinematic = false;
	}

	public void HangToClimb ()
	{
		if (_climbCheck) {
			float climbTime = 1.5f;
			Animator.SetBool (_hashClimb, true);
			Vector3 posA = RigidBody.position;
			Vector3 posB = _hangTarget.transform.position;
			Vector3 offsetUp = _hangTarget.transform.up * 0.1f;
			Vector3 offsetFoward = _hangTarget.transform.forward * 0.5f;
			Vector3 offsetRight = _hangTarget.transform.InverseTransformPoint (posA);
			offsetRight = _hangTarget.transform.right * offsetRight.x * _hangTarget.transform.localScale.x;
			posB += offsetUp + offsetFoward + offsetRight;
			StartCoroutine (IEHangToDrop (climbTime));
			StartCoroutine (IEHangToClimb (climbTime, _hangTarget));
			StartCoroutine (MathHelper.IELerpPositionOverTime (RigidBody, posA, posB, climbTime));
			StartCoroutine (CoroutineHelper.IEChangeBool ((x) => _climbCheck = x, true, climbTime));
			_climbCheck = false;
			_hangTarget = null;
		}
	}

	private IEnumerator IEHangToDrop (float waitSeconds)
	{
		yield return new WaitForSeconds (waitSeconds);
		HangToDrop ();
	}

	private IEnumerator IEHangToClimb (float waitSeconds, Collider hangTarget)
	{
		RigidBody.isKinematic = true;
		yield return new WaitForSeconds (waitSeconds);
		Animator.SetBool (_hashClimb, false);
		RigidBody.isKinematic = false;
//		Vector3 posA = _rigidBody.position;
//		Vector3 posB = hangTarget.transform.position;
//		Vector3 offsetUp = hangTarget.transform.up * 1.0f;
//		Vector3 offsetFoward = hangTarget.transform.forward * 0.5f;
//		Vector3 offsetRight = hangTarget.transform.InverseTransformPoint (posA);
//		offsetRight = hangTarget.transform.right * offsetRight.x * hangTarget.transform.parent.localScale.x;
//		posB += offsetUp + offsetFoward + offsetRight;
	}

	/// <summary>
	/// Slow down, face the hang target and match position with the target
	/// </summary>
	/// <param name="secondsVel">Alignment will be completed in this seconds</param>
	public void AlignToHang (float secondsVel, float secondsRot, float secondsPos, Collider hangTarget)
	{
		Vector3 velA = RigidBody.velocity;
		Vector3 velB = Vector3.zero;
		Vector3 posA = RigidBody.position;
		Vector3 posB = hangTarget.transform.position;
		//		Vector3 offsetUp = hangTarget.transform.up * -1.69f;
		Vector3 offsetUp = hangTarget.transform.up * YOffSet;
		Vector3 offsetFoward = hangTarget.transform.forward * -0.45f;
		Vector3 offsetRight = hangTarget.transform.InverseTransformPoint (posA);
//		print (offsetRight.ToString ());
		offsetRight = hangTarget.transform.right * offsetRight.x * hangTarget.transform.localScale.x;
//		print (offsetRight.ToString ());
//		offsetRight = hangTarget.transform.right * offsetRight.x;
		Quaternion rotA = RigidBody.rotation;
		Quaternion rotB = _hangTarget.transform.rotation;
//		print (rotA.eulerAngles.ToString () + ", " + rotB.eulerAngles.ToString ());
		posB += offsetUp + offsetFoward + offsetRight;
		StartCoroutine (MathHelper.IELerpVelocityOverTime (RigidBody, velA, velB, secondsVel));
		StartCoroutine (MathHelper.IELerpRotationOverTime (RigidBody, rotA, rotB, secondsRot));
		StartCoroutine (MathHelper.IELerpPositionOverTime (RigidBody, posA, posB, secondsPos));
	}
}
