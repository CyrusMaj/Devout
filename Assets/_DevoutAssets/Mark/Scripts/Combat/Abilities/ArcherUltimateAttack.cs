using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUltimateAttack : Ability {

	[SerializeField] PlayerMovementControl _player;
	[SerializeField] Transform shootingPoint;
//	[SerializeField] Transform targetTransform;
	[SerializeField] GameObject targetPrefab;
	[SerializeField] GameObject projectilePrefab;
	[SerializeField] LineRenderer lineR;

	private GameObject target;
	private GameObject projectile;
	private float playerHeight;
	private ThirdPersonOrbitCam tpoc;
	private bool isOn;

	// Use this for initialization
	protected override void Start () {
		isOn = false;
		playerHeight = 1.6f;
		tpoc = CameraController.CC.CombatCamera.GetComponent<ThirdPersonOrbitCam> ();
		base.Start ();
	}

	/// <summary>
	/// Toggle aim
	/// </summary>
	public override void Activate ()
	{
		isOn = !isOn;
		if (isOn) {
			_player.IsOverheadAiming = true;
//			SetStatus (ABILITY_STATUS.IN_USE);
//			_player.IsAiming = true;
			target = (GameObject)Instantiate (targetPrefab.gameObject);
//			tpoc.SetVerticalAngle (-30f);
//			target.transform.position = GetTargetPos ();
		} else {
			//discharge (fire)
//			List<Vector3> pts = ShowTrajectory();
//			projectile = (GameObject)Instantiate (projectilePrefab.gameObject);
//			ThrowProjectile (projectile, pts, Time.deltaTime);

			_player.IsOverheadAiming = false;
//			SetStatus (ABILITY_STATUS.AVAILABLE);
//			_player.IsAiming = false;
			Destroy (target.gameObject);
		}

		base.Activate ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isOn) {
//			target.transform.position = GetTargetPos ();
			ShowTrajectory();
			if (Input.GetMouseButtonDown (0)) {
				//should toggle off
				Activate ();
			}
		} else {
			lineR.positionCount = 0;
		}
	}

	//Throw projectile over last projected arc
	private void ThrowProjectile(GameObject proj, List<Vector3> pts, float dt)
	{
		if (pts.Count > 0) {
			proj.transform.position = pts [0];

		}
	}


	private Vector3 GetTargetPos()
	{
//		Vector3 targetPos = shootingPoint.transform.position;
//		targetPos += shootingPoint.transform.forward;
//		Vector3 targetPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
//		float angle = tpoc.GetVerticalAngle();
//		float deltaX = (angle / 30) * 5;
//		Vector3 targetPos = target.transform.position;
//		targetPos.x += deltaX;
//		targetPos.y -= playerHeight;
		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL | 1 << LayerHelper.NEAUTRAL | 1 << LayerHelper.DEFAULT;
		RaycastHit hit;

		if (Physics.Raycast (Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f)), out hit, 10f, lm)) {
			Debug.Log ("Raycast HIT");
		} else {
			Debug.Log ("Raycast did NOT hit");
			Vector3 targetPos = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.5f)).GetPoint(10);
//			targetPos.y -= playerHeight;
			hit.point = targetPos;
		}

		return hit.point;
	}

	private List<Vector3> ShowTrajectory()
	{
		List<RaycastHit2D> hits = new List<RaycastHit2D> ();
		List<Vector3> points = new List<Vector3> ();

		LayerMask lm = 1 << LayerHelper.FLOOR | 1 << LayerHelper.WALL | 1 << LayerHelper.NEAUTRAL | 1 << LayerHelper.DEFAULT;

		Transform st = _player.transform;
//		st.rotation = Quaternion.LookRotation (st.up);
//		st.Rotate (new Vector3 (90f, 0f, 0f));

		Vector3 start = st.position;
		start.y += (playerHeight * 0.6f);
		Vector3 velocity = st.forward + st.up;

		float angleFactor = VerticalAngleVelocity (tpoc.GetVerticalAngle ());
		velocity.Scale (new Vector3 (0.05f, 0.05f * angleFactor, 0.05f));
		velocity.Normalize ();
		Vector3 accel = velocity.normalized;
		accel.Scale(new Vector3 (0f, -0.2f, 0f));

		GetArcHits (out hits, out points, lm.value, start, velocity, accel, 0.1f, 25f, false, true);
		lineR.positionCount = points.Count;
		lineR.SetPositions (points.ToArray());
		target.transform.position = lineR.GetPosition (lineR.positionCount - 1);
		return points;
	}

	private float VerticalAngleVelocity(float vAngle)
	{
		//vertical aiming with mouse has angle range of +30:-60
		//vertical velocity component will always be positive
		vAngle += 60;
		return 1f + (vAngle / 30);
	}

	public void GetArcHits( out List<RaycastHit2D> Hits, out List<Vector3> Points, int iLayerMask, Vector3 vStart, Vector3 vVelocity, Vector3 vAcceleration, float fTimeStep = 0.05f, float fMaxtime = 10f, bool bIncludeUnits = false, bool bDebugDraw = false )
	{
		Hits = new List<RaycastHit2D>();
		Points = new List<Vector3>();
		RaycastHit realHit;
		Vector3 prev = vStart;
		Points.Add( vStart );

		for ( int i = 1; ; i++ )
		{
			float t = fTimeStep*i;
			if ( t > fMaxtime ) break;
			Vector3 pos = PlotTrajectoryAtTime (vStart, vVelocity, vAcceleration, t);

			var result = Physics2D.Linecast (prev,pos, iLayerMask);
//			if ( result.collider != null )
			if (Physics.Raycast (prev, pos, out realHit, 20f, iLayerMask))
			{
				Hits.Add( result );
				Points.Add( pos );
				break;
			}
			else
			{
				//save fewer points for graphics
				if (i % 5 == 0) {
					Points.Add( pos );
				}
			}
//			Debug.DrawLine( prev, pos, Color.Lerp( Color.yellow, Color.red, 0.35f ), 0.5f );

			prev = pos;
		}
	}

	public Vector3 PlotTrajectoryAtTime (Vector3 start, Vector3 startVelocity, Vector3 acceleration, float fTimeSinceStart)
	{
		return start + startVelocity*fTimeSinceStart + acceleration*fTimeSinceStart*fTimeSinceStart*0.5f;
	}
}
