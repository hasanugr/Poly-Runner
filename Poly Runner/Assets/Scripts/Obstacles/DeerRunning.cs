using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class DeerRunning : MonoBehaviour
{
    [SerializeField] private GameObject[] movingObjects;
    [SerializeField] private float animationDelay;
    [SerializeField] private ShakePreset fallShake;

    private ShakeInstance shakeInstance;
    private bool isTriggered;
    private float cameraShakeStartSecond = 1f;
    private float cameraShakeEndSecond;
    private float animateDurationMultiple;

    private void Start()
    {
        animateDurationMultiple = 0.44f * animationDelay; // 0.22f for 0.5 animationDelay

        movingObjects = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            movingObjects[i] = gameObject.transform.GetChild(i).GetChild(0).gameObject;
        }

        cameraShakeEndSecond = (movingObjects.Length * animationDelay) + 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(CameraShakeWithDelay(fallShake, cameraShakeStartSecond, cameraShakeEndSecond));
            for (int i = 0; i < movingObjects.Length; i++)
            {
                StartCoroutine(MoveObj(movingObjects[i], -20f + (-10 * i * animationDelay), 4f + (animateDurationMultiple * i), animationDelay * i));
            }
            StartCoroutine(DeactiveTheObstacle(cameraShakeEndSecond + 5f));
        }
    }

    IEnumerator MoveObj(GameObject obj, float toValue, float durationTime, float delayTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(delayTime);

        LeanTween.moveLocalZ(obj, toValue, durationTime);
    }

    IEnumerator CameraShakeWithDelay(ShakePreset shakePreset, float startTime, float endTime)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(startTime);

        shakeInstance = Shaker.ShakeAll(shakePreset);

        yield return new WaitForSeconds(endTime);

        shakeInstance.Stop(shakePreset.FadeOut, true);
        shakeInstance = null;
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
