using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerListController : MonoBehaviour
{
	public static PlayerListController PLC;

	public const int MAX_NUM_PLAYERS = 4;

	List<PlayerInfo> _players = new List<PlayerInfo> ();

	PhotonView _photonView;

	// Use this for initialization
	void Start ()
	{
		if (PLC == null)
			PLC = this;

		_photonView = PhotonView.Get (this);
	}

	public void AddPlayer (PhotonPlayer newPlayer)
	{
		_photonView.RPC ("RPCAddPlayer", PhotonTargets.All, newPlayer);
		//		_photonView.RPC ("RPCSubtractHealth", PhotonTargets.All, amount);
//		if (_players.Contains(newPlayer))
//			print ("WARNING : Player already exists");
//		else
//			_players.Add (newPlayer);
	}
	public void RemovePlayer(PhotonPlayer playerToRemove){
		_photonView.RPC ("RPCRemovePlayer", PhotonTargets.All, playerToRemove);
//		_players.Remove (playerToRemove);
	}
	public List<PlayerInfo> GetPlayers(){
		return _players;
	}
	public PlayerInfo GetPlayerInfo(Transform target){
		foreach (var pi in _players) {
			if (pi.Target == target)
				return pi;
		}
		print ("WARNING : MATCH NOT FOUND");
		PlayerInfo piNull = new PlayerInfo ();
		piNull.Number = -1;
		piNull.Target = GameController.GC.CurrentPlayerCharacter;
		piNull.PP = PhotonNetwork.player;
		return piNull;
	}
	public int FindAvailablePlayerNumber(){
		if (PhotonNetwork.isMasterClient) {
			print ("this is master");
			foreach (var p in _players) {
				if (p.Number == 1) {
					print ("WARNING : Master exists already");
				}
			}
			return 1;
		} else {
			print ("this isn't master");
			List<bool> numSlots = new List<bool> {false, false, false, false, false};
			foreach (var p in _players) {
				numSlots [p.Number] = true;	
			}
			for (int i = 1; i < PlayerListController.MAX_NUM_PLAYERS; i++) {
				if (!numSlots[i]) {
					return i;
				}
			}
		}
		print ("WARNING : NO AVAILABLE PLAYER NUMBER");
		return -1;
	}
	public Transform FindTarget(PhotonPlayer player){
//		print ("go found count : " + GameObject.FindGameObjectsWithTag (TagHelper.PLAYER).Length);
		foreach (var go in GameObject.FindGameObjectsWithTag (TagHelper.PLAYER)) {
//			print (go.name + ", " + go.GetComponent<PhotonView> ().owner.ToString() + " compare to " + player.ToString());
			if (go.GetComponent<PhotonView> ().owner == player) {
				return go.transform;
			}
		}
		print ("WARNING : MATCH NOT FOUND");
		return transform;
	}
	[PunRPC]
	public void RPCAddPlayer(PhotonPlayer newPlayer){
		PlayerInfo pi = new PlayerInfo ();
		pi.PP = newPlayer;
		pi.Number = FindAvailablePlayerNumber ();
		pi.Target = FindTarget (newPlayer);

//		pi.Target = ((GameObject)newPlayer.TagObject).transform;

		if (_players.Contains(pi))
			print ("WARNING : Player already exists");
		else
			_players.Add (pi);
	}
	[PunRPC]
	public void RPCRemovePlayer(PhotonPlayer playerToRemove){
		PlayerInfo pi = new PlayerInfo ();

		foreach (var p in _players) {
			if (p.PP == playerToRemove)
				pi = p;
		}
		if (pi.PP == null)
			print ("Warning : PlayerInfo not found/not matching");
		else
			_players.Remove (pi);
	}
}

public struct PlayerInfo{
	public Transform Target;
	public PhotonPlayer PP;
	public int Number;
}
