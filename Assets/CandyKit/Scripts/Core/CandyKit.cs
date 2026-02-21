using UnityEngine;
using UnityEngine.Events;
// using Firebase.Extensions;
// using Firebase.Analytics;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;




#if CANDYKIT || UNITY_ANDROID || UNITY_IOS
using GameAnalyticsSDK;
#endif

namespace CandyKitSDK
{
    public class CandyKit
    {
        private const string version = "1.3.2";
        private const float m_ReadinessWaitDuration = 2f;
        public static bool m_IsDebug = false;
        public static bool m_IsInitialized = false;
        public static CandyKitObject m_Instance;
        public static CkAdHandler m_CkAdHandler;
        public static CkIAPManager m_IAPManager;
        public static CkTenjinObject m_Tenjin;
        private static CandyKitSettingsScriptableObject m_Settings;
        public static CandyKitSettingsScriptableObject Settings
        {
            get
            {
                if (m_Settings == null)
                {
                    m_Settings = Resources.Load<CandyKitSettingsScriptableObject>("CandyKit/CandyKitSettings");
                }

                return m_Settings;
            }
        }

        public delegate void OnCandyKitReady();
        private static event OnCandyKitReady OnReady;

        #region -- API --

        public static void Initialize(OnCandyKitReady onReady, bool isDebug = false)
        {
            m_IsDebug = isDebug;

            if (!m_IsInitialized)
            {
                if (m_IsDebug)
                {
                    Debug.Log("CK--> Initialize CandyKit");
                }

                m_IsInitialized = true;
                OnReady = onReady;

                m_Settings = Resources.Load<CandyKitSettingsScriptableObject>("CandyKit/CandyKitSettings");

                // InitializeApplovin();
                InitializeGameAnalytics();
                // if (!CountryCode.IsInNoTenjinCountries())
                // {
                // }
                // InitializeTenjin();
                // InitializeFirebase();

                // GameObject adHandlerGo = new("CkAdHandler");
                // m_CkAdHandler = adHandlerGo.AddComponent<CkAdHandler>();
                // m_CkAdHandler.Initialize(m_Settings);

                GameObject instanceObject = new("CandyKitObject");
                m_Instance = instanceObject.AddComponent<CandyKitObject>();
                m_Instance.Initialize(m_ReadinessWaitDuration, m_Settings, OnReady);
                InitializeUnityGamingService();
                SetInstallDate();
            }
            else
            {
                if (m_IsDebug)
                {
                    Debug.Log("CK--> already initialized!");
                }
            }
        }
        public static int GetDaySinceInstall()
        {
            DateTime installDate = GetInstallDate().Date; // removes the time portion
            DateTime today = DateTime.Now.Date;
            TimeSpan timeSpan = today - installDate;
            return timeSpan.Days;
        }
        public static void SetInstallDate()
        {
            if (PlayerPrefs.GetString(CkConstants.FirstTime, string.Empty) == string.Empty)
            {
                DateTime dateTime = DateTime.Now;
                PlayerPrefs.SetString(CkConstants.FirstTime, dateTime.ToString());
            }
        }

        public static DateTime GetInstallDate()
        {
            string dateString = PlayerPrefs.GetString(CkConstants.FirstTime, null);

            if (string.IsNullOrEmpty(dateString))
            {
                return DateTime.Now;
            }

            if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate;
            }

            // Optional: handle invalid format fallback
            return DateTime.Now;
        }

        public static void NotifyLevelZero()
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> NotifyLevelZero");
            }
            if (!m_IsInitialized)
            {
                return;
            }


#if CANDYKIT || UNITY_IOS || UNITY_ANDROID
            if (PlayerPrefs.GetInt("LevelZero", 0) == 0)
            {
                PlayerPrefs.SetInt("LevelZero", 1);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_000");
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_000");
            }
#endif
        }

        public static void NotifyLevelStarted(int level)
        {
            if (m_IsDebug)
            {
                Debug.Log("CandyKit-> NotifyLevelStarted: " + level);
            }

            if (!m_IsInitialized)
            {
                return;
            }

            NotifyLevelZero();

#if CANDYKIT || UNITY_IOS || UNITY_ANDROID
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + level.ToString("000"));
#endif
        }

        public static void NotifyLevelCompleted(int level)
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> NotifyLevelCompleted: " + level);
            }

            if (!m_IsInitialized)
            {
                return;
            }

#if CANDYKIT || UNITY_IOS || UNITY_ANDROID
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + level.ToString("000"));
#endif
        }

        public static void NotifyLevelFailed(int level)
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> NotifyLevelFailed: " + level);
            }

            if (!m_IsInitialized)
            {
                return;
            }

