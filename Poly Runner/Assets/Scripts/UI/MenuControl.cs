using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject CharacterSelectMenu;
    public GameObject LevelSelectMenu;
    public GameObject SettingsMenu;

    public GameObject CharacterSelectShowScreen;

    private void OnEnable()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        CharacterSelectMenu.SetActive(false);
        CharacterSelectShowScreen.SetActive(false);
        LevelSelectMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void OpenCharacterList()
    {
        LevelSelectMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(false);
        CharacterSelectMenu.SetActive(true);
        CharacterSelectShowScreen.SetActive(true);
    }

    public void OpenLevelSelect()
    {
        CharacterSelectMenu.SetActive(false);
        CharacterSelectShowScreen.SetActive(false);
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(false);
        LevelSelectMenu.SetActive(true);
    }

    public void OpenSettingsPopup()
    {
        SettingsMenu.SetActive(true);
    }
    public void CloseSettingsPopup()
    {
        SettingsMenu.SetActive(false);
    }
}
