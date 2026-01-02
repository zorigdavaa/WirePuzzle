using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(fileName = "IAP - ", menuName = "IAP/IAP Product", order = 1)]
public class CkProduct : ScriptableObject
{

    [SerializeField] string id;
    [SerializeField] ProductType productType;

    public string ID => id;
    public ProductType ProductType => productType;
    public bool isRemoveADS;
    public int coin;
    internal int healthTimeHour;
}
