using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controls Coop Lobby Ui flows such as room selection, creation and other buttons.
/// </summary>
public class CoopLobbyController : MonoBehaviour {
	public static CoopLobbyController CLC;
	[SerializeField] GameObject _coopLobbyUI;
	[SerializeField] GameObject _coopLobbyCreateRoomUI;
	[SerializeField] GameObject _coopLobbySelectClass;
//	[SerializeField] UpdateCoopRoomList _coopRoomList;
	public RoomInfo SelectedRoom{ get; private set; }
	// Use this for initialization
	void Start () {
		if (CLC == null)
			CLC = this;
		else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTORLLER");
	}

	/// <summary>
	/// Toggles the coop lobby window.
	/// Hide / Show the lobby UI
	/// </summary>
	public void SetActiveCoopLobbyWindow(bool newActive){
		_coopLobbyUI.SetActive (newActive);
	}

	/// <summary>
	/// Toggles the coop lobby create room window.
	/// Hide / Show the lobby create room UI
	/// </summary>
	public void SetActiveCoopLobbyCreateRoomWindow(bool newActive){
		_coopLobbyCreateRoomUI.SetActive (newActive);
	}

	/// <summary>
	/// Toggles the coop lobby select character/class window.
	/// Hide / Show the lobby class selction UI
	/// </summary>
	public void SetActiveCoopLobbySelectClass(bool newActive){
		_coopLobbySelectClass.SetActive (newActive);
	}

	/// <summary>
	/// Sets the selected room.
	/// </summary>
	/// <param name="newRoom">New room.</param>
	public void SetSelectedRoom(RoomInfo newRoom){
		SelectedRoom = newRoom;
	}
}
