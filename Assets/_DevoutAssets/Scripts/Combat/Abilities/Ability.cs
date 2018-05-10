using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Written by Duke Im
/// On (Last trackable date) 2016-10-02
/// 
/// <summary>
/// Base class for abilities
/// Ability includes basic attack, root motion attacks, blocks and ultimate
/// I.e. Any input character mvoement that does damage
/// </summary>
public abstract class Ability : MonoBehaviour
{
	//dev
	public List<Ability> ChildAbilities = new List<Ability> ();

	/// <summary>
	/// The name of the input defined in project input settings
	/// </summary>
	[SerializeField] string _inputName;
	/// <summary>
	/// How much damage does this ability do on collision
	/// </summary>
	[SerializeField] protected int _damage;
	/// <summary>
	/// Weapon linked to this ability
	/// </summary>
	[SerializeField] protected Weapon _weapon;
	/// <summary>
	/// Does this ability assist aim?
	/// Aim assist rotates character towards closest target(to camera look position)
	/// </summary>
	[SerializeField] protected bool _aimAssist;
	/// <summary>
	/// Does this ability assist positioning?
	/// Position assist moves character towards closest target(to camera look position)
	/// </summary>
	[SerializeField] protected bool _positionAssist = false;
	/// <summary>
	/// Is character turning allowed during using this ability>
	/// </summary>
	[SerializeField] protected bool _isTurnAllowed = false;
	/// <summary>
	/// Is character movement allowed during using this abiltiy?
	/// </summary>
	[SerializeField] protected bool _isMovmentAllowed = false;
	/// <summary>
	/// Is character sprint movement allowed during using this ability?
	/// </summary>
	[SerializeField] protected bool _isSprintAllowed = false;
	/// <summary>
	/// Combathandler of character holding this ability
	/// </summary>
	protected CombatHandler _combatHandler;
	/// <summary>
	/// Status of this ability
	/// IN_USE while this ability is active
	/// UNAVAILABLE while not active(such as cooldown)
	/// AVAILABLE when ability can be used
	/// </summary>
	[SerializeField]protected ABILITY_STATUS _status = ABILITY_STATUS.UNAVAILABLE;

	//Coroutine for activating damagingpoint
	protected IEnumerator _IE_ActivateDamagingPoint;

	//	//Cache
	//	protected Vector3 _targetPos;
	//	protected bool _targetFound;

	protected virtual void Start ()
	{		
		_status = ABILITY_STATUS.AVAILABLE;
	}

	/// <summary>
	/// Activate / Use this ability.
	/// </summary>
	public virtual void Activate ()
	{
		if (_aimAssist) {
			assistAim (0.2f);
		}
		if (_positionAssist) {
			assistPosition (0.3f);
		}
		//lock player controls during activation
		//must be unlocked in child(better design?)
		lockControls ();
	}

	/// <summary>
	/// Gets the weapon this ability uses.
	/// </summary>
	/// <returns>The weapon.</returns>
	public Weapon GetWeapon ()
	{
		return _weapon;
	}

	/// <summary>
	/// Gets the ability status
	/// </summary>
	/// <returns>The status.</returns>
	public ABILITY_STATUS GetStatus ()
	{
		return _status;
	}

	/// <summary>
	/// Sets ability status
	/// </summary>
	/// <param name="newStatus">New status.</param>
	public void SetStatus (ABILITY_STATUS newStatus)
	{
		_status = newStatus;
//		print ("Ultimate ready");
	}

	/// <summary>
	/// Gets the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	public int GetDamage ()
	{
		return _damage;
	}

	/// <summary>
	/// Gets the name of the input used for activating this ability
	/// </summary>
	/// <returns>The input name.</returns>
	public string GetInputName ()
	{
		return _inputName;
	}

	/// <summary>
	/// Sets the combat handler linked to character holding this ability
	/// </summary>
	/// <param name="ch">Combat handler</param>
	public virtual void SetCombatHandler (CombatHandler ch)
	{
		_combatHandler = ch;
	}

	/// <summary>
	/// Activate damaging point so that it deals damage to enemies on collision after waittime and over duration
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="weapon">Weapon.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="waitTime">Wait time.</param>
	IEnumerator IEActivateDamagingPoint (Weapon weapon, float duration, float waitTime = 0f)
	{
		yield return new WaitForSeconds (waitTime);
		if(weapon)
			weapon.ActivateDamagingPoint (duration);
	}

