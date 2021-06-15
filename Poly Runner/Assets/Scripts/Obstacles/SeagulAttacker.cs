using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class SeagulAttacker : MonoBehaviour
{
    private enum SeagulType { TripleSeagul, BigSeagul };
    
    [SerializeField] private SeagulType seagulType;
    [SerializeField] private Animator[] animationControl;
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
            //StartCoroutine(DeactiveTheObstacle((animationDelay * (animationControl.Length - 1)) + 5f));
        }
    }

    IEnumerator AnimateWithDelay(Animator animation, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        if (seagulType == SeagulType.BigSeagul)
        {
            animation.SetTrigger("BigSeagulAttack");
        }
        else if(seagulType == SeagulType.TripleSeagul)
        {
            animation.SetTrigger("SeagulAttack");
        }
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
    public void ResetObstacle()
    {
        for (int i = 0; i < animationControl.Length; i++)
        {
            if (animationControl[i].GetCurrentAnimatorClipInfo(0).Length > 0)
                animationControl[i].SetTrigger("Passive");
        }
        isTriggered = false;
    }
}