#if CANDYKIT || UNITY_IOS || UNITY_ANDROID
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + level.ToString("000"));
#endif
        }

        public static void BuyNoAds(CkIAPManager.OnPurchaseCompleted onPurchaseCompleted)
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> Buy No Ads");
            }

            if (!m_IsInitialized)
            {
                onPurchaseCompleted(true);
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                BuyProduct(m_Settings.iOS.NoAdsProductID, onPurchaseCompleted);
            }
            else
            {
                BuyProduct(m_Settings.Android.NoAdsProductID, onPurchaseCompleted);
            }
        }

        public static void BuyProduct(string productId, CkIAPManager.OnPurchaseCompleted onPurchaseCompleted)
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> Buy Product: " + productId);
            }
#if UNITY_EDITOR
            onPurchaseCompleted(true);
#else

            if (!m_IAPManager.isIAPInitialized)
            {
                onPurchaseCompleted(false);
                return;
            }
            try
            {
                m_IAPManager.OnPurchaseClicked(productId, onPurchaseCompleted);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("CK IAP--> Purchase exception: " + ex.Message);
                onPurchaseCompleted(false);
            }
#endif
        }

        public static void RestorePurchase()
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> Restore Purchase");
            }

            if (!m_IsInitialized)
            {
                // onPurchaseCompleted(true);
                return;
            }

            // m_Instance.ckIAPManager.RestorePurchase(onPurchaseCompleted);
            m_IAPManager.RestorePurchase();
        }

        public static bool IsNoAds()
        {
            return PlayerPrefs.GetInt(CkConstants.PremiumUserPref, 0) == 1;
        }

        public static void DisableAds()
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> Restore Purchase");
            }

            if (!m_IsInitialized)
            {
                return;
            }

            m_CkAdHandler?.HideBanner();
        }

        public static void ShowBanner()
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> ShowBanner()");
            }

            if (!m_IsInitialized)
            {
                return;
            }

            m_CkAdHandler.ShowBanner();
        }

        public static void HideBanner()
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> HideBanner");
            }

            if (!m_IsInitialized)
            {
                return;
            }

            m_CkAdHandler.HideBanner();
        }

        public static void ShowInterstitial(string placement, UnityAction onSuccess = null)
        {
            if (m_IsDebug)
            {
                Debug.Log("CK--> ShowInterstitial()");
            }

            if (!m_IsInitialized)
            {
                return;
            }

            m_CkAdHandler.ShowInterstitial(placement, onSuccess);
        }

        public static bool IsRewardedAdReady()
        {
            if (!MaxSdk.IsInitialized())
                return false;

            return m_CkAdHandler.IsRewardedAdReady();
        }

        public static void ShowRewardedVideo(string placement, CkRewardedAdCallback callback)
        {

            if (m_IsDebug)
            {
                Debug.Log("CK--> ShowRewardedVideo()");
            }

            if (!m_IsInitialized)
            {
                Debug.LogWarning("CK--> ShowRewardedVideo called before initialization. Granting reward by default.");
                callback(true);
                return;
            }

            m_CkAdHandler.ShowRewardedVideo(placement, callback);
        }

        public static float GetBannerAdHeight()
        {
            if (!m_IsInitialized)
            {
                return 150f;
            }

            return m_CkAdHandler.GetBannerHeight();
        }

        #endregion


        public static void RequestReview()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            // m_Instance.ckInAppReviewObject.LaunchReview();
        }

        #region -- PRIVATE --

        private static void InitializeGameAnalytics()
        {
            GameObject gameAnalyticsInitializerObj = new("CkGAHandler");
            gameAnalyticsInitializerObj.AddComponent<CkGameAnalyticsInitializer>().Initialize();
            GameObject.DontDestroyOnLoad(gameAnalyticsInitializerObj);
        }

        // private static void InitializeApplovin()
        // {
        //     MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        //     {
        //         if (m_IsDebug)
        //         {
        //             MaxSdk.ShowMediationDebugger();
        //         }
        //     };

        //     MaxSdk.SetSdkKey(m_Settings.MaxSDKKey);
        //     MaxSdk.SetUserId("USER_ID");
        //     MaxSdk.InitializeSdk();
        //     MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxInitialized;
        //     // if (MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography == MaxSdkBase.ConsentFlowUserGeography.Gdpr)
        //     // {
        //     //     MaxSdk.GetSdkConfiguration().conse
        //     // }
        //     Debug.Log("CK--> Max Init");
        // }

        // private static void OnMaxInitialized(MaxSdkBase.SdkConfiguration configuration)
        // {
        //     InitializeGameAnalytics();
        //     // if (!CountryCode.IsInNoTenjinCountries())
        //     // {
        //     // }
        //     InitializeTenjin();
        //     // InitializeFirebase();

        //     GameObject adHandlerGo = new("CkAdHandler");
        //     m_CkAdHandler = adHandlerGo.AddComponent<CkAdHandler>();
        //     m_CkAdHandler.Initialize(m_Settings);

        //     GameObject instanceObject = new("CandyKitObject");
        //     m_Instance = instanceObject.AddComponent<CandyKitObject>();
        //     m_Instance.Initialize(m_ReadinessWaitDuration, m_Settings, OnReady);

        // }

        private static void InitializeTenjin()
        {
            if (!m_Tenjin)
            {
                GameObject tenjinGO = new GameObject("CkTenjinObject");
                Object.DontDestroyOnLoad(tenjinGO);
                m_Tenjin = tenjinGO.AddComponent<CkTenjinObject>();
                m_Tenjin.Initialize(m_Settings);
            }
        }

        private static void InitializeUnityGamingService()
        {
            GameObject gamingServiceGo = new("CkUnityGamingServiceInitializer");
            CkUnityGamingServiceObject gamingServiceObject = gamingServiceGo.AddComponent<CkUnityGamingServiceObject>();
            gamingServiceObject.Initialize(OnGamingServiceInitializationSuccess, OnGamingServiceInitializationFailed);
        }

        //         private static void InitializeFirebase()
        //         {
        // #if !UNITY_EDITOR
        //             Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        //             {
        //                 var dependencyStatus = task.Result;
        //                 if (dependencyStatus == Firebase.DependencyStatus.Available)
        //                 {
        //                     var app = Firebase.FirebaseApp.DefaultInstance;
        //                     if (MaxSdk.HasUserConsent())
        //                     {
        //                         FirebaseAnalytics.SetConsent
        //                         (
        //                             new Dictionary<ConsentType, ConsentStatus>
        //                             {
        //                                 { ConsentType.AnalyticsStorage, ConsentStatus.Granted },
        //                                 { ConsentType.AdStorage, ConsentStatus.Granted },
        //                                 { ConsentType.AdPersonalization, ConsentStatus.Granted },
        //                                 { ConsentType.AdUserData, ConsentStatus.Granted },
        //                             }
        //                         );

        //                         // Enable Firebase Analytics
        //                         FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        //                         Debug.Log("CK--> Firebase initialized with Google Analytics consent granted.");
        //                     }

        //                     // Log a message to confirm consent settings
        //                 }
        //                 else
        //                 {
        //                     UnityEngine.Debug.LogError($"CK--> Could not resolve all Firebase dependencies: {dependencyStatus}");
        //                 }
        //             });
        // #endif
        //         }


        private static void OnGamingServiceInitializationSuccess()
        {
            Debug.Log("CK--> UGS init Success");
            GameObject obj = new("CkIAPManager");
            m_IAPManager = obj.AddComponent<CkIAPManager>();
            m_IAPManager.Initialize();
        }

        private static void OnGamingServiceInitializationFailed(string error)
        {
            // Debug.Log("CK-> Gaming Service Initialization failed with error: " + error);
            Debug.Log("CK--> UGS init Failed " + error);
        }

        internal static bool IsInitialized()
        {
            return m_IsInitialized;
        }
        internal static void MaxDebugger()
        {
            MaxSdk.ShowMediationDebugger();
        }
        public static string GetGAABTestingValue()
        {
            if (PlayerPrefs.HasKey($"CKABTEST{Settings.AbTestKey}"))
            {
                string storedValue = PlayerPrefs.GetString($"CKABTEST{Settings.AbTestKey}", string.Empty);
                return storedValue;
            }
            string value = GameAnalytics.GetRemoteConfigsValueAsString(Settings.AbTestKey, "control");
            return value;
        }
        public static void SetGAABTestingValue(string index)
        {
            PlayerPrefs.SetString($"CKABTEST{Settings.AbTestKey}", index);
            Debug.Log($"CK--> Set GA AB Testing Value: {index}");
            // if (Settings.ABTestValues.Count < index)
            // {
            // PlayerPrefs.SetString($"CKABTEST{Settings.AbTestKey}", Settings.ABTestValues[index]);
            // }
            // else
            // {
            //     Debug.LogError("GAABTestingValue index out of range!");
            //     DeleteGAABTestingValue();
            // }
        }
        public static void DeleteGAABTestingValue()
        {
            PlayerPrefs.DeleteKey($"CKABTEST{Settings.AbTestKey}");
            Debug.Log($"Deleting Local Variants");
        }

        #endregion
    }
}
