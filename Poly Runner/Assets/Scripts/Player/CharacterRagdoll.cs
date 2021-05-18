using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRagdoll : MonoBehaviour
{
    //[SerializeField] private Character mainCharacter;
    [SerializeField] private float dieEffectDelay = 0.05f;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody characterSpineRb;
    [SerializeField] private Vector3 forceWay;

    // Start is called before the first frame update
    void Start()
    {

    }

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

}
