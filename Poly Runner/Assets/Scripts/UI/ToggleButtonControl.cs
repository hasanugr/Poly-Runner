using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonControl : MonoBehaviour
{
    public enum TogglePlatforms { Music, Sound };
    [SerializeField] private TogglePlatforms togglePlatofrm;
    [SerializeField] private Image bgOnImage;
    [SerializeField] private RectTransform togglePoint;
    [SerializeField] private float togglePointMin = -30;
    [SerializeField] private float togglePointMax = 30;
    [Range(0.001f, 1f)]
    [SerializeField] private float toggleAnimationTime = 0.1f;
    [Range(0f, 2f)]
    [SerializeField] private float toggleBlockTime = 0.5f;
     
    AudioManager _audioManager;

    private float _togglePointBetween;
    private bool _status;
    private bool _toggleBlock;

    private void Start()
    {
        _audioManager = AudioManager.instance;
    }

    private void OnEnable()
    {
        _toggleBlock = false;

        if (togglePlatofrm == TogglePlatforms.Music)
        {
            _status = GameManager.instance.pd.Music;
        }
        else if (togglePlatofrm == TogglePlatforms.Sound)
        {
            _status = GameManager.instance.pd.Sound;
        }

        _togglePointBetween = togglePointMax + (-1 * togglePointMin);

        if (_status)
        {
            Color tempColor = bgOnImage.color;
            tempColor.a = 1;
            bgOnImage.color = tempColor;
            togglePoint.localPosition = new Vector3(togglePointMax, 0, 0);
        }
        else
        {
            Color tempColor = bgOnImage.color;
            tempColor.a = 0;
            bgOnImage.color = tempColor;
            togglePoint.localPosition = new Vector3(togglePointMin, 0, 0);
        }
    }

    public void ToggleIt()
    {
        if (!_toggleBlock)
        {
            StartCoroutine(ToggleCooldown(toggleBlockTime));

            //_audioManager.PlayOneShot("ToggleButton");
            _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.UI, "ToggleButton");

            _status = !_status;
            Toggleing(_status);

            if (togglePlatofrm == TogglePlatforms.Music)
            {
                GameManager.instance.ChangeMusicStatus(_status);
            }
            else if (togglePlatofrm == TogglePlatforms.Sound)
            {
                GameManager.instance.ChangeSoundStatus(_status);
            }
        }
    }
    private void Toggleing(bool status)
    {
        if (status)
        {
            Color tempColor = bgOnImage.color;
            LeanTween.value(gameObject, 0f, 1f, toggleAnimationTime).setOnUpdate((float val) =>
            {
                tempColor.a = val;
                bgOnImage.color = tempColor;

                float xPos = _togglePointBetween * val;
                togglePoint.localPosition = new Vector3(togglePointMin + xPos, 0, 0);
            }).setIgnoreTimeScale(true);
        }else
        {
            Color tempColor = bgOnImage.color;
            LeanTween.value(gameObject, 1f, 0f, toggleAnimationTime).setOnUpdate((float val) =>
            {
                tempColor.a = val;
                bgOnImage.color = tempColor;

                float xPos = _togglePointBetween * val;
                togglePoint.localPosition = new Vector3(togglePointMin + xPos, 0, 0);
            }).setIgnoreTimeScale(true);
        }
    }

    IEnumerator ToggleCooldown(float time)
    {
        _toggleBlock = true;

        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSecondsRealtime(time);

        _toggleBlock = false;
    }
}
