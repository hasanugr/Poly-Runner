using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform target; // Self explanatory
	public float distance = 5.5f; // Standard distance to follow object
	public float height = 4.5f; // The height of the camera

	public float lookAtDown = 20.0f;
	public float positionSmoothTime = 0.05f;
	public float ZPositionTime = 5f;
	public float rotationSmoothTime = 0.2f;
	public bool isReverseCamera = false;

	private CharacterController targetCharacterController;

	public float distanceSnapTime;
	public float distanceMultiplier;

	private float usedDistance;

	Vector3 wantedPosition;
	Vector3 currentPosition;

	//private float posX;
	private float posY;
	private float zVelocity = 0.0F;
	private Vector3 xyzVelocity = new Vector3(0, 0, 0);

	void Start()
	{
		GameObject targetObject = GameObject.FindGameObjectWithTag("Player");
		
		target = targetObject.transform;
		targetCharacterController = targetObject.GetComponent<CharacterController>();
	}

	void LateUpdate()
	{
		//Vector3 followPos = target.position;
		wantedPosition.x = target.position.x * 0.7f;
		if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hit, 2.5f))
		{
			posY = Mathf.Lerp(posY, hit.point.y, Time.deltaTime * ZPositionTime);
		}
		else
		{
			posY = Mathf.Lerp(posY, target.position.y, Time.deltaTime * ZPositionTime);
		}
		wantedPosition.y = posY + height;
		// Camera Hight and Distance functions
		usedDistance = Mathf.SmoothDampAngle(usedDistance, isReverseCamera ? -distance : distance + (targetCharacterController.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);
		wantedPosition.z = target.position.z - usedDistance;
		//wantedPosition = target.position + (target.right.normalized * posX) + (target.up * (posY + height)) + (target.rotation * new Vector3(0, 0, -usedDistance));
		currentPosition = transform.position;

		transform.position = Vector3.SmoothDamp(currentPosition, wantedPosition, ref xyzVelocity, positionSmoothTime);
		Quaternion targetRotation = isReverseCamera ? Quaternion.Euler(lookAtDown, 180f, 0) : Quaternion.Euler(lookAtDown, 0, 0);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothTime);
	}
}