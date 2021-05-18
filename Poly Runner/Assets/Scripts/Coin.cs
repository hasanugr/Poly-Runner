using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject collectedEffect;

    InGameManager _igm;
    MeshRenderer _meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _igm = FindObjectOfType<InGameManager>();
        _meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        //LeanTween.rotateAround(this.gameObject, transform.up, 360, 2f).setLoopClamp();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _igm.AddGold(1);
            collectedEffect.SetActive(true);
            _meshRenderer.enabled = false;
            Destroy(gameObject, 0.25f);
            /*this.gameObject.transform.parent = col.gameObject.transform;

            LeanTween.moveY(this.gameObject, 2.2f, 0.1f);
            LeanTween.scale(this.gameObject, new Vector3(0, 0, 0), 0.5f).setEaseInCubic();
            Destroy(gameObject, 0.9f);*/
        }
    }
}
