using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureBar : MonoBehaviour
{
    public TextMeshProUGUI featureText;
    public Image barFillImage;
    public Image featureIcon;
    
    
    public void UpdateBar(Sprite icon, int max, int value)
    {
        featureIcon.sprite = icon;
        barFillImage.fillAmount = (float)value / max;
        featureText.text = $"{value}/{max}";
    }
}
