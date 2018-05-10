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
	/// Trigger depending on remaining enemy count.
	/// </summary>
	public class TriggerEnemyCount : Trigger
	{
		//spawnenr to get pool information
		[SerializeField] EventSpawnNPC _spawner;

		/// <summary>
		/// This or below will satisfy this condition
		/// </summary>
		[SerializeField] int _AIAliveCount = 0;

		#region implemented abstract members of Trigger
		protected override void checkCondition ()
		{
			if (_spawner.TotalEnemiesPool < 1 && AIStatusHandler.Get_PVs (true).Count <= _AIAliveCount) {
				//condition met
//				print ("enemy count < 1");
				triggerNext ();
			} else {
//				print (_spawner.TotalEnemiesPool + ", " + AIStatusHandler.Get_PVs (true).Count);
			}
		}
		#endregion
	}
}