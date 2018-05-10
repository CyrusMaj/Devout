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
	/// A single event that has start to end
	/// Ex. Playing animation, spawining enemies
	/// </summary>
	public abstract class Event : MonoBehaviour
	{
		/// <summary>
		/// Events to be triggered when this event is ended
		/// </summary>
		[SerializeField] List<Event> _nextEvents;

		/// <summary>
		/// Conditions to be checked for trigger when this event is ended
		/// </summary>
		[SerializeField] List<Trigger> _nextTriggers;

		/// <summary>
		/// Current state of this event
		/// </summary>
		/// <value>The state of the event.</value>
		[SerializeField] protected EVENT_STATE _state;

		public void SetState (EVENT_STATE newState)
		{
			_state = newState;
		}
		public EVENT_STATE GetState(){
			return _state;
		}

		/// <summary>
		/// What happens when this event starts
		/// </summary>
		public virtual void StartEvent (){
			if (_state != EVENT_STATE.Available)
				return;
			SetState (EVENT_STATE.InProgress);
		}

		/// <summary>
		/// What happens when this event ends
		/// </summary>
		public virtual void EndEvent (){
			SetState (EVENT_STATE.Ended);
			StartNext ();
		}

		/// <summary>
		/// Starts the next events & triggers.
		/// </summary>
		public void StartNext ()
		{
			foreach (var e in _nextEvents)
				e.StartEvent ();

			foreach (var t in _nextTriggers)
				t.StartConditionCheck ();
		}
	}

	/// <summary>
	/// State of an event
	/// </summary>
	public enum EVENT_STATE
	{
		Available,
		Disabled,
		InProgress,
		Ended,
	}
}
