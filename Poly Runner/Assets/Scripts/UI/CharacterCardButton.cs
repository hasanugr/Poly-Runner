using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardButton : MonoBehaviour
{
    [SerializeField] private MenuControl menuControl;
    [SerializeField] private bool isSelectedCard;

    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI name1;
    [SerializeField] private TextMeshProUGUI name2;
    [SerializeField] private TextMeshProUGUI price;

    [SerializeField] private GameObject purchasedBadge;
    [SerializeField] private GameObject priceBadge;

    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject buyFailWarn;
    [SerializeField] private GameObject selectButton;

    [SerializeField] private Image characterImage;

    public int Id;

    private float _price;
    private bool _isActive;
    private bool _buttonBlock;

    private void OnEnable()
    {
        _buttonBlock = false;
    }

    public void ApplyCardData(CharacterSkin characterSkin)
    {
        _isActive = characterSkin.isActive;
        
        int idPlus = characterSkin.SkinId + 1;
        Id = characterSkin.SkinId;
        id.text = idPlus <= 9 ? "0" + idPlus.ToString() : idPlus.ToString();

        string[] subs = characterSkin.skinName.Split(' ');
        if (subs.Length > 1)
        {
            name1.text = subs[0];
            name2.text = subs[1];
        }
        else
        {
            name1.text = "Runner";
            name2.text = subs[0];
        }

        price.text = characterSkin.skinPrice.ToString();
        _price = characterSkin.skinPrice;

        characterImage.sprite = characterSkin.BodySkinSprite;

        if (_isActive)
        {
            purchasedBadge.SetActive(true);
            priceBadge.SetActive(false);
        }
        else
        {
            purchasedBadge.SetActive(false);
            priceBadge.SetActive(true);
        }
    }

    public void ProccessCard()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            menuControl.CardClicked(this);
            if (_isActive)
            {
                if (!selectButton.activeSelf)
                {
                    selectButton.SetActive(true);
                    LeanTween.scale(selectButton, new Vector3(1, 1, 1), 0.1f).setEaseOutBack();
                }
            }
            else
            {
                if (GameManager.instance.pd.Gold < _price)
                {
                    buyFailWarn.SetActive(true);
                    LeanTween.scale(buyFailWarn, new Vector3(1, 1, 1), 0.1f).setEaseOutBack();
                    StartCoroutine(RemoveWarn(2f));
                }
                else
                {
                    if (!buyButton.activeSelf)
                    {
                        buyButton.SetActive(true);
                        LeanTween.scale(buyButton, new Vector3(1, 1, 1), 0.1f).setEaseOutBack();
                    }
                }
            }
        }
    }

    public void UnproccessCard()
    {
        if (selectButton.activeSelf)
        {
            LeanTween.scale(selectButton, new Vector3(0, 0, 0), 0.1f).setEaseInBack().setOnComplete(() => { 
                selectButton.SetActive(false);
            });
        }

        if (buyButton.activeSelf)
        {
            LeanTween.scale(buyButton, new Vector3(0, 0, 0), 0.1f).setEaseInBack().setOnComplete(() => {
                buyButton.SetActive(false);
            });
        }

        if (buyFailWarn.activeSelf)
        {
            LeanTween.scale(buyFailWarn, new Vector3(0, 0, 0), 0.1f).setEaseInBack().setOnComplete(() => {
                buyFailWarn.SetActive(false);
            });
        }
    }

    public void BuyButton()
    {
        LeanTween.scale(buyButton, new Vector3(0, 0, 0), 0.1f);
        menuControl.BuyCharacter(Id, _price);
    }

    public void SelectButton()
    {
        LeanTween.scale(selectButton, new Vector3(0, 0, 0), 0.1f);
        menuControl.SelectCharacter(Id);
    }

    IEnumerator RemoveWarn(float time)
    {
        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSecondsRealtime(time);

        LeanTween.scale(buyFailWarn, new Vector3(0, 0, 0), 0.1f).setEaseInBack();
    }

    IEnumerator ButtonCooldown(float time)
    {
        _buttonBlock = true;

        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSecondsRealtime(time);

        _buttonBlock = false;
    }
}
