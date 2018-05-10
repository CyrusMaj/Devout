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
	/// Conditions that triggers an event or another condition when satisfied
	/// Ex. Start an event when all enemies are defeated
	/// </summary>
	public abstract class Trigger : MonoBehaviour
	{
		/// <summary>
		/// Should this trigger start checking for condition when scene starts?
		/// </summary>
		[SerializeField] protected bool _checkOnStart = false;

		/// <summary>
		/// Time out for this trigger to be self disable
		/// </summary>
		[SerializeField] protected float _timeout;
		/// <summary>
		/// timer for timeout, infinite when set to 0
		/// </summary>
		float _timer = 0f;

		/// <summary>
		/// Events to be triggered when condition is satisfied
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

		protected virtual void Start(){
			if (_checkOnStart)
				StartConditionCheck ();
		}

		public void SetState (EVENT_STATE newState)
		{
			_state = newState;

			switch (_state) {
			case EVENT_STATE.Disabled:
			case EVENT_STATE.Ended:
				CancelInvoke ();
				break;
			default : 
				break;
			}
		}

		public EVENT_STATE GetState(){
			return _state;
		}

		public virtual void StartConditionCheck()
		{
			if (_state != EVENT_STATE.Available)
				return;
			SetState (EVENT_STATE.InProgress);

			startTimeoutCheck ();

			InvokeRepeating ("checkCondition", 0.5f, 0.5f);
		}

		protected void startTimeoutCheck(){
			_timer = Time.time + _timeout;
			if(_timeout != 0f)
				InvokeRepeating ("checkTimeout", 0.5f, 0.5f);
		}

		protected abstract void checkCondition ();

		protected void triggerNext(bool endState = true){
			if(endState)
				SetState (EVENT_STATE.Ended);

			foreach (var e in _nextEvents)
				e.StartEvent ();

			foreach (var t in _nextTriggers)
				t.StartConditionCheck ();
		}

		protected void checkTimeout ()
		{
			if (Time.time > _timer)
				SetState (EVENT_STATE.Ended);
		}
	}
}