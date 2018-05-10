using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-7
	/// 
	/// <summary>
	/// Event that plays animation(s)
	/// </summary>
	public class EventMouseLock : Event
	{
		[SerializeField] bool Locked;

		public override void StartEvent ()
		{
			base.StartEvent ();

			if (Locked) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			EndEvent ();
		}
	}
}