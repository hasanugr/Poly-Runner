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

    [Header("In Game Fields")]
    public GameObject InGameHolder;
    public InGameManager _igm;

    private void Awake()
    {
        MakeSingleton();
        LoadPlayerData();
    }

    #region Standarts
    public void StartLevel(int level)
    {
        MenuHolder.SetActive(false);
        InGameHolder.SetActive(true);
        _igm.StartGame(level);
    }

    public void GoToMenu()
    {
        InGameHolder.SetActive(false);
        MenuHolder.SetActive(true);
    }
    #endregion

    public void ChangeMusicStatus(bool status)
    {
        Debug.Log("Music: " + status);
        //pd.Music = status;
    }
    public void ChangeSoundStatus(bool status)
    {
        Debug.Log("Sound: " + status);
        //pd.Sound = status;
    }

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