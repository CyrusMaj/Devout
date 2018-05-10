using UnityEngine;
using System.Collections;

public class TrampActivator : MonoBehaviour {

	public Collider CharCollider;
	public bool ActivateGO = false;
	public GameObject GOEffect;
	public Animation AnimatorEffect;
	public AnimationClip AnimPlay;
	public AnimationClip AnimReset;
	public float TimeToReset;
	private float timer;
	private bool collide = false;

void	OnTriggerEnter(Collider col)
	{
		if(col.GetComponent<Collider>() == CharCollider)
		{
			AnimatorEffect.Play(AnimPlay.name);
			timer = 0f;
			if (!collide)
			{
			collide = true;
			Debug.Log("TrampActive");
			}
			if (ActivateGO)
			{
				GOEffect.SetActive(true);
			}
		}
	}
void Update()
	{
		if (collide) 
		{
			timer = timer + Time.fixedDeltaTime;
			if (timer > TimeToReset)
			{
				AnimatorEffect.Play(AnimReset.name);
				GOEffect.SetActive(false);
				timer=0f;
				collide=false;
			}

		}
	}
}
