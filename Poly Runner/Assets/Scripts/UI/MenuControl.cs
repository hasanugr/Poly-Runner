using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    [Header("Main Menues")]
    public GameObject MainMenu;
    public GameObject CharacterSelectMenu;
    public GameObject LevelSelectMenu;
    public GameObject SettingsMenu;

    public TextMeshProUGUI goldText;

    [Header("Character Select Menu")]
    public CharacterCardButton[] CharacterCardObjects;
    public CharacterCardButton SelectedCharacterCardObject;

    private CharacterCardButton _lastClickedCard;

    [Header("Level Select Menu")]
    public LevelButton[] LevelObjects;
    public Sprite lockedLevelMainSprite;
    public Sprite openLevelMainSprite;
    public Sprite starSprite;
    public Sprite starMissingSprite;

    AudioManager _audioManager;

    private bool _buttonBlock;
    
    private void Awake()
    {
        _audioManager = AudioManager.instance;

        OpenMainMenu();
    }

    private void OnEnable()
    {
        _buttonBlock = false;
    }

    private void OnDisable()
    {
        LevelSelectMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(false);
        CharacterSelectMenu.SetActive(false);
    }

    public void MenuStart(int openMenuIndex)
    {
        // 0 Main Menu
        // 1 Level Select Menu

        if (openMenuIndex == 0)
        {
            OpenMainMenu();
        }
        else if (openMenuIndex == 1)
        {
            OpenLevelSelect();
        }
    }

    public void OpenMainMenu()
    {
        if (!_buttonBlock) {
            StartCoroutine(ButtonCooldown(0.5f));

            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");

            goldText.text = GameManager.instance.pd.Gold.ToString();
            CharacterSelectMenu.SetActive(false);
            LevelSelectMenu.SetActive(false);
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    #region Character Select Menu
    public void OpenCharacterList()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");

            ResetCharacterCards();
            LevelSelectMenu.SetActive(false);
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(false);
            CharacterSelectMenu.SetActive(true);
        }
    }

    private void ResetCharacterCards()
    {
        CharacterSkin[] skins = GameManager.instance.characterSkins;
        int selectedSkinId = GameManager.instance.pd.SelectedCharacterId;
        int counter = 0;

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].SkinId == selectedSkinId)
            {
                SelectedCharacterCardObject.ApplyCardData(skins[i]);
            }else
            {
                CharacterCardObjects[counter].ApplyCardData(skins[i]);
                counter++;
            }
        }
    }

    public void CardClicked(CharacterCardButton cardButton)
    {
        if (_lastClickedCard != null && _lastClickedCard.Id != cardButton.Id)
        {
            _lastClickedCard.UnproccessCard();
        }

        _lastClickedCard = cardButton;
    }

    public void SelectCharacter(int skinId)
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "SelectCharacter");

            GameManager.instance.ChangeSkin(skinId);

            ResetCharacterCards();
        }
    }

    public void BuyCharacter(int skinId, float skinPrice)
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            GameManager.instance.BuySkin(skinId, skinPrice);

            ResetCharacterCards();
        }
    }

    #endregion

    #region Level Select Menu
    public void OpenLevelSelect()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");

            ResetLevelButtons();
            CharacterSelectMenu.SetActive(false);
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(false);
            LevelSelectMenu.SetActive(true);
        }
    }

    private void ResetLevelButtons()
    {
        int[] playerLevels = GameManager.instance.pd.Levels;
        LevelButtonData levelButtonData = new LevelButtonData();

        for (int i = 0; i < LevelObjects.Length; i++)
        {
            if (i < playerLevels.Length)
            {
                levelButtonData.isOpen = true;
                levelButtonData.levelNumber = i + 1;
                levelButtonData.levelStar = playerLevels[i];
            }
            else
            {
                levelButtonData.isOpen = false;
                levelButtonData.levelNumber = i + 1;
                levelButtonData.levelStar = 0; // Locked Level Has No Star
            }

            levelButtonData.lockedLevelMainSprite = lockedLevelMainSprite;
            levelButtonData.openLevelMainSprite = openLevelMainSprite;
            levelButtonData.star = starSprite;
            levelButtonData.starMissing = starMissingSprite;

            LevelObjects[i].ApplyLevelData(levelButtonData);
        }
    }

    #endregion

    public void OpenSettingsMenu()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");

            LevelSelectMenu.SetActive(false);
            MainMenu.SetActive(false);
            CharacterSelectMenu.SetActive(false);
            SettingsMenu.SetActive(true);
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

[System.Serializable]
public class LevelButtonData
{
    public bool isOpen;
    public Sprite lockedLevelMainSprite;
    public Sprite openLevelMainSprite;
    public int levelNumber;
    public int levelStar;
    public Sprite star;
    public Sprite starMissing;
}

public class CharacterSkins
{
    public enum SkinTypes { Normal, Elite, Rear, Premium, Gift };
    public enum SkinGender { Male, Female };

    public bool isActive;
    public int Id;
    public string Name;
    public SkinTypes Type;
    public SkinGender Gender;
    public float Price;
    public Sprite Image;
}