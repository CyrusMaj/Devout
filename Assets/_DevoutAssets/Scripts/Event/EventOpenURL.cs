using UnityEngine;
using System.Collections;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-8
	/// 
	/// <summary>
	/// Open URL link
	/// </summary>
	public class EventOpenURL : Event
	{
		/// <summary>
		/// URL link to open
		/// </summary>
		[SerializeField] string _urlToOpen;

		public override void StartEvent ()
		{
			base.StartEvent ();

			Application.OpenURL (_urlToOpen);

			EndEvent ();
		}
	}
}
