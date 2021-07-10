using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishLevelUI : MonoBehaviour
{
    public TextMeshProUGUI TotalGoldValue;
    public TextMeshProUGUI GoldValue;
    public TextMeshProUGUI RewardedGoldValue;
    public GameObject TouchBlockPanel;
    public GameObject WatchAdsButton;
    public GameObject RewardedShow;

    public Sprite star;
    public Sprite starMissing;
    public GameObject StarsHolder;
    public GameObject[] StarObjects;
    public Image[] Stars;

    private int _totalPlayerGold;
    private int _collectedGold;
    private int _levelMaxGold;
    private LTDescr _rewardTween;

    private InGameManager _igm;
    AudioManager _audioManager;

    private void OnEnable()
    {
        AdmobManager.instance.OnAwardWon += RewardedGold;

        _igm = FindObjectOfType<InGameManager>();
        _audioManager = AudioManager.instance;

        _totalPlayerGold = GameManager.instance.pd.Gold;
        TotalGoldValue.text = _totalPlayerGold.ToString();
        _collectedGold = _igm.GetGold();
        _levelMaxGold = _igm.GetMaxGold();
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "Finish");

        ResetStars();
        CalculateGold();
        AddEarnedGold();

        AdmobManager.instance.ShowInterstitial();
        StartCoroutine(DisableTheTouchBlock(1.5f));
    }

    private void OnDisable()
    {
        AdmobManager.instance.OnAwardWon -= RewardedGold;

        LeanTween.cancel(RewardedShow);
        RewardedGoldValue.text = "";
        WatchAdsButton.SetActive(true);
        RewardedShow.SetActive(false);
        TouchBlockPanel.SetActive(true);
    }

    private void RewardedGold()
    {
        Debug.Log("Rewarded.!!!");
        RewardedGoldValue.text = _collectedGold.ToString();
        WatchAdsButton.SetActive(false);
        RewardedShow.SetActive(true);
        LeanTween.scale(RewardedShow, new Vector3(1.05f, 1.05f, 1f), 1.5f).setEasePunch().setLoopClamp();
        _totalPlayerGold = GameManager.instance.pd.Gold;

        LeanTween.value(gameObject, 0, _collectedGold, 3f).setOnUpdate((float val) =>
        {
            int roundedValue = Mathf.RoundToInt(val);
            GoldValue.text = (_collectedGold + roundedValue).ToString();
            TotalGoldValue.text = (_totalPlayerGold + roundedValue).ToString();
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "CollectDiamondIncrease");
        }).setOnComplete(() => {
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "StarGain3");
        }).setIgnoreTimeScale(true);

        AddEarnedGold();
    }

    private void ResetStars()
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].sprite = starMissing;
        }
    }

    private void CalculateGold()
    {
        var seq = LeanTween.sequence();
        int addedStarCount = 0;

        seq.append(LeanTween.value(gameObject, 0, _collectedGold, 3f).setOnUpdate((float val) =>
        {
            int roundedValue = Mathf.RoundToInt(val);
            GoldValue.text = roundedValue.ToString();
            TotalGoldValue.text = (_totalPlayerGold + roundedValue).ToString();
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "CollectDiamondIncrease");

            float percentGold = ((float)val / ((float)_levelMaxGold * 10)) * 100;
            if (percentGold > 70 && addedStarCount < 3)
            {
                Stars[addedStarCount].sprite = star;
                LeanTween.scale(StarObjects[addedStarCount], new Vector3(1.5f, 1.5f, 1f), .5f).setIgnoreTimeScale(true).setEasePunch();
                _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "StarGain3");
                addedStarCount++;
            }
            else if (percentGold > 40 && addedStarCount < 2)
            {
                Stars[addedStarCount].sprite = star;
                LeanTween.scale(StarObjects[addedStarCount], new Vector3(1.5f, 1.5f, 1f), .5f).setIgnoreTimeScale(true).setEasePunch();
                _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "StarGain2");
                addedStarCount++;
            }
            else if (addedStarCount < 1)
            {
                Stars[addedStarCount].sprite = star;
                LeanTween.scale(StarObjects[addedStarCount], new Vector3(1.5f, 1.5f, 1f), .5f).setIgnoreTimeScale(true).setEasePunch();
                _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "StarGain1");
                addedStarCount++;
            }
        }).setIgnoreTimeScale(true));

        seq.append(LeanTween.scale(StarsHolder, new Vector3(1.5f, 1.5f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch());
    }

    private void AddEarnedGold()
    {
        int existingGold = _totalPlayerGold;
        existingGold += _collectedGold;
        GameManager.instance.pd.Gold = existingGold;
        GameManager.instance.SavePlayerData();
    }

    IEnumerator DisableTheTouchBlock(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        TouchBlockPanel.SetActive(false);
    }
}
