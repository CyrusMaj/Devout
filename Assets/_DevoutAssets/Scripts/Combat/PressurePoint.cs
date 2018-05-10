using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Written by Duke Im
/// On 2016-10-04
///
/// <summary>
/// Pressure point when enabled, crush any target in between this and structures
/// </summary>
public class PressurePoint : MonoBehaviour
{
//	//dev
//	bool isHit = false;
//	RaycastHit hit;
//	float maxDistance = 1f;
//	Vector3 origin;
//	Vector3 dir;
	/// <summary>
	/// Damage of this pressurepoint
	/// </summary>
	[SerializeField] int _damage = 50;
	//	float radius = 0.4f;
	//dev
	/// <summary>
	/// List of objects this is pressing
	/// </summary>
	List<OSHTimer> _pressingObjects = new List<OSHTimer> ();
	/// <summary>
	/// how many time does it deal damage per second
	/// </summary>
	float _damagingRate = 1f;
	/// <summary>
	/// How often does this script check to deal damage(in seconds)
	/// </summary>
	float _pressureDamagingUpdateRate = 0.1f;
	/// <summary>
	/// ObjectStatusHandler of linked object
	/// </summary>
	protected CombatHandler _ch;

	void Start ()
	{
		//update pressuring
		InvokeRepeating ("updatePressureDamaging", 0f, _pressureDamagingUpdateRate);
	}

	void OnDisable(){
		//clear pressing list when disabled
		_pressingObjects.Clear ();
	}

	void OnCollisionEnter (Collision collision)
	{
		if (collision.transform.GetComponent<CombatHandler> () != null) {
			CombatHandler osh = collision.transform.GetComponent<CombatHandler> ();
			//add if not already in the list
			if (_pressingObjects.Where (x => x.Target == osh).Count () < 1) {
				OSHTimer tt = new OSHTimer ();
				tt.Target = collision.transform.GetComponent<ObjectStatusHandler> ();
				tt.Timer = Time.time;
				_pressingObjects.Add (tt);
//				print ("added");
			}
		}
	}

	void OnCollisionExit (Collision collision)
	{
		if (collision.transform.GetComponent<CombatHandler> () != null) {
			CombatHandler osh = collision.transform.GetComponent<CombatHandler> ();

			//dev
//			var comparedList = _pressingObjects.Where (x => x.Target == osh);
//			if (comparedList.Count () > 0) {
//				print ("removed");
//			}

//			print ("before : " + _pressingObjects.Count ());
			//remove if in the list
			_pressingObjects.RemoveAll (x => x.Target == osh);
//			print ("after : " + _pressingObjects.Count ());
		}
	}

	/// <summary>
	/// Updates the timers of objects being pressed and deal damage if timer runs out
	/// </summary>
	void updatePressureDamaging ()
	{
		//if target is dead, remove from the list
		_pressingObjects.RemoveAll (x => !x.Target.Alive());

		for (int i = 0; i < _pressingObjects.Count (); i++) {			
//			foreach (var po in _pressingObjects) {

			//check if target is also colliding with a wall
			if (_pressingObjects [i].Target.IsWalled) {
//				print ("Walled, timer : " + _pressingObjects [i].Timer + ", current time : " + Time.time);
				//check if timer ran out
				if (Time.time > _pressingObjects [i].Timer + _damagingRate) {
//					print ("damaging");
					dealDamange (_pressingObjects [i].Target);

					//point of collision
					Vector3? pos = null;

					if (_pressingObjects [i].Target is CharacterStatusHandler) {
						if (_pressingObjects [i].Target.GetComponent<CapsuleCollider> ())
							pos = new Vector3 (_pressingObjects [i].Target.transform.position.x, _pressingObjects [i].Target.transform.position.y + _pressingObjects [i].Target.GetComponent<CapsuleCollider> ().center.y, _pressingObjects [i].Target.transform.position.z);
						else
							Debug.LogWarning ("WARNING : No capsule collider in CharacterStatusHandelr");
					} else
						pos = _pressingObjects [i].Target.transform.position;

					if(pos != null)
						ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, (Vector3)pos);
					
					_pressingObjects [i].Timer = Time.time;

					//play sound
					SoundManager.SINGLETON.PlayHitSound(Weapon.TYPE.MACE, SoundManager.HitResult.Hit, (Vector3)pos);
				}
			}
//			}
		}
	}

	//dev
	//collider attached to the rig seems to be spinning like hell
	//Thus force-prevent spinning
