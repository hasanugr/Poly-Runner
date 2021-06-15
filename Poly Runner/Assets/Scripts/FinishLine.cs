using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FinishLine : MonoBehaviour
{
    public GameObject Bear;
    public Animator BearAnimator;
    [SerializeField] float bearStartDistance = -18f;
    public Animator CageAnimator;
    public GameObject DustEffect;
    public ShakePreset BigShake;
    public GameObject[] humans;

    private GameObject _player;

    InGameManager _igm;

    private bool isTriggered;

    void Start()
    {
        _igm = FindObjectOfType<InGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            _player = other.transform.gameObject;
            isTriggered = true;
            _igm.Finish();
            CageAnimator.SetTrigger("FinishGateClose");
            StartCoroutine(ActivateWithDelay(DustEffect, .9f));
            StartCoroutine(CameraShakeWithDelay(BigShake, .9f));
            Bear.SetActive(true);
            BearAnimator.SetBool("isRunning", true);
            LeanTween.moveLocalZ(Bear, -3.5f, 2f);
            StartCoroutine(AnimateWithDelay(BearAnimator, "isRunning", false, 2f));
        }
    }

    IEnumerator CameraShakeWithDelay(ShakePreset shakePreset, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        Shaker.ShakeAllSeparate(shakePreset);
    }

    IEnumerator ActivateWithDelay(GameObject activateObject, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        activateObject.SetActive(true);
    }

    IEnumerator AnimateWithDelay(Animator animator, string animateName, bool animateState, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        animator.SetBool(animateName, animateState);

        for (int i = 0; i< humans.Length; i++)
        {
            humans[i].transform.LookAt(_player.transform);
        }
    }

    public void ResetObstacle()
    {
        LeanTween.cancel(Bear);
        Bear.transform.localPosition = new Vector3(0, 0.1f, bearStartDistance);
        Bear.SetActive(false);
        CageAnimator.SetTrigger("Passive");
        isTriggered = false;
    }
}
