using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI TotalGoldValue;
    public GameObject TouchBlockPanel;

    private int _totalPlayerGold;

    AudioManager _audioManager;

    private void OnEnable()
    {
        _audioManager = AudioManager.instance;

        _totalPlayerGold = GameManager.instance.pd.Gold;
        TotalGoldValue.text = _totalPlayerGold.ToString();

        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "GameOver");

        StartCoroutine(DisableTheTouchBlock(1.5f));
    }

    private void OnDisable()
    {
        TouchBlockPanel.SetActive(true);
    }

    IEnumerator DisableTheTouchBlock(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        AdmobManager.instance.ShowInterstitial();
        TouchBlockPanel.SetActive(false);
    }
}
