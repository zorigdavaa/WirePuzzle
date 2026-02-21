using System;
using UnityEngine;
using UnityEngine.Events;
using ZPackage;

public class PowerUpHandler : MonoBehaviour
{
    public static PowerUpHandler Instance;
    public PowerUp[] powerUps;
    public PowerUpUI[] powerUpUIs;
    public int[] unlockLoseRequirement;
    public int[] unlockLevelRequirement;

    public PowerUp powerUpPrefab;
    public PowerUpUI powerUpUIPrefab;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePowerUpUIs();
    }

    private void OnEnable()
    {
        GameManager.Instance.StateChanged += OnGameStateChange;
        // CanvasManager.OnPlaying += CheckPowerUpTutorial;
    }

    private void OnGameStateChange(object caller, GameState state)
    {
        if (state == GameState.Playing)
        {
            CheckPowerUpLevelUnlock();
            // CheckPowerUpTutorial();
        }
    }



    public void UnlockPowerUp(PowerUp powerUp)
    {
        if (GameManager.Instance.Coin >= powerUp.unlockCoinCost)
        {
            CanvasManager.Instance.RemoveCoin(powerUp.unlockCoinCost);
            powerUp.Unlock();
        }
    }
    public void BuyPowerUp(PowerUp powerUp)
    {
        if (GameManager.Instance.Coin >= powerUp.addCoinCost)
        {
            CanvasManager.Instance.RemoveCoin(powerUp.addCoinCost);
            powerUp.AddCountPowerUp();
            powerUp.powerUpBuyUI.gameObject.SetActive(false);
            Debug.Log($"Bought powerup: {powerUp.name}");
        }
    }
    public void BuyPowerUpWithAd(PowerUp powerUp)
    {
        if (GameManager.Instance.Coin >= powerUp.addCoinCost)
        {
            CanvasManager.Instance.RemoveCoin(powerUp.addCoinCost);
            powerUp.AddCountPowerUp();
            powerUp.powerUpBuyUI.gameObject.SetActive(false);
            Debug.Log($"Bought powerup: {powerUp.name}");
        }
    }

    public void InitializePowerUpUIs()
    {
        foreach (var ui in powerUpUIs)
        {
            ui.Initialize(ui.powerUp);
        }
    }

    // public void CheckPowerUpTutorial()
    // {
    //     for (int i = 0; i < unlockLoseRequirement.Length; i++)
    //     {
    //         if (unlockLoseRequirement[i] <= PlayerPrefs.GetInt("loseTime", 0))
    //         {
    //             // Tutorial.instance.PowerUpTutorial();
    //         }
    //     }
    // }
    private void CheckPowerUpLevelUnlock()
    {
        foreach (var item in unlockLevelRequirement)
        {
            if (item <= GameManager.Instance.Level)
            {
                var index = Array.IndexOf(unlockLevelRequirement, item);
                if (index >= 0 && index < powerUps.Length)
                {
                    powerUps[index].Unlock();
                }
            }
        }
    }
}
