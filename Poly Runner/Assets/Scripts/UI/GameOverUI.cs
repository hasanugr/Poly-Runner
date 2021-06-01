using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Image DarkBackgroundImage;
    public TextMeshProUGUI PanelTitle;
    //public TextMeshProUGUI NewScoreName;
    //public TextMeshProUGUI NewScoreValue;
    //public TextMeshProUGUI HighScoreName;
    //public TextMeshProUGUI HighScoreValue;
    public TextMeshProUGUI GoldValue;
    public Image GoldIcon;
    public Image RestartButtonBG;
    public TextMeshProUGUI RestartButtonText;
    public Image MenuButtonBG;
    public TextMeshProUGUI MenuButtonText;
    public GameObject TouchBlockPanel;

    private int _newScore;
    private int _highScore;
    private int _gold;

    private InGameManager _igm;
    private GameManager _gm;

    private void OnEnable()
    {
        _igm = FindObjectOfType<InGameManager>();

        //AddUIValues();
        CalculateGold();
        StartCoroutine(DisableTheTouchBlock(1.5f));

        LeanTween.value(gameObject, 0, 1f, 1f).setOnUpdate((float val) =>
        {
            ApplyImageColor(DarkBackgroundImage, Mathf.Clamp(val, 0, 0.80f));
            ApplyImageColor(GoldIcon, val);
            ApplyImageColor(RestartButtonBG, val);
            ApplyImageColor(MenuButtonBG, val);

            ApplyTextColor(PanelTitle, val);
            /*ApplyTextColor(NewScoreName, val);
            ApplyTextColor(NewScoreValue, val);
            ApplyTextColor(HighScoreName, val);
            ApplyTextColor(HighScoreValue, val);*/
            ApplyTextColor(GoldValue, val);
            ApplyTextColor(RestartButtonText, val);
            ApplyTextColor(MenuButtonText, val);

        }).setIgnoreTimeScale(true);
    }

    private void OnDisable()
    {
        TouchBlockPanel.SetActive(true);
    }

    private void CalculateGold()
    {
        var seq = LeanTween.sequence();

        _gold = _igm.GetGold();

        seq.append(LeanTween.value(gameObject, 0, _gold, 3f).setOnUpdate((float val) =>
        {
            GoldValue.text = Mathf.RoundToInt(val).ToString();
        }).setIgnoreTimeScale(true));

        seq.append(LeanTween.scale(GoldIcon.gameObject.transform.parent.gameObject, new Vector3(1.2f, 1.2f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch());
    }

    /*private void AddUIValues()
    {
        _newScore = _igm.GetPlayerPoint();
        _highScore = _gm.pd.highScore;

        if (_newScore > _highScore)
        {
            _highScore = _newScore;
            _gm.pd.highScore = _newScore;
            LeanTween.scale(HighScoreName.gameObject.transform.parent.gameObject, new Vector3(1.2f, 1.2f, 1f), 1f).setIgnoreTimeScale(true).setEasePunch().setLoopPingPong();
        }

        NewScoreValue.text = _newScore.ToString();
        HighScoreValue.text = _highScore.ToString();
    }*/

    private void ApplyImageColor(Image image, float val)
    {
        Color textColor = image.color;
        textColor.a = val;
        image.color = textColor;
    }

    private void ApplyTextColor(TextMeshProUGUI text, float val)
    {
        Color textColor = text.color;
        textColor.a = val;
        text.color = textColor;
    }

    IEnumerator DisableTheTouchBlock(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        TouchBlockPanel.SetActive(false);
    }
}
