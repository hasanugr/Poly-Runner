using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class SnowballRolling : MonoBehaviour
{
    [SerializeField] private GameObject[] movingObjects;
    [SerializeField] private GameObject[] rotatingObjects;
    [SerializeField] private float animationDelay;
    [SerializeField] private ShakePreset fallShake;
    [SerializeField] private float startDistance = 70f;

    private ShakeInstance shakeInstance;
    private bool isTriggered;
    private float cameraShakeStartSecond = 1f;
    private float cameraShakeEndSecond;
    private float animateDurationMultiple;
    private float rotateAroundTime;
    private Coroutine _coroutinecameraShake;
    private Coroutine[] _coroutineMovingObject;
    private Coroutine[] _coroutineRotatingObjects;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.instance;

        animateDurationMultiple = 0.44f * animationDelay; // 0.22f for 0.5 animationDelay
        rotateAroundTime = 1f;

        movingObjects = new GameObject[gameObject.transform.childCount];
        rotatingObjects = new GameObject[gameObject.transform.childCount];
        _coroutineMovingObject = new Coroutine[movingObjects.Length];
        _coroutineRotatingObjects = new Coroutine[movingObjects.Length];
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

        _coroutinecameraShake = StartCoroutine(CameraShakeWithDelay(fallShake, cameraShakeStartSecond, cameraShakeEndSecond));
        for (int i = 0; i < movingObjects.Length; i++)
        {
            _coroutineMovingObject[i] = StartCoroutine(MoveObj(movingObjects[i], -20f + (-10 * i * animationDelay), 4f + (animateDurationMultiple * i), animationDelay * i));
            _coroutineRotatingObjects[i] = StartCoroutine(RotateObj(rotatingObjects[i], 4f + (animateDurationMultiple * i), animationDelay * i));
        }
        //StartCoroutine(DeactiveTheObstacle(cameraShakeEndSecond + 5f));
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
        _audioManager.Play(AudioManager.AudioSoundTypes.Environment, "Quake1");
        _audioManager.Play(AudioManager.AudioSoundTypes.Environment, "Quake2");

        yield return new WaitForSeconds(endTime);

        shakeInstance.Stop(shakePreset.FadeOut, false);
        shakeInstance = null;
        _audioManager.Stop(AudioManager.AudioSoundTypes.Environment, "Quake1");
        _audioManager.Stop(AudioManager.AudioSoundTypes.Environment, "Quake2");

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

    public void ResetObstacle()
    {
        isTriggered = false;
        if (_coroutinecameraShake != null)
            StopCoroutine(_coroutinecameraShake);

        if (shakeInstance != null)
        {
            shakeInstance.Stop(fallShake.FadeOut, false);
        }

        if (InGameManager.instance != null)
            InGameManager.instance.CurvedWorldDefault();

        for (int i = 0; i < movingObjects.Length; i++)
        {
            if (_coroutineMovingObject[i] != null)
                StopCoroutine(_coroutineMovingObject[i]);
            if (_coroutineRotatingObjects[i] != null)
                StopCoroutine(_coroutineRotatingObjects[i]);
            LeanTween.cancel(movingObjects[i]);
            LeanTween.cancel(rotatingObjects[i]);
            movingObjects[i].transform.localPosition = new Vector3(0, 0, startDistance);
            rotatingObjects[i].transform.localRotation = Quaternion.identity;
        }
    }
}
