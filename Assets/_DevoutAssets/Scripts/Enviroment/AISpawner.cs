using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Written by Duke Im
/// On 2016-12-11
/// 
/// <summary>
/// Base class for Spawning points.
/// </summary>
public abstract class NPCSpawner : MonoBehaviour
{
	/// <summary>
	/// Spawn AIs
	/// </summary>
	public abstract void Spawn(int count);

	/// <summary>
	/// Is this spawner ready to spawn?
	/// </summary>
	/// <value><c>true</c> if this instance is available; otherwise, <c>false</c>.</value>
	public bool IsAvailable{ get; protected set; }
}
