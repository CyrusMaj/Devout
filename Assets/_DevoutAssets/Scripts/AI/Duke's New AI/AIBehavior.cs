using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-12-15
/// 
/// <summary>
/// Base AI behavior class that provides common / virtual methods and variables
/// </summary>
public abstract class AIBehavior : Photon.PunBehaviour
{
//	//Enable disable this behavior for this instance only
//	public bool IsBehaviorAllowed{ get; private set; }
//
//	public void SetIsBehaviorAllowed (bool isAllowed)
//	{
//		IsBehaviorAllowed = isAllowed;
//	}

	//Caches
	protected PhotonView _pv;
	protected AIScanner _ais;
	protected AIMovementHandler _aimh;
	protected AIStatusHandler _aish;
	protected CombatHandler _ch;

	//
	[SerializeField] public bool IsBehaviorAllowed = true;

	virtual protected void Start ()
	{
		_pv = PhotonView.Get (this);
		_ais = GetComponent<AIScanner> ();
		_aimh = GetComponent<AIMovementHandler> ();
		_aish = GetComponent<AIStatusHandler> ();
		_ch = GetComponent<CombatHandler> ();
	}
}
