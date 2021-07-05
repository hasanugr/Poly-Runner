using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class DeerRunning : MonoBehaviour
{
    [SerializeField] private GameObject[] movingObjects;
    [SerializeField] private float animationDelay;
    [SerializeField] private ShakePreset fallShake;
    [SerializeField] private float startDistance = 70f;

    private ShakeInstance shakeInstance;
    private bool isTriggered;
    private float cameraShakeStartSecond = 1f;
    private float cameraShakeEndSecond;
    private float animateDurationMultiple;
    private Coroutine _coroutinecameraShake;
    private Coroutine[] _coroutineMovingObject;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.instance;

        animateDurationMultiple = 0.44f * animationDelay; // 0.22f for 0.5 animationDelay

        movingObjects = new GameObject[gameObject.transform.childCount];
        _coroutineMovingObject = new Coroutine[movingObjects.Length];
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
            _coroutinecameraShake = StartCoroutine(CameraShakeWithDelay(fallShake, cameraShakeStartSecond, cameraShakeEndSecond));
            for (int i = 0; i < movingObjects.Length; i++)
            {
                _coroutineMovingObject[i] = StartCoroutine(MoveObj(movingObjects[i], -20f + (-10 * i * animationDelay), 4f + (animateDurationMultiple * i), animationDelay * i));
            }
            //StartCoroutine(DeactiveTheObstacle(cameraShakeEndSecond + 5f));
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
        _audioManager.Play(AudioManager.AudioSoundTypes.Environment, "Quake1");
        _audioManager.Play(AudioManager.AudioSoundTypes.Environment, "Quake2");

        yield return new WaitForSeconds(endTime);

        shakeInstance.Stop(shakePreset.FadeOut, false);
        shakeInstance = null;
        _audioManager.Stop(AudioManager.AudioSoundTypes.Environment, "Quake1");
        _audioManager.Stop(AudioManager.AudioSoundTypes.Environment, "Quake2");
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

    public void ResetObstacle()
    {
        isTriggered = false;
        if (_coroutinecameraShake != null)
            StopCoroutine(_coroutinecameraShake);

        if (shakeInstance != null)
        {
            shakeInstance.Stop(fallShake.FadeOut, false);
        }

        for (int i = 0; i < movingObjects.Length; i++)
        {
            if (_coroutineMovingObject[i] != null)
                StopCoroutine(_coroutineMovingObject[i]);
            LeanTween.cancel(movingObjects[i]);
            movingObjects[i].transform.localPosition = new Vector3(0, 0, startDistance);
        }
    }
}
