using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    // public Renderer Renderer { get; set; }
    // private void Awake()
    // {
    //     Renderer = transform.GetChild(0).GetComponent<Renderer>();
    // }

    public void Upgrade()
    {
        Models[ModelIndex].SetActive(false);
        ModelIndex++;
        Models[ModelIndex].SetActive(true);
    }
    public void Shine()
    {
        StartCoroutine(LocalCor());
        IEnumerator LocalCor()
        {
            float t = 0f;
            float time = 0f;
            float duration = 1.0f;
            MeshRenderer render = transform.GetChild(0).GetComponent<MeshRenderer>();
            // Color initColor = render.material.color;
            Color initColor = Color.white;
            Color toColor = Color.yellow;
            render.material.EnableKeyword("_EMISSION");
            Color glowColor = Color.cyan * 1f;

            render.material.SetColor("_EmissionColor", glowColor);
            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                render.material.color = Color.Lerp(initColor, toColor, t);
                yield return null;
            }
        }
    }


    internal void Scale()
    {
        StartCoroutine(LocalCor());
        IEnumerator LocalCor()
        {
            float t = 0f;
            float time = 0f;
            float duration = 0.3f;
            Vector3 initScale = transform.localScale;
            Vector3 toScale = initScale * 0.05f;
            Vector3 initPos = transform.position;
            Vector3 toPos = initPos + new Vector3(Random.value, 0, Random.value).normalized * Random.value * 3;

            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                transform.position = Vector3.Lerp(transform.position, toPos, t);
                transform.localScale = Vector3.Lerp(transform.localScale, toScale, t);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
