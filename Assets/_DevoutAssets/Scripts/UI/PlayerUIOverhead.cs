using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIOverhead : MonoBehaviour
{

	#region Public Properties

	[Tooltip("Pixel offset from the player target")]
	public Vector3 ScreenOffset = new Vector3(0f,20f,0f);

	[Tooltip("Target Y offset from the player target")]
	public float HeightOffset = 1.8f;

	[Tooltip("UI Text to display Player's Name")]
	public Text PlayerNameText;

	[Tooltip("UI Slider to display Player's Health")]
	public Slider PlayerHealthSlider;

//	private Transform _parentCanvas;

	#endregion

	#region Private Properties

	ObjectStatusHandler _target;

//	float _characterControllerHeight = 0f;

	Transform _targetTransform;

	Renderer _targetRenderer;

	Vector3 _targetPosition;

	#endregion

	#region MonoBehaviour Messages

	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity during early initialization phase
	/// </summary>
	void Awake(){
		if (UIController.SINGLETON != null)
			this.GetComponent<Transform> ().SetParent (UIController.SINGLETON.PlayerUICanvas.transform);
		else
			Debug.LogWarning ("WARNING : UIController not found");

//		PlayerHealthSlider.gameObject
	}

	/// <summary>
	/// MonoBehaviour method called on GameObject by Unity on every frame.
	/// update the health slider to reflect the Player's health
	/// </summary>
	void Update()
	{
		// Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
		if (_target == null || _target.GetHealth() <= 0) {
			Destroy(this.gameObject);
			return;
		}


		// Reflect the Player Health
		if (PlayerHealthSlider != null) {
			PlayerHealthSlider.value = (float)_target.GetHealth() / (float)_target.GetMaxHealth();
		}
	}

	/// <summary>
	/// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution.
	/// In our case since we are following a moving GameObject, we need to proceed after the player was moved during a particular frame.
	/// </summary>
	void LateUpdate () {

		// Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
		if (_targetRenderer!=null) {
			this.gameObject.SetActive(_targetRenderer.isVisible);
		}

		// #Critical
		// Follow the Target GameObject on screen.
		if (_targetTransform!=null)
		{
			_targetPosition = _targetTransform.position;
			_targetPosition.y += HeightOffset;

			if(Camera.main != null)
				this.transform.position = Camera.main.WorldToScreenPoint (_targetPosition) + ScreenOffset;
		}
	}




	#endregion

	#region Public Methods

//	public void ParentCanvas(Transform parent){
//		this.GetComponent<Transform>().SetParent (parent);
//	}

	/// <summary>
	/// Assigns a Player Target to Follow and represent.
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetTarget(Transform target){

		if (target == null) {
			Debug.LogError("<Color=Red><b>Missing</b></Color> PlayerInfo target for PlayerUI.SetTarget.",this);
			return;
		}

		// Cache references for efficiency because we are going to reuse them.
		_target = target.GetComponent<ObjectStatusHandler>();
		_targetTransform = target;
		_targetRenderer = _target.GetComponent<Renderer>();

		if (PlayerNameText != null) {
			if (target.GetComponent<PhotonView> ().owner != null)//owner is null when offlineMode
				PlayerNameText.text = target.GetComponent<PhotonView> ().owner.name;
			else
				PlayerNameText.text = "Offline Player";
		}
	}

	#endregion

}
