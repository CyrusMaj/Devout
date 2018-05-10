using UnityEngine;
using System.Collections;

/// <summary>
/// User interface for player interaction.
/// When near interactable object / situation, this will show to tell player what to do(ex. press 'F' to jump)
/// Each client should have only one of this instance
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class UIPlayerInteraction : MonoBehaviour {
	/// <summary>
	/// TextMesh component of this gameobject
	/// </summary>
	[SerializeField] TextMesh _tm;

	/// <summary>
	/// The saved text.
	/// Text is emptied when disabled
	/// Text is saved for when enabled
	/// </summary>
	string _savedText;

	/// <summary>
	/// The target which this UI will hover over
	/// </summary>
	public Transform Target{get; private set;}

	/// <summary>
	/// Gets a value indicating whether this <see cref="UIPlayerInteraction"/> is enabled.
	/// </summary>
	/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
	public bool Enabled{ get; private set;}
	// Use this for initialization
	void Start () {
//		_tm = GetComponent<TextMesh> ();
		_savedText = _tm.text;
		SetEnabled (false);

//		//dev
//		Enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Enabled) {
			//move this 2 meters above target object
			if (Target != null) {
				transform.position = new Vector3 (Target.position.x, Target.position.y + 2f, Target.position.z);
			}

			//face the camera
			transform.rotation = Quaternion.LookRotation (transform.position - CameraController.CC.CombatCamera.transform.position);
		}
	}

	public void SetText(string newText){
		_savedText = newText;
		_tm.text = _savedText;
//		print ("Set : " + newText);
	}

	public void SetEnabled(bool newEnabled){
		Enabled = newEnabled;
		if (Enabled) {
			_tm.text = _savedText;
		} else {
//			print ("b : " + _savedText + ", tm : " + _tm.text);
//			_savedText = _tm.text;
//			print ("m : " + _savedText + ", tm : " + _tm.text);
			_tm.text = "";
//			print ("a : " + _savedText + ", tm : " + _tm.text);
		}
	}

	public void SetTarget(Transform newTarget){
		Target = newTarget;
	}
}
