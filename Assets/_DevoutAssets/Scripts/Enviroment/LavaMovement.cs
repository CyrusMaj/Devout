using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMovement : MonoBehaviour {
	[SerializeField] Transform _downTrans;
	[SerializeField] Transform _midTrans;
	[SerializeField] Transform _upTrans;

	[Space]

	[SerializeField] float _stayTime = 5f;
	[SerializeField] float _stayTimeDifference = 2f;
	[SerializeField] float _moveSpeedInSeconds = 2f;

	float _timer = 0f;

	DIR _currentDirection = DIR.DOWN_TO_MID;

	void Start(){
		if (!PhotonNetwork.isMasterClient) {
			Destroy (this);
		}
			
		_timer = Time.time + _stayTime + Random.Range (-_stayTimeDifference, _stayTimeDifference);
	}

	void Update(){
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		if (Time.time > _timer) {
			StopAllCoroutines ();
			switch (_currentDirection) {
			case DIR.DOWN_TO_MID:
				StartCoroutine (MathHelper.IELerpPositionOverTime (transform, _downTrans.position, _midTrans.position, _moveSpeedInSeconds));
				_currentDirection = DIR.MID_TO_UP;
				break;
			case DIR.MID_TO_UP:
			case DIR.MID_TO_DOWN:
				if (Random.Range (0, 2) > 0) {
					StartCoroutine (MathHelper.IELerpPositionOverTime (transform, _midTrans.position, _downTrans.position, _moveSpeedInSeconds));
					_currentDirection = DIR.DOWN_TO_MID;
				} else {
					StartCoroutine (MathHelper.IELerpPositionOverTime (transform, _midTrans.position, _upTrans.position, _moveSpeedInSeconds));
					_currentDirection = DIR.UP_TO_MID;
				}
				break;
//			case DIR.MID_TO_UP:
//				StartCoroutine (MathHelper.IELerpPositionOverTime (transform, _midTrans.position, _upTrans.position, _moveSpeedInSeconds));
//				_currentDirection = DIR.UP_TO_MID;
//				break;
			case DIR.UP_TO_MID:
				StartCoroutine (MathHelper.IELerpPositionOverTime (transform, _upTrans.position, _midTrans.position, _moveSpeedInSeconds));
				_currentDirection = DIR.MID_TO_DOWN;
				break;
			}
			_timer = Time.time + _stayTime + Random.Range (-_stayTimeDifference, _stayTimeDifference);
		}
	}

	enum DIR{
		DOWN_TO_MID,
		MID_TO_UP,
		UP_TO_MID,
		MID_TO_DOWN,
	}
}
