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
    public void Shine()
    {
        StartCoroutine(LocalCor());
    }
    IEnumerator LocalCor()
    {
        float t = 0f;
        float time = 0f;
        float duration = 1.0f;
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color initColor = render.material.color;
        Color toColor = Color.red;

        while (time < duration)
        {
            time += Time.deltaTime;
            t = time / duration;
            render.material.color = Color.Lerp(initColor, toColor, t);
            yield return null;
        }
    }
}
