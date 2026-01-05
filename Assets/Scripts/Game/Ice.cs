using System.Collections.Generic;
using UnityEngine;

public class Ice : PuzzleElement
{
    public List<GameObject> Models;
    public int ModelIndex = 0;

    public override void TakeDamage()
    {
        ModelIndex++;
        foreach (var item in Models)
        {
            item.SetActive(false);
        }
        if (Models.Count > ModelIndex)
        {
            Models[ModelIndex].SetActive(true);
        }
        else
        {
            OnDestroyed?.Invoke(this, this);
        }
    }
}
