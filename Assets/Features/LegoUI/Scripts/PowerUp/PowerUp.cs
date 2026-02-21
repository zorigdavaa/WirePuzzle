using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    [Header("PowerUP SO")]
    public PowerUpData powerUpData;

    [Header("UI Info")]
    public new string name;
    public string desc;
    public Sprite icon;

    [Header("State")]
    public bool isLocked = true;
    public bool isActive = false;
    public int count;
    
    [Header("Costs")]
    public int unlockCoinCost;
    public int addCoinCost;

    [Header("UI Refs")]
    public Text countText;
    public GameObject detailUI;
    public Image lockedImage;
    public PowerUpUI powerUpBuyUI;
    public Image iconUI;
    public Button useButton;
    
    public UnityAction OnPowerUpUse;
    public UnityAction OnPowerUpUnlock;

    public virtual void Awake()
    {
        LoadFromSO();
        
        OnPowerUpUse += () => { Debug.Log($"USING PowerUp: {name}"); };
        OnPowerUpUse += UpdateCountUI;
        OnPowerUpUnlock += UpdateUnlockUI;

        isLocked = PlayerPrefs.GetInt($"{name}-powerup", 0) == 0;
        count = PlayerPrefs.GetInt($"{name}-count", 0);
    }
    public virtual void Start()
    {
        HandleLockState();
        UpdateCountUI();

        isActive = false;
        
        if (useButton != null) useButton.onClick.AddListener(UsePowerUp);
        else Debug.LogWarning("PowerUp's use button not assigned in the inspector.");
    }
    private void LoadFromSO()
    {
        if (powerUpData == null) return;

        name = powerUpData.Name;
        desc = powerUpData.Description;
        icon = powerUpData.Icon;

        iconUI.sprite = icon;

        unlockCoinCost = powerUpData.unlockCoinCost;
        addCoinCost = powerUpData.addCoinCost;
    }
    
    public virtual void UsePowerUp()
    {
        if (isLocked)
        {
            Debug.Log("PowerUp is locked!");
            return;
        }

        if(isActive)
        {
            Debug.Log("PowerUp is active!");
            return;
        }

        if (count <= 0)
        {
            if (powerUpBuyUI != null)
            {
                powerUpBuyUI.gameObject.SetActive(true);
            }
        }
        else
        {
            count--;

            powerUpData.ApplyEffect(gameObject);

            OnPowerUpUse?.Invoke();

            isActive = true;

            PlayerPrefs.SetInt($"{name}-count", count);
        }
    }
    public virtual void UpdateCountUI()
    {
        countText.text = PlayerPrefs.GetInt($"{name}-count", 0).ToString();
    }
    public void HandleLockState()
    {
        if (isLocked) Lock();
        else Unlock();
    }
    public virtual void Unlock()
    {
        isLocked = false;
        OnPowerUpUnlock?.Invoke();

        PlayerPrefs.SetInt($"{name}-powerup", 1);
    }
    public virtual void Lock()
    {
        isLocked = true;
        PlayerPrefs.SetInt($"{name}-powerup", 0);
        UpdateLockUI();
    }
    public virtual void UpdateUnlockUI()
    {
        lockedImage.gameObject.SetActive(false);
        detailUI.gameObject.SetActive(true);
    }
    public virtual void UpdateLockUI()
    {
        lockedImage.gameObject.SetActive(true);
        detailUI.gameObject.SetActive(false);
    }
    public virtual void AddCountPowerUp()
    {
        count++;
        PlayerPrefs.SetInt($"{name}-count", count);
        
        UpdateCountUI();
    }

    public virtual void AddCountPowerUp(int amount)
    {
        count += amount;
        PlayerPrefs.SetInt($"{name}-count", count);
        
        UpdateCountUI();
    }
    
}
