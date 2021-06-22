using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] GameObject targetObject;
	[SerializeField] Vector3 startPosition;
	[SerializeField] Quaternion startRotation;

	[Header("Normal Way")]
	private Transform target; // Self explanatory
	public float distance = 5.5f; // Standard distance to follow object
	public float height = 4.5f; // The height of the camera

	public float lookAtDown = 20.0f;
	public float positionSmoothTime = 0.05f;
	public float ZPositionTime = 5f;
	public float XRotationTime = 0.2f;
	public float rotationSmoothTime = 0.2f;
	public float distanceSnapTime;

	[Header("Ramp Slide Way")]
	public float rampDistance = 5.5f; // Standard distance to follow object
	public float rampHeight = 4.5f; // The height of the camera
	public float rampLookHeight = 10f; // The look height of the follow object
	public float rampLookSmoothTime = 2f;

	[HideInInspector] public bool isReverseCamera = false;
	[HideInInspector] public bool isInCinematic = true;
	[HideInInspector] public bool isLookAtPlayer = false;
	[HideInInspector] public bool isRampSlidingMode = false;

	private float usedDistance;

	Vector3 wantedPosition;
	Vector3 currentPosition;

	//private float posX;
	private float _distance; // Standard distance to follow object
	private float _height; // The height of the camera
	private float _coreDistance;
	private float _coreHeight;
	private float posY;
	private float currentLookAtDown;
	private float zVelocity = 0.0F;
	private Vector3 xyzVelocity = new Vector3(0, 0, 0);

	void Awake()
	{
		target = targetObject.transform;
		currentLookAtDown = lookAtDown;
		_coreDistance = distance;
		_coreHeight = height;
	}

	void LateUpdate()
	{
		if (isInCinematic)
			return;

		FollowPlayerStraightWay();
	}

	private void FollowPlayerStraightWay()
    {
		_distance = Mathf.Lerp(_distance, (isRampSlidingMode ? rampDistance : distance), Time.deltaTime * rampLookSmoothTime);
		_height = Mathf.Lerp(_height, (isRampSlidingMode ? rampHeight : height), Time.deltaTime * rampLookSmoothTime);

		wantedPosition.x = target.localPosition.x * 0.7f;
		if (Physics.Raycast(target.localPosition, Vector3.down, out RaycastHit hit, 2.5f))
		{
			posY = Mathf.Lerp(posY, hit.point.y, Time.deltaTime * ZPositionTime);
		}
		else
		{
			posY = Mathf.Lerp(posY, target.localPosition.y, Time.deltaTime * ZPositionTime);
		}
		wantedPosition.y = posY + _height;
		// Camera Hight and Distance functions
		/*usedDistance = Mathf.SmoothDampAngle(usedDistance, isReverseCamera ? -_distance : _distance, ref zVelocity, distanceSnapTime);
		wantedPosition.z = target.localPosition.z - usedDistance;*/
		wantedPosition.z = target.localPosition.z - (isReverseCamera ? -_distance : _distance);
		//wantedPosition = target.position + (target.right.normalized * posX) + (target.up * (posY + height)) + (target.rotation * new Vector3(0, 0, -usedDistance));
		currentPosition = transform.localPosition;

		//Debug.Log(_distance + " + (" + targetCharacterController.velocity.magnitude + " * " + distanceMultiplier + ")  -->  " + _distance + (targetCharacterController.velocity.magnitude * distanceMultiplier));
		//Debug.Log("Target Z Pos: " + target.position.z + "  Used Distance: " + usedDistance);
		//Debug.Log("Used Distance: " + usedDistance);
		//Debug.Log("Target Pos: " + target.localPosition + "  Wanted Pos: " + wantedPosition);

		transform.localPosition = Vector3.SmoothDamp(currentPosition, wantedPosition, ref xyzVelocity, positionSmoothTime);

		float targetLookAtDown = isLookAtPlayer ? LookAtPlayerXAxis() : lookAtDown;
		currentLookAtDown = Mathf.Lerp(currentLookAtDown, targetLookAtDown, Time.deltaTime * XRotationTime);
		Quaternion targetRotation = isReverseCamera ? Quaternion.Euler(currentLookAtDown, 180f, 0) : Quaternion.Euler(currentLookAtDown, 0, 0);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rotationSmoothTime);
	}

	private float LookAtPlayerXAxis()
    {
		Vector3 relativePos = target.localPosition - transform.localPosition;
		float xRotAngle = Quaternion.LookRotation(relativePos).eulerAngles.x;

		return isRampSlidingMode ? xRotAngle - rampLookHeight : xRotAngle;
	}

	public void ResetCamera()
    {
		isReverseCamera = false;
		isInCinematic = true;
		isLookAtPlayer = false;
		isRampSlidingMode = false;
		distance = _coreDistance;
		_distance = _coreDistance;
		height = _coreHeight;
		_height = _coreHeight;
		usedDistance = 0;

		transform.localPosition = startPosition;
		transform.localRotation = startRotation;
	}
}