using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FallingTree : MonoBehaviour
{
    [SerializeField] private Animator animationControl;
    [SerializeField] private GameObject dustEffect;
    [SerializeField] private ShakePreset fallShake;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            animationControl.SetTrigger("FallTree");
            StartCoroutine(ParticleEffect(1f));
            //StartCoroutine(DeactiveTheObstacle(5f));
        }
    }

    IEnumerator ParticleEffect(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        dustEffect.SetActive(true);
        Shaker.ShakeAllSeparate(fallShake);
    }

    IEnumerator DeactiveTheObstacle(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        if (InGameManager.instance.isGameActive)
        {
            animationControl.SetTrigger("Passive");
            gameObject.SetActive(false);
        }
    }

    public void ResetObstacle()
    {
        if (animationControl.GetCurrentAnimatorClipInfo(0).Length > 0)
            animationControl.SetTrigger("Passive");
        isTriggered = false;
    }
}
