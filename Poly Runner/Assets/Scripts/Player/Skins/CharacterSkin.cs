using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSkin
{
    public enum SkinTypes { Normal, Elite, Rear, Premium, Gift };
    public enum SkinGender { Male, Female };

    public bool isActive;
    public int SkinId;
    public string skinName;
    public SkinTypes skinType;
    public SkinGender skinGender;
    public float skinPrice;
    public Sprite BodySkinSprite;
    public GameObject BodySkinObject;
    public GameObject RagdollSkinObject;

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
