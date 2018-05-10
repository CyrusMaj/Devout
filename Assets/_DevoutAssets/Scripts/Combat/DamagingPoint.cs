using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Damaging point.
/// Parented to weapon and holds collider
/// That deals damage on collision with other team's hitbox collider
/// </summary>
[RequireComponent (typeof(Collider))]
public class DamagingPoint : MonoBehaviour
{
	//If this is true, this damaging point deals damage once per object
	//If this is false, this damaing point deals damage to every(enemy) collider and everytime this enters.
	[SerializeField] protected bool _isOneTimeDamage = true;
	[SerializeField] TrailRenderer _trailRenderer;

	//this list keeps track of what OSH this damaging point has collided(and dealt damage), for oneTimeDamage.
	//register who already got damaged by this
	protected List<ObjectStatusHandler> _OSHList = new List<ObjectStatusHandler> ();
	/// <summary>
	/// The weapon this damagingpoint is attached to
	/// </summary>
	protected Weapon _weapon;
	/// <summary>
	/// The collider of this damaging point
	/// </summary>
	protected Collider _collider;
	/// <summary>
	/// Is this active(dealing damage?)
	/// </summary>
	protected bool _isActive = false;
	//active = will do damage
	/// <summary>
	/// List of damagable layers determined by ownwer's team
	/// </summary>
	protected List<int> _damagableLayers;

	//how long does this push(pushback) target on hit
	float _pushDuration = 0.5f;
	[SerializeField] float _staggerDuration = 0.5f;

	//Coroutine for slowing down animation on hit
	IEnumerator _IEnumAnimSpeed;

	protected virtual void Start ()
	{
		_collider = GetComponent<Collider> ();
		SetIsActive (false);
	}

	/// <summary>
	/// Sets the damagable layers.
	/// </summary>
	public void SetDamagableLayers ()
	{
		_damagableLayers = LayerHelper.GetDamagableLayers (gameObject.layer);
	}

	/// <summary>
	/// Clears the list of object status handlers this damaging point has collided(and dealt damage).
	/// </summary>
	public void ClearOSHList ()
	{
		_OSHList.Clear ();
	}

	/// <summary>
	/// Sets the weapon.
	/// Damaging point needs to have weapon to work properly
	/// </summary>
	/// <param name="weapon">Weapon.</param>
	public void SetWeapon (Weapon weapon)
	{
		_weapon = weapon;
	}

	/// <summary>
	/// Activate / Disable damaging point
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	public void SetIsActive (bool isActive)
	{
		_isActive = isActive;
		_collider.enabled = isActive;
		if (_trailRenderer != null) {
			_trailRenderer.enabled = isActive;
		}
	}

