using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-06
/// 
/// <summary>
/// Instantiated Arrow
/// </summary>
public class Arrow : DamagingPoint
{
	[SerializeField]float _lifeTime = 15f;
	[SerializeField]public float _speed = 0.5f;

	/// <summary>
	/// If this is true, it means that this instance is in the same client as the owner player
	/// If this is true, this arrow deals damage, instantiate particle, pushes back enemy, etc that needs to be done only once
	/// </summary>
	/// <value><c>true</c> if this instance is mine; otherwise, <c>false</c>.</value>
	public bool IsMine{ get; private set; }
	//	Collider _collider;

	protected override void Start ()
	{
		//		base.Start ();
		_collider = GetComponent<Collider> ();
		Destroy (gameObject, _lifeTime);
	}

	protected virtual void FixedUpdate ()
	{
//		transform.Translate (transform.forward * Speed * Time.fixedDeltaTime);
		transform.position += transform.forward * Time.fixedDeltaTime * _speed;
		_isActive = true;
	}

	protected override void OnCollisionEnter (Collision collision)
	{
		if (_isActive) {
			//When hitting hitbox
			if (collision.collider.GetComponent<Hitbox> () != null) {
				Hitbox hb = collision.collider.GetComponent<Hitbox> ();
				foreach (var l in _damagableLayers) {
					if (collision.gameObject.layer == l) {
						//When hitting blocking point
						if (hb is BlockingPoint) {
//							//disable this character's damaging points
////							disableDamagingPoints(_weapon.GetOwner());
//							_weapon.GetOwner().DisableDamagingPoints();
//
//							//stagger this character
//							//Note : need a way to stagger player characters as it's controlled by minion's animator for now
//							CharacterStatusHandler csh = _weapon.GetOwner().GetComponent<CharacterStatusHandler>();
//
//							//delay blocked staggering for fine-tuning
							float staggerDelay = 0.2f;
//							if (csh != null)
//								csh.Stagger (((BlockingPoint)hb).GetStaggerDuration (), staggerDelay);

							if (IsMine) {
								//instantiate particles
								//need to change to different particle to differentiate blocking vs damaging
								ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point, staggerDelay);

								//play sound
								SoundManager.SINGLETON.PlayHitSound (Weapon.TYPE.SWORD, SoundManager.HitResult.Blocked, collision.contacts [0].point);
							}
						}//if not a blocking point, then it means a hitpoint, deal damage 
						else {
							ObjectStatusHandler osh = collision.collider.GetComponent<Hitbox> ().GetOSH ();
							if (!_OSHList.Contains (osh)) {
								if (IsMine) {
									dealDamange (osh);
									hitEffect (collision.contacts [0].point);
									_OSHList.Add (osh);
								}
							} else if (!_isOneTimeDamage) {
								if (IsMine) {
									dealDamange (osh);
									hitEffect (collision.contacts [0].point);
								}
							}

							//play sound
							SoundManager.SINGLETON.PlayHitSound (Weapon.TYPE.SWORD, SoundManager.HitResult.Hit, collision.contacts [0].point);
						}
						stick (collision);
						return;
					}
				}
			}

			//when hit default layer, this may need to change when structures and stuff are in different layer
//			if (collision.gameObject.layer == LayerHelper.DEFAULT && !collision.gameObject.CompareTag (TagHelper.RAGDOLL)) {
			if (collision.gameObject.layer == LayerHelper.WALL || collision.gameObject.layer == LayerHelper.FLOOR || collision.gameObject.layer == LayerHelper.NEAUTRAL) {
//				print ("hit " + collision.collider.name);
				stick (collision);
				ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point);
			}
//			else
//				print (collision.gameObject.ToString ());
		}
	}

	protected override void OnTriggerEnter (Collider collider)
	{
		if (_isActive) {
			if (collider.GetComponent<Hitbox> () != null) {
				Hitbox hb = collider.GetComponent<Hitbox> ();
				foreach (var l in _damagableLayers) {
					if (collider.gameObject.layer == l) {
						ObjectStatusHandler osh = collider.GetComponent<Hitbox> ().GetOSH ();
						//When hitting blocking point
						if (hb is BlockingPoint) {
//							print ("BLOCK");
							//disable this char	acter's damaging points
							//							_weapon.GetOwner ().DisableDamagingPoints ();
							stick (collider.transform);
							//stagger this character
							//Note : need a way to stagger player characters as it's controlled by minion's animator for now
//							CharacterStatusHandler csh = _weapon.GetOwner ().GetComponent<CharacterStatusHandler> ();
							//instantiate particles
							//need to change to different particle to differentiate blocking vs damaging
							ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collider.transform.position);
							//play sound
							SoundManager.SINGLETON.PlayHitSound (Weapon.TYPE.SWORD, SoundManager.HitResult.Blocked, collider.transform.position);
						}					
					}
				}
			}
		}
	} 

	protected virtual void stick (Collision collision)
	{
//		print ("STICK");

		_collider.enabled = false;

		//dev
		if (collision.transform.parent != null && collision.transform.parent.GetComponent<CharacterStatusHandler> () != null) {
			transform.SetParent (collision.transform.parent.GetComponentInChildren<RagdollHelper> ().transform);			
		} else
			transform.SetParent (collision.transform);

		if (IsMine) {
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, collision.contacts [0].point);

			//play sound
			//SoundManager.SINGLETON.PlayHitSound(Weapon.TYPE.ARROW, SoundManager.HitResult.Hit, collision.contacts [0].point);
		}
		
		_speed = 0f;
//		_collider.enabled = false;
		SetIsActive (false);
	}

	protected virtual void stick (Transform trans)
	{
		_collider.enabled = false;

		//dev
		if (trans.parent != null && trans.parent.GetComponent<CharacterStatusHandler> () != null) {
			transform.SetParent (trans.parent.GetComponentInChildren<RagdollHelper> ().transform);			
		} else
			transform.SetParent (trans);

		if (IsMine) {
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.HIT, trans.position);

			//play sound
			//SoundManager.SINGLETON.PlayHitSound(Weapon.TYPE.ARROW, SoundManager.HitResult.Hit, collision.contacts [0].point);
		}

		_speed = 0f;
		//		_collider.enabled = false;
		SetIsActive (false);
	}

	public void SetIsMine (bool newIsMine)
	{
		IsMine = newIsMine;
//		print ("called");
	}
}
