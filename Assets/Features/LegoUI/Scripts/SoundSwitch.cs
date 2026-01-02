using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSwitch : MonoBehaviour
{
   [SerializeField] RectTransform uiHandle;
    private Color backgroundActive = new Color(86f / 255f, 134f / 255f, 25f / 255f, 1f);
    private Color fillActive = new Color(117f / 255f, 181f / 255f, 35f / 255f, 1f);

    Image backgroundImage, fillImage;
    Color backgroundDefaultColor, fillDefaultColor;

    Toggle toggle;
    Vector2 handlePosition;

    public Image soundOn, soundOff;
    public Toggle soundToggle;
    public Toggle vibrationToggle;
    [SerializeField] private bool isVibratorButton = false;

    private void Awake()
    {
        handlePosition = uiHandle.anchoredPosition;
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        fillImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;
        fillDefaultColor = fillImage.color;

        if (isVibratorButton)
            vibrationToggle.onValueChanged.AddListener(OnSwitch);
        else
            soundToggle.onValueChanged.AddListener(OnSwitchSound);

        Init();
    }
    private void OnEnable()
    {
        Init();
    }
    void Init()
    {
        if (isVibratorButton)
        {
            if (PlayerPrefs.GetInt("isVibratorOn", 0) == 1)
                OnSwitch(true);
            else
                OnSwitch(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("isSoundOn", 0) == 1)
                OnSwitchSound(true);
            else
                OnSwitchSound(false);
        }
    }

    void OnSwitch(bool on)
    {
        uiHandle.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        backgroundImage.color = on ? backgroundActive : backgroundDefaultColor;
        fillImage.color = on ? fillActive : fillDefaultColor;
        PlayerPrefs.SetInt("isVibratorOn", on ? 1 : 0);;
    }

    void OnSwitchSound(bool on)
    {
        uiHandle.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        backgroundImage.color = on ? backgroundActive : backgroundDefaultColor;
        fillImage.color = on ? fillActive : fillDefaultColor;
        soundOn.gameObject.SetActive(on);
        soundOff.gameObject.SetActive(!on);
        PlayerPrefs.SetInt("isSoundOn", on ? 1 : 0);;
    }

    void OnDestroy()
    {
        vibrationToggle.onValueChanged.RemoveListener(OnSwitch);
        soundToggle.onValueChanged.RemoveListener(OnSwitchSound);
    }
}
