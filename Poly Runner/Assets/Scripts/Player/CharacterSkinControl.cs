using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinControl : MonoBehaviour
{
    public PlayerSkin[] PlayerSkins;
    public PlayerSkin SelectedPlayerSkin;

    public int lastActiveSkinId = 0;

    private void OnEnable()
    {
        ChangeSkin(GameManager.instance.pd.SelectedCharacterId);
    }

    public void ChangeSkin(int skinId)
    {
        PlayerSkins[GetSkinIndex(lastActiveSkinId)].DeactivateSkin();
        PlayerSkins[GetSkinIndex(skinId)].ActivateSkin();
        lastActiveSkinId = skinId;
        SelectedPlayerSkin = PlayerSkins[GetSkinIndex(skinId)];
    }

    public int GetSkinIndex(int skinId)
    {
        int skinIndex = 0;
        for (int i = 0; i < PlayerSkins.Length; i++)
        {
            if (skinId == PlayerSkins[i].SkinId)
            {
                skinIndex = i;
                break;
            }
        }

        return skinIndex;
    }
}

[System.Serializable]
public class PlayerSkin
{
    public enum SkinTypes { Normal, Elite, Rear, Premium, Gift };
    public enum SkinGender { Male, Female };

    public int SkinId;
    public SkinTypes skinType;
    public SkinGender skinGender;
    public float skinPrice;
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