	/// <summary>
	/// IEnumerator encapsulator for force stopping
	/// Activate damaging point so that it deals damage to enemies on collision after waittime and over duration
	/// </summary>
	/// <param name="weapon">Weapon.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="waitTime">Wait time.</param>
	protected void ActivateDamagingPoint (Weapon weapon, float duration, float waitTime = 0f)
	{
		if (_IE_ActivateDamagingPoint != null) {
			StopCoroutine (_IE_ActivateDamagingPoint);
		}
		_IE_ActivateDamagingPoint = IEActivateDamagingPoint (weapon, duration, waitTime);
		StartCoroutine (_IE_ActivateDamagingPoint);

		//dev save 2017-02-25
//		if (_IEnumActivateDamagingPoint == null) {
//			_IEnumActivateDamagingPoint = IEActivateDamagingPoint (weapon, duration, waitTime);
//			StartCoroutine (_IEnumActivateDamagingPoint);
//		} else {
//			StopCoroutine (_IEnumActivateDamagingPoint);
//			_IEnumActivateDamagingPoint = IEActivateDamagingPoint (weapon, duration, waitTime);
//			StartCoroutine (_IEnumActivateDamagingPoint);
//		}
	}

	/// <summary>
	/// Stops the ActivateDamagingPoint IEnumerator in progress.
	/// </summary>
	public void StopIEADP ()
	{
		if (_IE_ActivateDamagingPoint != null)
			StopCoroutine (_IE_ActivateDamagingPoint);
	}

	/// <summary>
	/// Assists aim. In other words,
	/// Find the closest target to be hit & aims towards the target automatically.
	/// </summary>
	/// <param name="seconds">How fast does this character aim</param>
	protected void assistAim (float seconds)
	{	
		float aimValidDistance = 3f;
		PlayerCombatHandler pch = _weapon.GetOwner () as PlayerCombatHandler;
//		_targetFound = false;
		//find close enemy
//		if (EnemyAIHandler.EnemyList.Count > 0) {
		if (CombatHandler.GET_ENEMIES(pch.GetTeam(), true).Count > 0){
			Transform ownerTransform = _weapon.GetOwner ().transform;
			Vector3	camForward = Vector3.Scale (CameraController.CC.CombatCamera.transform.forward, new Vector3 (1, 0, 1)).normalized;
			Vector3 searchPos = ownerTransform.position + camForward;

			//if an enemy is close to valid distance, use aimAssist
			if (pch == null)
				Debug.LogWarning ("WARNING : Playercharacter must have PlayerCombatHandler");
			else {
				if (pch.GetClosestEnemyDistance (searchPos) < aimValidDistance) {
					CombatHandler closestEnemy = pch.GetClosestEnemy (searchPos);
//					EnemyAIHandler closeEnemey = EnemyAIHandler.GetClosestEnemy (searchPos);
//				_targetPos = closeEnemey.transform.position;
					Vector3 direction = closestEnemy.transform.position - ownerTransform.position;
					direction = new Vector3 (direction.x, 0f, direction.z);
					Quaternion toRotation = Quaternion.LookRotation (direction);
					StartCoroutine (MathHelper.IELerpRotationOverTime (ownerTransform, ownerTransform.rotation, toRotation, seconds));
//				_targetFound = true;
				} 
//			else {
//				print ("Not close enough, target : " + EnemyAIHandler.GetClosestEnemy (searchPos).name + ", distance : " + EnemyAIHandler.GetClosestEnemyDistance (searchPos));
//			}
			}
		}
	}

