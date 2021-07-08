using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using AmazingAssets.CurvedWorld;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("General")]
    public CameraFollow cameraFollow;
    public GameObject player;
    public Character playerController;
    public bool isGameActive = false;

    [Header("Game ReLoad")]
    [SerializeField] StartLine startLine;
    [SerializeField] FinishLine finishLine;
    [SerializeField] LevelHolderControl[] levelHolderControls;
    [SerializeField] LevelController levelController;
    [SerializeField] int activeLevel = 1;

    /*[Header("Action Control")]
    public bool IsBossActive = false;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;*/

    private int _collectedGold;
    private int _levelMaxGold;
    private int _calculatedStar;

    [Header("Game Countdown Timer")]
    public float TimerMaxValue = 3;
    public GameObject TimerUIObj;
    public TextMeshProUGUI TimerUI;

    private float _timerCountdown;

    [Header("UI Screen")]
    public GameObject InGameUI;
    public GameObject PausePanel;
    public GameObject SettingsPanel;
    public GameObject GameOverPanel;
    public GameObject FinishLevelPanel;
    public GameObject GoldIcon;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI PauseGoldText;

    [Header("Curved World Control")]
    public CurvedWorldController curverWorldController;
    public enum CurvedWorldType { Ramp, Normal, NormalLeft, NormalRight };
    public CurvedWorldType currentType = CurvedWorldType.NormalLeft;

    private float[] curvedDataNormal = { 0, 0, -1, 30 };
    private float[] curvedDataNormalLeft = { -1, 15, -1, 30 };
    private float[] curvedDataNormalRight = { 1, 15, -1, 30 };
    private float[] curvedDataRamp = { 0, 15, 2, 5 };

    private bool _buttonBlock;
    AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.instance;
        MakeSingleton();
        //StartGame(1);
    }

    private void OnEnable()
    {
        _buttonBlock = false;
    }

    IEnumerator CountdownToStart()
    {
        startLine.StartProcess();

        TimerUIObj.SetActive(true);
        Color textColor = TimerUI.color;
        textColor.a = 1;
        TimerUI.color = textColor;

        while (_timerCountdown > 0)
        {
            yield return new WaitForSeconds(1f);
            TimerUI.text = _timerCountdown.ToString();
            LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1f), 0.5f).setEasePunch();
            _timerCountdown--;
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "Countdown");
        }

        yield return new WaitForSeconds(1f);
        TimerUI.text = "GO";
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "CountdownGo");
        isGameActive = true;
        playerController.StartPlayer();
        LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1f), 1f).setEaseInExpo();
        LeanTween.value(TimerUIObj, 0, 1f, 1f).setOnUpdate((float val) =>
        {
            Color textColor = TimerUI.color;
            textColor.a = 1 - val;
            TimerUI.color = textColor;
        });
        cameraFollow.isInCinematic = false;
        yield return new WaitForSeconds(1f);
        TimerUI.text = "";
        LeanTween.scale(TimerUIObj, new Vector3(1f, 1f, 1f), 0.1f);
        TimerUIObj.SetActive(false);
    }

    public void StartGame(int level)
    {
        LeanTween.cancelAll();
        StopAllCoroutines();
        Time.timeScale = 1;

        cameraFollow.ResetCamera();
        playerController.ResetPlayer();
        levelController.CreateLoadedLevel(level);

        activeLevel = level;
        currentType = CurvedWorldType.NormalLeft;
        _collectedGold = 0;
        _levelMaxGold = levelController.Levels[0].LevelNumber;
        GoldText.text = _collectedGold.ToString();
        PauseGoldText.text = _collectedGold.ToString();
        TimerUI.text = "";
        GameOverPanel.SetActive(false);
        FinishLevelPanel.SetActive(false);
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(false);
        InGameUI.SetActive(true);

        _timerCountdown = TimerMaxValue;
        StartCoroutine(CountdownToStart());
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        InGameUI.SetActive(false);
        Time.timeScale = 0f;
        isGameActive = false;
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        InGameUI.SetActive(true);
        Time.timeScale = 1f;
        isGameActive = true;
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");
    }

    public void Settings()
    {
        SettingsPanel.SetActive(true);
        PausePanel.SetActive(false);
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");
    }

    
    public void BackToPause()
    {
        SettingsPanel.SetActive(false);
        PausePanel.SetActive(true);
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "MenuButton");
    }

    public void Restart()
    {
        StartGame(activeLevel);
    }

    public void NextLevel()
    {
        StartGame(activeLevel + 1);
    }

    public void GoHome()
    {
        GameManager.instance.GoToMenu();
    }

    public void GoLevelSelect()
    {
        GameManager.instance.GoToLevelSelect();
    }

    public void DoubleDiamondAds()
    {
        if (!_buttonBlock)
        {
            StartCoroutine(ButtonCooldown(0.5f));

            AdmobManager.instance.ShowRewardedAd();
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        float distance = cameraFollow.distance;
        float height = cameraFollow.height;
        LeanTween.value(GameOverPanel, 0, 2f, 3f).setOnUpdate((float val) =>
        {
            cameraFollow.distance = distance + val;
            cameraFollow.height = height + val;
        }).setIgnoreTimeScale(true);


        LeanTween.value(GameOverPanel, 1, 0.5f, 1f).setOnUpdate((float val) =>
        {
            Time.timeScale = val;
        }).setIgnoreTimeScale(true);

        GameOverPanel.SetActive(true);
        InGameUI.SetActive(false);

        AdmobManager.instance.ShowInterstitial();
    }

    #region Finish
    public void Finish()
    {
        cameraFollow.isReverseCamera = true;
        isGameActive = false;
        playerController.FinishMoves();

        AddFinishPoint();

        StartCoroutine(FinishWithDelay(3f));
    }
    private void AddFinishPoint()
    {
        int levelCollectedGold = GetGold();
        float percentComplete = ((float)levelCollectedGold / ((float)_levelMaxGold * 10)) * 100;
        Debug.Log("Max: " + _levelMaxGold + " Current: " + levelCollectedGold + " Percent: %" + percentComplete);

        if (percentComplete > 70)
        {
            _calculatedStar = 3;
        }
        else if(percentComplete > 40)
        {
            _calculatedStar = 2;
        }
        else
        {
            _calculatedStar = 1;
        }

        GameManager.instance.pd.NextLevel(activeLevel, _calculatedStar);
        GameManager.instance.SavePlayerData();
    }

    IEnumerator FinishWithDelay(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        FinishLevelPanel.SetActive(true);
        InGameUI.SetActive(false);

        AdmobManager.instance.ShowInterstitial();
    }
    #endregion


    public void AddGold(int count)
    {
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.Environment, "CollectDiamond");
        _collectedGold += count;
        LeanTween.scale(GoldIcon, new Vector3(1.5f, 1.5f, 1), 0.5f).setEasePunch();
        GoldText.text = _collectedGold.ToString();
        PauseGoldText.text = _collectedGold.ToString();
    }

    public int GetGold() { return _collectedGold; }
    public int GetMaxGold() { return _levelMaxGold; }
    public int GetCalculatedStar() { return _calculatedStar; }

    #region Curved World Control
    public void CurvedWorldRamp(bool isActive)
    { 
        if (isActive)
        {
            LeanTween.value(gameObject, -1, 0, 3f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendHorizontalSize(val);
            });

            LeanTween.value(gameObject, -1, 0, 1.5f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendVerticalSize(val);
            });

            LeanTween.value(gameObject, 30, 70, 1.5f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendVerticalOffset(val);
            }).setOnComplete(() => {

                LeanTween.value(gameObject, 0, 3, 1.5f).setOnUpdate((float val) =>
                {
                    curverWorldController.SetBendVerticalSize(val);
                });

                LeanTween.value(gameObject, 70, 15, 1.5f).setOnUpdate((float val) =>
                {
                    curverWorldController.SetBendVerticalOffset(val);
                });

            });
        }else
        {
            LeanTween.value(gameObject, 0, -1, 3f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendHorizontalSize(val);
            });

            LeanTween.value(gameObject, 3, 1, 1.5f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendVerticalSize(val);
            });

            LeanTween.value(gameObject, 15, 5, 1.5f).setOnUpdate((float val) =>
            {
                curverWorldController.SetBendVerticalOffset(val);
            }).setOnComplete(() => {

                LeanTween.value(gameObject, 1, -1, 1.5f).setOnUpdate((float val) =>
                {
                    curverWorldController.SetBendVerticalSize(val);
                });

                LeanTween.value(gameObject, 5, 30, 1.5f).setOnUpdate((float val) =>
                {
                    curverWorldController.SetBendVerticalOffset(val);
                });

            });
        }
    }
    public void CurvedWorldDefault()
    {
        curverWorldController.SetBendHorizontalSize(-1);
        curverWorldController.SetBendHorizontalOffset(15);
        curverWorldController.SetBendVerticalSize(-1);
        curverWorldController.SetBendVerticalOffset(30);
    }

    public void CurvedWorldChange(CurvedWorldType type, float time)
    {
        // Data Type -> [ HorizontalSize, HorizontalOffset, VerticalSize, VerticalOffset ]
        float[] oldData = GetCurvedData(currentType);
        float[] newData = GetCurvedData(type);
        currentType = type;

        LeanTween.value(gameObject, oldData[0], newData[0], time).setOnUpdate((float val) =>
        {
            curverWorldController.SetBendHorizontalSize(val);
        });
        LeanTween.value(gameObject, oldData[1], newData[1], time + (time * 0.5f)).setOnUpdate((float val) =>
        {
            curverWorldController.SetBendHorizontalOffset(val);
        });
        LeanTween.value(gameObject, oldData[2], newData[2], time).setOnUpdate((float val) =>
        {
            curverWorldController.SetBendVerticalSize(val);
        });
        LeanTween.value(gameObject, oldData[3], newData[3], time).setOnUpdate((float val) =>
        {
            curverWorldController.SetBendVerticalOffset(val);
        });
    }

    private float[] GetCurvedData(CurvedWorldType type)
    {
        return type switch
        {
            CurvedWorldType.Normal => curvedDataNormal,
            CurvedWorldType.NormalLeft => curvedDataNormalLeft,
            CurvedWorldType.NormalRight => curvedDataNormalRight,
            CurvedWorldType.Ramp => curvedDataRamp,
            _ => curvedDataNormal,
        };
    }
    #endregion

    IEnumerator ButtonCooldown(float time)
    {
        _buttonBlock = true;

        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSecondsRealtime(time);

        _buttonBlock = false;
    }

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
