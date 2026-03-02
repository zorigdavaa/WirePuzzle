using System;
using UnityEngine;
using Unity.Services.LevelPlay;
using UnityEngine.Events;
using GameAnalyticsSDK;
using CandyKitSDK;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using static CandyKitSDK.CandyKit;

public class CKLevelPlay : MonoBehaviour
{
    private string appKey;
    private string rewardedAdUnitId;
    private string interstitialAdUnitId;
    private string bannerAdUnitId;

    private CandyKitSettingsScriptableObject settings;

    private LevelPlayRewardedAd rewardedAd;
    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayBannerAd bannerAd;

    private CkRewardedAdCallback rewardedCallback;
    private UnityAction interstitialCallback;

    // ---------------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------------
    public void Initialize(CandyKitSettingsScriptableObject config)
    {
        settings = config;

#if UNITY_ANDROID || UNITY_EDITOR

        rewardedAdUnitId = settings.Android.RewardedVideoAdUnitId;
        interstitialAdUnitId = settings.Android.InterstitialAdUnitId;
        bannerAdUnitId = settings.Android.BannerAdUnitId;
#elif UNITY_IOS

            rewardedAdUnitId = settings.iOS.RewardedVideoAdUnitId;
            interstitialAdUnitId = settings.iOS.InterstitialAdUnitId;
            bannerAdUnitId = settings.iOS.BannerAdUnitId;
#endif
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;
        // Analytics integrations
        // GameAnalyticsILRD.SubscribeIronSourceImpressions();
        // CKILRD.ListenImpressionForTenjinIronSource();
        CreateRewardedAd();
        LoadRewardedAd();

        CreateInterstitialAd();
        LoadInterstitialAd();

        CreateBannerAd();
        LoadBannerAd();
        SceneManager.sceneLoaded += OnSceneLoaded;
        // IronSourceImpressionDataEvents.onImpressionDataReadyEvent
    }

    private void ImpressionDataReadyEvent(LevelPlayImpressionData data)
    {

        switch (data.AdFormat)
        {
            case "rewarded_video":
                IncreaseAdWatchCount(CkConstants.RVWatchCount);
                break;

            case "interstitial":
                IncreaseAdWatchCount(CkConstants.InterWatchCount);
                break;

            case "banner":
                // usually no watch count for banner
                break;
        }
        Debug.Log("CK--> ILRD data Sent" + data);
        // string json = ConvertImpressionToJson(data);
        // CandyKit.m_Tenjin.GetInstance().IronSourceImpressionFromJSON(json);
        CKCV.IncreaseRevenue((float)data.Revenue);
        CKCV.SendConversionValue();

    }
    // private string ConvertImpressionToJson(LevelPlayImpressionData data)
    // {
    //     var impression = new Dictionary<string, object>
    // {
    //     { "ad_platform", "ironSource" },
    //     { "ad_unit_name", data.AdUnitId },
    //     { "ad_format", data.AdFormat },          // REWARDED / INTERSTITIAL / BANNER
    //     { "ad_network", data.NetworkName },
    //     { "placement", data.Placement },
    //     { "revenue", data.Revenue },
    //     { "currency", data.Currency ?? "USD" },
    //     { "country", data.Country }
    // };