	/// <summary>
	/// Assists position. In other words,
	/// Find the closest target to be hit & moves to a position to better ensure hit.
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	protected void assistPosition (float seconds)
	{
		//how close does an enemy need to be for this to be activated
		float positionValidDistance = 3f;
		//move position that is this far from the target
		float distanceFromTarget = 2f;
		PlayerCombatHandler pch = _weapon.GetOwner () as PlayerCombatHandler;
		//find close enemy
		//		if (EnemyAIHandler.EnemyList.Count > 0) {
		if (CombatHandler.GET_ENEMIES(pch.GetTeam(), true).Count > 0){
			Transform ownerTransform = _weapon.GetOwner ().transform;
			Vector3	camForward = Vector3.Scale (CameraController.CC.CombatCamera.transform.forward, new Vector3 (1, 0, 1)).normalized;
			Vector3 searchPos = ownerTransform.position + camForward;

			//if an enemy is close to valid distance, assist position
//			if (EnemyAIHandler.GetClosestEnemyDistance (searchPos) < positionValidDistance) {
			if (pch.GetClosestEnemyDistance (searchPos) < positionValidDistance) {
				CombatHandler closestEnemy = pch.GetClosestEnemy (searchPos);
				//direction from target to this character
				Vector3 direction = ownerTransform.position - closestEnemy.transform.position;
				direction = new Vector3 (direction.x, 0f, direction.z);
				direction = direction.normalized * distanceFromTarget;

				//position a certain distance(distanceFromTarget) from the target
				direction = closestEnemy.transform.position + direction;

				//move towards that position
				StartCoroutine (MathHelper.IELerpRBPositionOverTime (ownerTransform.GetComponent<Rigidbody> (), ownerTransform.position, direction, seconds));

//				//dev
//				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//				cube.transform.position = direction;
//				Debug.Break ();
//				Quaternion toRotation = Quaternion.LookRotation (direction);
//				StartCoroutine (MathHelper.IELerpRotationOverTime (ownerTransform, ownerTransform.rotation, toRotation, seconds));
			} 
		}
	}

	/// <summary>
	/// Unlock player controls that was locked
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	protected virtual void unlockControls ()
	{
		if (_combatHandler.GetComponent<PlayerMovementControl> () != null) {
			PlayerMovementControl pmc = _combatHandler.GetComponent<PlayerMovementControl> ();
//			if (!_isTurnAllowed)
			pmc.SetIsTurnAllowed (true);
//			if (!_isMovmentAllowed)
			pmc.SetIsMovementAllowed (true);
//			if (!_isSprintAllowed)
			pmc.SetIsSprintAllowed (true);
		}
//			if (_weapon.GetOwner ().GetComponent<PlayerMovementControl> () != null) {
//				PlayerMovementControl pmc = _weapon.GetOwner ().GetComponent<PlayerMovementControl> ();
//				if (!_isTurnAllowed)
//					pmc.SetIsTurnAllowed (true);
//				if (!_isMovmentAllowed)
//					pmc.SetIsMovementAllowed (true);
//				if (!_isSprintAllowed)
//					pmc.SetIsSprintAllowed (true);
//			}
	}

	/// <summary>
	/// Locks player controls that was set in the editor
	/// </summary>
	protected virtual void lockControls ()
	{
		if (_combatHandler.GetComponent<PlayerMovementControl> () != null) {
			PlayerMovementControl pmc = _combatHandler.GetComponent<PlayerMovementControl> ();
			if (!_isTurnAllowed)
				pmc.SetIsTurnAllowed (false);
			if (!_isMovmentAllowed)
				pmc.SetIsMovementAllowed (false);
			if (!_isSprintAllowed)
				pmc.SetIsSprintAllowed (false);
		}
	}

	/// <summary>
	/// Unlock controls that was locked after duration
	/// </summary>
	/// <returns>IEnumerator</returns>
	/// <param name="seconds">Seconds.</param>
	protected IEnumerator IEUnlockControls (float seconds)
	{ 
		yield return new WaitForSeconds (seconds);
		unlockControls ();
	}

	/// <summary>
	/// Locks and unlocks the controls.
	/// </summary>
	/// <param name="isTurnAllowed">If set to <c>true</c> is turn allowed.</param>
	/// <param name="isMovementAllowed">If set to <c>true</c> is movement allowed.</param>
	/// <param name="isSprintAllowed">If set to <c>true</c> is sprint allowed.</param>
	protected void setControls (bool isTurnAllowed, bool isMovementAllowed, bool isSprintAllowed)
	{
		if (_combatHandler.GetComponent<PlayerMovementControl> () != null) {
			PlayerMovementControl pmc = _combatHandler.GetComponent<PlayerMovementControl> ();
			pmc.SetIsTurnAllowed (isTurnAllowed);
			pmc.SetIsMovementAllowed (isMovementAllowed);
			pmc.SetIsSprintAllowed (isSprintAllowed);
		}
	}

	public bool GetIsMovementAllowed ()
	{
		return _isMovmentAllowed;
	}
}

/// <summary>
/// Status of this ability
/// </summary>
public enum ABILITY_STATUS
{
	AVAILABLE,
	IN_USE,
	IN_COOLDOWN,
	UNAVAILABLE,
}
