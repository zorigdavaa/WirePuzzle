using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    Slider slider;
    public TextMeshProUGUI text;

    public void SetValue(float value)
    {
        text.text = value.ToString("0");
    }
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SetValue);
        SetValue(slider.value);
    }
}
