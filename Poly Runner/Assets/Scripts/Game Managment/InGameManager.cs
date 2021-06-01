using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("General")]
    public CameraFollow cameraFollow;
    public GameObject player;
    public Character playerController;
    public bool isGameActive = false;

    [Header("Action Control")]
    public bool IsBossActive = false;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;

    private int point;
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



    private void Start()
    {
        MakeSingleton();

        _timerCountdown = TimerMaxValue;
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while (_timerCountdown > 0)
        {
            yield return new WaitForSeconds(1f);
            TimerUI.text = _timerCountdown.ToString();
            LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1), 0.5f).setEasePunch();
            _timerCountdown--;
        }

        yield return new WaitForSeconds(1f);
        TimerUI.text = "GO";
        isGameActive = true;
        playerController.StartPlayer();
        LeanTween.scale(TimerUIObj, new Vector3(1.5f, 1.5f, 1), 1f).setEaseInExpo();
        LeanTween.value(TimerUIObj, 0, 1f, 1f).setOnUpdate((float val) =>
        {
            Color textColor = TimerUI.color;
            textColor.a = 1 - val;
            TimerUI.color = textColor;
        });
        cameraFollow.isInCinematic = false;
        yield return new WaitForSeconds(1f);
        TimerUIObj.SetActive(false);
    }

    public void Pause()
    {
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
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
