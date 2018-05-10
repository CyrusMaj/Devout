using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Provide common customzied coroutine functions
/// </summary>
public static class CoroutineHelper{
	public static IEnumerator IEChangeAnimBool(Animator animator, int animHash, bool boolTo,  float seconds){
		float timer = Time.time + seconds;
		while (timer > Time.time) {
			animator.SetBool (animHash, !boolTo);
			yield return null;
		}
//		yield return new WaitForSeconds(seconds);
		animator.SetBool (animHash, boolTo);
	}
	//ex.
	//coroutineWaitForCombo = CoroutineHelper.IEChangeBool ((x) => _comboReady1 = x, true, _comboWait1);
	public static IEnumerator IEChangeBool(System.Action<bool> boolFrom, bool boolTo, float waitSeconds){
		yield return new WaitForSeconds (waitSeconds);
		boolFrom (boolTo);
	}
	public static IEnumerator IEChangeFloat(System.Action<float> floatFrom, float floatTo, float waitSeconds){
		yield return new WaitForSeconds (waitSeconds);
		floatFrom (floatTo);
	}
	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}

	public static IEnumerator IELoadAsyncScene (string sceneName)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneName);
//		Debug.Log ("Scene loading progress : " + (int)operation.progress);
		yield return operation;
	}
}
