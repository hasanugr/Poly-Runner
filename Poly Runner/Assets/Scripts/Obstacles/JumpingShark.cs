using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingShark : MonoBehaviour
{
    [SerializeField] private Animation animationControl;
    [SerializeField] private GameObject particleEffect;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            animationControl.Play();
            particleEffect.SetActive(true);
            StartCoroutine(DeactiveTheObstacle(5f));
        }
    }

    public void AddFallEffect()
    {
        particleEffect.SetActive(true);
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
