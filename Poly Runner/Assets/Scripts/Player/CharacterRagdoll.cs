using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;
    [SerializeField] private float dieEffectDelay = 0.05f;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody characterSpineRb;
    [SerializeField] private Vector3 forceWay;

    private void OnEnable()
    {
        StartCoroutine(DieEffectActivate(dieEffectDelay));
    }

    IEnumerator DieEffectActivate(float time)
    {
        yield return new WaitForSeconds(time);

        animator.enabled = false;
        characterSpineRb.AddForce(forceWay, ForceMode.VelocityChange);
    }

    private void OnDisable()
    {
        rootObject.transform.localPosition = new Vector3(0, 1, 0);
        rootObject.transform.localRotation = Quaternion.Euler(0, -90, -90);
    }
}
