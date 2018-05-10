using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DukeIm
{
	public class EventMusicSetSnapshot : Event
	{
		public override void StartEvent ()
		{
//			base.StartEvent ();

//			MusicManager.SINGLETON.SetMusic ();

			StartNext ();

//			EndEvent ();
		}
	}
}
