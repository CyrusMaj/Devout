using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-2-8
	/// 
	/// <summary>
	/// Event that quits game entirely
	/// </summary>
	public class EventQuitApplication : Event
	{

		public override void StartEvent ()
		{
			base.StartEvent ();

			Application.Quit ();

			EndEvent ();
		}
	}
}
