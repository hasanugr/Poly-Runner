using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FallingTree : MonoBehaviour
{
    [SerializeField] private Animation animationControl;
    [SerializeField] private GameObject dustEffect;
    [SerializeField] private ShakePreset fallShake;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            animationControl.Play();
            StartCoroutine(DeactiveTheObstacle(5f));
        }
    }

    public void AddFallEffect()
    {
        dustEffect.SetActive(true);
        Shaker.ShakeAllSeparate(fallShake);
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
