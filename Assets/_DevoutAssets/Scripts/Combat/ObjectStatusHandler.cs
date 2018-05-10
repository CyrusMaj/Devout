using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Base status handler
/// Status handler holds status information about an object, for example, health and hitboxes.
/// </summary>
[RequireComponent (typeof(PhotonView))]
[RequireComponent (typeof(Rigidbody))]
public class ObjectStatusHandler: Photon.PunBehaviour
{
	/// <summary>
	/// List of hitboxes that will affect health of this object
	/// </summary>
	[SerializeField]protected List<Hitbox> _hitboxes = new List<Hitbox> ();
	/// <summary>
	/// Current health
	/// </summary>
	[SerializeField]protected int _health = 100;
	/// <summary>
	/// Maximum health
	/// </summary>
	[SerializeField]protected int _maxHealth = 100;
	protected PhotonView _photonView;
	protected Rigidbody _rigidbody;
	IEnumerator _IEPush;

	//Duke
	//Ask nico about reason behind adding this
	//	public AIContext context { get; private set; }
	//	public ObjectStatusHandler[] _attackPositions;
	//	public int _type = 0;

	//DEV
	//Destroy object if this is true
	[SerializeField] bool _destroyOnDeath;

	//dev
	/// <summary>
	/// Gets a value indicating whether this instance is walled(similar to grounded)
	/// </summary>
	/// <value><c>true</c> if this instance is walled; otherwise, <c>false</c>.</value>
	public bool IsWalled{ get; private set; }
	//	public void SetIsWalled(bool newIsWalled){
	//		IsWalled = newIsWalled;
	//	}
	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerHelper.WALL) {
			IsWalled = true;
//			print ("True");
		}
	}

	void OnCollisionExit (Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerHelper.WALL) {
			IsWalled = false;
//			print ("False");
		}
	}

	protected virtual void Start ()
	{
//		context = new AIContext (this);
//		_attackPositions = new ObjectStatusHandler[4];
//		for (int i = 0; i < _attackPositions.Length; i++) {
//			_attackPositions [i] = null;
//		}
		foreach (var hb in _hitboxes) {
			hb.SetOSH (this);
		}
		_photonView = PhotonView.Get (this);

		_rigidbody = GetComponent<Rigidbody> ();
	}

	/// <summary>
	/// Check if this object is alive
	/// </summary>
	/// <returns>True if alive, false otherwise</returns>
	public bool Alive ()
	{
		if (_health > 0)
			return true;
		else
			return false;
	}

	/// <summary>
	/// Gets the health.
	/// </summary>
	/// <returns>The health.</returns>
	public int GetHealth ()
	{
		return _health;
	}

	/// <summary>
	/// Gets the max health.
	/// </summary>
	/// <returns>The max health.</returns>
	public int GetMaxHealth ()
	{
		return _maxHealth;
	}

	/// <summary>
	/// Subtracts the health.
	/// Network unless offlinemode
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void SubtractHealth (int amount)
	{
		int newHealth = _health - amount;
		if (PhotonNetwork.offlineMode)
			setHealth (newHealth);
		else {
//			_photonView.RPC ("RPCSetHealth", PhotonTargets.All, _health - amount);
			NetworkManager.SINGLETON.Targeted_RPC (_photonView, "RPCSetHealth", _health - amount);
		}
	}

	/// <summary>
	/// Sets the health.
	/// Network unless offlinemode
	/// </summary>
	/// <param name="newHealth">New health.</param>
	public void SetHealth (int newHealth)
	{
		if (PhotonNetwork.offlineMode)
			setHealth (newHealth);
		else {
//			_photonView.RPC ("RPCSetHealth", PhotonTargets.All, newHealth);
			NetworkManager.SINGLETON.Targeted_RPC (_photonView, "RPCSetHealth", newHealth);
		}
	}

	[PunRPC]
	protected void RPCSetHealth (int newHealth)
	{
		setHealth (newHealth);
	}

	protected virtual void setHealth (int newHealth)
	{
		_health = newHealth;
		checkAlive ();
		if (GameController.GC.CurrentPlayerCharacter != null && GameController.GC.CurrentPlayerCharacter == transform) {
			HUDManager.HUD_M.HPUI.UpdateHPUI ();
		}
	}

	/// <summary>
	/// Gets the photon view.
	/// </summary>
	/// <returns>The photon view.</returns>
	public PhotonView GetPhotonView ()
	{
		return _photonView;
	}

	/// <summary>
	/// Check if object is alive,
	/// If not and conditions are met, destroy this object
	/// </summary>
	protected virtual void checkAlive ()
	{
		if (_destroyOnDeath && _health < 1) {
			//Destructable object breaks(shatters) if this is true
//			if (GetComponent<Exploder.ExploderObject> () != null) {
////				print ("EXPLODE");
//				GetComponent<Exploder.ExploderObject> ().Explode ();
//			} else
			Destroy (gameObject);
		}		
	}

	//	public void AddHealth (int amount)
	//	{
	//		_health += amount;
	//	}

	/// <summary>
	/// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
	/// </summary>
	/// <remarks>If your game starts with a certain number of players, this callback can be useful to check the
	/// Room.playerCount and find out if you can start.</remarks>
	/// <param name="newPlayer">New player.</param>
	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		//
