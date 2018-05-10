using UnityEngine;
using System.Collections;

public class ThirdPersonOrbitCam : MonoBehaviour
{
	public Transform player;
	public Texture2D crosshair;
	
	public Vector3 pivotOffset = new Vector3 (0.0f, 1.0f, 0.0f);
	public Vector3 camOffset = new Vector3 (0.0f, 0.7f, -3.0f);

	public float smooth = 10f;

	public Vector3 aimPivotOffset = new Vector3 (0.0f, 1.7f, -0.3f);
	public Vector3 aimCamOffset = new Vector3 (0.8f, 0.0f, -1.0f);

	public Vector3 overheadAimPivotOffset = new Vector3 (0f, 1.7f, -0.3f);
	public Vector3 overheadAimCamOffset = new Vector3 (0.6f, 0.5f, -1.0f);

	public float horizontalAimingSpeed = 400f;
	public float verticalAimingSpeed = 400f;
	public float maxVerticalAngle = 30f;
	public float flyMaxVerticalAngle = 60f;
	public float minVerticalAngle = -60f;
	
	public float mouseSensitivity = 0.3f;

	public float sprintFOV = 100f;
	
	private PlayerMovementControl playerControl;
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;

	private Vector3 relCameraPos;
	private float relCameraPosMag;
	
	private Vector3 smoothPivotOffset;
	private Vector3 smoothCamOffset;
	private Vector3 targetPivotOffset;
	private Vector3 targetCamOffset;

	private float defaultFOV;
	private float targetFOV;

	void Start ()
	{
		//dev
		if (player != null) {
			Initialize (player);
		}
	}

	public void Initialize (Transform target)
	{
		enabled = true;
		player = target;
		playerControl = player.GetComponent<PlayerMovementControl> ();
		transform.position = new Vector3 (player.position.x, player.position.y + 2, player.position.z - 3);
		transform.LookAt (player);
		cam = transform;

		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;

		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;

		defaultFOV = cam.GetComponent<Camera> ().fieldOfView;
	}

	void LateUpdate ()
	{
		if (!player)// || !GameController.GC.GetIsControlAllowed ())
			return;
		
		angleH += Mathf.Clamp (Input.GetAxis ("Mouse X"), -1, 1) * horizontalAimingSpeed * GameController.GC.MouseSensitivity * Time.deltaTime;
		angleV += Mathf.Clamp (Input.GetAxis ("Mouse Y"), -1, 1) * verticalAimingSpeed * GameController.GC.MouseSensitivity * Time.deltaTime;

		// fly
		if (playerControl.IsFlying) {
			angleV = Mathf.Clamp (angleV, minVerticalAngle, flyMaxVerticalAngle);
		} else {
			angleV = Mathf.Clamp (angleV, minVerticalAngle, maxVerticalAngle);
		}

		Quaternion aimRotation = Quaternion.Euler (-angleV, angleH, 0);
		Quaternion camYRotation = Quaternion.Euler (0, angleH, 0);
		cam.rotation = aimRotation;

		if (playerControl.IsAiming) {
			targetPivotOffset = aimPivotOffset;
			targetCamOffset = aimCamOffset;
			minVerticalAngle = -60f;
		} else if (playerControl.IsOverheadAiming) {
			targetPivotOffset = overheadAimPivotOffset;
			targetCamOffset = overheadAimCamOffset;
			minVerticalAngle = -40f;
		} else {
			targetPivotOffset = pivotOffset;
			targetCamOffset = camOffset;
			minVerticalAngle = -60f;
		}

		if (playerControl.IsSprinting) {
			targetFOV = sprintFOV;
		} else {
			targetFOV = defaultFOV;
		}
		cam.GetComponent<Camera> ().fieldOfView = Mathf.Lerp (cam.GetComponent<Camera> ().fieldOfView, targetFOV, Time.deltaTime);

		// Test for collision
		Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
		Vector3 tempOffset = targetCamOffset;
		for (float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.5f) {
			tempOffset.z = zOffset;
			if (DoubleViewingPosCheck (baseTempPosition + aimRotation * tempOffset) || zOffset == 0) {
				targetCamOffset.z = tempOffset.z;
				break;
			} 
		}

		// fly
		if (playerControl.IsFlying) {
			targetCamOffset.y = 0;
		}

		smoothPivotOffset = Vector3.Lerp (smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp (smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);

		cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

	}

	// concave objects doesn't detect hit from outside, so cast in both directions
	bool DoubleViewingPosCheck (Vector3 checkPos)
	{
		float playerFocusHeight = player.GetComponent<CapsuleCollider> ().height * 0.5f;
		return ViewingPosCheck (checkPos, playerFocusHeight) && ReverseViewingPosCheck (checkPos, playerFocusHeight);
	}

	bool ViewingPosCheck (Vector3 checkPos, float deltaPlayerHeight)
	{
		RaycastHit hit;

		//Duke Im 2016-10-29
		//setting layermast to set that camera collides with environments only
		LayerMask lm = 1 << LayerHelper.DEFAULT;
		lm |= 1 << LayerHelper.WALL;
		
		// If a raycast from the check position to the player hits something...
//		if(Physics.Raycast(checkPos, player.position+(Vector3.up* deltaPlayerHeight) - checkPos, out hit, relCameraPosMag))
		if (Physics.Raycast (checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit, relCameraPosMag, lm)) {
			// ... if it is not the player...
			if (hit.transform.root != player && !hit.transform.GetComponent<Collider> ().isTrigger) {
//				print (hit.transform.name);
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		return true;
	}

	bool ReverseViewingPosCheck (Vector3 checkPos, float deltaPlayerHeight)
	{
		RaycastHit hit;
		//Duke Im 2016-10-29
		//setting layermast to set that camera collides with environments only
		LayerMask lm = 1 << LayerHelper.DEFAULT;
		lm |= 1 << LayerHelper.WALL;

//		if(Physics.Raycast(player.position+(Vector3.up* deltaPlayerHeight), checkPos - player.position, out hit, relCameraPosMag))
		if (Physics.Raycast (player.position + (Vector3.up * deltaPlayerHeight), checkPos - player.position, out hit, relCameraPosMag, lm)) {
			if (hit.transform.root != player && hit.transform.root != transform && !hit.transform.GetComponent<Collider> ().isTrigger) {
//				print ("collided with " + hit.transform.name);
				return false;
			}
		}
		return true;
	}

	// Crosshair
	void OnGUI ()
	{
		float mag = Mathf.Abs ((aimPivotOffset - smoothPivotOffset).magnitude);
		if (playerControl.IsAiming && mag < 0.05f)
			GUI.DrawTexture (new Rect (Screen.width / 2 - (crosshair.width * 0.5f), 
				Screen.height / 2 - (crosshair.height * 0.5f), 
				crosshair.width, crosshair.height), crosshair);
	}

	public float GetVerticalAngle ()
	{
		return angleV;
	}

	public void SetVerticalAngle (float newAngle)
	{
		angleV = newAngle;
	}
}