	/// <summary>
	/// Raises the collision enter event.
	/// </summary>
	/// <param name="collision">Collision.</param>
	protected virtual void OnCollisionEnter (Collision collision)
	{
//		print ("hit : " + collision.collider.name);	
		if (_isActive) {
			//When hitting hitbox
			if (collision.collider.GetComponent<Hitbox> () != null) {
				Hitbox hb = collision.collider.GetComponent<Hitbox> ();
				foreach (var l in _damagableLayers) {
//					print ("hit : " + collision.collider.name);	
					if (collision.gameObject.layer == l) {
						print (l.ToString () + ", " + LayerHelper.HITBOX_TEAM_5.ToString ());
						//When hitting blocking point
						if (hb is BlockingPoint) {
//
//							//disable this character's damaging points
//							_weapon.GetOwner ().DisableDamagingPoints ();
////							_collider.enabled = false;
////							Debug.Break ();
//							//stagger this character
//							//Note : need a way to stagger player characters as it's controlled by minion's animator for now
//							CharacterStatusHandler csh = _weapon.GetOwner ().GetComponent<CharacterStatusHandler> ();
//
//							//delay blocked staggering for fine-tuning
//							float staggerDelay = 0.1f;
//							if (csh != null)
//								csh.Stagger (((BlockingPoint)hb).GetStaggerDuration (), staggerDelay);
//							//instantiate particles
//							//need to change to different particle to differentiate blocking vs damaging
//							ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point, staggerDelay);
//							//play sound
//							SoundManager.SINGLETON.PlayHitSound (_weapon.Type, SoundManager.HitResult.Blocked, collision.contacts [0].point);
////							Debug.Break ();
						}//if not a blocking point, then it means a hitpoint, deal damage 
						else {
							ObjectStatusHandler osh = collision.collider.GetComponent<Hitbox> ().GetOSH ();
							if (!_OSHList.Contains (osh)) {
								dealDamange (osh);
								hitEffect (collision.contacts [0].point);
								ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point);
								_OSHList.Add (osh);
							} else if (!_isOneTimeDamage) {
								dealDamange (osh);
								hitEffect (collision.contacts [0].point);
								ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point);
							}
							//play sound
							SoundManager.SINGLETON.PlayHitSound (_weapon.Type, SoundManager.HitResult.Hit, collision.contacts [0].point);
						}
					}
				}
			}
		}
	}

	protected virtual void OnTriggerEnter (Collider collider)
	{
		if (_isActive) {
//			print (collider.name);
			//When hitting hitbox
			if (collider.GetComponent<Hitbox> () != null) {
				Hitbox hb = collider.GetComponent<Hitbox> ();
				foreach (var l in _damagableLayers) {
//					print (l.ToString () + ", " + LayerHelper.HITBOX_TEAM_5.ToString () + ", " + collider.name);
					if (collider.gameObject.layer == l) {

						ObjectStatusHandler osh = collider.GetComponent<Hitbox> ().GetOSH ();
						//When hitting blocking point
						if (hb is BlockingPoint) {
//							print ("BLOCK");
							//disable this character's damaging points
							_weapon.GetOwner ().DisableDamagingPoints ();
							//							_collider.enabled = false;
							//							Debug.Break ();
							//stagger this character
							//Note : need a way to stagger player characters as it's controlled by minion's animator for now
							CharacterStatusHandler csh = _weapon.GetOwner ().GetComponent<CharacterStatusHandler> ();

							//delay blocked staggering for fine-tuning
							float staggerDelay = 0.1f;
							if (csh != null)
								csh.Stagger (((BlockingPoint)hb).GetStaggerDuration (), staggerDelay);
							//instantiate particles
							//need to change to different particle to differentiate blocking vs damaging
							ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collider.transform.position, staggerDelay);
							//play sound
							SoundManager.SINGLETON.PlayHitSound (_weapon.Type, SoundManager.HitResult.Blocked, collider.transform.position);
							//							Debug.Break ();
						}
						else if (osh) {
							if (osh is BridgeStatusHandler && RoomLevelHelper.PLAYER_CLASS != CHARACTER_CLASS.TANK)
								return;
//							print ("hit : " + osh.name);
							if (!_OSHList.Contains (osh)) {
								dealDamange (osh);
								hitEffect (collider.transform.position);
								ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collider.transform.position);
								_OSHList.Add (osh);
							} else if (!_isOneTimeDamage) {
								dealDamange (osh);
								hitEffect (collider.transform.position);
								ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collider.transform.position);
							}
						} else
							print (collider.name);
						//play sound
						SoundManager.SINGLETON.PlayHitSound (_weapon.Type, SoundManager.HitResult.Hit, collider.transform.position);						
					}
				}
			}
		}
	}

	/// <summary>
	/// Handles dealing damage to the contact object(enemy)
	/// Also handles pushback and staggering the target.
	/// </summary>
	/// <param name="osh">Target's ObjectStatusHandler</param>
	protected void dealDamange (ObjectStatusHandler osh)
	{
		//inflict damage
		osh.SubtractHealth (_weapon.GetDamage ());

		//disable hit target's damaging points
		if (_weapon.GetDisablesTargetDamagingPointOnhit ()) {
			if (osh.GetComponent<CombatHandler> ()) {
//				disableDamagingPoints (osh.GetComponent<CombatHandler> ());
				osh.GetComponent<CombatHandler> ().DisableDamagingPoints ();
			}
		}

		//add to ultimate guage
		_weapon.GetOwner ().AddUltimatePoint (_weapon.GetDamage () / 25);
		//add more if it's a killing blow
		if (osh.GetHealth () - _weapon.GetDamage () <= 0) {
			_weapon.GetOwner ().AddUltimatePoint (5);
		}



		//drop if hanging
		CharacterMovementHandler cmh = osh.GetComponent<CharacterMovementHandler> ();
//		HangHandler hh = cmh.HangHandler;
		if (cmh && cmh.Hanging) {			
//			hh.HangToDrop ();
			cmh.HangToDrop ();
		} else {
			//push object back
			//Duke (8/6/2016)
			//BUG : disabled because push direction changes as target rotates. make it absolute direction
			//Log :
			//Duke(8/11/2016) : Code is correct, suspect problem with pre-existing photon view enemies. Instantiating all characters may solve it. Enabled pushback to test it.
			//Duke(8/27/2016) : Problem occured again, there must be something wrong
			//					Seems to be fixed with setting position i nstead of translate
			pushBack (osh, _pushDuration);
			//if character, stagger it
			//Note : Stagger animation will transit from AnyState for minions and certain character via animator
			//		Player characters at the moment does not have AnyState linked to Stagger, so their action will not get interrupted by getting attack
			CharacterStatusHandler csh = osh.GetComponent<CharacterStatusHandler> ();

			if (csh != null) {
//				csh.Stagger (_pushDuration);
				csh.Stagger (_staggerDuration);
			}
		}
	}


	//replacing

	/// <summary>
	/// Disables the target's damaging points
	/// Used when target is hit, so that hit target doesn't hurt attackers accidentally
	/// </summary>
	/// <param name="ch">CombatHandler of the target</param>
	//	protected void disableDamagingPoints (CombatHandler targetCH)
	//	{
	//		foreach (var abil in targetCH.GetAbilities()) {
	//			abil.GetWeapon ().DisableDamagingPoints ();
	//			foreach (var dp in abil.GetWeapon().GetDamagingPoints()) {
	//				dp.SetIsActive (false);
	//			}
	//		}
	//	}

	/// <summary>
	/// Pushes the target backwards
	/// </summary>
	/// <param name="osh">Target's ObjectStatusHandler</param>
	/// <param name="duration">Duration of push</param>
	void pushBack (ObjectStatusHandler osh, float duration)
	{
		Vector3 relativeDirection = CombatHelper.getRelativeDirection (_weapon.GetOwner ().transform.position, osh.transform.position, PUSHBACK_DIRECTION.OUTWARD);
//		print (relativeDirection.ToString ());
		//
		Debug.DrawLine (_weapon.GetOwner ().transform.position, _weapon.GetOwner ().transform.position + relativeDirection, Color.yellow);
		osh.PushBurst (relativeDirection * _weapon.GetPushbackForce (), duration);
	}

	/// <summary>
	/// Effects when hit(collided)
	/// </summary>
	protected void hitEffect (Vector3 collisionPoint)
	{
		//slow animation to simulate weapons going through enemey
		Animator ownerAnim = _weapon.GetOwner ().GetComponent<Animator> ();
		float duration = 0.2f;
		if (_IEnumAnimSpeed == null) {
			_IEnumAnimSpeed = slowAnim (ownerAnim, duration);
			StartCoroutine (_IEnumAnimSpeed);
		} else {
			StopCoroutine (_IEnumAnimSpeed);
			_IEnumAnimSpeed = slowAnim (ownerAnim, duration);
			StartCoroutine (_IEnumAnimSpeed);
		}
	}

	/// <summary>
	/// Slows the animation.
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="animator">Animator.</param>
	/// <param name="duration">Duration.</param>
	IEnumerator slowAnim (Animator animator, float duration)
	{
		//todo : make speed vary by damage dealt
//		float speed = Mathf.Clamp (0f, 0.5f, 1f);
		float speed = 0.1f;	
		animator.SetFloat (AnimationHashHelper.PARAM_ANIM_SPEED, speed);
		yield return new WaitForSeconds (duration);
		animator.SetFloat (AnimationHashHelper.PARAM_ANIM_SPEED, 1f);
	}

	/// <summary>
	/// Gets the is one time damage.
	/// </summary>
	/// <returns><c>true</c>, if is one time damage was gotten, <c>false</c> otherwise.</returns>
	public bool GetIsOneTimeDamage ()
	{
		return _isOneTimeDamage;
	}

	/// <summary>
	/// Sets the is one time damage.
	/// </summary>
	/// <param name="isOneTimeDamage">If set to <c>true</c> is one time damage.</param>
	public void SetIsOneTimeDamage (bool isOneTimeDamage)
	{
		_isOneTimeDamage = isOneTimeDamage;
	}

	/// <summary>
	/// Set oneTimeDamage over duration
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="dp">DamagingPoint</param>
	/// <param name="seconds">Duration in seconds</param>
	/// <param name="isOneTimeDamage">If set to <c>true</c> is one time damage.</param>
	public static IEnumerator IESetOneTimeDamage (DamagingPoint dp, float seconds, bool isOneTimeDamage)
	{
		bool originalState = dp.GetIsOneTimeDamage ();
		dp.SetIsOneTimeDamage (isOneTimeDamage);
//		print ("Setting");
//		Debug.Break ();
		yield return new WaitForSeconds (seconds);
		dp.SetIsOneTimeDamage (originalState);
	}
}
