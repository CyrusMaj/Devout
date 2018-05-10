using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PvPLobbyManager : Photon.PunBehaviour
{
	public static PvPLobbyManager SINGLETON;
	//	[SerializeField] GameObject _pvpLobbyUI;
	//	[SerializeField] GameObject _pvpLobbyCreateRoomUI;
	[SerializeField] GameObject _pvpLobbySelectClass;
	//	[SerializeField] UpdateCoopRoomList _coopRoomList;
	public RoomInfo SelectedRoom{ get; private set; }

	// Use this for initialization
	void Start ()
	{
		if (SINGLETON == null)
			SINGLETON = this;
		else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTORLLER");


	}

	/// <summary>
	/// Toggles the coop lobby window.
	/// Hide / Show the lobby UI
	/// </summary>
	//	public void SetActivePvpLobbyWindow(bool newActive){
	//		_pvpLobbyUI.SetActive (newActive);
	//	}

	/// <summary>
	/// Toggles the coop lobby create room window.
	/// Hide / Show the lobby create room UI
	/// </summary>
	//	public void SetActivePvpLobbyCreateRoomWindow(bool newActive){
	//		_pvpLobbyCreateRoomUI.SetActive (newActive);
	//	}

	/// <summary>
	/// Toggles the coop lobby select character/class window.
	/// Hide / Show the lobby class selction UI
	/// </summary>
	public void SetActivePvpLobbySelectClass (bool newActive)
	{
		_pvpLobbySelectClass.SetActive (newActive);
	}

	/// <summary>
	/// Sets the selected room.
	/// </summary>
	/// <param name="newRoom">New room.</param>
	public void SetSelectedRoom (RoomInfo newRoom)
	{
		SelectedRoom = newRoom;
	}

	public override void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		base.OnMasterClientSwitched (newMasterClient);

		_pvpLobbySelectClass.GetComponent<UISelectClass> ().BackButton ();
		SetActivePvpLobbySelectClass (false);

		PhotonNetwork.LeaveRoom ();
	}

//	[PunRPC]
//	void RPCleaveRoom ()
//	{
//		if (PhotonNetwork.room == null) {
//			print ("ERROR : Room is empty");
//			return;
//		}
//
//		if (_pvpLobbySelectClass.GetComponent<UISelectClass> ())
////			print (_pvpLobbySelectClass.GetComponent<UISelectClass> ().name);
//			_pvpLobbySelectClass.GetComponent<UISelectClass> ().BackButton (true);
//		else
//			print ("NULL");
//		SetActivePvpLobbySelectClass (false);
////		gameObject.SetActive (false);
//		print ("Forced Leaving Room");
//	}

//	public void ForceClientsLeaveRoom ()
//	{
////		print ("Force Clients to leave room total count : " + PhotonNetwork.playerList.Length);
//		for(int i = 0; i < PhotonNetwork.playerList.Length ; i++){
////			if (PhotonNetwork.playerList [i].IsMasterClient)
////				continue;
//			photonView.RPC ("RPCleaveRoom", PhotonNetwork.playerList[i]);
//			print ("Sent force leave RPC");
//		}
////		photonView.RPC ("RPCleaveRoom", PhotonTargets.All);
////		print ("Sent force leave RPC");
//	}
}
