using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using System;
using UnityEngine.iOS;
#endif

namespace CandyKitSDK
{
    public class CkTenjinObject : MonoBehaviour
    {

        private CandyKitSettingsScriptableObject m_Settings;
        const string step_save = "step_save";
        const string first_open = "first_open";
        public string CampaignName { get; private set; }

        public void Initialize(CandyKitSettingsScriptableObject settings)
        {
            m_Settings = settings;
            SetAppStore();
            if (PlayerPrefs.GetInt(first_open, 0) == 0)
            {
                PlayerPrefs.SetInt(first_open, 1);
                SendConversionValue(0);
            }
            TenjinConnect();
            SendSavedStepEvent();
            // TryGetCampaignName();
        }

        private void SendSavedStepEvent()
        {
            int savedStep = PlayerPrefs.GetInt(step_save, 0);
            if (savedStep > 0)
            {
                StartCoroutine(NewMethod());
            }

            IEnumerator NewMethod()
            {
                int sendiCount = savedStep;
                for (int i = 0; i < savedStep; i++)
                {
                    GetInstance().SendEvent("revenue_step");
                    Debug.Log("CK--> Tenjin Revenue Step");
                    sendiCount--;
                    PlayerPrefs.SetInt(step_save, sendiCount);
                    yield return null;
                }
                PlayerPrefs.SetInt(step_save, 0);
            }
        }
        public BaseTenjin GetInstance()
        {
#if UNITY_ANDROID
            return Tenjin.getInstance(m_Settings.Android.TenjinSDKKey);
#elif UNITY_IOS
            return Tenjin.getInstance(m_Settings.iOS.TenjinSDKKey);
#else
            return null;
#endif
        }

        private void SetAppStore()
        {
            BaseTenjin instance = GetInstance();
#if UNITY_ANDROID
            instance.SetAppStoreType(AppStoreType.googleplay);
#elif UNITY_IOS
            instance.SetAppStoreType(AppStoreType.other);
#endif
            Debug.Log("CK--> Tenjin SetAppstore ");
            if (instance)
            {
                instance.SetCacheEventSetting(true);
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                TenjinConnect();
            }
        }

        public void TenjinConnect()
        {
            BaseTenjin instance = GetInstance();
#if UNITY_IOS && !UNITY_EDITOR
            if (new Version(Device.systemVersion).CompareTo(new Version("14.0")) >= 0) {
                    instance.RequestTrackingAuthorizationWithCompletionHandler((status) => {
                    Debug.Log("===> App Tracking Transparency Authorization Status: " + status);

                    instance.Connect();
                });
            } else {
                instance.Connect();
            }

#elif UNITY_ANDROID
            instance.Connect();
#endif
        }

        public void SendConversionValue(int val, string coarse = "Low")
        {

#if UNITY_IOS || UNITY_EDITOR
            BaseTenjin tenjinInstance = Tenjin.getInstance(m_Settings.iOS.TenjinSDKKey);
            Debug.Log("Tenjin SKAD Conversion Value: " + val);
            tenjinInstance.UpdatePostbackConversionValue(val, coarse);
            // CKILRD.SendSKAdNetworkDataToGA(val.ToString());
#endif


            //             float totalValue =
            //                 PlayerPrefs.GetFloat(CkConstants.BannerRevenuePref, 0f) +
            //                 PlayerPrefs.GetFloat(CkConstants.RewardedRevenuePref, 0f) +
            //                 PlayerPrefs.GetFloat(CkConstants.InterstitialRevenuePref, 0f) +
            //                 PlayerPrefs.GetFloat(CkConstants.IAPRevenuePref, 0f);
            // #if UNITY_IOS || UNITY_EDITOR
            //             BaseTenjin tenjinInstance = Tenjin.getInstance(m_Settings.iOS.TenjinSDKKey);

            //             tenjinInstance.UpdatePostbackConversionValue((int)totalValue);
            // #endif
        }
        public void TenjinCompletedPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string transactionId, string Receipt, string Signature = null)
        {
            BaseTenjin tenjinInstance = GetInstance();

            tenjinInstance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, transactionId, Receipt, Signature);
        }

        internal void SentStepEvent(int valueDif)
        {
            StartCoroutine(NewMethod());

            IEnumerator NewMethod()
            {
                int sendiCount = valueDif;
                PlayerPrefs.SetInt(step_save, sendiCount);
                for (int i = 0; i < valueDif; i++)
                {
                    GetInstance().SendEvent("revenue_step");
                    Debug.Log("CK--> Tenjin Revenue Step");
                    sendiCount--;
                    PlayerPrefs.SetInt(step_save, sendiCount);
                    yield return null;
                }
                PlayerPrefs.SetInt(step_save, 0);
            }
        }

        public void SentCustomEvent(string eventname)
        {
            GetInstance().SendEvent(eventname);
        }

        void SetCampName(string name)
        {
            CampaignName = name;
        }
        public int NameGetRetryCount = 7;
        const string CampaignNameKey = "campaign_name";
        void TryGetCampaignName()
        {
            if (PlayerPrefs.GetString(CampaignNameKey) != string.Empty)
            {
                SetCampName(PlayerPrefs.GetString(CampaignNameKey));
                Debug.Log("CK--> Tenjin Campaign Name Found from Pref: " + CampaignName);
            }
            else
            {
                GetCampaignName((name) =>
                {
                    if (name == null)
                    {
                        if (NameGetRetryCount > 0)
                        {
                            NameGetRetryCount--;
                            Debug.Log("CK--> Tenjin Campaign Name retrying " + NameGetRetryCount);
                            Invoke(nameof(TryGetCampaignName), 1f);
                            // TryGetCampaignName(); // Safe recursive retry
                        }
                        else
                        {
                            Debug.Log("CK--> Tenjin Campaign Name Giving UP setting to Organic");
                            SetCampName("Organic");
                        }
                    }
                    else
                    {
                        Debug.Log("CK--> Tenjin Campaign Name: " + name);
                        SetCampName(name);
                        PlayerPrefs.SetString(CampaignNameKey, name);
                    }
                });
            }
        }

        public void GetCampaignName(Action<string> CallBack)
        {
            BaseTenjin instance = GetInstance();
            instance.GetAttributionInfo((Dictionary<string, string> attributionInfoData) =>
            {
                print("CK--> Tenjin Attribution Info Count: " + attributionInfoData.Count);
                if (attributionInfoData.Count > 0)
                {
                    foreach (var item in attributionInfoData)
                    {
                        Debug.Log("CK--> Tenjin Attribution Info Key: " + item.Key + " Value: " + item.Value);
                    }
                }
                if (attributionInfoData.ContainsKey("campaign_name"))
                {
                    string campaignName = attributionInfoData["campaign_name"];
                    CallBack?.Invoke(campaignName);
                }
                else
                {
                    CallBack?.Invoke(null);
                }
            });
        }
    }
}
