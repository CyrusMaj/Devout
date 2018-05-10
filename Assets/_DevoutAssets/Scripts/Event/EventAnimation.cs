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
	public class EventAnimation : Event
	{
		[SerializeField] List<Animation> _animations = new List<Animation> ();

		public override void StartEvent ()
		{
			base.StartEvent ();

			float longestAnimTime = 0f;
			foreach (var a in _animations) {
				a.Play ();
				if (longestAnimTime < a.clip.length)
					longestAnimTime = a.clip.length;
			}
			Invoke ("EndEvent", longestAnimTime);
		}
	}
}