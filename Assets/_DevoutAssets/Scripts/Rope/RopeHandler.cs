using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RootMotion.FinalIK;

/// Written by Duke Im
/// On 2017-03-02
/// 
/// <summary>
/// Handles Rope behavior
/// </summary>
public class RopeHandler : Photon.PunBehaviour
{
	public Transform StartPoint;
	[HideInInspector]public Transform StartAttachPoint;
	[HideInInspector]public Transform StartTarget;
	public Transform EndPoint;
	[HideInInspector]public Transform EndAttachPoint;
	[HideInInspector]public Transform EndTarget;

	//Final IK Hand position to grab rope
	[SerializeField]InteractionObject _handGrabTarget;
	//	[SerializeField]InteractionObject _BodyTarget;
	InteractionSystem _intSys;

	//	Vector3 StartPoint, EndPoint;

	LineRenderer _lr;
	[HideInInspector] public CombatHandler Owner;

	public Transform RC_Collider;

	float _maxRopeDistance = 19f;

	public float GetMaxRopeDistance ()
	{
		return _maxRopeDistance;
	}

	[HideInInspector]public float _currentRopeDistance;
	[HideInInspector]float _holdRopeDistance;

	[HideInInspector]public bool IsSetup = false;

	Rigidbody _rb;

	bool _grounded = true;
	bool _isEndPointHigher = false;
	//endPoint is higer than startPoint(with offset)

	void Start ()
	{
		_lr = GetComponent<LineRenderer> ();

		if (!photonView.isMine)
			return;
		
		if (StartTarget.GetComponent<CombatHandler> () != null)
			StartTarget.GetComponent<CombatHandler> ().RopeSlotStart = this;		
		
		if (EndTarget.GetComponent<CombatHandler> () != null)
			EndTarget.GetComponent<CombatHandler> ().RopeSlotEnd = this;
		
		//make line collider ignore two drivers
		if (StartTarget.GetComponent<Collider> () && StartTarget.GetComponent<PhotonView> ()) {
//			Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), StartTarget.GetComponent<Collider> (), true);
			IgnoreCollision (StartTarget.GetComponent<PhotonView> ().viewID, true);
		}

		if (EndTarget.GetComponent<Collider> () && EndTarget.GetComponent<PhotonView> ()) {
			//			Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), EndTarget.GetComponent<Collider> (), true);
			IgnoreCollision (EndTarget.GetComponent<PhotonView> ().viewID, true);
		}

//		_rb = Owner.GetComponent<Rigidbody> ();

		_intSys = StartTarget.GetComponent<InteractionSystem> ();
	}

	public void IgnoreCollision (int pvID, bool ignore)
	{
		if (PhotonNetwork.offlineMode) {
			IgnoreCollision (pvID, ignore);
		} else
			NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCIgnoreCollision", pvID, ignore);
	}

	[PunRPC]
	void RPCIgnoreCollision (int pvID, bool ignore)
	{
		ignoreCollision (pvID, ignore);
	}

	void ignoreCollision (int pvID, bool ignore)
	{
		Collider c = PhotonView.Find (pvID).GetComponent<Collider> ();
		Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), c, ignore);

