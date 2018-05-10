using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Character status handler.
/// Animation, ragdoll and character only status info
/// </summary>
[RequireComponent (typeof(Animator))]
public class CharacterStatusHandler : ObjectStatusHandler
{
	/// <summary>
	/// Type of character
	/// </summary>
	public enum TYPE
	{
		//Non-player character controlled by AI
		NON_PLAYER_AI,
		//Player cahracter controlled by AI
		PLAYER_AI,
		//Player character controlled by user
		PLAYER,
	}

	[SerializeField]TYPE _type;

	public TYPE Type{ get { return _type; } set { _type = value; } }

	/// <summary>
	/// Ragdoll helper script that is attached to this character's skeleton / rig
	/// </summary>
	[SerializeField] RagdollHelper _ragdollHelper;
	/// <summary>
	/// The animator for this character
	/// </summary>
	Animator _animator;
	/// <summary>
	/// Character collider used for movement only(Not a hitbox)
	/// </summary>
	Collider _collider;
	/// <summary>
	/// Stagger coroutine
	/// </summary>
	IEnumerator _IEStagger;

	//dev
	bool _isWalled = false;

	protected override void Start ()
	{
		base.Start ();
		if (_ragdollHelper == null) {
			print ("WARNING : RagdollHelper is null");
		}

		_animator = GetComponent<Animator> ();
		_collider = GetComponent<Collider> ();
	}

	/// <summary>
	/// Check if object is alive,
	/// If not and conditions are met, destroy this object
	/// </summary>
	protected override void checkAlive ()
	{
		if (_health < 1) {
			die ();
		}
	}

	/// <summary>
	/// Kill this instance and play ragdoll,
	/// </summary>
	protected virtual void die ()
	{
		//disable animator
		if (_animator != null)
			_animator.enabled = false;

		//disable rigidbody
		if (_rigidbody != null) {
			_rigidbody.useGravity = false;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.isKinematic = true;
		}

		//disable collider
		if (_collider != null)
			_collider.enabled = false;

		//play ragdoll
		_ragdollHelper.EnableColliders (true);
		_ragdollHelper.SetKinematic (false);
		_ragdollHelper.UseGravity (true);

		//disable hitboxes
		foreach (var hb in _hitboxes) {
			hb.GetCollider ().enabled = false;
		}

		//disable control if player
		//Is there better way / place to put this?
		setControlAllowedIfPlayer (false);

		//disable any active enabled damaging points
//		if (GetComponent<CombatHandler> () != null)
//			GetComponent<CombatHandler> ().DisableDamagingPoints ();
//		else
//			Debug.LogWarning ("WARNING : No combathandler found");

//		Debug.Break ();
	}

	/// <summary>
	/// Stagger this character for the specified duration.
	/// Staggered character is not able to be controlled(if player controlled character)
	/// I.E. Frozen or stunned
	/// </summary>
	/// <param name="duration">Duration.</param>
	public void Stagger (float duration, float waitTime = 0f)
	{
		//don't do anything if duration is 0 or less
		if (duration <= 0)
			return;
		
		if (PhotonNetwork.offlineMode)
			stagger (duration, waitTime);
		else
			_photonView.RPC ("RPCStagger", PhotonTargets.All, duration, waitTime);
	}

	void stagger (float duration, float waitTime = 0f)
	{
		if (_health > 0) {
			if (_IEStagger != null) {
				StopCoroutine (_IEStagger);
				setControlAllowedIfPlayer (true, false);
			}
			_IEStagger = IEStagger (duration, waitTime);
			StartCoroutine (_IEStagger);
		}
	}
	//Stagger method is made into RPC as animation and movement(via adding force) does not automatically sync
	[PunRPC]
	protected void RPCStagger (float duration, float waitTime = 0f)
	{
		stagger (duration, waitTime);
	}

	IEnumerator IEStagger (float duration, float waitTime = 0f)
	{
		
		//wait before staggering(to fine-tune animations)
		//if(waitTime > 0f)
		yield return new WaitForSeconds (waitTime);

		//prevent player control while staggered
		setControlAllowedIfPlayer (false, false);
		_animator.SetBool (AnimationHashHelper.PARAM_STAGGERED, true);


		//adds delay when ai is attacked
		AIBehavior aib = GetComponent<AIBehavior> ();
		if (aib != null) {
			aib.IsBehaviorAllowed = false;
		}

		yield return new WaitForSeconds (duration);

		if (aib != null) {
			//dev
			aib.IsBehaviorAllowed = true;
		}

		_animator.SetBool (AnimationHashHelper.PARAM_STAGGERED, false);
		if (_health > 0)
			setControlAllowedIfPlayer (true, false);
	}

	/// <summary>
	/// allow or disallow control if this charcter is player character
	/// </summary>
	/// <param name="isAllowed">If set to <c>true</c> is allowed.</param>
	void setControlAllowedIfPlayer (bool isAllowed, bool isCursorVisible = true)
	{
		if (transform == GameController.GC.CurrentPlayerCharacter) {
			GameController.GC.SetIsControlAllowed (isAllowed, isCursorVisible);
		}
	}
}
