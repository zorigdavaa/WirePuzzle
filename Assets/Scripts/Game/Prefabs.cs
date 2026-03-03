using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using Random = UnityEngine.Random;

public class Prefabs : GenericSingleton<Prefabs>
{
    Camera cam;
    public GameObject FireWork;
    public GameObject CoinPF;
    public GameObject Lightning;
    public GameObject ConnectFiller;

    void Start()
    {
        cam = Camera.main;
    }

    public void ShortCircuit(int item, RowCol type, Grid<GridNode> grid, Action onComplete = null, bool withEffect = true)
    {
        Debug.Log("Firework");
        // Vector3 initialPos;
        // Vector3 tagetPos;
        List<GridNode> DestroyNodes = new();
        if (type == RowCol.Row)
        {
            // initialPos = grid.GetWorldPosition(item, 0).SwitchYZ();
            // tagetPos = grid.GetWorldPosition(item, grid.GetWidth()).SwitchYZ();
            for (int i = 0; i < grid.GetHeight(); i++)
            {
                DestroyNodes.Add(grid.GetGridObject(item, i));
            }
        }
        else
        {
            // initialPos = grid.GetWorldPosition(0, item).SwitchYZ();
            // tagetPos = grid.GetWorldPosition(grid.GetHeight(), item).SwitchYZ();
            for (int i = 0; i < grid.GetWidth(); i++)
            {
                DestroyNodes.Add(grid.GetGridObject(i, item));
            }
        }
        if (Random.value > 0.5f)
        {
            // Vector3 temp = initialPos;
            // initialPos = tagetPos;
            // tagetPos = temp;
            DestroyNodes.Reverse();
        }
        // Debug.Log($"{initialPos}  and {tagetPos}");
        // Vector3 direction = tagetPos - initialPos;
        if (DestroyNodes.Count == 0)
        {
            Debug.LogError("No Nodes to Destroy");
        }
        Vector3 direction = (DestroyNodes[^1].transform.position - DestroyNodes[0].transform.position).normalized;
        Vector3 initialPos = DestroyNodes[0].transform.position;
        Vector3 tagetPos = DestroyNodes[^1].transform.position;
        GameObject fireWork = Instantiate(FireWork, Vector3.zero, Quaternion.LookRotation(direction), transform);
        // fireWork.transform.position = initialPos;
        StartCoroutine(LocalCor());
        IEnumerator LocalCor()
        {
            float t;
            float time = 0f;
            float duration = 0.3f;

            int nodeIndex = 0;
            // Vector3 initial = transform.position;
            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                fireWork.transform.position = Vector3.Lerp(initialPos, tagetPos, t);
                // DestroyNodes[0].Slot.ScaledDestroy();
                // DestroyNodes.RemoveAt(0);
                if (withEffect)
                {
                    int targetIndex = Mathf.FloorToInt(t * DestroyNodes.Count);

                    while (nodeIndex <= targetIndex && nodeIndex < DestroyNodes.Count)
                    {
                        DestroyNodes[nodeIndex].Slot.ScaledDestroy();
                        nodeIndex++;
                    }
                }

                yield return null;
            }
            onComplete?.Invoke();
            time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                fireWork.transform.position = Vector3.Lerp(fireWork.transform.position, tagetPos + direction, t);
                fireWork.transform.localScale = Vector3.Lerp(fireWork.transform.localScale, Vector3.zero, t);
                yield return null;
            }
            Destroy(fireWork);
        }
    }
    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    public void GetCoin(Vector3 worldPos)
    {
        GameObject Coin = Instantiate(Prefabs.Instance.CoinPF, worldPos + Vector3.up, Quaternion.identity, transform);
        StartCoroutine(LocalCor(Coin));

    }
    IEnumerator LocalCor(GameObject coin)
    {
        float t = 0f;
        float time = 0f;
        float duration = 1.0f;
        Vector3 initial = coin.transform.position;
        // Use the UI Camera (often the same as Main Camera, but check your Canvas)
        // 1. Get the screen position of the UI element (use null for Overlay)
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, Z.CanM.coinText.transform.position);

        // 2. Define how far in front of the camera the 3D target should be
        float distanceFromCamera = 10f;

        // 3. Create a Vector3 for ScreenToWorldPoint (x, y, and z as depth)
        Vector3 screenPosWithDepth = new Vector3(screenPos.x, screenPos.y, distanceFromCamera);

        // 4. Convert that screen point to a position in the 3D world
        Vector3 targetWorldPos = cam.ScreenToWorldPoint(screenPosWithDepth);


        while (time < duration)
        {
            time += Time.deltaTime;
            t = time / duration;
            coin.transform.position = Vector3.Lerp(initial, targetWorldPos, t);
            yield return null;
        }
        Destroy(coin);
        Z.GM.Coin++;
    }
}

public enum RowCol
{
    Row, Column
}