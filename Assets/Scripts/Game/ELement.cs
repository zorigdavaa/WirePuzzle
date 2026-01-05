using System;
using Unity.Android.Gradle;
using UnityEngine;

public abstract class PuzzleElement : MonoBehaviour
{
    public EventHandler<PuzzleElement> OnDestroyed;
    public abstract void TakeDamage();
}
