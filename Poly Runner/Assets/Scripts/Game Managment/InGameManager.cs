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
    [SerializeField] int activeLevelIndex = 1;

    [Header("Action Control")]
    public bool IsBossActive = false;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;

    private int _collectedGold;

    [Header("Game Countdown Timer")]
    public float TimerMaxValue = 3;
    public GameObject TimerUIObj;
    public TextMeshProUGUI TimerUI;

    private float _timerCountdown;

    [Header("UI Screen")]
    public GameObject InGameUI;
    public GameObject PausePanel;
    public GameObject GameOverPanel;
    public GameObject FinishLevelPanel;
    public GameObject GoldIcon;
    public TextMeshProUGUI GoldText;

    [Header("Curved World Control")]
    public CurvedWorldController curverWorldController;
    public enum CurvedWorldType { Ramp, Normal, NormalLeft, NormalRight };
    public CurvedWorldType currentType = CurvedWorldType.NormalLeft;

    private float[] curvedDataNormal = { 0, 0, -1, 30 };
    private float[] curvedDataNormalLeft = { -1, 15, -1, 30 };
    private float[] curvedDataNormalRight = { 1, 15, -1, 30 };
    private float[] curvedDataRamp = { 0, 15, 2, 5 };

    private void Start()
    {
        MakeSingleton();

        _timerCountdown = TimerMaxValue;
        StartCoroutine(CountdownToStart());
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
        }

        yield return new WaitForSeconds(1f);
        TimerUI.text = "GO";
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

    public void Pause()
    {
        Debug.Log("Pause Clicked!!!");
        PausePanel.SetActive(true);
        InGameUI.SetActive(false);
        Time.timeScale = 0f;
        isGameActive = false;
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        InGameUI.SetActive(true);
        Time.timeScale = 1f;
        isGameActive = true;
    }

    public void Restart()
    {
        LeanTween.cancelAll();
        StopAllCoroutines();
        Time.timeScale = 1;

        startLine.ResetObstacle();
        finishLine.ResetObstacle();
        cameraFollow.ResetCamera();
        playerController.ResetPlayer();
        levelController.CreateLoadedLevel(activeLevelIndex);
        //levelHolderControls[activeLevelIndex].ResetLevel();

        currentType = CurvedWorldType.NormalLeft;
        _collectedGold = 0;
        GoldText.text = _collectedGold.ToString();
        GameOverPanel.SetActive(false);
        FinishLevelPanel.SetActive(false);
        PausePanel.SetActive(false);
        InGameUI.SetActive(true);

        _timerCountdown = TimerMaxValue;
        StartCoroutine(CountdownToStart());

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    }

    public void Finish()
    {
        cameraFollow.isReverseCamera = true;
        isGameActive = false;
        playerController.FinishMoves();

        StartCoroutine(FinishWithDelay(3f));
    }

    IEnumerator FinishWithDelay(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        FinishLevelPanel.SetActive(true);
        InGameUI.SetActive(false);
    }

    public void BossActivate()
    {
        if (IsBossActive)
        {
            IsBossActive = false;
            cameraFollow.isReverseCamera = false;
            playerController.isReverseMovement = false;
        }
        else
        {
            IsBossActive = true;
            cameraFollow.isReverseCamera = true;
            playerController.isReverseMovement = true;
        }
    }

    public void AddGold(int count)
    {
        _collectedGold += count;
        LeanTween.scale(GoldIcon, new Vector3(1.5f, 1.5f, 1), 0.5f).setEasePunch();
        GoldText.text = _collectedGold.ToString();
    }

    public int GetGold() { return _collectedGold; }

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
