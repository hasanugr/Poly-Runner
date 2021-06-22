using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonControl : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private GameObject bgOff;
    [SerializeField] private Image bgOffImage;
    [SerializeField] private RectTransform togglePoint;


    private bool _status;
    private void Start()
    {
        _status = GameManager.instance.pd.Music;
        toggle.isOn = _status;
    }
    public void ToggleIt(bool status)
    {
        Debug.Log("Toggled: " + status);
        if (status)
        {
            Color tempColor = bgOffImage.color;
            LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((float val) =>
            {
                tempColor.a = val;
                bgOffImage.color = tempColor;

            }).setIgnoreTimeScale(true);
        }else
        {
            Color tempColor = bgOffImage.color;
            LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((float val) =>
            {
                tempColor.a = val;
                bgOffImage.color = tempColor;

            }).setIgnoreTimeScale(true);
        }
    }
}
