using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class BreakingIce : MonoBehaviour
{
    [SerializeField] private Animation animationControl;
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private ShakePreset earthQuakeShake;

    private ShakeInstance shakeInstance;
    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            shakeInstance = Shaker.ShakeAll(earthQuakeShake);
            animationControl.Play();
            StartCoroutine(DeactiveTheObstacle(5f));
        }
    }

    public void AddFallEffect()
    {
        particleEffect.SetActive(true);
        shakeInstance.Stop(earthQuakeShake.FadeOut, true);
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
