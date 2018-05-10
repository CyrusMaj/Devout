using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UIClassButton : MonoBehaviour {
	[SerializeField] UISelectClass _uisc;
	//Character class that this class represents
	[SerializeField] CHARACTER_CLASS _class;

	[SerializeField] TEAM _team;
	Button _btn;

	void Awake(){
		_btn = GetComponent<Button> ();
	}

	/// <summary>
	/// Gets the character class that this button represent
	/// </summary>
	/// <returns>The character class</returns>
	public CHARACTER_CLASS GetClass(){
		return _class;
	}

	public TEAM GetTeam(){
		return _team;
	}

	/// <summary>
	/// Selects This Character Class
	/// </summary>
	public void SelectClass(){
		if (RoomLevelHelper.PLAYER_CLASS != CHARACTER_CLASS.NULL || RoomLevelHelper.PLAYER_TEAM != TEAM.NULL) {
			RoomLevelHelper.SetRoomCustomPropertyPlayerClass (RoomLevelHelper.PLAYER_CLASS, true, RoomLevelHelper.PLAYER_TEAM);
		} else {
//			print ("NULL");
		}

		//deselect all other buttons(visual)
		_uisc.DeselectAllClassButtons ();

//		//mark as selected(visual only)
//		_btn.image.color = _btn.colors.highlightedColor;

		_uisc.SetSelectedClass (_class, _team);
	}
}
