using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkin", menuName = "Cytris/Character Skin")]
public class CharacterSkinSO : ScriptableObject
{
    public enum SkinTypes { Normal, Elite, Rear, Premium, Gift };

    public int SkinId;
    public GameObject BodySkinObject;
    public GameObject RagdollSkinObject;
    public SkinTypes SkinType;


    public void ActivateSkin()
    {
        BodySkinObject.SetActive(true);
        RagdollSkinObject.SetActive(true);
    }

    public void DeactivateSkin()
    {
        BodySkinObject.SetActive(false);
        RagdollSkinObject.SetActive(false);
    }
}
