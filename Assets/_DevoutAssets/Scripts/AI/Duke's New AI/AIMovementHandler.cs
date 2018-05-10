using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Apex;
using Apex.Steering;
using Apex.Units;
using Apex.PathFinding;
using System;
using Apex.Services;
using Apex.WorldGeometry;
using Apex.Common;

public class AIMovementHandler : CharacterMovementHandler, IMoveUnits
{
	/// <summary>
	/// The dynamic obstacle for local avoidance
	/// </summary>
	[SerializeField] DynamicObstacle _dynamicObstacle;

	/// <summary>
	/// The position slots where other characters can use to pathfind to surround this character
	/// </summary>
	[HideInInspector]public List<PositionSlot> MeleeSlots = new List<PositionSlot> ();
	/// <summary>
	/// Gets a value indicating whether this instance is moving.
	/// </summary>
	/// <value><c>true</c> if this instance is moving; otherwise, <c>false</c>.</value>
	public bool IsMoving{ get; private set;}
	//dev
//	public bool IsMoving;
	/// <summary>
	/// The melee position slots when surrouding this unit
	/// </summary>
	int _meleeSlotCount = 6;
	float _meleeSlotDistance = 2.0f;
	public float GetMeleeSlotDistance(){
		return _meleeSlotDistance;
	}
	void setUpPositionSlots(){
		//setup meleePositionSlots
		MeleeSlots.Clear();
		for (int i = 0; i < _meleeSlotCount; i++) {
			PositionSlot ps = new PositionSlot ();
			MeleeSlots.Add (ps);
		}
	}
	void slowUpdatePositionSlots ()
	{
		//parametric equation for a circle(center (cx,cz))
		//x = cx + r * cos(a)
		//z = cz + r * sin(a)
		//r = distance
		//get 6 slots(positions) around the target
		float cx = this.transform.position.x;
		float cy = this.transform.position.y;//Note : How will this affect in slope?s
		float cz = this.transform.position.z;
		float a = 360 / (float)_meleeSlotCount;
		for (int i = 0; i < _meleeSlotCount; i++) {
			MeleeSlots [i].POS = new Vector3 (cx + _meleeSlotDistance * Mathf.Cos (i * a * Mathf.Deg2Rad), cy, cz + _meleeSlotDistance * Mathf.Sin (i * a * Mathf.Deg2Rad));
			//set csh to null if the character is dead
			if (MeleeSlots [i].CHS != null && !MeleeSlots [i].CHS.Alive ()) {
				MeleeSlots [i].CHS = null;
			}
		}
	}

	/// <summary>
	/// The apex unit used to move this unit using apex path
	/// </summary>
	private IMovable _apexUnit;

	//cache
	IUnitFacade _unitFacade;

	void Awake ()
	{
		_unitFacade = this.GetUnitFacade ();
		_apexUnit = this.As<IMovable> ();

		if (_dynamicObstacle != null) {
			setDynamicAINumber ();
		}
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

		//default is running for AIs
		_hsc.Run ();

		setUpPositionSlots();

		//update slots
		InvokeRepeating("slowUpdatePositionSlots", 0f, 0.1f);
	}

	public void MoveTo (Vector3 position, bool append = false)
	{
		_apexUnit.MoveTo (position, append);
	}

	#region IMoveUnits implementation

	/// <summary>
	/// When unit is moving, this gets called(once per frame?)
	/// </summary>
	/// <param name="velocity">The velocity.</param>
	/// <param name="deltaTime">Time since last invocation in ms</param>
	public void Move (Vector3 velocity, float deltaTime)
	{
		if(_checkGrounded)
			CheckGroundStatus ();

		if (!IsGrounded)
			HandleAirborneMovement (velocity);
		
		ScaleCapsuleForCrouching ();
		if (velocity.magnitude > 0.5f) {
			UpdateAnimator (1f, 0f);
			IsMoving = true;
		} else {
			UpdateAnimator (0f, 0f);
			IsMoving = false;
		}

		//don't move when not movment animation is playing
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash != AnimationHashHelper.STATE_GROUNDED)
			return;

