using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof(RectTransform))]
public class UIRoomList : MonoBehaviour
{
	RectTransform _rectTransform;
	public Transform RoomsParent;
	[SerializeField] GameObject _roomPrefab;
	[SerializeField] Text _logText;
	[SerializeField] ROOM_TYPE _roomType;

	void Awake ()
	{
		_rectTransform = GetComponent<RectTransform> ();

		//clear everything in the room list before
//		foreach (Transform child in RoomsParent) {
//			Destroy (child.gameObject);
		//		}
		InvokeRepeating ("updateRooms", 0f, 0.25f);
		
	}
	//
	//	void Start ()
	//	{
	//		InvokeRepeating ("updateRooms", 0f, 1.25f);
	//	}

	void updateRooms ()
	{
		//clear everything in the room list before
//		foreach (Transform child in RoomsParent) {
//			Destroy (child.gameObject);
//		}

		//update room button size from grid layout so it fits to dynamic parent size
		GridLayoutGroup glg = RoomsParent.GetComponent<GridLayoutGroup> ();
		glg.cellSize = new Vector2 (_rectTransform.rect.width - 60f, glg.cellSize.y);

		//get room list, instantiate & fill the list
		if (PhotonNetwork.connected) {
			List<RoomInfo> rooms = PhotonNetwork.GetRoomList ().ToList ();
			List<UIRoomListElement> currentRooms = RoomsParent.GetComponentsInChildren<UIRoomListElement> ().ToList ();

			bool deleted = false;
			//delete all rooms with empty roominfo
			foreach (var r in currentRooms.Where(x=>x.RoomInfo == null)) {
				Destroy (r.gameObject);
				deleted = true;
			}
			if (deleted)
				return;

			//delete all rooms that doesn't exist
			foreach (var n in currentRooms.Select(x=>x.RoomInfo.Name).Except(rooms.Select(x=>x.Name))) {
				Destroy (currentRooms.First (x => x.RoomInfo.Name == n).gameObject);
				deleted = true;
			}
			if (deleted)
				return;

			foreach (var room in rooms) {
				//check room type
				ROOM_TYPE rt = (ROOM_TYPE)room.customProperties [RoomLevelHelper.CUSTOM_ROOM_PROPERTY_ROOM_TYPE];
				if (rt != _roomType)
					continue;

				GameObject instance;

				if (currentRooms.Select (x => x.RoomInfo.Name).Contains (room.Name)) {
					//if already exsists, don't instantiate
					instance = currentRooms.First (x => x.RoomInfo.Name == room.Name).gameObject;
				} else {
					instance = Instantiate (_roomPrefab);
				}
				
//				GameObject instance = Instantiate (_roomPrefab);
				instance.transform.SetParent (RoomsParent);

				UIRoomListElement roomElement = instance.GetComponent<UIRoomListElement> ();
				roomElement.ButtonText.text = "(" + room.playerCount + "/" + room.maxPlayers + ") " + room.name;
				roomElement.SetRoomInfo (room);
				roomElement.UIRL = this;

				Button btn = instance.GetComponent<Button> ();
				//disable button is room is full
				if (room.playerCount >= room.maxPlayers) {
					btn.interactable = false;
				}
			}
			_logText.text = "Room list updated";
		} else {
			Debug.Log ("Not connected to Photon Server");
		}
	}

	/// <summary>
	/// Deselects all rooms.
	/// This only changes visual(color) of the room buttons
	/// </summary>
	public void DeselectAllRooms ()
	{
		foreach (Transform child in RoomsParent) {
			Button btn = child.GetComponent<Button> ();
			btn.image.color = btn.colors.normalColor;
		}
	}

	/// <summary>
	/// Update list of rooms using invoke repeating
	/// </summary>
	/// <param name="repeatRate">Repeat rate.</param>
	public void UpdateRooms (/*float repeatRate = 0.5f*/)
	{
		//update room list
//		InvokeRepeating("updateRooms",0.5f, repeatRate);
//		updateRooms ();
	}

	/// <summary>
	/// Cancels all invoke from this component
	/// </summary>
	public void StopUpdate ()
	{
		CancelInvoke ();
	}
}
