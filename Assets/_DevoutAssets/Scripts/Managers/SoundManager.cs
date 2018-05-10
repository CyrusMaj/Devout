using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// Written by Duke Im
/// On 2017-1-12
/// 
/// <summary>
/// Manages sound(effects, not music) of the game
/// SINGLETON
/// </summary>
public class SoundManager : Photon.PunBehaviour {
	public static SoundManager SINGLETON;

	/// <summary>
	/// Hit sound effects
	/// </summary>
	public List<AudioClips> HitSound = new List<AudioClips>();
	/// <summary>
	/// Blocked sound effects
	/// </summary>
	public List<AudioClips> BlockedSound = new List<AudioClips>();

	// Use this for initialization
	void Start () {
		if(!SINGLETON) SINGLETON = this;
		else if(SINGLETON != this) Destroy(gameObject);

		//check if there are duplicated audio types
		if (HitSound.Select (x => x.Type)
			.GroupBy (y => y)
			.Where (z => z.Count () > 1)
			.Select (z => z.Key).Count () > 0)
			Debug.Log ("Warning : There are duplicated audio types");
		if (BlockedSound.Select (x => x.Type)
			.GroupBy (y => y)
			.Where (z => z.Count () > 1)
			.Select (z => z.Key).Count () > 0)
			Debug.Log ("Warning : There are duplicated audio types");
	}

	public void PlayHitSound(Weapon.TYPE weaponType, HitResult hitResult, Vector3 position){
		if (PhotonNetwork.offlineMode)
			playHitSound (weaponType, hitResult, position);
		else
			NetworkManager.SINGLETON.Targeted_RPC (photonView, "RPCPlayHitSound", weaponType, hitResult, position);
	}
	void playHitSound(Weapon.TYPE weaponType, HitResult hitResult, Vector3 position){
//		print ("playing hit sound");

		AudioClip clip;
		List<AudioClips> soundClips = null;
		if (hitResult == HitResult.Hit)
			soundClips = HitSound;
		else if (hitResult == HitResult.Blocked)
			soundClips = BlockedSound;

		if (soundClips == null) {
			Debug.Log ("Warning : no matching soundclip found");
			return;
		}

		//get a random clip from the selection
		clip = soundClips.Where (x => x.Type == weaponType).ElementAt (0).Clips
			.ElementAt (Random.Range (0, soundClips.Where (x => x.Type == weaponType).ElementAt (0).Clips.Count));

		AudioSource.PlayClipAtPoint (clip, position);
	}
	[PunRPC]
	void RPCPlayHitSound(Weapon.TYPE weaponType, HitResult hitResult, Vector3 position){
		playHitSound (weaponType, hitResult, position);
	}

//	/// <summary>
//	/// Hit sound
//	/// Ex. when sword hits an enemy body or shield
//	/// </summary>
//	public enum WEAPON_TYPE{
//		//categorized by weapon types
//		SWORD,
//		AXE,
//		ARROW,
//		MACE,
//	}

	/// <summary>
	/// Clips of audio(group of sound files used for the same effect)
	/// </summary>
	[System.Serializable]
	public class AudioClips{
		public Weapon.TYPE Type;
		public List<AudioClip> Clips = new List<AudioClip>();
	}

	public enum HitResult{
		Hit,
		Blocked,
	}
}
