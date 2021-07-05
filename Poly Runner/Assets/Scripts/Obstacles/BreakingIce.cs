using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class BreakingIce : MonoBehaviour
{
    //[SerializeField] private Animation animationControl;
    [SerializeField] private Animator animationControl;
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private GameObject waterSplashEffect;
    [SerializeField] private ShakePreset earthQuakeShake;

    private Character _playerScript;
    private ShakeInstance shakeInstance;
    private int triggerCounter = 0;
    private bool isTriggered;

    private AudioManager _audioManager;

    private void Start()
    {
        _playerScript = FindObjectOfType<Character>();
        _audioManager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            if (triggerCounter == 0)
            {
                isTriggered = true;
                triggerCounter++;
                StartCoroutine(ParticleEffect(1.1f));
                StartCoroutine(TriggeredDeactive(1f));
                shakeInstance = Shaker.ShakeAll(earthQuakeShake);
                animationControl.ResetTrigger("BreakingIceDefault");
                animationControl.SetTrigger("BreakingIce");
                StartCoroutine(PlaySoundWithDelay("IceCrack", 1.1f));
                StartCoroutine(PlaySoundWithDelay("WaterSplash", 1.2f));
            }
            else if (triggerCounter == 1)
            {
                isTriggered = true;
                _playerScript.SetAnimate("FallToWater");
                StartCoroutine(ParticleWithDelay(waterSplashEffect, 0.17f));
                StartCoroutine(PlaySoundWithDelay("WaterSplash", 0.17f));
                StartCoroutine(TriggerDie(0f));
            }
        }
    }

    IEnumerator ParticleEffect(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        particleEffect.SetActive(true);
        shakeInstance.Stop(earthQuakeShake.FadeOut, true);
        shakeInstance = null;
    }

    IEnumerator PlaySoundWithDelay(string soundName, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.Environment, soundName);
    }

    IEnumerator TriggeredDeactive(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        isTriggered = false;
    }

    IEnumerator TriggerDie(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        _playerScript.DieProcess("Drown");
    }

    IEnumerator ParticleWithDelay(GameObject particleObject, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        particleObject.SetActive(true);
    }

    public void ResetObstacle()
    {
        //if (animationControl.GetCurrentAnimatorClipInfo(0).Length > 0)
        animationControl.SetTrigger("BreakingIceDefault");
        isTriggered = false;
        triggerCounter = 0;
    }
}
