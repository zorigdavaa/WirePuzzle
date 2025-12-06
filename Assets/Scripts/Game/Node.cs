using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, ISlotObj
{
    [SerializeField] List<GameObject> Models;
    public bool IsUpgradeAble
    {
        get
        {
            return ModelIndex < Models.Count - 1;
        }
    }
    public int ModelIndex { get; set; }
    public Slot Slot { get; set; }

    public void Upgrade()
    {
        Models[ModelIndex].SetActive(false);
        ModelIndex++;
        Models[ModelIndex].SetActive(true);
    }
}
