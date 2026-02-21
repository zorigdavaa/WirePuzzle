using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUI : MonoBehaviour
{
    public PowerUp powerUp;
    public Image icon;
    public TextMeshProUGUI title;
    public Text desc;
    public Text priceText;
    public Button adBuyButton;
    public Button coinBuyButton;

    void Start()
    {
        if (powerUp != null)
        {
            Initialize(powerUp);
        }

        adBuyButton.onClick.AddListener(BuyPowerUpWithAd);
        coinBuyButton.onClick.AddListener(BuyPowerUp);
    }

    public void Initialize(PowerUp newPowerUp)
    {
        powerUp = newPowerUp;
        powerUp.powerUpBuyUI = this;
        icon.sprite = newPowerUp.icon;
        title.text = newPowerUp.name;
        desc.text = newPowerUp.desc;
        priceText.text = powerUp.addCoinCost.ToString();
    }

    public void BuyPowerUp()
    {
        PowerUpHandler.Instance.BuyPowerUp(powerUp);
    }
    public void BuyPowerUpWithAd()
    {
        Debug.Log($"Attempting to buy {powerUp.name} with ad...");
        PowerUpHandler.Instance.BuyPowerUpWithAd(powerUp);
    }
}