using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject coinModel;
    [SerializeField] Animation coinAnimation;
    [SerializeField] GameObject coinCellsModel;
    [SerializeField] Animator coinCellsAnimator;
    [SerializeField] GameObject collectedEffect;

    InGameManager _igm;

    void Start()
    {
        _igm = FindObjectOfType<InGameManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _igm.AddGold(10);
            coinAnimation.Stop();
            coinModel.SetActive(false);
            collectedEffect.SetActive(true);
            //coinCellsModel.SetActive(true);
            coinCellsAnimator.ResetTrigger("Passive");
            coinCellsAnimator.SetTrigger("Explode");
            //Destroy(gameObject, 2f);
        }
    }

    public void ResetCoin()
    {
        coinAnimation.Play();
        coinModel.SetActive(true);
        coinCellsAnimator.SetTrigger("Passive");
    }
}
