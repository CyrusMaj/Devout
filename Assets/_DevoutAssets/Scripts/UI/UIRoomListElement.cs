using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// What fills room list in the lobby scene, represents a single room and clickable by button UI
/// </summary>
[RequireComponent(typeof(Button))]
public class UIRoomListElement : MonoBehaviour {
	[HideInInspector] public UIRoomList UIRL;
	public Text ButtonText;
	public RoomInfo RoomInfo{ get; private set;}
	Button _btn;

	void Start(){
		_btn = GetComponent<Button> ();

//		RoomInfo = null;
	}

	/// <summary>
	/// Sets the room info, used when this is initialized
	/// </summary>
	/// <param name="newRoomInfo">New room info.</param>
	public void SetRoomInfo(RoomInfo newRoomInfo){
		RoomInfo = newRoomInfo;
	}

	/// <summary>
	/// Sets the selected room of the controller
	/// </summary>
	public void SetSelectedRoom(){
		ROOM_TYPE rt = (ROOM_TYPE)RoomInfo.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE];
		if (rt == ROOM_TYPE.COOP)
			CoopLobbyController.CLC.SetSelectedRoom (RoomInfo);
		else
			PvPLobbyManager.SINGLETON.SetSelectedRoom (RoomInfo);

		//toggle-like look
		_btn.image.color = _btn.colors.pressedColor;

		UIRL.DeselectAllRooms ();
	}
}
