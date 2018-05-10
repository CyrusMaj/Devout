using UnityEngine;
using System.Collections;

public class DialogueEventConditional : DialogueEvent {
	[SerializeField] bool _triggerOnStart;//trigger this event when this scripted is loaded

	protected override void Start ()
	{
		base.Start ();

		if (_triggerOnStart)
			register ();
	}
}