		this.transform.position = this.transform.position + (velocity * deltaTime);
	}

	public void Rotate (Vector3 targetOrientation, float angularSpeed, float deltaTime)
	{ 
//		this.transform.Rotate(
		//		this.transform.localRotation
		var targetRotation = Quaternion.LookRotation (targetOrientation);
		this.transform.rotation = Quaternion.RotateTowards (this.transform.rotation, targetRotation, angularSpeed * Mathf.Rad2Deg * deltaTime);
	}

	/// <summary>
	/// Whem unit reached target
	/// </summary>
	public void Stop ()
	{
		//		testDObstacleMove ();
		UpdateAnimator (0f, 0f);
		IsMoving = false;
//		print ("stopped");
	}

	#endregion
	/// <summary>
	/// Stop / cancel current movement / path
	/// </summary>
	public void StopFacade(){
		_unitFacade.Stop ();
		UpdateAnimator (0f, 0f);
		IsMoving = false;
	}

	/// <summary>
	/// Check path(from a vectore to another) validity
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="unit">Unit.</param>
	//	private void QueuePathRequest (Vector3 from, Vector3 to, IUnitFacade unit)
	public void IsPathValid (Vector3 from, Vector3 to)
	{
		Action<PathResult> callback = (result) => {
			if (result.status == PathingStatus.Complete) {
				// Path Exists...
				print ("Path Validity : Complete");
			} else {
				print ("Path Validity : Something else : " + result.status.ToString ());
			}
		};

		var request = new CallbackPathRequest (callback) {
			from = from,
			to = to,
			requesterProperties = _unitFacade,
			pathFinderOptions = _unitFacade.pathFinderOptions
		};

		GameServices.pathService.QueueRequest (request);
	}

	/// <summary>
	/// Check if position is walkable(accounts for dynamic obstacles and possibly unit properties)
	/// </summary>
	/// <returns><c>true</c> if this instance is cell walkable the specified unitFacade position; otherwise, <c>false</c>.</returns>
	/// <param name="unitFacade">Unit facade.</param>
	/// <param name="position">Position.</param>
	public bool IsCellWalkable (Vector3 position)
	{
		// Other solution => accounts for dynamic obstacles and possibly unit properties
		var grid = GridManager.instance.GetGrid (position);
		if (grid == null)
			return false;
		
		var cell = grid.GetCell (position);

		/*Note only one of the following calls is needed*/
//		if (!cell.IsWalkable(unitFacade.attributes)) // Does not account for unit size
//		{
//			return false;
//		}

		if (!cell.IsWalkableWithClearance (_unitFacade)) { // Accounts for unit size provided Clearance is enabled
			return false;
		}

		return true;
	}


	/// <summary>
	/// Up to 31 enums, used for excluding dynamic obstacle of each unit
	/// </summary>
	[Flags, EntityAttributesEnum]
	public enum DynamicAINumber
	{
		ONE = 1,
		TWO = 2,
		THREE = 4,
		FOUR = 8,
		FIVE = 16,
		SIX = 32,
		SEVEN = 64,
		EIGHT = 128,
		NINE = 256,
		TEN = 512,
		ELEVEN = 1024,
		TWELVE = 2048,
		THIRTEEN = 4096,
		FOURTEEN = 8192,
		FIFTEEN = 16384,
		//add more up to 31
	}

	static List<DynamicAINumber> _dynamicAINumbers = new List<DynamicAINumber> ();

	/// <summary>
	/// Updates the dynamic AI numbers.(needs testing, note : add it after character death?, needs to be network synced?)
	/// </summary>
	public static void UpdateDynamicAINumbers ()
	{
		_dynamicAINumbers.Clear ();
		foreach (PhotonView p in AIStatusHandler.Get_ALL_PVs(true)) {
			if (p.GetComponent<UnitComponent> () == null) {
				Debug.LogWarning ("WARNING : AIStatusHandler without UnitComponent");
				continue;
			} else {
				_dynamicAINumbers.Add ((DynamicAINumber)p.GetComponent<UnitComponent> ().attributes.value);
			}
		}
	}

	void setDynamicAINumber ()
	{
		if (_dynamicAINumbers.Count < Enum.GetValues (typeof(DynamicAINumber)).Length) {
			UnitComponent uc = GetComponent<UnitComponent> ();
			DynamicAINumber availAINum = (DynamicAINumber)getAvailableDynamicAINumber ();

			//set values to ignore its own dynamic obstacle
			uc.attributes = (int)availAINum;
//			DynamicObstacle dyna = GetComponent<DynamicObstacle> ();
			_dynamicObstacle.exceptionsMask = (int)availAINum;

			_dynamicAINumbers.Add (availAINum);
		} else {
			Debug.LogWarning ("DynamicAINumber is full(more than 31 AI using pathfinding)");
//			GetComponent<DynamicObstacle> ().enabled = false;
			_dynamicObstacle.enabled = false;
		}
	}

	static DynamicAINumber? getAvailableDynamicAINumber ()
	{
		foreach (DynamicAINumber d in Enum.GetValues(typeof(DynamicAINumber))) {
			if (!_dynamicAINumbers.Contains (d))
				return d;
		}
		return null;
	}

	/// <summary>
	/// Position slot where other characters can use to pathfind to surround this character
	/// </summary>
	public class PositionSlot
	{
		/// <summary>
		/// Character in this slot
		/// </summary>
		public CharacterStatusHandler CHS;
		/// <summary>
		/// Position of this slot
		/// </summary>
		public Vector3 POS;

		/// <summary>
		/// Initializes a new instance of the <see cref="AIStatusHandler+PositionSlot"/> struct.
		/// </summary>
		/// <param name="chs">Character in this slot</param>
		/// <param name="pos">Position of this slot</param>
		public PositionSlot (Vector3 pos, CharacterStatusHandler chs = null)
		{
			CHS = chs;
			POS = pos;
		}
		public PositionSlot ()
		{
			CHS = null;
			POS = Vector3.zero;
		}
	}
}

//	/// <summary>
//	/// Check if position is walkable(Does not acocunt for dynamic obstacles)
//	/// </summary>
//	/// <returns><c>true</c> if this instance is cell walkable the specified position; otherwise, <c>false</c>.</returns>
//	/// <param name="position">Position.</param>
//	private bool IsCellWalkable (Vector3 position)
//	{
//		var grid = GridManager.instance.GetGrid (position);
//		var cell = grid.GetCell (position);
//		if (cell.isPermanentlyBlocked) {
//			// cell is not walkable
//			return false;
//		}
//
//		// cell is walkable
//		return true;
//	}