    //     return MiniJson.JsonEncode(data);
    // }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (!CandyKit.IsNoAds())
        {
            CreateBannerAd();
            LoadBannerAd();
        }
    }

    private void CreateRewardedAd()
    {
        if (rewardedAd != null)
            return;

        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewardedAd.OnAdLoaded += OnRewardedLoaded;
        rewardedAd.OnAdLoadFailed += OnRewardedLoadFailed;
        rewardedAd.OnAdDisplayed += OnRewardedDisplayed;
        rewardedAd.OnAdDisplayFailed += OnRewardedDisplayFailed;
        rewardedAd.OnAdClicked += OnRewardedClicked;
        rewardedAd.OnAdClosed += OnRewardedClosed;
        rewardedAd.OnAdRewarded += OnRewardedRewarded;
        Debug.Log("CK--> Rewarded Object Created");
        // CKILRD.ListenImpressionLevelplayForTenjinRewarded(rewardedAd);
    }

    private void OnRewardedDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
    {
        Debug.LogError("CK--> Rewarded Display Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
        rewardedCallback?.Invoke(false);
        rewardedCallback = null;
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        rewardedAd?.LoadAd();
    }

    public void ShowRewardedVideo(string placement, CkRewardedAdCallback callback)
    {
        rewardedCallback = callback;

        if (IsRewardedAdReady())
        {
            rewardedAd.ShowAd(placement);
        }
        else
        {
            rewardedCallback?.Invoke(false);
            rewardedCallback = null;
            LoadRewardedAd();
        }
    }

    private void OnRewardedLoaded(LevelPlayAdInfo info) { }

    private void OnRewardedLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("CK--> Rewarded Load Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
        rewardedCallback?.Invoke(false);
        rewardedCallback = null;
        LoadRewardedAd();
    }

    private void OnRewardedDisplayed(LevelPlayAdInfo info) { }



    private void OnRewardedClicked(LevelPlayAdInfo info) { }

    private void OnRewardedClosed(LevelPlayAdInfo info)
    {
        LoadRewardedAd();
        // IncreaseAdWatchCount(CkConstants.RVWatchCount);
        // CKCV.IncreaseRevenue((float)info.Revenue);
        // CKCV.SendConversionValue();
    }

    private void OnRewardedRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
    {
        rewardedCallback?.Invoke(true);
        rewardedCallback = null;
    }
    internal bool IsRewardedAdReady()
    {
        return rewardedAd.IsAdReady();
    }

    // ---------------------------------------------------------
    // INTERSTITIAL ADS
    // ---------------------------------------------------------
    private void CreateInterstitialAd()
    {
        if (interstitialAd != null)
            return;

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += OnInterstitialLoaded;
        interstitialAd.OnAdLoadFailed += OnInterstitialLoadFailed;
        interstitialAd.OnAdDisplayed += OnInterstitialDisplayed;
        interstitialAd.OnAdDisplayFailed += OnInterstitialDisplayFailed;
        interstitialAd.OnAdClicked += OnInterstitialClicked;
        interstitialAd.OnAdClosed += OnInterstitialClosed;
        // CKILRD.ListenImpressionLevelplayForTenjinInter(interstitialAd);
        Debug.Log("CK--> Interstitial Object Created");
    }

    private void OnInterstitialDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
    {
        // Debug.LogError("CK--> Interstitial Display Failed: " + error.LevelPlayError + " Code: " + error.ErrorCode);
        Debug.LogError("CK--> Interstitial Display Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
        interstitialCallback?.Invoke();
        interstitialCallback = null;
        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        interstitialAd?.LoadAd();
    }

    public void ShowInterstitial(string placement, UnityAction callback = null)
    {
        interstitialCallback = callback;

        if (interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd(placement);
        }
        else
        {
            interstitialCallback?.Invoke();
            interstitialCallback = null;
            LoadInterstitialAd();
        }
    }

    private void OnInterstitialLoaded(LevelPlayAdInfo info) { }

    private void OnInterstitialLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("CK--> Interstitial Load Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
        LoadInterstitialAd();
    }

    private void OnInterstitialDisplayed(LevelPlayAdInfo info) { }



    private void OnInterstitialClicked(LevelPlayAdInfo info) { }

    private void OnInterstitialClosed(LevelPlayAdInfo info)
    {
        interstitialCallback?.Invoke();
        interstitialCallback = null;
        LoadInterstitialAd();
        // IncreaseAdWatchCount(CkConstants.InterWatchCount);
        // CKCV.IncreaseRevenue((float)info.Revenue);
        // CKCV.SendConversionValue();
    }

    // ---------------------------------------------------------
    // BANNER ADS
    // ---------------------------------------------------------
    private void CreateBannerAd()
    {
        // if (bannerAd != null)
        //     return;

        // var config = new LevelPlayBannerAd.Config.Builder()
        //     .SetSize(LevelPlayAdSize.BANNER)
        //     .SetPlacementName("Banner")
        //     .SetPosition(LevelPlayBannerPosition.BottomCenter)
        //     .SetDisplayOnLoad(true)
        //     .SetRespectSafeArea(true)
        //     .Build();
        var config = new LevelPlayBannerAd.Config.Builder()
            .SetSize(LevelPlayAdSize.BANNER)
            .SetPlacementName("Banner")
            .SetPosition(LevelPlayBannerPosition.BottomCenter)
            .SetDisplayOnLoad(true)
            .SetRespectSafeArea(true)
            .Build();

        bannerAd = new LevelPlayBannerAd(bannerAdUnitId, config);

        bannerAd.OnAdLoaded += OnBannerLoaded;
        bannerAd.OnAdLoadFailed += OnBannerLoadFailed;
        bannerAd.OnAdDisplayed += OnBannerDisplayed;
        bannerAd.OnAdDisplayFailed += OnBannerDisplayFailed;
        bannerAd.OnAdClicked += OnBannerClicked;
        bannerAd.OnAdCollapsed += OnBannerCollapsed;
        bannerAd.OnAdExpanded += OnBannerExpanded;
        bannerAd.OnAdLeftApplication += OnBannerLeftApp;
        // CKILRD.ListenImpressionLevelplayForTenjinBanner(bannerAd);
        Debug.Log("CK--> Banner Object Created");
    }

    private void OnBannerDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
    {
        Debug.LogError("CK--> Banner Display Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
    }

    public void LoadBannerAd() => bannerAd?.LoadAd();
    public void ShowBanner()
    {
        CreateBannerAd();
        bannerAd?.ShowAd();
    }
    public void HideBanner() => bannerAd?.HideAd();
    public void DestroyBannerAd() => bannerAd?.DestroyAd();
    public float GetBannerHeight() => bannerAd?.GetAdSize().Height ?? 0f;

    private void OnBannerLoaded(LevelPlayAdInfo info) { }
    private void OnBannerLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("CK--> Banner Load Failed: " + error.ErrorMessage + " Code: " + error.ErrorCode);
    }
    private void OnBannerDisplayed(LevelPlayAdInfo info)
    {
        // CKCV.IncreaseRevenue((float)info.Revenue);
        // CKCV.SendConversionValue();
    }

    private void OnBannerClicked(LevelPlayAdInfo info) { }
    private void OnBannerCollapsed(LevelPlayAdInfo info) { }
    private void OnBannerExpanded(LevelPlayAdInfo info) { }
    private void OnBannerLeftApp(LevelPlayAdInfo info) { }

    // ---------------------------------------------------------
    // CLEANUP
    // ---------------------------------------------------------
    private void OnDestroy()
    {
        bannerAd?.DestroyAd();
    }

    public void IncreaseAdWatchCount(string Key)
    {
        int Count = PlayerPrefs.GetInt(Key, 0);
        Count++;
        PlayerPrefs.SetInt(Key, Count);
    }
}
