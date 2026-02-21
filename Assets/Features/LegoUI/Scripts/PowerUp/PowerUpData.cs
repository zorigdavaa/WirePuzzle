using System;
using UnityEngine;

// [CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp")]
[Serializable]
public abstract class PowerUpData : ScriptableObject
{
    [Header("Details")]
    public string Name;
    public string Description;
    public Sprite Icon;

    [Header("Cost")]
    public int unlockCoinCost;
    public int addCoinCost;
    public abstract void ApplyEffect(GameObject target, object context = null);
}
