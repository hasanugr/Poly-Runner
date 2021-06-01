using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject collectedEffect;
    [SerializeField] GameObject coinModel;

    InGameManager _igm;

    void Start()
    {
        _igm = FindObjectOfType<InGameManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _igm.AddGold(1);
            collectedEffect.SetActive(true);
            coinModel.SetActive(false);

            Destroy(gameObject, 0.25f);
        }
    }
}
