using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinControl : MonoBehaviour
{
    public PlayerSkin[] PlayerSkins;

    private int _lastActiveSkinId = 0;

    public void ChangeSkin(int skinId)
    {
        PlayerSkins[_lastActiveSkinId].DeactivateSkin();
        PlayerSkins[skinId].ActivateSkin();
        _lastActiveSkinId = skinId;
    }
}

[System.Serializable]
public class PlayerSkin
{
    public enum SkinTypes { Normal, Elite, Rear, Premium, Gift };

    public int SkinId;
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