//		if (_photonView.isMine)
//			SetHealth (_health);
	}

	/// <summary>
	/// Push this object towards force over duration
	/// </summary>
	/// <param name="force">Force.</param>
	/// <param name="duration">Duration.</param>
	public void PushBurst (Vector3 force, float duration)
	{
		if (PhotonNetwork.offlineMode)
			pushBurst (force, duration);
		else {
//			_photonView.RPC ("RPCPushBurst", PhotonTargets.All, force, duration);
			NetworkManager.SINGLETON.Targeted_RPC (_photonView, "RPCPushBurst", force, duration);
		}
	}

	[PunRPC]
	protected void RPCPushBurst (Vector3 force, float duration)
	{
		if (_photonView.isMine)
			pushBurst (force, duration);
	}

	void pushBurst (Vector3 force, float duration)
	{
		if (_IEPush != null)
			StopCoroutine (_IEPush);
		_IEPush = IEPushBurst (force, duration);
		StartCoroutine (_IEPush);
	}

	IEnumerator IEPushBurst (Vector3 force, float duration)
	{
		float timer = Time.time + duration;
		while (Time.time < timer) {
			float t = duration - (timer - Time.time);
			t = t / duration;
			Vector3 lerpForce = Vector3.Lerp (force, Vector3.zero, t);
			transform.position = new Vector3 (transform.position.x + lerpForce.x, transform.position.y + lerpForce.y, transform.position.z + lerpForce.z);
			yield return new WaitForFixedUpdate ();
		}
	}

	public void AddVelocity (Vector3 vel)
	{
		if (PhotonNetwork.offlineMode)
			addVelocity (vel);
		else
			NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCAddVelocity", vel);
	}

	[PunRPC]
	protected void RPCAddVelocity (Vector3 vel)
	{
		addVelocity (vel);
	}

	protected void addVelocity (Vector3 vel)
	{
		_rigidbody.velocity += vel;
//		print ("pulling : " + vel.ToString ());
	}
	//	public void ReceiveCommunicatedMemory(List<Observation> observations) {
	//		this.context.memory.BulkAddOrUpdateObservation (observations);
	//	}

	//	public void AddToSlot(ObjectStatusHandler entity) {
	//		for (int i = 0; i < _attackPositions.Length; i++) {
	//			if (_attackPositions [i] == null) {
	//				_attackPositions [i] = entity;
	//				return;
	//			}
	//		}
	//	}

	//	public Vector3? GetDesiredAttackPosition(ObjectStatusHandler entity) {
	//		for (int i = 0; i < _attackPositions.Length; i++) {
	//			if (_attackPositions [i] != null) {
	//				if (_attackPositions [i].Equals (entity)) {
	//					if (i == 0) {
	//						return transform.position + transform.forward * .1f;
	//					} else if (i == 1) {
	//						return transform.position + transform.right * .1f;
	//					} else if (i == 2) {
	//						return transform.position - transform.forward * .1f;
	//					} else if (i == 3) {
	//						return transform.position - transform.right * .1f;
	//					}
	//				}
	//			}
	//		}
	//
	//		return null;
	//	}

	//	public Vector3? GetDesiredTacticalPosition(ObjectStatusHandler entity, bool ignoreAttackPositions = false) {
	//		if (ignoreAttackPositions) {
	//			return transform.position + (entity.transform.position - transform.position).normalized * 4f;
	//		}
	//
	//		for (int i = 0; i < _attackPositions.Length; i++) {
	//			if(_attackPositions[i] == null || !_attackPositions[i].Alive()) {
	//				_attackPositions[i] = null;
	//			}
	//		}
	//
	//		for (int i = 0; i < _attackPositions.Length; i++) {
	//			if (_attackPositions [i] != null) {
	//				if (_attackPositions [i].Equals (entity)) {
	//					if (i == 0) {
	//						return transform.position + Vector3.forward * 4f;
	//					} else if (i == 1) {
	//						return transform.position + Vector3.right * 4f;
	//					} else if (i == 2) {
	//						return transform.position - Vector3.forward * 4f;
	//					} else if (i == 3) {
	//						return transform.position - Vector3.right * 4f;
	//					}
	//				}
	//			}
	//		}
	//
	//		return transform.position + (entity.transform.position - transform.position);
	//	}
}