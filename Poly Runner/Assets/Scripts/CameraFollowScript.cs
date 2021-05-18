using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speedFollow = 5f;
    [SerializeField] private float rotationSmoothTime = 0.2f;
    [SerializeField] private float lookDown = 20f;
    [SerializeField] private bool isReverseCamera;

    private Vector3 Offset;
    private float posY;

    void Start()
    {
        Offset = transform.position;
    }

    void LateUpdate()
    {
        Vector3 followPos = target.position + Offset;
        followPos.x = target.position.x * 0.7f;

        if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hit, 2.5f))
        {
            posY = Mathf.Lerp(posY, hit.point.y, Time.deltaTime * speedFollow);
        }
        else
        {
            posY = Mathf.Lerp(posY, target.position.y, Time.deltaTime * speedFollow);
        }

        followPos.y = Offset.y + posY;
        if (isReverseCamera) followPos.z *= -1;
        transform.position = followPos;
        Debug.Log(followPos);
        
        Quaternion targetRotation = target.rotation * Quaternion.Euler(lookDown, 0, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, isReverseCamera ? (targetRotation * Quaternion.Euler(0, 180f, 0)) : targetRotation, rotationSmoothTime);
    }
}
