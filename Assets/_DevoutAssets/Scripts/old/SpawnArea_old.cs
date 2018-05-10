using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]
/// <summary>
/// Handles spawning(Instantiating) area with trigger collider, so that the area is clear to spawn a unit
/// </summary>
public class SpawnArea_old : Photon.PunBehaviour {
	Collider _collider;
	GameObject _spawningUnit;
	bool _isAreaOccupied;
	PhotonView _photonView;
//	[SerializeField] SpawnAreaProtector _protector;

	void Start(){
		_collider = GetComponent<Collider> ();
		_collider.enabled = true;
		_collider.isTrigger = false;
		gameObject.layer = LayerHelper.COMMON_BODY;
		_photonView = PhotonView.Get (this);

//		_protector = GetComponent<SpawnAreaProtector> ();
	}
	void OnTriggerExit(Collider other) {
		if (_spawningUnit != null && other.gameObject == _spawningUnit) {
//			print ("Exited spawn area");

			ResetSpawnArea ();
//			gameObject.layer = LayerHelper.COMMON_BODY;
//			if (_protector)
//				_protector.DisableProtector ();
		}
	}
	/// <summary>
	/// Sets the spawn area which is used to detect if this spawn area is being used or not.
	/// </summary>
	/// <param name="spawningUnit">Spawning unit.</param>
	public void SetSpawnArea(GameObject spawningUnit){
//		print ("SetSapwnArea");
		_spawningUnit = spawningUnit;
		_photonView.RPC ("RPCUpdate", PhotonTargets.All, true);
	}
	/// <summary>
	/// Sets the spawn area which is used to detect if this spawn area is being used or not. Also enables linked protector
	/// </summary>
	/// <param name="spawningUnit">Spawning unit.</param>
	/// <param name="team">Spawning unit's Team.</param>
	public void SetSpawnArea(GameObject spawningUnit, TEAM team){
		SetSpawnArea (spawningUnit);
//		gameObject.layer = LayerHelper.GetAttackBoxLayer (team);
//		if (_protector)
//			_protector.EnableProtector (team);
	}
	public void ResetSpawnArea(){
		_spawningUnit = null;
		_photonView.RPC ("RPCUpdate", PhotonTargets.All, false);
	}
	/// <summary>
	/// Determines whether this spawnarea is in use. I.e. is spawned unit still in the spawn area?
	/// </summary>
	/// <returns><c>true</c> if this instance is in use; otherwise, <c>false</c>.</returns>
	public bool IsInUse(){
		return _isAreaOccupied;
	}
	[PunRPC]
	public void RPCUpdate(bool isAreaOccupied){//, int playerViewID){//in progress(playerViewID is not needed, delete it after getting used to finding objects with viewID)
		_isAreaOccupied = isAreaOccupied;
		if (_isAreaOccupied)
			_collider.isTrigger = true;
		else
			_collider.isTrigger = false;
//		if (_isAreaOccupied)
//			SetSpawnArea ();
//		else
//		_spawningUnit = PhotonView.Find(playerViewID).gameObject;
	}
	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		Start ();
		if (_photonView.isMine)
			_photonView.RPC ("RPCUpdate", PhotonTargets.All, _isAreaOccupied);//, _spawningUnit.GetComponent<PhotonView>().viewID);
	}
}
