using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class SeagulAttacker : MonoBehaviour
{
    [SerializeField] private Animation[] animationControl;
    [SerializeField] private float animationDelay;
    [SerializeField] private ShakePreset fallShake;
    [SerializeField] private float fallShakeDelay;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            for (int i = 0; i < animationControl.Length; i++)
            {
                StartCoroutine(AnimateWithDelay(animationControl[i], animationDelay * i));
                StartCoroutine(CameraShakeWithDelay(fallShake, (animationDelay * i) + fallShakeDelay));
            }
            StartCoroutine(DeactiveTheObstacle((animationDelay * (animationControl.Length - 1)) + 5f));
        }
    }

    IEnumerator AnimateWithDelay(Animation animation, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        animation.Play();
    }

    IEnumerator CameraShakeWithDelay(ShakePreset shakePreset, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        Shaker.ShakeAllSeparate(shakePreset);
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
