using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Duke Im namespace
namespace DukeIm
{
	public class TriggerOnStart : Trigger
	{
		#region implemented abstract members of Trigger

		protected override void checkCondition ()
		{
		}

		#endregion

		protected override void Start ()
		{
			triggerNext (true);
		}
	}
}
