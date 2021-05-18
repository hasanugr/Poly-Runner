using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameManager : MonoBehaviour
{
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



    private void Start()
    {
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
        yield return new WaitForSeconds(1f);
        TimerUIObj.SetActive(false);
    }

    public void Reset()
    {
        player.transform.position = startPosition;
        player.transform.rotation = startRotation;
    }

    public void GameOver()
    {
        isGameActive = false;
    }

    public void Finish()
    {
        isGameActive = false;
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
    }
}
