using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Provide customized math functions
/// </summary>
public static class MathHelper {
	public static IEnumerator IELerpVelocityOverTime(Rigidbody lerpTarget, Vector3 velA, Vector3 velB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.velocity = Vector3.Lerp (velA, velB, percentage);
			yield return null;
		}
		lerpTarget.velocity = velB;
	}
	public static IEnumerator IELerpRotationOverTime(Rigidbody lerpTarget, Quaternion rotA, Quaternion rotB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.rotation = Quaternion.Lerp (rotA, rotB, percentage);
			yield return null;
		}
		lerpTarget.rotation = rotB;
	}
	public static IEnumerator IELerpRotationOverTime(Transform lerpTarget, Quaternion rotA, Quaternion rotB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.rotation = Quaternion.Lerp (rotA, rotB, percentage);
			yield return null;
		}
		lerpTarget.rotation = rotB;
	}
	public static IEnumerator IELerpPositionOverTime(Rigidbody lerpTarget, Vector3 posA, Vector3 posB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.position = Vector3.Lerp (posA, posB, percentage);
			yield return null;
		}
		lerpTarget.position = posB;
	}
	public static IEnumerator IELerpPositionOverTime(Transform lerpTarget, Vector3 posA, Vector3 posB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.position = Vector3.Lerp (posA, posB, percentage);
			yield return null;
		}
		lerpTarget.position = posB;
	}
	public static IEnumerator IELerpLocalPositionOverTime(Transform lerpTarget, Vector3 posA, Vector3 posB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.localPosition = Vector3.Lerp (posA, posB, percentage);
			yield return null;
		}
		lerpTarget.localPosition = posB;
	}
	//Using Rigidbody movment instead of moving transform
	public static IEnumerator IELerpRBPositionOverTime(Rigidbody lerpTarget, Vector3 posA, Vector3 posB, float seconds){
		float startTime = Time.time;
		float percentage = Time.time - startTime;
		while (Time.time - startTime <= seconds) {
			percentage = (Time.time - startTime) / seconds;
			lerpTarget.MovePosition(Vector3.Lerp (posA, posB, percentage));
			yield return null;
		}
		lerpTarget.MovePosition(posB);
	}
	/// <summary>
	/// Shuffle(randomize) the specified list.
	/// </summary>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Shuffle<T>(this IList<T> list)  
	{  
		System.Random rng = new System.Random ();
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
