using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Written by Duke Im
/// On 2016-11-28
/// 
/// <summary>
/// Base class for AI scanning for possible target
/// Ex. Search for closest (visible) enemy and check if there's room for melee attack position to attack that target, similar goes with ranges and roaming around the target
/// </summary>
public class AIScanner : MonoBehaviour
{
	/// <summary>
	/// Eye position of this unit
	/// Ex. head skeleton / joint
	/// </summary>
	[SerializeField] Transform _eyePosition;
	/// <summary>
	/// The angle of vision in Euler angles from left to right centered in forward vector. 
	/// Min = 1, Max = 180
	/// </summary>
	[SerializeField] float _angleOfVision = 180f;
	/// <summary>
	/// The range of vision in radius and in meters
	/// </summary>
	[SerializeField] float _rangeOfVision = 10f;
	/// <summary>
	/// The range of vision in radius that doesn't get effected by angle of vision(field of vision)
	/// </summary>
	[SerializeField] float _nearRoV = 2f;
	//	/// <summary>
	//	/// How often does this unit scan for target?(per/second)
	//	/// </summary>
	//	[SerializeField] float _scanInterval = 1f;
	/// <summary>
	/// If set to <c>true</c> get closest enemy, if set to <c>false</c> randomly select when scanning
	/// </summary>
	[SerializeField] bool _checkClosest = false;

	/// <summary>
	/// The current target, null if none
	/// </summary>
	public CharacterStatusHandler Target{ get; private set; }

	//Cache
	CombatHandler _ch;
	AIMovementHandler _aimh;
	CharacterStatusHandler _csh;

	void Start ()
	{
		_ch = GetComponent<CombatHandler> ();
		_aimh = GetComponent<AIMovementHandler> ();
		_csh = GetComponent<CharacterStatusHandler> ();

		//dev
//		InvokeRepeating ("Scan", 1f, _scanInterval);
	}

	/// <summary>
	/// Scan for visible enemy
	/// </summary>
	public CharacterStatusHandler Scan ()
	{
		CharacterStatusHandler foundTarget = null;
		CharacterStatusHandler tempTarget = null;

		//direction from eyeposition to target
		Vector3 dir;

//		//dev 
//		//performance test between loop order and raycast
//		float startTime = Time.time;

		//cache
		List<CombatHandler> enemies = CombatHandler.GET_ENEMIES (_ch.GetTeam (), true);

		if (!_checkClosest)
			enemies.Shuffle ();

		//get all enemies in range
		foreach (var ch in enemies.Where (x => 
			Vector3.Distance (x.transform.position, this.transform.position) < _rangeOfVision		//Check if target are in range of vision
			&& Vector3.Angle (transform.forward, x.transform.position - this.transform.position) < _angleOfVision / 2f	//Check if target are in angle of vision(Field of vision) 
		    )) {
			//check visibility with raycast
			LayerMask layerMask = 1 << LayerHelper.COMMON_BODY;
			RaycastHit hit;
			if (Physics.Raycast (_eyePosition.position
				, ch.transform.position - transform.position	//Direction from eye to target
				, out hit, _rangeOfVision + 1f
				, layerMask
			    )) {
				tempTarget = ch.GetComponent<CharacterStatusHandler> ();
				//get closest
				if (_checkClosest &&
				    (foundTarget == null || Vector3.Distance (foundTarget.transform.position, this.transform.position) > Vector3.Distance (tempTarget.transform.position, this.transform.position))) {
					foundTarget = tempTarget;
				} else {
					if (tempTarget != null) {
						print ("scanned : " + tempTarget.name);
					}
					//otherwise, get random
					Target = tempTarget;
					return tempTarget;
				}
			}
		}

		//dev
//		if (foundTarget != null) {
//			print ("scanned : " + foundTarget.name);
//		}

		//if nothing found, do nearcheck
		if (foundTarget == null) {
			foreach (var ch in enemies.Where (x => 
				Vector3.Distance (x.transform.position, this.transform.position) < _nearRoV		//Check if target are in range of vision
			)) {
				tempTarget = ch.GetComponent<CharacterStatusHandler> ();
				//get closest
				if (_checkClosest &&
				    (foundTarget == null || Vector3.Distance (foundTarget.transform.position, this.transform.position) > Vector3.Distance (tempTarget.transform.position, this.transform.position))) {
					foundTarget = tempTarget;
				} else {
					if (tempTarget != null) {
						print ("scanned : " + tempTarget.name);
					}
					//otherwise, get random
					Target = tempTarget;
					return tempTarget;
				}
			}
		}

		Target = tempTarget;
		return foundTarget;
	}

