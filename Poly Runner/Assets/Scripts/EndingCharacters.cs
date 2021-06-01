using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationTypes { Waving, MixWaving, MixTorchBurn}
public class EndingCharacters : MonoBehaviour
{
    [SerializeField] private AnimationTypes animationTypes;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        switch (animationTypes)
        {
            case AnimationTypes.Waving:
                _animator.SetBool("isWaving", true);
                break;
            case AnimationTypes.MixWaving:
                _animator.SetBool("isMixWaving", true);
                break;
            case AnimationTypes.MixTorchBurn:
                _animator.SetBool("isMixTorchBurn", true);
                break;
            default:
                Debug.Log("There is no selected animation..");
                break;
        }
    }
}