//	void LateUpdate ()
//	{
//		transform.localPosition = Vector3.zero;
//		transform.localRotation = Quaternion.identity;
//		//		else
//		//			print ("osh : " + _osh.name);
//	}

	//	void OnCollisionStay (Collision collision)
	//	{
	////		print ("Collided with " + collision.transform.name);
	//		//damagable things discovered
	////		if (collision.transform.GetComponent<ObjectStatusHandler> () != null) {
	////			ObjectStatusHandler osh = collision.transform.GetComponent<ObjectStatusHandler> ();
	////			if (osh.IsWalled) {
	////				foreach (var po in _pressingObjects) {
	////					if (po.Timer + _damagingRate < Time.time) {
	////						dealDamange (osh);
	////					}
	////				}
	////			}
	//////			print (osh.name);
	////			origin = osh.transform.position;
	////			origin = new Vector3 (origin.x, transform.position.y, origin.z);
	////			dir = osh.transform.position - collision.contacts [0].point;
	////			dir = new Vector3 (dir.x, 0f, dir.z);
	////			dir.Normalize ();
	////
	////			// Physics.SphereCast (레이저를 발사할 위치, 구의 반경, 발사 방향, 충돌 결과, 최대 거리)
	////			LayerMask lm = (1 << LayerHelper.DEFAULT);
	////			isHit = false;
	//////			isHit = Physics.SphereCast (origin, radius, dir, out hit, maxDistance, lm);
	////			isHit = Physics.Raycast (origin, dir, out hit, maxDistance, lm);
	////			if (isHit) {
	////				print ("Hit : " + hit.collider.name);
	////				dealDamange (osh);
	//////				Debug.Break ();
	////				ParticleController.PC.InstantiateParticle(collision.contacts [0].point);
	////			}
	////		}
	//	}

	//	void OnDrawGizmos ()
	//	{
	//		if (isHit) {
	//			Gizmos.color = Color.red;
	//			Gizmos.DrawRay (origin, dir * hit.distance);
	////			Gizmos.DrawWireSphere (origin + dir * hit.distance, radius); 
	//		} else {
	//			Gizmos.DrawRay (origin, dir * maxDistance);
	//		}
	//	}
	/// <summary>
	/// Handles dealing damage to the contact object(enemy)
	/// Also handles pushback and staggering the target.
	/// dev
	/// </summary>
	/// <param name="osh">Target's ObjectStatusHandler</param>
	protected void dealDamange (ObjectStatusHandler osh)
	{
		//inflict damage
		osh.SubtractHealth (_damage);

		//disable hit target's damaging points
		if (true) {
			if (osh.GetComponent<CombatHandler> ()) {
				//				disableDamagingPoints (osh.GetComponent<CombatHandler> ());
				osh.GetComponent<CombatHandler> ().DisableDamagingPoints ();
			}
		}

		//add to ultimate guage
		_ch.AddUltimatePoint (_damage / 25);
		//add more if it's a killing blow
		if (osh.GetHealth () - _damage <= 0) {
			_ch.AddUltimatePoint (5);
		}

		//if character, stagger it
		//Note : Stagger animation will transit from AnyState for minions and certain character via animator
		//		Player characters at the moment does not have AnyState linked to Stagger, so their action will not get interrupted by getting attack
		CharacterStatusHandler csh = osh.GetComponent<CharacterStatusHandler> ();
		if (csh != null)
			csh.Stagger (_damagingRate / 5f);
		//		if (csh != null)
		//			((CharacterStatusHandler)osh).Stagger (_pushDuration);
	}

	/// <summary>
	/// Set ObjectStatusHandler
	/// </summary>
	/// <param name="osh">Osh.</param>
	public void SetCombatHandler(CombatHandler ch){
		_ch = ch;
	}
}

