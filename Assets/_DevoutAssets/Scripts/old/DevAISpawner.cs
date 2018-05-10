using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class DevAISpawner : Photon.PunBehaviour {
	public static DevAISpawner DAIS;
	[SerializeField] List<SpawnArea> _spawnAreaList = new List<SpawnArea>();
//	[SerializeField] Transform _spawnPos;
	[SerializeField] int _enemyCountAtOnce = 4;
	[SerializeField] int _enemyCountTotal= 20;
	int _stage = 1;
	public bool _stageCleared{ get; private set; }
	float _randomTimer;
	PhotonView _pv;
	int _startingEnemyCount;
	int _startingEnemyCountAtOnce;
	int _stageDifficulty = 3;

	// Use this for initialization
	void Start () {
		if (DAIS == null)
			DAIS = this;
		else
			Debug.LogWarning ("WARNING : There are more than one controller");

		_pv = PhotonView.Get (this);

		_randomTimer = 1f;
		_stageCleared = false;
		_startingEnemyCount = _enemyCountTotal;
		_startingEnemyCountAtOnce = _enemyCountAtOnce;

//		if(PhotonNetwork.connected)
//			_pv.RPC ("RPCDisplayStageStarted", PhotonTargets.All);
	}

	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
	{
		base.OnPhotonPlayerConnected (newPlayer);

		//new players needs to be updated with some stage infos
		_pv.RPC ("RPCSetEnemyCount", newPlayer, _enemyCountTotal);
		_pv.RPC ("RPCSetStage", newPlayer, _stage);
	}

	public override void OnJoinedRoom ()
	{
		base.OnJoinedRoom ();
//		UIController.DC.SetDevText("Stage "+_stage+ " started", 2f);	
	}
	
	// Update is called once per frame
	void Update () {
//		if (PhotonNetwork.isMasterClient) {
//			if (_randomTimer < 0f) {
//				_randomTimer = 0.5f;
//				if (_enemyCountTotal > 0 && EnemyAIHandler.EnemyList.Count < _enemyCountAtOnce && PhotonNetwork.inRoom) {
//					spawnAI ();
//					_randomTimer += Random.Range (0.75f, 2f);
//				} else if (EnemyAIHandler.EnemyList.Count <= 0 && _enemyCountTotal <= 0 && !_stageCleared) {
//					//stage is cleared, get ready for the next stage
//					UIController.DC.SetDevText("Stage "+_stage+" cleared! Press C to begin the next stage...");
//					_pv.RPC ("RPCDisplayStageClearedText", PhotonTargets.Others);
//					_stage++;
//					setStage (_stage);
//					_stageCleared = true;
//				}
//			} else
//				_randomTimer -= Time.deltaTime;
//		}
	}

	/// <summary>
	/// Tell non-master clients to display stage cleared text(string excluding input key to proceed to next stage)
	/// </summary>
	[PunRPC]
	void RPCDisplayStageClearedText(){
		UIController.SINGLETON.SetDevText("Stage "+_stage+" cleared! Waiting for the host to begin the next stage...");		
	}
	/// <summary>
	/// Tell all clients to display when a new stage is beginning
	/// </summary>
	[PunRPC]
	void RPCDisplayStageStarted(){
		UIController.SINGLETON.SetDevText("Stage "+_stage+ " started", 2f);		
	}

	/// <summary>
	/// Displays the number of remaining enemies of the current stage
	/// </summary>
	public void DisplayRemainingEnemyCount(){
		if (PhotonNetwork.isMasterClient)
			_pv.RPC ("RPCDisplayRemainingEnemyCount", PhotonTargets.All);
	}

	[PunRPC]
	void RPCDisplayRemainingEnemyCount(){
//		UIController.DC.SetDevText("Enemies remaining : " + ((int)_enemyCountTotal + (int)EnemyAIHandler.EnemyList.Count), 1f);
		UIController.SINGLETON.SetDevText("Enemies remaining : " + ((int)_enemyCountTotal + (int)CombatHandler.GET_ENEMIES(TEAM.ONE, true).Count), 1f);
	}

	void setEnemyCount(int newCount){
		if (PhotonNetwork.offlineMode)
			_enemyCountTotal = newCount;
		else
			_pv.RPC ("RPCSetEnemyCount", PhotonTargets.All, newCount);
	}

	[PunRPC]
	void RPCSetEnemyCount(int newCount){
		_enemyCountTotal = newCount;
	}
		
	void setStage(int stageNum){
		if (PhotonNetwork.offlineMode)
			_stage = stageNum;
		else
			_pv.RPC ("RPCSetStage", PhotonTargets.All, stageNum);		
	}

	[PunRPC]
	void RPCSetStage(int stageNum){
		_stage = stageNum;
	}


	void spawnAI(){
		_enemyCountTotal--;
		setEnemyCount(_enemyCountTotal);
//		bool isThereEmptySpawnArea = false;
		SpawnArea freeSA = null;
		_spawnAreaList.Shuffle ();
		foreach (var sa in _spawnAreaList) {
			if (sa._spawnedTarget == null) {
//				isThereEmptySpawnArea = true;
				freeSA = sa;
				break;
			}
		}

		if (freeSA != null) {
			GameObject AIInstance = PhotonNetwork.Instantiate ("Paladin", new Vector3 (0f, 5f, 0f), Quaternion.identity, 0);
			AIInstance.transform.position = freeSA.transform.position;
			freeSA.SetSpawnedTarget (AIInstance.transform);
		}
	}

	/// <summary>
	/// Starts the next stage.
	/// </summary>
	public void StartNextStage(){
		_enemyCountAtOnce = _startingEnemyCountAtOnce + (_stage + 1) / _stageDifficulty;
//		_enemyCountTotal = _startingEnemyCount + (int)Mathf.Pow((float)_stage,2f) + _stageDifficulty;
		setEnemyCount (_startingEnemyCount + (int)Mathf.Pow ((float)_stage, 2f) + _stageDifficulty);
		_stageCleared = false;
		_pv.RPC ("RPCDisplayStageStarted", PhotonTargets.All);
	}
}
