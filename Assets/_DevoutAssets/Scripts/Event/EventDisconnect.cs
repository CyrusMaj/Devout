using UnityEngine;
using System.Collections;

//Duke Im namespace
namespace DukeIm
{
	/// Written by Duke Im
	/// On 2017-1-8
	/// 
	/// <summary>
	/// Disconnect from Photon
	/// </summary>
	public class EventDisconnect : Event
	{
		public override void StartEvent ()
		{
			base.StartEvent ();

			if(PhotonNetwork.connected)
				PhotonNetwork.Disconnect ();

			EndEvent ();
		}
	}
}