	/// <summary>
	/// Gets the attack position of a target
	/// Note : Only supports melee attack position for now
	/// </summary>
	/// <returns>The attack position.</returns>
	/// <param name="target">Target.</param>
	/// <param name="distance">Distance.</param>
	public Vector3? GetAttackPosition (CharacterStatusHandler target = null)//, float distance = 2f)
	{
//		Vector3? attackPosition = null;
		AIMovementHandler.PositionSlot ps = null;

		//if target not specified, use current target
		if (target == null) {
			if (this.Target != null) {
				target = this.Target;
			} else {
				//if current target doesn't exist, scan
				if (Scan () != null) {
					target = this.Target;
				} else {
					//no target available
					return null;
				}
			}
		}
		//look for position
//		foreach (var pos in GetPositionSlots(distance, target)) {
		foreach (var slot in target.GetComponent<AIMovementHandler>().MeleeSlots) {
			//check if position is movable
			if (!_aimh.IsCellWalkable (slot.POS))
				continue;
			else { 
				if (slot.CHS != null) {
					//if slot has been mine, clear it
					if (slot.CHS == _csh) {
						slot.CHS = null;
					} else {
						//slot is taken, possibly take it away if this is closer
						if ((Vector3.Distance (slot.POS, slot.CHS.transform.position) <= Vector3.Distance (slot.POS, transform.position))) {
							continue;
						} else if (Vector3.Distance (slot.CHS.transform.position, target.transform.position) <= Vector3.Distance (this.transform.position, target.transform.position)) {
							//if slot owner is further away from the point, but closer to the target, don't take it
							continue;
						}
					}
				}

				if (ps == null) {
					ps = slot;
					ps.CHS = _csh;
				} else if (Vector3.Distance (this.transform.position, ps.POS) > Vector3.Distance (this.transform.position, slot.POS)) {
					//get new attack position if it's available and closer
					ps.CHS = null;
					ps = slot;
					ps.CHS = _csh;
				}	
			}
		}
		if (ps == null)
			return null;
		else
			return ps.POS;
	}

	//	/// <summary>
	//	/// Gets the roaming position.
	//	/// Ex. If there is no valid attack position available, roam in roaming position
	//	/// </summary>
	//	/// <returns>The roaming position.</returns>
	//	/// <param name="target">Target.</param>
	//	public Vector3? GetRoamingPosition (CharacterStatusHandler target = null)
	//	{
	//		Vector3? roamingPosition = null;
	//
	//		//if target not specified, use current target
	//		if (target == null) {
	//			if (this.Target != null) {
	//				target = this.Target;
	//			} else {
	//				//if current target doesn't exist, scan
	//				if (Scan () != null) {
	//					target = this.Target;
	//				} else {
	//					//no target available
	//					return null;
	//				}
	//			}
	//		}
	//
	//		//look for position
	//
	//		return roamingPosition;
	//	}

	/// <summary>
	/// Gets 6 evenly spaced position slots around the target
	/// </summary>
	/// <returns>The position slots around target</returns>
	/// <param name="distance">Distance to the slots, from target</param>
	/// <param name="target">Target.</param>
	/// <param name="getOnlyMovableSlots">Whether to check and get movable slots(excluding occupied or inaccecible positions)</param>
	public List<Vector3> GetPositionSlots (float distance, CharacterStatusHandler target = null, bool getOnlyMovableSlots = true)
	{
		List<Vector3> slots = new List<Vector3> ();

		//if target not specified, use current target
		if (target == null) {
			if (this.Target != null) {
				target = this.Target;
			} else {
				//if current target doesn't exist, scan
				if (Scan () != null) {
					target = this.Target;
				} else {
					//no target available
					return slots;
				}
			}
		}

		//parametric equation for a circle(center (cx,cz))
		//x = cx + r * cos(a)
		//z = cz + r * sin(a)
		//r = distance
		//get 6 slots(positions) around the target
		float cx = target.transform.position.x;
		float cy = target.transform.position.y;//Note : How will this affect in slope?s
		float cz = target.transform.position.z;
		float a = 60f;
		int count = 360 / (int)a;
		for (int i = 0; i < count; i++) {
			slots.Add (new Vector3 (cx + distance * Mathf.Cos (i * a * Mathf.Deg2Rad), cy, cz + distance * Mathf.Sin (i * a * Mathf.Deg2Rad)));
		}

		return slots;
	}
}

//		//look for position
//		foreach (var pos in GetPositionSlots(distance, target)) {
//			//check if position is movable
//			if (!_aimh.IsCellWalkable (pos))
//				continue;
//			else if (attackPosition == null) {
//				attackPosition = pos;
//			} else if (Vector3.Distance (this.transform.position, (Vector3)attackPosition) > Vector3.Distance (this.transform.position, pos)) {
//				//get new attack position if it's available and closer
//				attackPosition = pos;
//			}
//
////			//then check if path to the position is valid
////			//Note : Path validity check doesn't return result imediately(separate thread), so this will not work
////			if (_aimh.IsPathValid (this.transform.position, pos)) {
////				if (attackPosition == null) {
////					attackPosition = pos;
////				}
////				else if(Vector3.Distance(this.transform.position, (Vector3)attackPosition) > Vector3.Distance(this.transform.position, pos)) {
////					//get new attack position if it's available and closer
////					attackPosition = pos;
////				}
////			}
//		}
