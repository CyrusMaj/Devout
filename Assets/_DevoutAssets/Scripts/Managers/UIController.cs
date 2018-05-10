using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : Photon.PunBehaviour
{
	public static UIController SINGLETON;
	[SerializeField] Text _devText;
	IEnumerator _coroutineSetDevText;
	public GameObject PlayerUICanvas;

	public GameObject InGameMenu;

	public Text Counter;
	public Text TextWinner;
	public Text MasterGameControlText;

	public Text Score_Team_1;
	public Text Score_Team_2;
	public Text Timer;

	/// <summary>
	/// The player Interaction UI prefab
	/// </summary>
	[SerializeField] GameObject PlayerUIInteractPrefab;
	[HideInInspector]public UIPlayerInteraction PlayerUIInteractInstance;

	//	void Start ()
	void Awake ()
	{
		if (SINGLETON == null) {
			SINGLETON = this;
		} else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTROLLER");

		GameObject PUII = (GameObject)Instantiate (PlayerUIInteractPrefab);
		PlayerUIInteractInstance = PUII.GetComponent<UIPlayerInteraction> ();
	}

	public void CountTo (int countFrom, int countTo)
	{
		targeted_RPC ("RPCCountTo", false, countFrom, countTo);
	}

	public void GameWinner (TEAM winner)
	{
		targeted_RPC ("RPCGameWinner", false, winner);
	}

	[PunRPC]
	void RPCGameWinner(TEAM winner){
		StartCoroutine (IEGameWinner (winner));
	}

	IEnumerator IEGameWinner (TEAM winner)
	{
		float timer = Time.time + 1f;

		TextWinner.gameObject.SetActive (true);

		int counter = GameCapturePoints.TIMER_COUNTER_GAME;
		while (counter > 0) {
			if (Time.time > timer) {
				timer = Time.time + 1f;
				counter--;

				if (winner == TEAM.NULL) {
					TextWinner.text = "Draw";
				} else if (winner == (TEAM)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) {
					TextWinner.text = "Victory";
				}
				else{
					TextWinner.text = "Defeat";
				}
				TextWinner.text += "\n" + counter;
			}
			yield return null;
		}

		TextWinner.text = "";
		TextWinner.gameObject.SetActive (false);
	}

	public void RoundWinner (TEAM winner)
	{
		targeted_RPC ("RPCRoundWinner", false, winner);
	}

	[PunRPC]
	void RPCRoundWinner(TEAM winner){
		StartCoroutine (IERoundWinner (winner));
	}

	IEnumerator IERoundWinner (TEAM winner)
	{
		float timer = Time.time + 1f;

		TextWinner.gameObject.SetActive (true);

		int counter = GameCapturePoints.TIMER_COUNTER_ROUND;
		while (counter > 0) {
			if (Time.time > timer) {
				timer = Time.time + 1f;
				counter--;

				if (winner == TEAM.NULL) {
					TextWinner.text = "Round " + PvpManager.SINGLETON.GetGameInfo().CurrentRound + " Draw";
				} else if (winner == (TEAM)PhotonNetwork.player.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_TEAM]) {
					TextWinner.text = "Round " + PvpManager.SINGLETON.GetGameInfo().CurrentRound + " Won";
				}
				else{
					TextWinner.text = "Round " + PvpManager.SINGLETON.GetGameInfo().CurrentRound + " Lost";
				}
				TextWinner.text += "\n" + counter;
			}
			yield return null;
		}
		TextWinner.text = "";
		TextWinner.gameObject.SetActive (false);
	}

	[PunRPC]
	void RPCCountTo (int countFrom, int countTo)
	{
		if (Counter != null && countFrom > countTo) {
			StartCoroutine (IECountTo (countFrom, countTo));
		}
	}

	//	void Start ()
	//	{
	//		InvokeRepeating ("slowUpdate", 1f, 1f);
	//	}
	//
	//	void slowUpdate ()
	//	{
	////		int time = (int)(Time.time - PvpManager.SINGLETON.GetGameInfo ().GetCurrentRound ().StartTime);
	////		if (time < 0 || time > (PvpManager.SINGLETON.GetGameInfo().MaxTimer * 60f))
	////			Timer = 0;
	//	}

	public void UpdateTimer (float timer)
	{
		targeted_RPC ("updateTimer", false, (int)timer);
	}

	//send rpc to players who are in same room and same scene
	void targeted_RPC (string RPC, bool excludeMyself, params object[] parameters)
	{
		foreach (var p in PhotonNetwork.playerList) {
			if ((bool)p.CustomProperties [RoomLevelHelper.CUSTOM_PLAYER_PROPERTY_IN_GAME_SCENE]) {
				if (excludeMyself && p == PhotonNetwork.player) {
				} else {
					photonView.RPC (RPC, p, parameters);
				}
			}
		}
	}

	[PunRPC]
	void updateTimer (int timer)
	{
		int min = timer / 60;
		int sec = timer % 60;
		string text;
		if (sec > 9) {
			text = min + ":" + sec;
		} else {
			text = min + ":0" + sec;
		}
		Timer.text = text;
	}

	IEnumerator IECountTo (int countFrom, int countTo)
	{
		float timer = Time.time + 1f;

		Counter.gameObject.SetActive (true);
		Counter.text = countFrom.ToString ();

		while (countFrom > countTo) {
			if (Time.time > timer) {
				timer = Time.time + 1f;
				countFrom--;
				Counter.text = "Round " + PvpManager.SINGLETON.GetGameInfo().CurrentRound +
					"\n" + countFrom.ToString ();
			}
			yield return null;
		}

		Counter.text = "";

		Counter.gameObject.SetActive (false);
	}

	void Update ()
	{
		//In-game menu
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!InGameMenu)
				return;

			if (InGameMenu.activeSelf) {
				InGameMenu.SetActive (false);
				GameController.GC.SetIsControlAllowed (true, false);
			} else {
				InGameMenu.SetActive (true);				
				GameController.GC.SetIsControlAllowed (false, true);
			}
		}
	}

	/// <summary>
	/// Sets the Development Purpose-Only Text.
	/// </summary>
	/// <param name="devText">Text to display</param>
	/// <param name="duration">Duration. After duration, text will be empty.</param>
	public void SetDevText (string devText, float duration)
	{
		if (_coroutineSetDevText != null)
			StopCoroutine (_coroutineSetDevText);
		
		_coroutineSetDevText = IESetDevText (devText, duration);
		StartCoroutine (_coroutineSetDevText);
	}

	/// <summary>
	/// Sets the Development Purpose-Only Text.
	/// </summary>
	/// <param name="devText">Text to display</param>
	public void SetDevText (string devText)
	{
		if (_coroutineSetDevText != null)
			StopCoroutine (_coroutineSetDevText);

		if (_devText != null)
			_devText.text = devText;
	}

	IEnumerator IESetDevText (string devText, float duration)
	{
		_devText.text = devText;
		yield return new WaitForSecondsRealtime (duration);
		_devText.text = "";
	}
}
