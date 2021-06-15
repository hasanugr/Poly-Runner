using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class StartLine : MonoBehaviour
{
    public GameObject CameraObj;
    public GameObject Bear;
    [SerializeField] float bearStartDistance = 25f;
    public Animator BearAnimator;
    public GameObject DustEffect;
    public ShakePreset BigShake;
    public float dustDelay = 1.5f;
    public float runDelay = 1.7f;
    private enum VectorType { X, Y, Z };

    void Start()
    {
        StartProcess();
    }

    public void StartProcess()
    {
        Bear.SetActive(true);
        /*LeanTween.moveLocalZ(CameraObj, -25f, runDelay - 0.3f);
        LeanTween.rotateX(CameraObj, -15f, runDelay - 0.3f);*/
        LeanTween.moveLocal(CameraObj, new Vector3(0, 1, -45), runDelay - 0.3f);
        LeanTween.rotate(CameraObj, new Vector3(-15, 180, 0), runDelay - 0.3f);
        BearAnimator.SetBool("isStanding", true);
        StartCoroutine(AnimateWithDelay(BearAnimator, "isStanding", false, runDelay - 0.3f));
        StartCoroutine(AnimateWithDelay(BearAnimator, "isRunning", true, runDelay));
        StartCoroutine(ActivateWithDelay(DustEffect, true, runDelay));
        StartCoroutine(CameraShakeWithDelay(BigShake, runDelay));
        StartCoroutine(MoveObj(Bear, 0f, 3f, VectorType.X, runDelay));
        StartCoroutine(MoveObj(CameraObj, -0.5f, (4f - runDelay), VectorType.Z, runDelay));
        StartCoroutine(MoveObj(CameraObj, 4.5f, (4f - runDelay), VectorType.Y, runDelay));
        StartCoroutine(RotateObj(CameraObj, 20f, (4f - runDelay), VectorType.X, runDelay));
        StartCoroutine(RotateObj(CameraObj, 0, 0.2f, VectorType.Y, 3.8f));
        StartCoroutine(ActivateWithDelay(Bear, false, 4.5f));
    }

    IEnumerator CameraShakeWithDelay(ShakePreset shakePreset , float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        Shaker.ShakeAllSeparate(shakePreset);
    }

    IEnumerator ActivateWithDelay(GameObject activateObject, bool state, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        activateObject.SetActive(state);
    }

    IEnumerator AnimateWithDelay(Animator animator, string animateName, bool animateState, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        animator.SetBool(animateName, animateState);
    }

    IEnumerator MoveObj(GameObject obj, float toValue, float durationTime, VectorType vectorType, float delayTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(delayTime);

        switch (vectorType)
        {
            case VectorType.X:
                LeanTween.moveLocalX(obj, toValue, durationTime);
                break;
            case VectorType.Y:
                LeanTween.moveLocalY(obj, toValue, durationTime);
                break;
            case VectorType.Z:
                LeanTween.moveLocalZ(obj, toValue, durationTime);
                break;
            default:
                Debug.Log("Missing Vector Type.!!");
                break;
        }
    }

    IEnumerator RotateObj(GameObject obj, float toValue, float durationTime, VectorType vectorType, float delayTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(delayTime);

        switch (vectorType)
        {
            case VectorType.X:
                LeanTween.rotateX(obj, toValue, durationTime);
                break;
            case VectorType.Y:
                LeanTween.rotateY(obj, toValue, durationTime);
                break;
            case VectorType.Z:
                LeanTween.rotateZ(obj, toValue, durationTime);
                break;
            default:
                Debug.Log("Missing Vector Type.!!");
                break;
        }
    }

    public void ResetObstacle()
    {
        BearAnimator.SetBool("isRunning", false);
        LeanTween.cancel(CameraObj);
        LeanTween.cancel(Bear);
        Bear.transform.localPosition = new Vector3(16f, 0, bearStartDistance);
        Bear.SetActive(false);
    }
}
