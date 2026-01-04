using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using CandyKitSDK;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZPackage;
using Random = UnityEngine.Random;

public class CanvasManager : GenericSingleton<CanvasManager>
{
    public int Coin => GameManager.Instance.Coin;
    public int health = 0;

    [Header("Canvas Control")]
    public bool useMenuOnStart = false;
    public bool useLeaderboardProfile = false;
    public bool usePowerUpSlots = false;

    [Header("Elements")]
    public GameObject menu;
    public GameObject leaderboard;
    public GameObject powerUp;
    public GameObject IAPShop;
    public GameObject featureUnlocked;
    public Image featureUnlockedImage;

    [Header("HUD elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    public HealthManager healthManager;

    [Header("Level Complete")]
    public LevelCompleteUI levelCompleteUI;

    [Header("GameOver")]
    public GameObject outOfSpaceGameOver;
    public GameObject leaveGameOver;
    public GameObject youFailedGameOver;
    public GameObject outOfLiveGameOver;

    [Header("Screens")]
    public GameObject loadingScreen;

    [Header("Buttons")]
    public GameObject noAdsButton;

    [Header("Sprites")]
    public GameObject imagePrefab;
    public Sprite coinSprite;
    public Sprite heartSprite;


    void OnEnable()
    {
        GameManager.Instance.StateChanged += OnGameStateChange;
        GameManager.Instance.OnRevive += DeactivateAllGameOver;
        // GameController.OnTryAgain += DeactivateAllGameOver;
    }
    void OnDisable()
    {
        GameManager.Instance.StateChanged -= OnGameStateChange;
        GameManager.Instance.OnRevive -= DeactivateAllGameOver;
        // GameController.OnTryAgain -= DeactivateAllGameOver;
    }

    private void OnGameStateChange(object caller, GameState state)
    {
        if (state == GameState.Starting)
        {
            UpdateLevelText();
        }
        if (state == GameState.Playing)
        {
            SetActiveMenu(false);

        }
        else if (state == GameState.LevelCompleted)
        {
            LevelCompleteUI();
        }
        else if (state == GameState.GameOver)
        {
            // LeaveUI();
            OutOfSpaceUI();
            // if (GameManager.Instance.RetryCount > 2)
            // {

            // }
            // else
            // {
            //     OutOfSpaceUI();
            // }
        }
    }



    private void Start()
    {
        UpdateCoinText();
        UpdateLevelText();

        if (health != 0)
        {
            SetHealth(health);
        }

        if (useMenuOnStart) SetActiveMenu(true);
        if (useLeaderboardProfile) SetActiveLeaderboard(true);
        if (usePowerUpSlots) SetActivePowerUp(true);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //         YouFailedUI();
    //     }
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         LevelCompleteUI();
    //     }
    // }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddCoin(int amount)
    {
        GameManager.Instance.Coin += amount;
        UpdateCoinText();
        PlayerPrefs.SetInt("PlayerCoin", Coin);
    }
    public void RemoveCoin(int amount)
    {
        GameManager.Instance.Coin = Mathf.Max(0, GameManager.Instance.Coin - amount);
        UpdateCoinText();
        PlayerPrefs.SetInt("PlayerCoin", Coin);
    }

    public void UpdateLevelText()
    {
        // levelText.text = $"Level: {GameManager.Instance.Level}";
    }

    public bool TryRemoveCoin(int amount)
    {
        if (Coin >= amount)
        {
            RemoveCoin(amount);
            return true;
        }
        return false;
    }
    public void SetHealth(int amount)
    {
        healthManager.SetHealth(amount);
    }

    private void UpdateCoinText()
    {
        coinText.text = GameManager.Instance.Coin.ToString();
    }

    public void LevelCompleteUI()
    {
        // int levelCoin = Z.LS.currentLevel.FullBoxes.Count;
        levelCompleteUI.Activate(1, 10);
    }

    public void OutOfSpaceUI()
    {
        outOfSpaceGameOver.SetActive(true);
        // ChangeGameState(GameState.GameOver);
    }
    public void LeaveUI()
    {
        leaveGameOver.SetActive(true);

    }
    public void YouFailedUI()
    {
        youFailedGameOver.SetActive(true);

    }
    public void OutOfLiveUI()
    {
        outOfLiveGameOver.SetActive(true);
    }

    public void LevelCompleteButton()
    {
        GameManager.Instance.LevelComplete(this, 0);
        Restart();
    }
    public void ReviveCoin()
    {
        if (TryRemoveCoin(50))
        {
            GameManager.Instance.Revive();
            // Level.Instance.AddGraySlot(true);
            // A.LS.currentLevel.AddGraySlot(true);
        }
        else
        {
            //HAVE NOT ENOUGH COIN TO KEEP PLAYING
        }
    }
    public void ReviveAD()
    {
        //DISPLAY AD
        CandyKit.ShowRewardedVideo("Revivde", (complete) =>
        {
            if (complete)
            {
                GameManager.Instance.Revive();
                // Level.Instance.AddGraySlot(true);
                // A.LS.currentLevel.AddGraySlot(true);
            }

        });

    }
    public void GiveUp()
    {
        //DISPLAY LEAVE UI
        healthManager.HealthLose(1);
        Restart();
        //TRY AGAIN
    }
    public void Leave()
    {
        healthManager.HealthLose(1);
        //LEAVE LEVEL
        Restart();
    }
    public void TryAgainButton()
    {
        if (healthManager.health > 0)
        {
            healthManager.HealthLose(1);
            GameManager.Instance.TryAgain();
            // TryAgain();
        }
        else
        {
            OutOfLiveUI();
        }
    }
    public void RefillHealthCoin()
    {
        RemoveCoin(250);
        healthManager.SetFullHealth();
    }
    public void AddOneLifeAD()
    {
        //DISPLAY AD
        healthManager.IncreaseHealth(1);
    }

    public void SetActiveLoadingScreen(bool active)
    {
        loadingScreen.SetActive(active);
    }
    public void CloseNoAdsButton()
    {
        noAdsButton.SetActive(false);
    }

    public void SetActiveMenu(bool active)
    {
        menu.SetActive(active);
    }
    public void SetActiveLeaderboard(bool active)
    {
        if (leaderboard == null) return;

        leaderboard.SetActive(active);
    }
    public void SetActivePowerUp(bool active)
    {
        powerUp.SetActive(active);
    }

    public void DeactivateAllGameOver(object sender, EventArgs e)
    {
        if (outOfSpaceGameOver != null)
            outOfSpaceGameOver.SetActive(false);

        if (leaveGameOver != null)
            leaveGameOver.SetActive(false);

        if (youFailedGameOver != null)
            youFailedGameOver.SetActive(false);
    }


    public void CloseIAPShop()
    {
        IAPShop.gameObject.SetActive(false);
    }

    public void OurGames()
    {
        //TODO OUR GAMES BUTTON PRESSED

    }

    public void FeatureUnlocked(Sprite icon)
    {
        featureUnlocked.SetActive(true);
        featureUnlockedImage.sprite = icon;
    }

    public void CoinGoHud()
    {
        for (int i = 0; i < 20; i++)
        {
            ImageMoveToUI(coinSprite, coinText.transform);
        }
    }
    public void HeartGoHud(float delay = 0)
    {
        StartCoroutine(DelayedHeartGoHud(delay));
    }

    private void ImageMoveToUI(Sprite sprite, Transform target)
    {
        var image = Instantiate(imagePrefab, Vector3.zero, Quaternion.identity, transform);
        image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        image.GetComponent<Image>().sprite = sprite;
        var randomOffset = Random.insideUnitCircle * 300;

        image.GetComponent<RectTransform>().DOAnchorPos(randomOffset, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            image.transform.DOMove(target.position, 0.5f).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    image.transform.DOScale(0, 0.3f).OnComplete((() =>
                    {
                        Destroy(image, 0.1f);
                    }));

                });
        });
    }

    private IEnumerator DelayedHeartGoHud(float delay)
    {
        yield return new WaitForSeconds(delay);
        ImageMoveToUI(heartSprite, healthManager.healthTxt.transform);
    }
}
