using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPProduct : MonoBehaviour
{
    public CkProduct product;

    public void TryBuy()
    {
        Debug.Log($"TRY BUY: {product.ID}");
        IAPShopHandler.Instance.Buy(product);
        CanvasManager.Instance.CloseIAPShop();
    }
}
