using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterEffectTypes { Stunned };
public class CharacterEffects : MonoBehaviour
{
    [SerializeField] private GameObject stunned;

    public void EffectProcess(CharacterEffectTypes type, bool state)
    {
        switch (type)
        {
            case CharacterEffectTypes.Stunned:
                StunnedEffect(state);
                break;
            default:
                Debug.LogWarning("Unexpected Effect Type.!");
                break;
        }
    }

    private void StunnedEffect(bool state)
    {
        if (state)
        {
            stunned.SetActive(true);
            LeanTween.rotateAround(stunned, transform.up, 360, 2f).setLoopClamp();
        }
        else
        {
            stunned.SetActive(false);
        }
    }
}
