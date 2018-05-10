using UnityEngine;
using System.Collections;

public class DevKillZone : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (!PhotonNetwork.isMasterClient || other.gameObject.isStatic)
			return;
		
		if (other.GetComponent<ObjectStatusHandler> ()) {			
			//kill anything that enters and if mine
			if (PhotonView.Get (other)) {
//				print ("Lava Kill");
				other.GetComponent<ObjectStatusHandler> ().SetHealth (0);
			}
		}

		if(other.GetComponent<MovingObject>() == null)
			ParticleController.PC.InstantiateParticle (ParticleController.PARTICLE_TYPE.DEATH_LAVA, other.transform.position + new Vector3(0f, 2f, 0f));
//		other.enabled = false;
	}
}
