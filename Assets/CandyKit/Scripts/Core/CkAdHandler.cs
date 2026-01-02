using UnityEngine;
using UnityEngine.Events;
using GameAnalyticsSDK;
using System;

namespace CandyKitSDK
{
    public delegate void CkRewardedAdCallback(bool isSuccess);

    public class CkAdHandler : MonoBehaviour
    {
        private int interstitialRetryAttempt;
        private int rewardedVideoRetryAttempt;

        private string bannerAdUnitId;
        private string interstitialAdUnitId;
        private string rewardedVideoAdUnitId;

        private string currentRewardedAdPlacement = "Rewarded";
        private string CurrentInterstitialAdPlacement = "InterStitial";

        private static CkRewardedAdCallback m_RewardedAdSuccessCallback;
        private CandyKitSettingsScriptableObject m_Settings;
        // private BaseTenjin tenjinInstance = null;

        private UnityAction onInterstitialShowSuccess;

        public void Initialize(CandyKitSettingsScriptableObject settings)
        {
            m_Settings = settings;
            GameAnalyticsILRD.SubscribeMaxImpressions();
            // CKILRD.ListenForImpressionForFirebase();
            CKILRD.ListenImpressionForTenjin();
            SubscribeBannerAdEvents();
            SubscribeInterstitialAdEvents();
            SubscribeRewardedAdEvents();

#if UNITY_ANDROID || UNITY_EDITOR
            bannerAdUnitId = settings.Android.BannerAdUnitId;
            interstitialAdUnitId = settings.Android.InterstitialAdUnitId;
            rewardedVideoAdUnitId = settings.Android.RewardedVideoAdUnitId;
#elif UNITY_IOS
            bannerAdUnitId        = settings.iOS.BannerAdUnitId;
            interstitialAdUnitId  = settings.iOS.InterstitialAdUnitId;
            rewardedVideoAdUnitId = settings.iOS.RewardedVideoAdUnitId;
#endif

            // GameAnalyticsILRD.SubscribeMaxImpressions();
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadBannerAd();

            DontDestroyOnLoad(gameObject);
            Debug.Log("CK--> init adHandler ");
        }

        private void SubscribeInterstitialAdEvents()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaid;
        }



