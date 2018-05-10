using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
	public static GameController GC;

	[Range (0, 1f)]
	[HideInInspector] public float MouseSensitivity = 1f;

	//

	//	public const int MAX_NUM_PLAYERS = 4;
	//	List<PlayerInfo> _players = new List<PlayerInfo> ();

	/// <summary>
	/// How many times does the current player character died so far
	/// </summary>
	/// <value>The death count.</value>
	public int DeathCount{ get; private set; }

	public void AddDeathCount (int amount)
	{
		DeathCount += amount;
	}

	public int Lives{ get; private set; }

	public void SetLives (int newLives)
	{
		Lives = newLives;
	}

	public bool IsGameOver{ get; private set; }

	public  void GameOver ()
	{
		IsGameOver = true;
	}

	//	public Camera mainCamera;

	//	[HideInInspector]
	public Transform CurrentPlayerCharacter;

	bool _isControlAllowed = false;

	public bool LimitFPSto60 = true;

	public bool Dev;

	void Awake ()
	{		
		if (GC == null)
			GC = this;
		else
			print ("WARNING : Controller loaded twice");

		if (LimitFPSto60)
			Application.targetFrameRate = 60;

		DeathCount = 0;
		IsGameOver = false;
		Lives = 1;

		if (Dev)
			SetIsControlAllowed (true, false);

//		SetIsControlAllowed (false);
//		SetIsControlAllowed (false, false);
	}

	public void SetIsControlAllowed (bool isAllowed, bool isCursorVisible = true)
	{
		_isControlAllowed = isAllowed;
		if (isAllowed) {//player moving camera/character. Lock cursor and hide it
//			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		} else {//show cursor and unlock it
//			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		if (isCursorVisible) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;	
//			print ("Visibility set to false");
//			Debug.Break ();
		}
		//dev
//		print("Last SetIsControl chagnes, Cursor.lockState = " + Cursor.lockState.ToString());
	}

	public bool GetIsControlAllowed ()
	{
		return _isControlAllowed;
	}

	//	void OnApplicationFocus(bool hasFocus){
	//		if (hasFocus) {
	//			Cursor.lockState = CursorLockMode.Locked;
	//		}
	//	}
}
