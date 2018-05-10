using UnityEngine;
using System.Collections;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-8
	/// 
	/// <summary>
	/// Load(jump to) a scene
	/// </summary>
	public class EventLoadScene : Event
	{
		/// <summary>
		/// Name of the scene to load
		/// </summary>
		[SerializeField] string _sceneName;

		public override void StartEvent ()
		{
			base.StartEvent ();

			if(PhotonNetwork.connected)
				PhotonNetwork.Disconnect ();
			StartCoroutine (CoroutineHelper.IELoadAsyncScene (_sceneName));
//			Cursor.lockState = CursorLockMode.None;

			EndEvent ();
		}
	}
}