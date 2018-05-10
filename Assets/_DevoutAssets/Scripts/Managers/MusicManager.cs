using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DukeIm
{

	/// Written by Duke Im
	/// On 2017-1-12
	/// 
	/// <summary>
	/// Manages music of the game
	/// SINGLETON
	/// </summary>
	public class MusicManager : MonoBehaviour
	{
		public static MusicManager SINGLETON;

		public AudioSource MainMenu;
		public AudioSource InCombat;
		public AudioSource RoundStart;

		public AudioMixerSnapshot SS_MainMenu;
		public AudioMixerSnapshot SS_InCombat;
		public AudioMixerSnapshot SS_RoundStart;

		/// <summary>
		/// Status of combat which requires different music when it changes
		/// </summary>
//		public COMBAT_STATUS CombatStatus;

		/// <summary>
		/// Minimum playtime for music
		/// </summary>
//		[SerializeField] float _minPlayTime = 10;

//		public float GetMinPlayTime ()
//		{
//			return _minPlayTime;
//		}

		//for transitions
//		public float BPM = 128;
//		private float _transitionIn;
//		private float _transitionOut;
//		private float _quarterNote;


		// Use this for initialization
		void Awake ()
		{
			if (!SINGLETON)
				SINGLETON = this;
			else if (SINGLETON != this)
				Destroy (gameObject);



//			_quarterNote = 60 / BPM;
//			_transitionIn = _quarterNote * 8;
//			_transitionOut = _quarterNote * 16;
		}

		/// <summary>
		/// Sets background music to fit the current state
		/// </summary>
		public void SetMusic (COMBAT_STATUS status)
		{
			switch (status) {
			case COMBAT_STATUS.IN_COMBAT:
				//dev
				SS_InCombat.TransitionTo (1f);
				break;
			case COMBAT_STATUS.MAIN_MENU:
				SS_MainMenu.TransitionTo (1f);
				break;
			case COMBAT_STATUS.ROUND_COUNTDOWN:
				SS_RoundStart.TransitionTo (0f);
				break;
			default :
				break;
			}
		}
		public enum COMBAT_STATUS{
			IN_COMBAT,
			MAIN_MENU,
			ROUND_COUNTDOWN,
		}
	}
}
