using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Collider))]
public class DialogueEventLocational : DialogueEvent {
	Collider _collider;
	[SerializeField] bool _triggerOnce = true;//should this event be triggered only once?
	bool _triggered = false;
	protected override void Start ()
	{
		base.Start ();
		_collider = GetComponent<Collider> ();
		_collider.isTrigger = true;
	}
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerMovementControl> () != null) {
			if (!(_triggered && _triggerOnce)) {
				if (!_triggerOnce) {//reset event for multiple triggering
					reset();
				}
//				print ("TRIGGERED");
				register ();
				_triggered = true;
			}
		}
	}
	protected override void checkCmd ()
	{
		base.checkCmd ();
	}
}
