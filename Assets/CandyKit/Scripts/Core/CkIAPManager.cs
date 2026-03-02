using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using CandyKitSDK;
using UnityEngine.Purchasing.Extension;
using GameAnalyticsSDK;
// using AppLovinMax.ThirdParty.MiniJson;
using UnityEngine.Purchasing.Security;
using System;
using UnityEngine.Purchasing.MiniJSON;

public class CkIAPManager : MonoBehaviour, IDetailedStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    private IAppleExtensions m_AppleExtensions;
    private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
    private CandyKitSettingsScriptableObject m_Settings;
    public delegate void OnPurchaseCompleted(bool success);
    private event OnPurchaseCompleted m_OnPurchaseCompleted;
    public bool isIAPInitialized = false;
    public string ErrorMessage = "";
    private int retry = 0;

    public void Initialize()
    {
        DontDestroyOnLoad(gameObject);

        m_Settings = CandyKit.Settings;
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

#if UNITY_IOS
        builder.AddProduct(m_Settings.iOS.NoAdsProductID, ProductType.NonConsumable);
        foreach (var item in m_Settings.iOS.ckProducts)
        {
            Debug.Log("CK IAP--> Initializing product: " + item.ID + "(" + item.ProductType.ToString() + ")");
            builder.AddProduct(item.ID, item.ProductType);
        }

#else
        builder.AddProduct(m_Settings.Android.NoAdsProductID, ProductType.NonConsumable);
        foreach (var item in m_Settings.Android.ckProducts)
        {
            Debug.Log("CK IAP--> Initializing product: " + item.ID + "(" + item.ProductType.ToString() + ")");
            builder.AddProduct(item.ID, item.ProductType);
        }
#endif

        UnityPurchasing.Initialize(this, builder);
    }
    bool isFirsttime()
    {
        if (PlayerPrefs.GetInt(CkConstants.FirstIAP, 0) == 0)
        {
            PlayerPrefs.SetInt(CkConstants.FirstIAP, 1);
            return true;
        }
        return false;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("CK IAP--> IAPMANGER initialized");
        this.controller = controller;
        this.extensions = extensions;
        isIAPInitialized = true;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        ErrorMessage = "CK IAP--> IAP initialization failed: " + ", " + error;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("CK IAP--> IAP initialization failed: " + message + ", " + error);
        ErrorMessage = "CK IAP--> IAP initialization failed: " + message + ", " + error;
    }

    public void RestorePurchase()
    {
        m_OnPurchaseCompleted = null;
        Debug.Log("CK IAP--> RestorePurchase");
#if UNITY_IOS
        m_AppleExtensions.RestoreTransactions(OnRestore);
#else
        m_GooglePlayStoreExtensions.RestoreTransactions(OnRestore);
#endif
    }

    private void OnRestore(bool success, string error)
    {
        string message;
        if (success)
        {
            PlayerPrefs.SetInt(CkConstants.PremiumUserPref, 1);
            CandyKit.DisableAds();
            // m_OnPurchaseCompleted?.Invoke(true);
            message = "CK-> Restored purchase successfully";

        }
        else
        {
            message = "CK-> Restore failed with error: " + error;
        }

        Debug.Log(message);
    }

    public void OnPurchaseClicked(string productId, OnPurchaseCompleted onPurchaseCompleted)
    {
        m_OnPurchaseCompleted = null;
        if (productId == null)
        {
            Debug.LogError("CK IAP--> productId is null");
            onPurchaseCompleted?.Invoke(false);

            return;
        }

        if (onPurchaseCompleted == null)
        {
            Debug.LogError("CK IAP--> onPurchaseCompleted is null");
            onPurchaseCompleted?.Invoke(false);
            return;
        }
        if (controller == null)
        {
            Debug.LogError("CK IAP--> controller is null");
            onPurchaseCompleted?.Invoke(false);
            return;
        }
        m_OnPurchaseCompleted = onPurchaseCompleted;
        Debug.Log("CK IAP--> Purchase clicked: " + productId);
        controller.InitiatePurchase(productId);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        bool validPurchase = true; // Presume valid for platforms with no R.V.
        Debug.Log("CK IAP--> starting ProcessPurchase:");
        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
#if RECEIPT_VALIDATION
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("CK IAP--> Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log("CK IAP--> " + productReceipt.productID);
                Debug.Log("CK IAP--> " + productReceipt.purchaseDate);
                Debug.Log("CK IAP--> " + productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("CK IAP--> Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#else
        Debug.LogWarning("CK IAP--> Receipt validation is not enabled or GooglePlayTangle/AppleTangle classes are missing.");
#endif
#endif
        if (validPurchase)
        {
            Debug.Log("CK IAP--> ProcessPurchase");
            Debug.Log("CK IAP--> ProcessPurchase item: " + e.purchasedProduct.definition.id);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (e.purchasedProduct.definition.id == m_Settings.iOS.NoAdsProductID)
                {
                    PlayerPrefs.SetInt(CkConstants.PremiumUserPref, 1);
                    CandyKit.DisableAds();
                }
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                if (e.purchasedProduct.definition.id == m_Settings.Android.NoAdsProductID)
                {
                    Debug.Log("CK IAP--> ProcessPurchase matches! Settings as PremiumUser!");
                    PlayerPrefs.SetInt(CkConstants.PremiumUserPref, 1);
                    CandyKit.DisableAds();
                }
            }

            // TODO:
            // Clean this plz
            Product product = e.purchasedProduct;
            decimal price = product.metadata.localizedPrice;
            double unitPrice = decimal.ToDouble(price);
            string productId = product.definition.id;
            string currencyCode = product.metadata.isoCurrencyCode;
            decimal GPrice = product.metadata.localizedPrice * 100;

            Dictionary<string, object> wrapper = Json.Deserialize(product.receipt) as Dictionary<string, object>;
            if (null == wrapper)
            {
                return PurchaseProcessingResult.Complete;
            }

            string payload = (string)wrapper["Payload"];
            Dictionary<string, object> gpDetails = Json.Deserialize(payload) as Dictionary<string, object>;
            string transactionId = product.transactionID;

#if UNITY_ANDROID && !UNITY_EDITOR
        string gpJson = (string)gpDetails["json"];
        string gpSig = (string)gpDetails["signature"];

        // Send revenue data
        CandyKit.m_Tenjin?.TenjinCompletedPurchase(productId, currencyCode, 1, unitPrice, transactionId, gpJson, gpSig);
        GameAnalytics.NewBusinessEventGooglePlay(currencyCode, (int)GPrice, product.metadata.localizedTitle, productId, "Home Screen", gpJson, gpSig);
#elif UNITY_IOS
        CandyKit.m_Tenjin?.TenjinCompletedPurchase(productId, currencyCode, 1, unitPrice, transactionId, payload);
        GameAnalytics.NewBusinessEventIOS(currencyCode, (int)GPrice, product.metadata.localizedTitle, productId, "Home Screen", payload);
#endif

            // PlayerPrefs.SetFloat(CkConstants.IAPRevenuePref, PlayerPrefs.GetFloat(CkConstants.IAPRevenuePref, 0f) + (float)unitPrice);
            Debug.Log("CK IAP--> After sending pruchase analytics data");
            Debug.Log("CK IAP--> price " + unitPrice);
            // CKILRD.LogPurchaseToFirebase(productId, (float)unitPrice, currencyCode);
            CKCV.IncreaseRevenue((float)unitPrice);
            CKCV.SendConversionValue();
            m_OnPurchaseCompleted?.Invoke(true);
            m_OnPurchaseCompleted = null;
            Debug.Log("CK IAP--> ProcessPurchase completed");
            int level = -1;
            try
            {
                level = GetLevel();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"CK IAP--> Error getting level: {ex}");
            }
            finally
            {
                GameAnalytics.NewDesignEvent($"iap_level:level_{level:D3}");
                if (isFirsttime())
                {
                    GameAnalytics.NewDesignEvent($"iap_first_level:level_{level:D3}");
                    GameAnalytics.NewDesignEvent($"iap_first_day:day_{CandyKit.GetDaySinceInstall():D3}");
                }
            }

            // Unlock the appropriate content here.
        }
        IncreaseIAPCount();
        return PurchaseProcessingResult.Complete;
    }

    private int GetLevel()
    {
        return PlayerPrefs.GetInt("level", 1);
    }

    public bool IsInitialized()
    {
        return controller != null && extensions != null;
    }
    public string GetProductPrice(string id)
    {
        if (IsInitialized())
        {
            Product product = controller.products.WithID(id);
            if (product != null && product.metadata != null)
            {
                return product.metadata.localizedPriceString;
            }
        }

        // Fallback price for testing in the editor
        return Application.isEditor ? "$4.99 (Test Price)" : "N/A";
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.LogWarning($"CK IAP--> Purchase failed: {i.definition.id}, Reason: {p}");
        m_OnPurchaseCompleted?.Invoke(false);
        m_OnPurchaseCompleted = null;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"CK IAP--> Purchase failed: {product.definition.id}, Reason: {failureDescription.reason}");
        m_OnPurchaseCompleted?.Invoke(false);
        m_OnPurchaseCompleted = null;
    }

    public void IncreaseIAPCount()
    {
        int Count = PlayerPrefs.GetInt(CkConstants.IAPCount, 0);
        Count++;
        PlayerPrefs.SetInt(CkConstants.IAPCount, Count);
    }
    // public void IncreaseRevenueCount(float amount)
    // {
    //     float Count = PlayerPrefs.GetFloat(CkConstants.IAPRevenuePref, 0);
    //     Count += amount;
    //     PlayerPrefs.SetFloat(CkConstants.IAPRevenuePref, Count);
    // }
    // public int GetIAPConversionValue()
    // {
    //     float Count = PlayerPrefs.GetFloat(CkConstants.IAPRevenuePref, 0);
    //     switch (Count)
    //     {
    //         case < 0.5f: return 0;
    //         case < 1: return 8;
    //         case < 3: return 16;
    //         case < 7: return 16;
    //         case < 12: return 24;
    //         default: return 36;
    //     }
    // }
}