        private void UnsubscribeInterstitialAdEvents()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnInterstitialAdRevenuePaid;
        }

        private void SubscribeBannerAdEvents()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        }

        private void UnsubscribeBannerAdEvents()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnBannerAdExpandedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent -= OnBannerAdCollapsedEvent;
        }

        private void SubscribeRewardedAdEvents()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        private void UnsubscribeRewardedAdEvents()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
        }

        private void OnDestroy()
        {
            UnsubscribeInterstitialAdEvents();
            UnsubscribeBannerAdEvents();
            UnsubscribeRewardedAdEvents();
        }

        public float GetBannerHeight()
        {
            Rect bannerLayout = MaxSdk.GetBannerLayout(bannerAdUnitId);
            Debug.Log(bannerLayout);

            // Debug.Log("CK--> screen dpi: " + Screen.dpi);
            // Debug.Log("CK--> screen density: " + MaxSdkUtils.GetScreenDensity());
            // Debug.Log("CK--> adaptive banner height: " + MaxSdkUtils.GetAdaptiveBannerHeight());
            float bannerHeight = 0f;
            float adaptiveHeight = MaxSdkUtils.GetAdaptiveBannerHeight();

            bannerHeight = adaptiveHeight / (Screen.dpi / 160);

            return bannerHeight;
        }

        public void LoadBannerAd()
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);

            MaxSdk.StartBannerAutoRefresh(bannerAdUnitId);
        }

        public void ShowBanner()
        {
            if (!CandyKit.IsNoAds())
            {
                LoadBannerAd();
                MaxSdk.ShowBanner(bannerAdUnitId);
            }
            else
            {
                Debug.Log("CK--> Not showing banner ad because NoAds purchased!");
            }
        }

        public void HideBanner()
        {
            MaxSdk.HideBanner(bannerAdUnitId);
        }

        private void LoadInterstitialAd()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        public void ShowInterstitial(string placement, UnityAction onSuccess)
        {
            if (!CandyKit.IsNoAds())
            {
                if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
                {
                    onInterstitialShowSuccess = onSuccess;
                    CurrentInterstitialAdPlacement = placement;
                    MaxSdk.ShowInterstitial(interstitialAdUnitId, placement);
                }
                else
                {
                    LoadInterstitialAd();
                }
            }
            else
            {
                Debug.Log("CK--> Not showing ad because NoAds purchased!");
            }
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedVideoAdUnitId);
        }

        public bool IsRewardedAdReady()
        {
            return MaxSdk.IsRewardedAdReady(rewardedVideoAdUnitId);
        }

        public void ShowRewardedVideo(string placement, CkRewardedAdCallback callback)
        {
            if (MaxSdk.IsRewardedAdReady(rewardedVideoAdUnitId))
            {
                m_RewardedAdSuccessCallback = callback;
                currentRewardedAdPlacement = placement;
                MaxSdk.ShowRewardedAd(rewardedVideoAdUnitId, placement);
            }
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            interstitialRetryAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, interstitialRetryAttempt));

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onInterstitialShowSuccess?.Invoke();
            GameAnalytics.StartTimer(CurrentInterstitialAdPlacement);
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            GameAnalytics.StopTimer(CurrentInterstitialAdPlacement);
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitialAd();
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, adInfo.NetworkName, adInfo.Placement);
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, adInfo.NetworkName, adInfo.Placement);
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            long dur = GameAnalytics.StopTimer(CurrentInterstitialAdPlacement);
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, adInfo.NetworkName, adInfo.Placement, dur);
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitialAd();

        }
        private void OnInterstitialAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // PlayerPrefs.SetFloat(CkConstants.InterstitialRevenuePref, (float)adInfo.Revenue);
            IncreaseAdWatchCount(CkConstants.InterWatchCount);
            CKCV.IncreaseRevenue((float)adInfo.Revenue);
            CKCV.SendConversionValue();
        }

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            GetBannerHeight();
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {

        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // PlayerPrefs.SetFloat(CkConstants.BannerRevenuePref, (float)adInfo.Revenue);
            // IncreaseAdWatchCount();
            CKCV.IncreaseRevenue((float)adInfo.Revenue);
            CKCV.SendConversionValue();
        }

        private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }

        private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            rewardedVideoRetryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            rewardedVideoRetryAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, rewardedVideoRetryAttempt));

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            GameAnalytics.StartTimer(currentRewardedAdPlacement);

        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            GameAnalytics.StopTimer(currentRewardedAdPlacement);
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();

            m_RewardedAdSuccessCallback(false);
            m_RewardedAdSuccessCallback = null;
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, adInfo.NetworkName, adInfo.Placement);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, adInfo.NetworkName, adInfo.Placement);
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (m_RewardedAdSuccessCallback != null)
            {
                m_RewardedAdSuccessCallback(false);
                m_RewardedAdSuccessCallback = null;
            }
            long elapsedTime = GameAnalytics.StopTimer(currentRewardedAdPlacement);
            GameAnalytics.NewAdEvent(
                GAAdAction.Show,
                GAAdType.RewardedVideo,
                adInfo.NetworkName ?? "UnknownNetwork",
                adInfo.Placement ?? "UnknownPlacement",
                elapsedTime
            );
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            m_RewardedAdSuccessCallback(true);
            m_RewardedAdSuccessCallback = null;
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, adInfo.NetworkName, adInfo.Placement);
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            // PlayerPrefs.SetFloat(CkConstants.RewardedRevenuePref, (float)adInfo.Revenue);
            IncreaseAdWatchCount(CkConstants.RVWatchCount);
            CKCV.IncreaseRevenue((float)adInfo.Revenue);
            CKCV.SendConversionValue();
        }
        public void IncreaseAdWatchCount(string Key)
        {
            int Count = PlayerPrefs.GetInt(Key, 0);
            Count++;
            PlayerPrefs.SetInt(Key, Count);
        }
        public int GetAdConversionValue()
        {
            int Count = PlayerPrefs.GetInt(CkConstants.AdWatchCount, 0);
            switch (Count)
            {
                case 0: return 0;
                case < 11: return 1;
                case < 21: return 2;
                case < 61: return 3;
                case < 86: return 4;
                case < 201: return 5;
                case < 301: return 6;
                default: return 7;
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                if (currentRewardedAdPlacement != null)
                {
                    GameAnalytics.PauseTimer(currentRewardedAdPlacement);
                }
                if (CurrentInterstitialAdPlacement != null)
                {
                    GameAnalytics.PauseTimer(CurrentInterstitialAdPlacement);
                }
            }
            else
            {
                if (currentRewardedAdPlacement != null)
                {
                    GameAnalytics.ResumeTimer(currentRewardedAdPlacement);
                }
                if (CurrentInterstitialAdPlacement != null)
                {
                    GameAnalytics.ResumeTimer(CurrentInterstitialAdPlacement);
                }
            }
        }

    }
}
