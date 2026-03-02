//TODO FIRST IMPLEMENT CANDYKITSDK GAMEANALYTICSSDK

using CandyKitSDK;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;

public class IAPShopHandler : MonoBehaviour
{
    public static IAPShopHandler Instance;
    [Header("Purchase Succesful")]
    public GameObject purchaseParent;
    public GameObject Bundle1;
    public GameObject Bundle2;
    public GameObject Bundle3;
    public GameObject NoAdsBundle;
    public GameObject NoAds;
    public GameObject Coin;
    public Text coinText;

    private void Awake()
    {
        Instance = this;
    }
    //
    public void Buy(CkProduct product)
    {
        if (CandyKit.m_IAPManager == null)
        {
            Debug.Log($"NO IAP MANAGER");
            return;
        }
        if (!CandyKit.m_IAPManager.isIAPInitialized)
        {
            Debug.Log($"IAP MANAGER NO INITIALIZED");
            return;
        }
        CanvasManager.Instance.SetActiveLoadingScreen(true);

        CkProduct succesfulProduct = null;
        CandyKit.BuyProduct(product.ID, (success) =>
        {
            if (success)
            {
                succesfulProduct = product;
                OnPurchaseComplete(succesfulProduct);
                CanvasManager.Instance.SetActiveLoadingScreen(false);
                Debug.Log($"IAP success: {product.ID}");
            }
            else
            {
                CanvasManager.Instance.SetActiveLoadingScreen(false);
                Debug.Log($"IAP fail: {product.ID}");
            }
        });
    }
    private void OnPurchaseComplete(CkProduct succesfulProduct)
    {
        if (succesfulProduct != null)
        {
            if (succesfulProduct.isRemoveADS)
            {
                PlayerPrefs.SetInt("CkIsPremium", 1);
                CandyKit.DisableAds();
                CanvasManager.Instance.CloseNoAdsButton();
                CanvasManager.Instance.SetActiveLoadingScreen(false);
            }

            if (succesfulProduct.coin > 0)
            {
                CanvasManager.Instance.CoinGoHud();
                CanvasManager.Instance.AddCoin(succesfulProduct.coin); ;
            }

            if (succesfulProduct.healthTimeHour > 0)
            {
                CanvasManager.Instance.HeartGoHud(0.5f);
                Debug.Log("Start health timer");
            }

            // PurchaseSuccessfulPopup(succesfulProduct);
        }
    }
    //
    public void PurchaseSuccessfulPopup(CkProduct succesfulProduct)
    {
        purchaseParent.SetActive(true);
        switch (succesfulProduct.ID)
        {
            case "io.blackcandy.basketballjam.iap.bundle1":
                Bundle1.SetActive(true);
                break;
            case "io.blackcandy.basketballjam.iap.bundle2":
                Bundle2.SetActive(true);
                break;
            case "io.blackcandy.basketballjam.iap.bundle3":
                Bundle3.SetActive(true);
                break;
            case "io.blackcandy.basketballjam.iap.noadsbundle2":
                NoAdsBundle.SetActive(true);
                break;
            case "io.blackcandy.basketballjam.iap.noads":
                NoAds.SetActive(true);
                break;
            case "io.blackcandy.basketballjam.iap.coin":
                Coin.SetActive(true);
                coinText.text = succesfulProduct.coin.ToString();
                break;
        }
    }
}
