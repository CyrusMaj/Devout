using UnityEngine;
using System.Collections;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-8
	/// 
	/// <summary>
	/// Triggers when something collides with this trigger collider
	/// </summary>
	[RequireComponent (typeof(Collider))]
	public class TriggerOnTrigger : Trigger
	{

		Collider _collider;

		protected override void Start ()
		{
			_collider = GetComponent<Collider> ();
			_collider.isTrigger = true;
			//disable until this trigger starts checking
			_collider.enabled = false;

			base.Start ();
		}

		public override void StartConditionCheck ()
		{
			if (_state != EVENT_STATE.Available)
				return;
			SetState (EVENT_STATE.InProgress);

			startTimeoutCheck ();

			_collider.enabled = true;
		}

		#region implemented abstract members of Trigger

		protected override void checkCondition ()
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		void OnTriggerEnter (Collider other)
		{
			if (other.GetComponent<PlayerCharacterStatusHandler> () && other.GetComponent<PhotonView> () && other.GetComponent<PhotonView> ().isMine) {
				triggerNext ();
				_collider.enabled = false;
			}
		}
	}
}
