using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVisibilites : MonoBehaviour
{
    [SerializeField] private GameObject[] coinHolders;
    private int activeIndex = 0;
    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(TriggerNormalize(2f));
            coinHolders[activeIndex].SetActive(false);
            activeIndex++;
            coinHolders[activeIndex].SetActive(true);
        }
    }

    IEnumerator TriggerNormalize(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        isTriggered = false;
    }
}
