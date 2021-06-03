using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class TreeTrunkRolling : MonoBehaviour
{
    [SerializeField] private GameObject[] movingObjects;
    [SerializeField] private GameObject[] rotatingObjects;
    [SerializeField] private float animationDelay;
    [SerializeField] private ShakePreset fallShake;

    private ShakeInstance shakeInstance;
    private bool isTriggered;
    private float cameraShakeStartSecond = 1f;
    private float cameraShakeEndSecond;
    private float animateDurationMultiple;
    private float rotateAroundTime;

    private void Start()
    {
        animateDurationMultiple = 0.44f * animationDelay; // 0.22f for 0.5 animationDelay
        rotateAroundTime = 0.5f;

        movingObjects = new GameObject[gameObject.transform.childCount];
        rotatingObjects = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            movingObjects[i] = gameObject.transform.GetChild(i).GetChild(0).gameObject;
            rotatingObjects[i] = gameObject.transform.GetChild(i).GetChild(0).GetChild(0).gameObject;
        }

        cameraShakeEndSecond = (movingObjects.Length * animationDelay) + 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            InGameManager.instance.CurvedWorldRamp(true);
            StartCoroutine(StartProcess(1.5f));
        }
    }

    IEnumerator StartProcess(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        StartCoroutine(CameraShakeWithDelay(fallShake, cameraShakeStartSecond, cameraShakeEndSecond));
        for (int i = 0; i < movingObjects.Length; i++)
        {
            StartCoroutine(MoveObj(movingObjects[i], -20f + (-10 * i * animationDelay), 4f + (animateDurationMultiple * i), animationDelay * i));
            StartCoroutine(RotateObj(rotatingObjects[i], 4f + (animateDurationMultiple * i), animationDelay * i));
        }
        StartCoroutine(DeactiveTheObstacle(cameraShakeEndSecond + 5f));
    }

    IEnumerator MoveObj(GameObject obj, float toValue, float durationTime, float delayTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(delayTime);

        LeanTween.moveLocalZ(obj, toValue, durationTime);
    }

    IEnumerator RotateObj(GameObject obj, float durationTime, float delayTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(delayTime);

        LeanTween.rotateAroundLocal(obj, Vector3.left, 360f, rotateAroundTime).setLoopClamp();

        yield return new WaitForSeconds(durationTime);

        LeanTween.cancel(obj);
    }

    IEnumerator CameraShakeWithDelay(ShakePreset shakePreset, float startTime, float endTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(startTime);

        shakeInstance = Shaker.ShakeAll(shakePreset);

        yield return new WaitForSeconds(endTime);

        shakeInstance.Stop(shakePreset.FadeOut, true);
        shakeInstance = null;
        if (InGameManager.instance.isGameActive)
        {
            InGameManager.instance.CurvedWorldRamp(false);
        }
    }

    IEnumerator DeactiveTheObstacle(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        if (InGameManager.instance.isGameActive)
        {
            gameObject.SetActive(false);
        }
    }
}
