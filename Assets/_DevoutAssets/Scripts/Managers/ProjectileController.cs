using UnityEngine;
using System.Collections;

/// Written by Duke Im
/// On 2016-10-06
///
/// <summary>
/// Projectile controller.
/// Handles instantiation of projectiles.
/// Handlers RPC call of instantiation if connected on network.
/// </summary>
[RequireComponent (typeof(PhotonView))]
public class ProjectileController : MonoBehaviour
{
	public static ProjectileController PC;
	PhotonView _pv;
	[SerializeField] Weapon _arrowPrefab;
	[SerializeField] Weapon _ropeArrowPrefab;

	void Start ()
	{		
		if (PC == null)
			PC = this;
		else
			Debug.LogWarning ("WARNING : MORE THAN ONE CONTROLLER");
		
		_pv = PhotonView.Get (this);

		if (!_arrowPrefab)
			Debug.LogWarning ("WARNING : arrow prefab shouldn't be empty");
			
	}

	public void InstantiateProjectile (Vector3 position, Quaternion rotation, int photonViewID, int damage, TYPE type)
	{
		if (PhotonNetwork.offlineMode)
			instantiateProjectile (position, rotation, photonViewID, damage, type);
		else {
//			return instantiateParticle (position, photonViewID);
			_pv.RPC ("RPCInstantiateProjectile", PhotonTargets.All, position, rotation, photonViewID, damage, type);
		}
	}

	[PunRPC]
	void RPCInstantiateProjectile (Vector3 position, Quaternion rotation, int photonViewID, int damage, TYPE type)
	{
		instantiateProjectile (position, rotation, photonViewID, damage, type);
	}

	void instantiateProjectile (Vector3 position, Quaternion rotation, int photonViewID, int damage, TYPE type)
	{
		GameObject projectileInstance;// = (GameObject)Instantiate (_arrowPrefab.gameObject);
		if (type == TYPE.ARROW_ROPE) 
			projectileInstance = (GameObject)Instantiate (_ropeArrowPrefab.gameObject);
		else
			projectileInstance = (GameObject)Instantiate (_arrowPrefab.gameObject);

		projectileInstance.transform.position = position;

//		print (position.ToString ());
//		Debug.Break ();

		projectileInstance.transform.rotation = rotation;
		PhotonView pv = PhotonView.Find (photonViewID);
		CombatHandler ch = pv.GetComponent<CombatHandler> ();

		Weapon weapon = projectileInstance.GetComponent<Weapon> ();
		weapon.SetOwner (ch);

		Arrow arrow = weapon.GetDamagingPoints()[0].GetComponent<Arrow>();

		//
		arrow.gameObject.layer = LayerHelper.GetAttackBoxLayer (ch.GetTeam ());

		arrow.SetDamagableLayers ();
		arrow.GetComponent<Collider> ().enabled = true;

		if (pv.isMine) {
			arrow.SetIsMine (true);
			weapon.SetDamage (damage);
		} else
			arrow.SetIsMine (false);



		#if UNITY_EDITOR

//		CameraController.CC.DevCutsceneCam();
//		CameraController.CC.CutsceneCamera.transform.SetParent(projectileInstance.transform);
//		CameraController.CC.CutsceneCamera.transform.localPosition = new Vector3(0.075f,0.04f,-0.3f);
//		CameraController.CC.CutsceneCamera.transform.localRotation = Quaternion.identity;

		#endif
	}

	public enum TYPE{
		ARROW_NORMAL,
		ARROW_ROPE,
	}

	//	IEnumerator IEInstantiateParticle (Vector3 position, Vector3 angle)
	//	{
	//		if (_arrowPrefab) {
	//			GameObject particleInstance = (GameObject)Instantiate (_arrowPrefab.gameObject);
	//			particleInstance.transform.position = position;
	//			particleInstance.transform.eulerAngles = angle;
	//		}
	//	}
}
