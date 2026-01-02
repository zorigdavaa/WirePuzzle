using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;
    public int health = 5;
    public int MaxHealth = 5;
    public Image infinityImg;
    private DateTime activationTime;
    private TimeSpan infiniteHealthDuration;
    public bool infiniteHealthActive = false;

    private double healthCountdown = 0;
    private double infiniteHealthCountdown = 0;
    bool isInited = false;
    public Text healthTxt;
    public Text countDownTxt;

    void Awake()
    {
        Instance = this;
        if (PlayerPrefs.GetString("TimeRecord", "null") == "null")
        {
            PlayerPrefs.SetString("TimeRecord", DateTime.Now.Add(new TimeSpan(0, 0, -30 * MaxHealth, 0)).ToSafeString());
        }
        CalculateHealth();
        CalculateInfiniteHealthDuration();
        if (!isInited)
        {
            isInited = true;
        }

    }
    void OnDestroy()
    {
        if (isInited)
        {
            isInited = false;
        }
    }

    // void OnGameStateChange(GameState state)
    // {
    //     if (state == GameState.Starting)
    //     {

    //     }
    //     else if (state == GameState.Playing)
    //     {
    //         HealthLose(1);
    //         // A.CC.ChangeTheParent(A.CC.transform, 1, true);
    //     }
    //     else if (state == GameState.Revive)
    //     {
    //         HealthLose(-1);
    //     }
    //     else if (state == GameState.LevelCompleted)
    //     {
    //         HealthLose(-1);
    //     }
    // }

    void Update()
    {
        if (infiniteHealthCountdown > 0)
        {
            infiniteHealthCountdown -= Time.deltaTime;
            UpdateUI(true);
        }
        else
        {
            DeactivateInfiniteHealth();
        }

        if (health < MaxHealth)

        {
            healthCountdown -= Time.deltaTime;

            if (healthCountdown < 0)
            {
                health++;
                healthCountdown = 1800;
            }
            UpdateUI(false);
        }
        else
        {

            UpdateUI(true);
        }
    }

    public void CalculateHealth()
    {
        TimeSpan temp = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("TimeRecord"));
        double tempHealth = temp.TotalSeconds / 1800;
        double countdown = temp.TotalSeconds % 1800;

        health = Mathf.Clamp((int)tempHealth, 0, MaxHealth);
        if (health == MaxHealth) countdown = 0;


        healthCountdown = 1800 - countdown;

        double zeroHealthTime = health * 1800 + countdown;
        DateTime temp2 = DateTime.Now.Add(-TimeSpan.FromSeconds(zeroHealthTime));

        PlayerPrefs.SetString("TimeRecord", temp2.ToSafeString());
    }
    void CalculateInfiniteHealthDuration()
    {
        if (PlayerPrefs.GetString("InfiniteHealth", "null") == "null")
        {
            return;
        }
        TimeSpan temp = DateTime.Parse(PlayerPrefs.GetString("InfiniteHealth")) - DateTime.Now;
        infiniteHealthCountdown = temp.TotalSeconds;

        if (infiniteHealthCountdown > 0)
        {
            infinityImg.gameObject.SetActive(true);
            healthTxt.gameObject.SetActive(false);
            infiniteHealthActive = true;
        }
    }
    public void IncreaseHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, MaxHealth);

        if (amount > 0)
        {
            PlayerPrefs.SetString("TimeRecord", DateTime.Parse(PlayerPrefs.GetString("TimeRecord")).Add(new TimeSpan(0, 0, -30 * amount, 0)).ToSafeString());
        }
        CalculateHealth();
    }
    public void HealthLose(int amount)
    {
        if (infiniteHealthCountdown > 0) return;

        health = Mathf.Clamp(health - amount, 0, MaxHealth);

        double countdown = 1800 - healthCountdown;
        double totalSeconds = health * 1800 + countdown;
        DateTime newTime = DateTime.Now.AddSeconds(-totalSeconds);
        PlayerPrefs.SetString("TimeRecord", newTime.ToSafeString());

        CalculateHealth();
    }
    public void SetHealth(int amount)
    {
        health = amount;
        UpdateUI(health >= MaxHealth);
        // Debug.Log($"Set Health {health} amount   {amount}");
    }

    public void UpdateUI(bool isMax = false)
    {
        healthTxt.text = health.ToString();
        int countdownHealthTxt = (int)healthCountdown;
        int infiniteCountdownHealthTxt = (int)infiniteHealthCountdown;
        string addOne = "";
        string addTwo = "";
        if (isMax)
        {
            countDownTxt.text = "MAX";
        }
        else
        {

            if (countdownHealthTxt / 60 < 10)
                addOne = "0";
            if (countdownHealthTxt % 60 < 10)
                addTwo = "0";

            countDownTxt.text = addOne + (countdownHealthTxt / 60).ToString() + ":" + addTwo + (countdownHealthTxt % 60).ToString();
        }

        if (infiniteHealthCountdown > 0)
        {
            if (infiniteCountdownHealthTxt / 3600 < 10)
                addOne = "0";
            if (infiniteCountdownHealthTxt % 3600 / 60 < 10)
                addTwo = "0";

            countDownTxt.text = $"{infiniteCountdownHealthTxt / 3600:D2}" + ":" + $"{infiniteCountdownHealthTxt % 3600 / 60:D2}";
        }
    }

    public void ActivateInfiniteHealth(TimeSpan duration)
    {
        if (infiniteHealthActive)
        {
            DateTime existingEndTime = DateTime.Parse(PlayerPrefs.GetString("InfinitHealth"));
            PlayerPrefs.SetString("InfiniteHealth", existingEndTime.Add(duration).ToSafeString());

        }
        else
        {
            PlayerPrefs.SetString("InfiniteHealth", DateTime.Now.Add(duration).ToSafeString());
        }
        infinityImg.gameObject.SetActive(true);
        healthTxt.gameObject.SetActive(false);
        infiniteHealthActive = true;

        CalculateInfiniteHealthDuration();
        // UpdateUI(false, duration);
    }

    private void DeactivateInfiniteHealth()
    {
        infinityImg.gameObject.SetActive(false);
        healthTxt.gameObject.SetActive(true);
        infiniteHealthActive = false;
        // UpdateUI(false);
    }
    public void SetFullHealth()
    {
        int previousMax = MaxHealth;
        health = MaxHealth;
        PlayerPrefs.SetString("TimeRecord", DateTime.Now.ToSafeString());
        //healthCountdown = 0;
        UpdateUI(true);
    }

}
