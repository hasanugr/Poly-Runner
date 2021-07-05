using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Image levelMainImage;
    [SerializeField] private Button levelButton;

    [SerializeField] private GameObject levelNumberObject;
    [SerializeField] private GameObject levelLockedObject;
    [SerializeField] private GameObject starsObject;

    [SerializeField] private TextMeshProUGUI levelNumberText;

    [SerializeField] private Image[] starImages;

    private int _levelNumber;
    private bool _buttonBlock;

    private void OnEnable()
    {
        _buttonBlock = false;
    }

    public void ApplyLevelData(LevelButtonData levelData)
    {
        if (levelData.isOpen)
        {
            levelMainImage.sprite = levelData.openLevelMainSprite;
            levelButton.interactable = true;
            levelLockedObject.SetActive(false);
            levelNumberObject.SetActive(true);
            starsObject.SetActive(true);
            levelNumberText.text = levelData.levelNumber.ToString();
            _levelNumber = levelData.levelNumber;

            for (int i = 0; i < starImages.Length; i++)
            {
                if (i < levelData.levelStar)
                {
                    starImages[i].sprite = levelData.star;
                }else
                {
                    starImages[i].sprite = levelData.starMissing;
                }
            }
        }
        else
        {
            levelMainImage.sprite = levelData.lockedLevelMainSprite;
            levelButton.interactable = false;
            levelNumberObject.SetActive(false);
            starsObject.SetActive(false);
            levelLockedObject.SetActive(true);
        }
    }

    public void StartLevel()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            GameManager.instance.StartLevel(_levelNumber);
        }
    }

    IEnumerator ButtonCooldown(float time)
    {
        _buttonBlock = true;

        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSecondsRealtime(time);

        _buttonBlock = false;
    }
}