//		if (side == ATTACH_SIDE.START) {
//		} else {
//			Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), EndAttachPoint.parent.GetComponent<Collider> (), ignore);
//		}
//		switch (type) {
//		case ATTACH_TYPE.CHARACTER:
//			if (side == ATTACH_SIDE.START) {
//				Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), StartAttachPoint.parent.GetComponent<Collider> (), ignore);
//			} else {
//				Physics.IgnoreCollision (RC_Collider.GetComponent<Collider> (), EndAttachPoint.parent.GetComponent<Collider> (), ignore);
//			}
//			break;
//		case ATTACH_TYPE.SPIKE:
//			if (side == ATTACH_SIDE.START) {
//
//			} else {
//
//			}
//			break;
//		}
	}

	void FixedUpdate ()
	{
		//update points to attach points
		if (StartAttachPoint != null && EndAttachPoint != null) {
			if (EndPoint != null && StartPoint != null) {
				StartPoint.position = StartAttachPoint.position;
				EndPoint.position = EndAttachPoint.position;
			} else {
//				print ("null 1");
			}
		} else{
//			print ("null 2");
		}

		//draw rope with line renderer
		drawRope ();

		updateCollider ();

		_currentRopeDistance = Vector3.Distance (StartPoint.position, EndPoint.position);

		if (!photonView.isMine) {
			return;
		}

		//null mean death(destroyed target)?
		if (EndPoint == null || StartPoint == null) {
			Destroy ();
			return;
		}

		if (StartTarget != null) {
			if (StartTarget.GetComponent<CharacterMovementHandler> ())
				_grounded = StartTarget.GetComponent<CharacterMovementHandler> ().IsGrounded;
			else
				_grounded = true;	
		}

		if (EndPoint.position.y > StartPoint.position.y + 0.5f)
			_isEndPointHigher = true;
		else
			_isEndPointHigher = false;

		if (_intSys != null) {
			if (_grounded)
				_intSys.ResumeInteraction (FullBodyBipedEffector.RightHand);
			else if (_isEndPointHigher && !_intSys.IsInInteraction (FullBodyBipedEffector.RightHand) && !_intSys.IsPaused ())
				_intSys.StartInteraction (RootMotion.FinalIK.FullBodyBipedEffector.RightHand, _handGrabTarget, true);
		}
		//update raycasted version of the rope
		if (IsSetup)
			tether ();

		//position hand grab target
		_handGrabTarget.transform.position = StartPoint.position + (EndPoint.position - StartPoint.position).normalized * 0.5f;
		_handGrabTarget.transform.LookAt (EndPoint); 
		_handGrabTarget.transform.Rotate (new Vector3 (90f, 0f, 0f));

//		//draw rope with line renderer
//		drawRope ();

//		updateCollider ();

		checkForBending ();
	}

	void LateUpdate ()
	{
		if (!photonView.isMine)
			return;
		
		if (GameController.GC.CurrentPlayerCharacter == null || Owner == null || Owner.transform != GameController.GC.CurrentPlayerCharacter) {
			return;
		}

		CharacterMovementHandler cmh = StartTarget.GetComponent<CharacterMovementHandler> ();
		if (cmh) {
			cmh.Roped = false;
			if (_rb != null && _rb.gameObject == GameController.GC.CurrentPlayerCharacter) {
				_rb.drag = 1f;
			}
		}
		cmh = EndPoint.GetComponent<CharacterMovementHandler> ();
		if (cmh) {
			cmh.Roped = false;
		}

		//rotate body
		//Since it moves character rig, it must be in LateUpdate(after animation calculation is finished)
		//Also cannot smooth(Lerp or etc) as last position isn't accurate
		if (!_grounded && _isEndPointHigher) {
//			print ("Endpoint is higher than startpoint");

			Vector3 lookDir;
			if (_bendingPoint != null)
				lookDir = ((Vector3)_bendingPoint - StartPoint.position).normalized;
			else
				lookDir	= (EndPoint.position - StartPoint.position).normalized;

			Quaternion lookRot = Quaternion.LookRotation (lookDir);
//			StartAttachPoint.parent.transform.rotation = lookRot;
			StartAttachPoint.transform.rotation = lookRot;

			//
			cmh = StartTarget.GetComponent<CharacterMovementHandler> ();
			if (cmh) {
				cmh.Roped = true;
				if (_rb != null && _rb.transform == GameController.GC.CurrentPlayerCharacter) {
					_rb.drag = 0.1f;
//					print ("Setting drag : " + _rb.name);
//					Debug.Break ();
				} else {
//					print (_rb.name);
				}
			}
			cmh = EndPoint.GetComponent<CharacterMovementHandler> ();
			if (cmh) {
				cmh.Roped = true;

			}
		} else {
//			print (_grounded.ToString () + ", " + _isEndPointHigher.ToString ());
		}
	}

	float _lengthAdjustStrength = 5f;

	void Update ()
	{
		if (!photonView.isMine || GameController.GC.CurrentPlayerCharacter == null || Owner == null)
			return;
		
		if (Owner.transform != GameController.GC.CurrentPlayerCharacter) {
			return;
		}

		_intSys = StartTarget.GetComponent<InteractionSystem> ();

		_rb = StartTarget.GetComponent<Rigidbody> ();

		//retrieve if close to either of the point
		float retrievalDist = 2f;
		if (Owner == GameController.GC.CurrentPlayerCharacter.GetComponent<CombatHandler> ()
		    && (Vector3.Distance (Owner.transform.position, EndPoint.position) < retrievalDist
		    || Vector3.Distance (Owner.transform.position, StartPoint.position) < retrievalDist)) {
			if (Input.GetButtonDown (InputHelper.UNROPE) && !Owner.CheckAbilitiesInUse ()) {
//				print ("Rope retrieved");
				Destroy ();
				Owner.SetRopeCount (Owner.GetRopeCount () + 1);
			}
		}

		if (StartTarget.transform != GameController.GC.CurrentPlayerCharacter) {
			return;
		}

		if (IsSetup && StartTarget.gameObject == Owner.gameObject && _rb != null) {
			//move while hanging(swinging)
			if (Input.GetKey (KeyCode.W) && !_grounded && _isEndPointHigher) {
				if (_holdRopeDistance > 0) {
					_rb.velocity += CameraController.CC.CombatCamera.transform.forward * 3f * Time.deltaTime;
				}
			}

			//destroy when unrope
			if (Input.GetButtonDown (InputHelper.UNROPE) && !Owner.CheckAbilitiesInUse ()) {
				if (EndTarget.GetComponent<RopeArrow> ()) {
					Destroy (EndTarget.gameObject);
				}
				Destroy ();
//				StartTarget.GetComponent<CombatHandler> ().SetRopeCount (StartTarget.GetComponent<CombatHandler> ().GetRopeCount () + 1);
			}

//			//rope length adjust when hanging
//			if (!_grounded && _isEndPointHigher && _holdRopeDistance != 0f) {
//				if (Input.GetKey (KeyCode.LeftShift)) {
//					if (_holdRopeDistance < 0f) {
//						_holdRopeDistance = 0f;
////						print ("Rope retrieved by adjusting length");
////						Destroy ();
////						Owner.SetRopeCount (Owner.GetRopeCount () + 1);
//					} else {
////						print ("shortening : " + _holdRopeDistance);
//						_holdRopeDistance -= _lengthAdjustStrength * Time.deltaTime;
//					}
//				}
//				if (Input.GetKey (KeyCode.Space)) {
//					if (_holdRopeDistance > _maxRopeDistance) {
//						_holdRopeDistance = _maxRopeDistance;
//					} else {
////						print ("extending : " + _holdRopeDistance);
//						_holdRopeDistance += _lengthAdjustStrength * Time.deltaTime;
//					}
//				}
//			}
		}
	}

	void drawRope ()
	{
		//+ 1 is since EndPoint is not in _children list
		if (_bendingPoint != null)
			_lr.positionCount = 3;
		else
			_lr.positionCount = 2;

		_lr.SetPosition (0, StartPoint.position);

		if (_bendingPoint != null)
			_lr.SetPosition (1, (Vector3)_bendingPoint);

		_lr.SetPosition (_lr.positionCount - 1, EndPoint.position);
	}

	public void Destroy ()
	{
		CharacterMovementHandler cmh = StartTarget.GetComponent<CharacterMovementHandler> ();
		if (cmh) {
			cmh.Roped = false;
			if (_rb != null && _rb.gameObject == GameController.GC.CurrentPlayerCharacter) {
				_rb.drag = 1f;
			}
		}
		cmh = EndPoint.GetComponent<CharacterMovementHandler> ();
		if (cmh)
			cmh.Roped = false;
		
//		print ("Destroying rope");

		//destroy spikes
		if (StartAttachPoint.parent != null && StartAttachPoint.parent.name.Contains ("RopeSpike"))
			Destroy (StartAttachPoint.parent.gameObject);
		
		if (EndAttachPoint.parent != null && EndAttachPoint.parent.name.Contains ("RopeSpike"))
			Destroy (EndAttachPoint.parent.gameObject);

		//destory arrows
		if (StartAttachPoint.name.Contains ("RopeArrow"))
			Destroy (StartAttachPoint.gameObject);

		if (EndAttachPoint.name.Contains ("RopeArrow"))
			Destroy (EndAttachPoint.gameObject);
		
		Owner.MyRope = null;
		if (StartTarget.GetComponent<CombatHandler> ())
			StartTarget.GetComponent<CombatHandler> ().RopeSlotStart = null;		
		
		if (EndTarget.GetComponent<CombatHandler> ())
			EndTarget.GetComponent<CombatHandler> ().RopeSlotEnd = null;
		
		PhotonNetwork.Destroy (gameObject);
	}

	void tether ()
	{
		if (StartTarget == null || EndTarget == null)
			return;

		//both side must have rigidbody
		if (!StartTarget.GetComponent<Rigidbody> () || !EndTarget.GetComponent<Rigidbody> ()) {
			return;
		}
		
		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL;
		RaycastHit hit;

		if (_bendingPoint != null)
			_currentRopeDistance = Vector3.Distance (StartPoint.position, (Vector3)_bendingPoint) + Vector3.Distance ((Vector3)_bendingPoint, EndPoint.position);
		else
			_currentRopeDistance = Vector3.Distance (StartPoint.position, EndPoint.position);

		float ropeDistToCheck;

		//lock rope distance when hanging from the rope(when tied & not grounded)
		if (StartTarget.GetComponent<CharacterMovementHandler> ()) {
			if (StartTarget.GetComponent<CharacterMovementHandler> ().IsGrounded) {
				ropeDistToCheck = _maxRopeDistance;
				if (_holdRopeDistance != -5f) {
					_holdRopeDistance = -5f;
//				_rb.drag = 5f;
//				_rb.angularDrag = 0.5f;
				}
			} else {
				if (_holdRopeDistance == -5f) {
					if (_isEndPointHigher) {
//					_intSys.StartInteraction (RootMotion.FinalIK.FullBodyBipedEffector.RightHand, _handGrabTarget, true);
						_holdRopeDistance = _currentRopeDistance;
					} else {
						_holdRopeDistance = _currentRopeDistance + 3f;
					}
//				_rb.drag = 0.05f;
//				_rb.angularDrag = 0.01f;
				}
				ropeDistToCheck = _holdRopeDistance;
			}
		} else
			ropeDistToCheck = _maxRopeDistance / 3.5f;


		// Tether circular motion
		//restrict movment range
		if (_currentRopeDistance > ropeDistToCheck) {
			//Tether position inside the sphere around the EndPoint
			//			pull strength proportional to the distance
			float pullMultiplier = Mathf.Clamp (_currentRopeDistance - ropeDistToCheck, 0f, 10f);

//			print(_currentRopeDistance);

			//find(my character's proportion of) weights of both targets tied to this rope & distribute proportional to each weight
			float myWeightProportion;

			//pulling velocity/force
			Vector3 myPullVel;
			Vector3 targetPullVel;

			//pull directions for each end
			Vector3 myPullDir;
			Vector3 targetPullDir;

			if (_bendingPoint != null) {
				myPullDir = (Vector3)_bendingPoint - StartPoint.position;
				targetPullDir = (Vector3)_bendingPoint - EndPoint.position;
			} else {
				myPullDir = EndPoint.position - StartPoint.position;
				targetPullDir = -myPullDir;				
			}
			// && !EndTarget.gameObject.isStatic 
			if (EndTarget.GetComponent<RopeArrow> () == null && !EndTarget.gameObject.isStatic && EndTarget.GetComponent<Rigidbody> () != null && _rb != null) {
//				myWeightProportion = _rb.mass / (_rb.mass + EndTarget.GetComponent<Rigidbody> ().mass);
				myWeightProportion = EndTarget.GetComponent<Rigidbody> ().mass / (_rb.mass + EndTarget.GetComponent<Rigidbody> ().mass);
			} else {
				myWeightProportion = 1f;
			}

//			print (myWeightProportion.ToString ());

			if (pullMultiplier > 0f) {
				//finetune tethering
				pullMultiplier = 1f + pullMultiplier * 3f;
//				myPullVel = pullMultiplier * (EndPoint.position - StartPoint.position) * Time.fixedDeltaTime;
				myPullVel = pullMultiplier * myPullDir * myWeightProportion * Time.fixedDeltaTime;
				targetPullVel = pullMultiplier * targetPullDir * (1f - myWeightProportion) * Time.fixedDeltaTime;
			} else {
				myPullVel = Vector3.zero;
				targetPullVel = Vector3.zero;
			}

			//
			ObjectStatusHandler osh;
			if (_rb != null) {
				osh = _rb.GetComponent<ObjectStatusHandler> ();
				if (osh) {
//					if (PhotonView.Get (osh).isMine) {
//						_rb.velocity += myPullVel;
////						print ("mine : " + _rb.velocity.ToString());
//					} else
						osh.AddVelocity (myPullVel);
				} 
//				else
//					_rb.velocity += myPullVel;
//				print ("Pulled : " + myPullVel.ToString ());
//				print (_rb.name + ", " + EndTarget.GetComponent<Rigidbody> ().name);
			}

			if (EndTarget.GetComponent<Rigidbody> ()) {
				osh = EndTarget.GetComponent<ObjectStatusHandler> ();
				if (osh) {
//					if (PhotonView.Get (osh).isMine) {
////						print ("mine : " + targetPullVel.ToString ());
//						EndTarget.GetComponent<Rigidbody> ().velocity += targetPullVel;
//					} else
						osh.AddVelocity (targetPullVel);
				} 
//				else
//					EndTarget.GetComponent<Rigidbody> ().velocity += targetPullVel;
//				print ("Pulling : " + myPullVel.ToString ());
			}

			//dev
//			print ((_currentRopeDistance - ropeDistToCheck) + ", " + myPullVel.ToString ());
		}
	}

	void updateCollider ()
	{
		//rope collider is handled here
//		if (_currentRopeDistance < 3f) {
//			//if too close, disable it
//			_RC_Collider.gameObject.SetActive (false);
//		} else {
		RC_Collider.gameObject.SetActive (true);
		RC_Collider.position = StartPoint.position + (EndPoint.position - StartPoint.position) / 2f;
		RC_Collider.LookAt (StartPoint.transform);
		RC_Collider.Rotate (new Vector3 (90f, 0f, 0f));
		RC_Collider.GetComponent<CapsuleCollider> ().height = _currentRopeDistance;
//		}
	}

	/// <summary>
	/// There are 2 bending points for each rope where rope intersects with certain colliders
	/// if rope bends more than 2 times, it breaks
	/// </summary>
	Vector3? _bendingPoint = null;
	//	Vector3? _bendingPoint2 = null;

	void checkForBending ()
	{
		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL;
		RaycastHit hit;
		float range = Vector3.Distance (StartPoint.position, EndPoint.position);
		Vector3 dir = EndPoint.position - StartPoint.position;

		Debug.DrawRay (StartPoint.position, dir, Color.green);

		if (Physics.Raycast (StartPoint.position, dir, out hit, range, lm)) {
			if (_bendingPoint == null) {
//				_bendingPoint = (Vector3?)hit.point;
				SetBendingPoint ((Vector3?)hit.point);
//				print ("Bending");
			}
		} else {
//			_bendingPoint = null;
			SetBendingPoint (null);
		}

		if (_bendingPoint != null) {

		}
	}

	void SetBendingPoint (Vector3? pos)
	{
		if (PhotonNetwork.offlineMode) {
			setBendingPoint (pos);
		} else {
			NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCSetBendingPoint", pos);
		}
	}



	[PunRPC]
	void RPCSetBendingPoint (Vector3? pos)
	{
		setBendingPoint (pos);
	}

	void setBendingPoint (Vector3? pos)
	{
		_bendingPoint = pos;
	}

	//dev
	//for networking
	public void SetAttachPoint (ATTACH_SIDE side, ATTACH_TYPE type, int pvID)
	{
		if (PhotonNetwork.offlineMode) {
			setAttachPoint (side, type, pvID);
		} else {
			NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCSetAttachPoint", side, type, pvID);
		}
	}

	void setAttachPoint (ATTACH_SIDE side, ATTACH_TYPE type, int pvID)
	{
//		print (pvID);
//		Debug.Break ();
		
		GameObject go = PhotonView.Find (pvID).gameObject;
//		Transform attachPoint;
//
//		if (side == ATTACH_SIDE.START) {
//			attachPoint = StartAttachPoint;
//		} else {
//			attachPoint = EndAttachPoint;
//		}

		switch (type) {
		case ATTACH_TYPE.CHARACTER:
			if (side == ATTACH_SIDE.START)
				StartAttachPoint = go.GetComponent<PlayerCombatHandler> ().RopeTiePoint;
			else
				EndAttachPoint = go.GetComponent<PlayerCombatHandler> ().RopeTiePoint;
			break;
		case ATTACH_TYPE.SPIKE:
			if (side == ATTACH_SIDE.START)
				StartAttachPoint = go.transform.Find ("RopeAttachPoint");
			else
				EndAttachPoint = go.transform.Find ("RopeAttachPoint");
			break;
		case ATTACH_TYPE.OTHER:
			if (side == ATTACH_SIDE.START)
				StartAttachPoint = go.transform;
			else
				EndAttachPoint = go.transform;
			break;
		}

//		if (StartAttachPoint != null)
//			print ("Set : " + StartAttachPoint.name);
//		if (EndAttachPoint != null)
//			print ("Set : " + EndAttachPoint.name);
	}

	[PunRPC]
	void RPCSetAttachPoint (ATTACH_SIDE side, ATTACH_TYPE type, int pvID)
	{
		setAttachPoint (side, type, pvID);
	}

	public enum ATTACH_TYPE
	{
		CHARACTER,
		SPIKE,
		OTHER,
	}

	public enum ATTACH_SIDE
	{
		START,
		END
	}

	#region OldRope

	//
	//	public Rigidbody StartPoint;
	//	public Transform StartAttachPoint;
	//	public Transform StartTarget;
	//	public Rigidbody EndPoint;
	//	public Transform EndAttachPoint;
	//	public CharacterMovementHandler EndTarget;
	//	public Transform SegmentsParent;
	//	List<ConfigurableJoint> _segments = new List<ConfigurableJoint> ();
	//	public float JointDistance = 1f;
	//	public float PullSpeed = 50f;
	//	Vector3 _endTargetOffset;
	//	Vector3 _startTargetOffset;
	//	//position to move to
	//	Vector3? targetPos = null;
	//	LineRenderer _lr;
	//	[HideInInspector] public CombatHandler Owner;
	//	TYPE _type;
	//	float _maxRopeDistance;
	//
	//	//dev
	//	[SerializeField]Transform _RC_Collider;
	//
	//	void Start ()
	//	{
	//		//make line collider ignore two drivers
	//		Physics.IgnoreCollision(_RC_Collider.GetComponent<Collider>(), StartTarget.GetComponent<Collider>(), true);
	//		Physics.IgnoreCollision(_RC_Collider.GetComponent<Collider>(), EndTarget.GetComponent<Collider>(), true);
	//
	//		//default type is RC
	//		setType (TYPE.RC);
	//		//		setType(TYPE.CJ);
	//
	//		if (EndTarget) {
	//			EndPoint.transform.SetParent (EndAttachPoint);
	//			EndPoint.transform.position = EndAttachPoint.position;
	//			EndPoint.rotation = Quaternion.identity;
	//			//			EndPoint.transform.localRotation = Quaternion.Euler(new Vector3 (0, 0, 90f));
	//
	//			_endTargetOffset = EndTarget.transform.position - EndAttachPoint.position;
	//			_startTargetOffset = StartTarget.position - StartAttachPoint.position;
	//		} else
	//			EndPoint.transform.parent = null;
	//
	//		_segments.Clear ();
	//
	//		foreach (Transform c in SegmentsParent) {
	//			ConfigurableJoint cj = c.GetComponent<ConfigurableJoint> ();
	//			//			if (!cj)
	//			//				Destroy (c.gameObject);
	//			//			else
	//			if (cj)
	//				_segments.Add (cj);
	//		}
	//
	//		if (StartTarget) {
	//			StartPoint.transform.SetParent (StartAttachPoint);
	//			StartPoint.transform.position = StartAttachPoint.position;
	//			StartPoint.rotation = Quaternion.identity;
	//		}
	//
	//		_lr = GetComponent<LineRenderer> ();
	//		_lr.numPositions = _segments.Count + 1;
	//
	//		_maxRopeDistance = _segments.Count * JointDistance - JointDistance;
	//		//		print ("Segments cound : " + _segments.Count + ", jointDist : " + JointDistance + ", maxRopeDist : " + _maxRopeDistance);
	//
	//		//		print (_endTargetOffset.ToString ());
	//
	//		//		InvokeRepeating ("slowUpdate", 0.1f, 0.25f);
	//	}
	//
	//	void FixedUpdate ()
	//	{
	//
	//		//apply gravity and cap velocity
	//		foreach (var cj in _segments) {
	//			Rigidbody cjrb = cj.GetComponent<Rigidbody> ();
	//
	//			if (cjrb.velocity.sqrMagnitude > 15f) {
	//				cjrb.velocity *= 0.99f;
	//			}
	//
	//			if (_type == TYPE.CJ) {
	//				cjrb.AddForce (Vector3.down * 15f);
	//			}
	//		}
	//
	//		//update raycasted version of the rope
	//		UpdateRaycastRope ();
	//
	//		//update each joints of the rope
	//		updateConfigurableJointRope ();
	//
	//	}
	//
	//	//	void Update ()
	//	//	{
	//	//		print (EndTarget.IsGrounded.ToString ());
	//	//	}
	//
	//	void LateUpdate ()
	//	{
	//		//draw rope with line renderer
	//		drawRope ();
	//
	//	}
	//
	//	void setType (TYPE type)
	//	{
	//		_type = type;
	//
	//		if (_type == TYPE.CJ) {
	//			_RC_Collider.gameObject.SetActive (false);
	//		} else {
	//			_RC_Collider.gameObject.SetActive (true);
	//		}
	//
	//		print ("Changing type : " + _type.ToString ());
	//	}
	//
	//	/// <summary>
	//	/// There are two types of rope,
	//	/// Configurable Joint(CJ) for when character is hanging from the rope(Roped)
	//	/// Ray Casting(RC) for when character is grounded
	//	/// </summary>
	//	public enum TYPE
	//	{
	//		CJ,
	//		RC,
	//	}
	//
	//	void drawRope ()
	//	{
	//		if (_type == TYPE.CJ && EndTarget.Roped) {
	//			//+ 1 is since EndPoint is not in _children list
	//			_lr.numPositions = _segments.Count;// + 1;
	//
	//			for (int i = 1; i < _segments.Count; i++) {
	//				_lr.SetPosition (i, _segments [i].transform.position);
	//			}
	//			if (StartTarget) {
	//				//				_lr.SetPosition (0, StartTarget.position - _startTargetOffset);
	//				_lr.SetPosition (0, StartPoint.position);
	//			}
	//			if (EndTarget) {
	//				//				_lr.SetPosition (_lr.numPositions - 1, EndTarget.transform.position - _endTargetOffset);
	//				_lr.SetPosition (_lr.numPositions - 1, EndPoint.position);
	//			}
	//		} else {
	//			//+ 1 is since EndPoint is not in _children list
	//			_lr.numPositions = 2;
	//
	//			_lr.SetPosition (0, StartPoint.position);
	//			_lr.SetPosition (1, EndPoint.position);
	//		}
	//	}
	//
	//	void RopeUnHang ()
	//	{
	//		foreach (var cj in _segments) {
	//			cj.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	//		}
	//
	//		EndTarget.Roped = false;
	//		//		EndTarget.transform.parent = null;
	//
	//		EndPoint.transform.SetParent (EndAttachPoint);
	//
	//		EndPoint.transform.position = EndAttachPoint.position;
	//
	//		if (EndTarget is PlayerMovementControl) {
	//			PlayerMovementControl pmc = (PlayerMovementControl)EndTarget;
	//			pmc.SetIsMovementAllowed (true);
	//		}
	//		EndPoint.isKinematic = true;
	//		EndPoint.useGravity = false;
	//		//		EndTarget.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
	//		EndTarget.GetComponent<Rigidbody> ().useGravity = true;
	//
	//		setType (TYPE.RC);
	//		//		print ("unhang");
	//	}
	//
	//	void RopeHang ()
	//	{
	//		EndTarget.Roped = true;
	//		EndTarget.GetComponent<Rigidbody> ().useGravity = false;
	//
	//		EndPoint.transform.SetParent (_segments [_segments.Count / 2].transform.parent);
	//
	//		//		EndTarget.transform.SetParent (EndPoint.transform);
	//
	//		Vector3 offset = EndAttachPoint.position - EndTarget.transform.position;
	//		EndTarget.transform.position = EndPoint.transform.position - offset;
	//		//		EndTarget.transform.position = EndAttachPoint.position;
	//
	//		EndPoint.rotation = Quaternion.Euler (new Vector3 (0f, EndTarget.transform.rotation.eulerAngles.y, 180f));
	//
	//		EndTarget.transform.rotation = Quaternion.Euler (new Vector3 (0f, EndTarget.transform.rotation.eulerAngles.y, 0f));
	//
	//		//		EndTarget.transform.localPosition = Vector3.zero;
	//		//		EndTarget.transform.localPosition = _endTargetOffset * 10f;
	//		//		EndTarget.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
	//
	//		if (EndTarget is PlayerMovementControl) {
	//			PlayerMovementControl pmc = (PlayerMovementControl)EndTarget;
	//			pmc.SetIsMovementAllowed (false);
	//		}
	//		EndPoint.isKinematic = false;
	//		//		EndPoint.useGravity = true;
	//	}
	//
	//	public void Destroy ()
	//	{
	//		Owner.MyRope = null;
	//		StartTarget.GetComponent<CombatHandler> ().RopeSlotStart = null;
	//		EndTarget.GetComponent<CombatHandler> ().RopeSlotEnd = null;
	//		PhotonNetwork.Destroy (gameObject);
	//	}
	//
	//	void UpdateRaycastRope ()
	//	{
	//		if (_type == TYPE.RC) {
	//			//		public static int TEAM_1_BODY = ~(1 << LayerMask.NameToLayer ("RCB") | 1 << LayerMask.NameToLayer ("RC1H") | 1 << LayerMask.NameToLayer ("RC1A") | 1 << LayerMask.NameToLayer("Rope") | 1 << LayerMask.NameToLayer("RCN"));
	//			LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL;
	//			RaycastHit hit;
	//			float range = Vector3.Distance (StartPoint.position, EndPoint.position);
	//
	//			Vector3 dir = EndPoint.position - StartPoint.position;
	//			//			Debug.DrawRay (StartPoint.position, dir, Color.green);
	//
	//			// in a function
	//			if (Physics.Raycast (StartPoint.position, dir, out hit, range, lm)) {
	//				//if hit and not grounded, change it to CJ
	//				if (!EndTarget.IsGrounded) {
	//					setType (TYPE.CJ);
	//					//					print ("hit : " + hit.collider.name);
	//					return;
	//					//					Debug.Break ();
	//				} else {
	//					//					print ("hit : " + hit.collider.name + " EndTarget not grounded");
	//					//					setType (TYPE.RC);
	//				}
	//			}
	//
	//			//restrict movment range
	//			if (Vector3.Distance (StartPoint.position, EndPoint.position) > _maxRopeDistance) {
	//				Vector3 targetPos = EndPoint.position - StartPoint.position;
	//				targetPos.Normalize ();
	//				targetPos = targetPos * (_maxRopeDistance - JointDistance);
	//				//				targetPos = StartTarget.transform.position + targetPos * (_maxRopeDistance);
	//				targetPos = StartTarget.transform.position + targetPos;
	//				EndTarget.GetComponent<Rigidbody> ().MovePosition (Vector3.Lerp (EndTarget.transform.position, (Vector3)targetPos, Time.fixedDeltaTime * PullSpeed));
	//				//				print ("pulling");
	//			}
	//			//collider is handled here
	//			//if too close, disable it
	//			if (range < 3f) {
	//				_RC_Collider.gameObject.SetActive (false);
	//			} else {
	//				//				_RC_Collider.gameObject.SetActive (false);
	//				_RC_Collider.gameObject.SetActive (true);
	//				//				print ("setting collider true, _type : " + _type.ToString() + ", enteringValue : " + devType);
	//				_RC_Collider.position = StartPoint.position + (EndPoint.position - StartPoint.position) / 2f;
	//				_RC_Collider.LookAt (StartPoint.transform);
	//				_RC_Collider.Rotate (new Vector3 (90f, 0f, 0f));
	//				//			_RC_Collider.localScale = new Vector3 (5f, range * 5f, 5f);
	//				//				_RC_Collider.GetComponent<CapsuleCollider> ().height = range * 1f - 1.4f;
	//				_RC_Collider.GetComponent<CapsuleCollider> ().height = range;
	//			}
	//		} else if (EndTarget.IsGrounded) {
	//			setType (TYPE.RC);
	//		}
	//	}
	//
	//	/// <summary>
	//	/// When rope is collided with an
	//	/// </summary>
	//	void updateConfigurableJointRope ()
	//	{
	//		//restrict movement of connected parts
	//		int restrictCount = 0;
	//		//things that is too far off
	//		int restrictUrgentCount = 0;
	//
	//		foreach (ConfigurableJoint cj in _segments) {
	//			if (!cj.connectedBody) {
	//				continue;
	//			}
	//
	//			//			Rigidbody cjrb = cj.GetComponent<Rigidbody> ();
	//			//
	//			//			if (cjrb.velocity.sqrMagnitude > 10f) {
	//			//				cjrb.velocity *= 0.99f;
	//			//			}
	//			//
	//			//			cjrb.AddForce (Vector3.down * 10f);
	//
	//			if (Vector3.Distance (cj.connectedBody.position, cj.transform.position) > JointDistance * 2.5f) {
	//				restrictUrgentCount++;
	//				//				print ("too far!");
	//			}
	//
	//			if (Vector3.Distance (cj.connectedBody.position, cj.transform.position) > JointDistance * 1.1f) {
	//				Vector3 offset = cj.transform.position - cj.connectedBody.position;
	//				offset.Normalize ();
	//				offset *= JointDistance * 1f;
	//				cj.transform.position = cj.connectedBody.position + offset;
	//				//				cj.transform.position = Vector3.Lerp (cj.transform.position, cj.connectedBody.position + offset, Time.deltaTime * 100f);
	//				//				cj.GetComponent<Rigidbody> ().MovePosition (cj.connectedBody.position + offset);
	//				restrictCount++;
	//			} else {
	//				//add weak gravity
	//				//				cj.GetComponent<Rigidbody> ().AddForce (Vector3.down * 10f);
	//			}
	//			//			cj.GetComponent<Rigidbody>().velocity = Vector3.down * 10f;
	//		}
	//
	//		if (!EndTarget)
	//			return;
	//
	//		if (_type == TYPE.CJ) {
	//			//find the last(one before "End" joint)
	//			if ((restrictCount > _segments.Count * 2 / 4 || restrictUrgentCount > 1)
	//				||
	//				(EndTarget.IsGrounded && restrictUrgentCount > 0)) {
	//				//		if (restrictCount > _children.Count * 3 / 4 || restrictUrgentCount > 1) {
	//				//				print ("Endpoint : " + cj.name);
	//				if ((Vector3.Distance (EndAttachPoint.position, _segments [_segments.Count - 1].transform.position) > JointDistance * 1f)) {
	//					//					print ("adjusting endTarget position");
	//					Vector3 attachOffset = EndAttachPoint.position - _segments [_segments.Count - 1].transform.position;
	//
	//					//average / estimate pull direction
	//					Vector3 est = _segments [_segments.Count - 2].transform.position - _segments [_segments.Count - 1].transform.position;
	//					Vector3 est1 = _segments [_segments.Count - 3].transform.position - _segments [_segments.Count - 2].transform.position;
	//					Vector3 est2 = _segments [_segments.Count - 4].transform.position - _segments [_segments.Count - 3].transform.position;
	//					Vector3 est3 = _segments [_segments.Count - 5].transform.position - _segments [_segments.Count - 4].transform.position;
	//
	//					Vector3 average = attachOffset + est + est1 + est2 + est3;
	//					//				pull = new Vector3 (pull.x, 0f, pull.z);
	//					average.Normalize ();
	//					average *= JointDistance;
	//
	//					attachOffset.Normalize ();
	//					attachOffset *= JointDistance;
	//					//				Vector3 result = _children [_children.Count - 1].transform.position + attachOffset + _endTargetOffset;
	//					Vector3 result = _segments [_segments.Count - 1].transform.position + average * 1f + _endTargetOffset;
	//
	//					if (EndTarget.IsGrounded) {
	//						//					EndTarget.GetComponent<Rigidbody> ().MovePosition (new Vector3 (result.x, EndTarget.transform.position.y, result.z));
	//						//					EndTarget.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	//						targetPos = (Vector3?)new Vector3 (result.x, EndTarget.transform.position.y, result.z);
	//					} else {
	//						if (result.y > EndTarget.transform.position.y)
	//							result = new Vector3 (result.x, EndTarget.transform.position.y, result.z);
	//						targetPos = (Vector3?)result;
	//						//					EndTarget.GetComponent<Rigidbody> ().MovePosition (result);
	//						//					EndTarget.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	//						//					print("Vel : " + EndTarget.GetComponent<Rigidbody>().velocity.ToString());
	//					}
	//				}
	//
	//				if (targetPos != null) {
	//					if (Vector3.Distance (EndTarget.transform.position, (Vector3)targetPos) < JointDistance) {
	//						targetPos = null;
	//						EndTarget.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	//					} else {
	//						EndTarget.GetComponent<Rigidbody> ().MovePosition (Vector3.Lerp (EndTarget.transform.position, (Vector3)targetPos, Time.fixedDeltaTime * PullSpeed));
	//						//					EndTarget.GetComponent<Rigidbody> ().MovePosition ((Vector3)targetPos);
	//						EndTarget.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	//					}
	//				}
	//
	//				if (!EndTarget.IsGrounded) {
	//					RopeHang ();
	//				} else {
	//					//				EndTarget.Roped = false;
	//				}
	//			}
	//		}
	//
	//		if (EndTarget.IsGrounded && EndTarget.Roped) {
	//			RopeUnHang ();
	//		} else if (!EndTarget.IsGrounded && EndTarget.Roped) {
	//			RopeHang ();
	//		}
	//	}

	#endregion
}
