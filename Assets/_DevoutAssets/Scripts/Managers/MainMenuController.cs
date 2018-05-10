using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DukeIm;

public class MainMenuController : Photon.PunBehaviour
{
	public static MainMenuController MMC;
	//	[SerializeField] string _version = "XXX";
	bool _startSceneLocked = false;
	[SerializeField] Text _logTxt;
	//	[SerializeField] Button _btnCoopLobby;
	//	[SerializeField] Button _btnPvpLobby;
	//	[SerializeField] Button _btnSinglePlayer;
	[SerializeField] Button _reconnect;

	[SerializeField] Button _btnPlay;

	void Start ()
	{
		//dev for network improvement
//		print("Sendrate : " + PhotonNetwork.sendRate + ", SendRateOnSerialize : " + PhotonNetwork.sendRateOnSerialize);
//		PhotonNetwork.sendRate = 20;
//		PhotonNetwork.sendRateOnSerialize = 10;
//		print("New Sendrate : " + PhotonNetwork.sendRate + ", SendRateOnSerialize : " + PhotonNetwork.sendRateOnSerialize);

		//dev for disconnection fix
//		PhotonNetwork.CrcCheckEnabled = true;
		PhotonNetwork.MaxResendsBeforeDisconnect = 10;
		PhotonNetwork.QuickResends = 3;



		if (MMC == null)
			MMC = this;
		else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTROLLER");

		if (PhotonNetwork.connected)
			PhotonNetwork.Disconnect ();

		PhotonNetwork.offlineMode = false;
//		PhotonNetwork.autoCleanUpPlayerObjects = false;
		
		_logTxt.text = "";
		PhotonNetwork.player.name = System.Environment.UserName;

//		if (Application.internetReachability != NetworkReachability.NotReachable) {
//			_logTxt.text = "Connecting...";
//			PhotonNetwork.ConnectUsingSettings (NetworkHelper.VERSION);
//		}

		//dev
//		_btnSinglePlayer.interactable = false;
//		_btnCoopLobby.interactable = false;
//		_btnPvpLobby.interactable = false;
		_reconnect.gameObject.SetActive (false);

		InvokeRepeating ("TryConnecting", 1f, 1f);
//		TryConnecting ();
	}

	/// <summary>
	/// Try connecting to photon server if not connected
	/// </summary>
	public void TryConnecting ()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			_logTxt.text = "Cannot connect to server";
			_reconnect.gameObject.SetActive (true);
			_btnPlay.interactable = false;
			return;
		}
		if (!PhotonNetwork.connected) {
			_logTxt.text = "Connecting...";
			PhotonNetwork.ConnectUsingSettings (NetworkHelper.VERSION);
			_reconnect.gameObject.SetActive (false);
			_btnPlay.interactable = false;
		}
	}

//	public override void OnConnectedToMaster ()
//	{
//		// when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
//		if (!PhotonNetwork.offlineMode) {
//			_logTxt.text = "Connected to the game server";
////			_btnCoopLobby.interactable = true;
////			_btnPvpLobby.interactable = true;
//			_btnPlay.interactable = true;
//		}
////		Debug.Log ("OnConnectedToMaster");
////		//
////		foreach (var ri in PhotonNetwork.GetRoomList()) {
////			print (ri.name);
////			_logTxt.text += "\n" + ri.name;
////		}
//	}

	public override void OnJoinedLobby ()
	{
		_logTxt.text = "Connected to the game server";
		_btnPlay.interactable = true;
//		_btnCoopLobby.interactable = true;
//		_btnPvpLobby.interactable = true;
//		Debug.Log ("OnJoinedLobby");
//		print ("Room count : " + PhotonNetwork.GetRoomList ().Length);
//		PhotonNetwork.JoinRandomRoom ();

		PhotonNetwork.autoCleanUpPlayerObjects = true;
	}

	public void OnPhotonRandomJoinFailed ()
	{
		Debug.Log ("Joining room failed");
//		RoomOptions ro = new RoomOptions ();
//		ro.maxPlayers = 4;
//		PhotonNetwork.CreateRoom ("TheSummitStudio_DevRoom", ro, TypedLobby.Default);
	}

	public override void OnCreatedRoom ()
	{
//		print ("room created");
	}

	public override void OnJoinedRoom ()
	{
//		print ("Room joined in MainMenu Scene");
	}

	public void StartSinglePlayer (/*RoomLevelHelper.SCENE scene*/)
	{
//		if (!_startSceneLocked) {
//			StartCoroutine (CoroutineHelper.IELoadAsyncScene (sceneName));	
//			_startSceneLocked = true;
//		}
		if (!_startSceneLocked) {
			PhotonNetwork.Disconnect ();
			PhotonNetwork.offlineMode = true;
			MainMenuController.MMC.StartScene (RoomLevelHelper.GetSceneName (RoomLevelHelper.SCENE.PRISON/*scene*/));
			_startSceneLocked = true;
		}

	}

	//above and below could be merged into one emthod
	public void StartScene (string sceneName)
	{
		if (!_startSceneLocked) {
			if (PhotonNetwork.connected) {
				_startSceneLocked = true;
				StartCoroutine (CoroutineHelper.IELoadAsyncScene (sceneName));
			} else {
				_logTxt.text = "Cannot play multiplayer when not connected";
				print ("Error : Not connected");
			}
		}
	}

	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	/// <remarks>In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
	/// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().</remarks>
	public override void OnDisconnectedFromPhoton ()
	{
//		print ("disconnected");
		base.OnDisconnectedFromPhoton ();
		Cursor.lockState = CursorLockMode.None;
		StartCoroutine (CoroutineHelper.IELoadAsyncScene (RoomLevelHelper.GetSceneName (RoomLevelHelper.SCENE.MAIN_MENU)));
	}

	public override void OnConnectionFail (DisconnectCause cause)
	{
		base.OnConnectionFail (cause);

		print ("[DEV_BUILD_LOG][Duke Im] - Disconnection cause : " + cause.ToString ());
	}

	public void Quit ()
	{
		Application.Quit ();
	}

	public void ClearRoomInfo ()
	{
		RoomLevelHelper.ROOM_INFO = null;
	}
}
