using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Props")]
    public PlayerData pd;

    [Header("Menu Fields")]
    public GameObject MenuHolder;
    public MenuControl MenuControlScript;

    [Header("In Game Fields")]
    public GameObject InGameHolder;
    public InGameManager _igm;
    public CharacterSkin[] characterSkins;
    public CharacterSkin selectedCharacterSkin;
    public int lastSelectedSkinId = 0;

    private AudioManager _audioManager;
    private bool _inGameMode;

    private void Awake()
    {
        MakeSingleton();
        LoadPlayerData();
        CheckCharacters();
    }

    private void Start()
    {
        _audioManager = AudioManager.instance;

        if (pd.Music)
        {
            _audioManager.Play(AudioManager.AudioSoundTypes.Music, "MenuMusic");
        }
    }

    #region Standarts
    public void StartLevel(int level)
    {
        MenuHolder.SetActive(false);
        InGameHolder.SetActive(true);
        _igm.StartGame(level);
        _inGameMode = true;

        //_audioManager.Stop("MenuMusic");
        _audioManager.Stop(AudioManager.AudioSoundTypes.Music, "MenuMusic");
        if (pd.Music)
        {
            //_audioManager.PlayMusic("GameMusic");
            _audioManager.Play(AudioManager.AudioSoundTypes.Music, "GameMusic");
        }
    }

    public void GoToMenu()
    {
        InGameHolder.SetActive(false);
        MenuHolder.SetActive(true);
        MenuControlScript.MenuStart(0);
        Time.timeScale = 1f;
        _inGameMode = false;

        //_audioManager.Stop("GameMusic");
        _audioManager.Stop(AudioManager.AudioSoundTypes.Music, "GameMusic");
        if (pd.Music)
        {
            //_audioManager.PlayMusic("MenuMusic");
            _audioManager.Play(AudioManager.AudioSoundTypes.Music, "MenuMusic");
        }
    }

    public void GoToLevelSelect()
    {
        InGameHolder.SetActive(false);
        MenuHolder.SetActive(true);
        MenuControlScript.MenuStart(1);
        Time.timeScale = 1f;
        _inGameMode = false;

        //_audioManager.Stop("GameMusic");
        _audioManager.Stop(AudioManager.AudioSoundTypes.Music, "GameMusic");
        if (pd.Music)
        {
            //_audioManager.PlayMusic("MenuMusic");
            _audioManager.Play(AudioManager.AudioSoundTypes.Music, "MenuMusic");
        }
    }
    #endregion

    public void ChangeMusicStatus(bool status)
    {
        pd.Music = status;

        if (status)
        {
            if (_inGameMode)
            {
                //_audioManager.PlayMusic("GameMusic");
                _audioManager.Play(AudioManager.AudioSoundTypes.Music, "GameMusic");
            }
            else
            {
                //_audioManager.PlayMusic("MenuMusic");
                _audioManager.Play(AudioManager.AudioSoundTypes.Music, "MenuMusic");
            }
        }
        else
        {
            if (_inGameMode)
            {
                //_audioManager.Stop("GameMusic");
                _audioManager.Stop(AudioManager.AudioSoundTypes.Music, "GameMusic");
            }
            else
            {
                //_audioManager.Stop("MenuMusic");
                _audioManager.Stop(AudioManager.AudioSoundTypes.Music, "MenuMusic");
            }
        }

        SavePlayerData();
    }
    public void ChangeSoundStatus(bool status)
    {
        pd.Sound = status;
        SavePlayerData();
    }

    #region Character Select

    public void CheckCharacters()
    {
        for (int i = 0; i < characterSkins.Length; i++)
        {
            if (pd.IsCharacterActive(characterSkins[i].SkinId))
            {
                characterSkins[i].isActive = true;
            }else
            {
                characterSkins[i].isActive = false;
            }
        }
        ChangeSkin(pd.SelectedCharacterId);
    }

    public void BuySkin(int skinId, float skinPrice)
    {
        int playerGold = pd.Gold;
        int characterPrice = (int)skinPrice;
        if (playerGold >= characterPrice)
        {
            // Buy Character
            Debug.Log("Buy Character");
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "BuyCharacter");

            pd.ActivateCharacter(skinId);
            playerGold -= characterPrice;

            pd.Gold = playerGold;

            characterSkins[GetSkinIndex(skinId)].isActive = true;

            SavePlayerData();
        }
        else
        {
            // Need more money..
            Debug.Log("Need More Money.!!");
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "BuyFailCharacter");
        }
    }

    public void ChangeSkin(int skinId)
    {
        characterSkins[GetSkinIndex(lastSelectedSkinId)].DeactivateSkin();
        characterSkins[GetSkinIndex(skinId)].ActivateSkin();
        lastSelectedSkinId = skinId;
        pd.SelectedCharacterId = skinId;
        selectedCharacterSkin = characterSkins[GetSkinIndex(skinId)];

        SavePlayerData();
    }

    public int GetSkinIndex(int skinId)
    {
        int skinIndex = 0;
        for (int i = 0; i < characterSkins.Length; i++)
        {
            if (skinId == characterSkins[i].SkinId)
            {
                skinIndex = i;
                break;
            }
        }

        return skinIndex;
    }

    #endregion

    #region Save & Load
    private void LoadPlayerData()
    {
       pd = SaveLoadManager.Load();
    }

    public void SavePlayerData()
    {
        SaveLoadManager.Save(pd);
    }
    #endregion
    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}