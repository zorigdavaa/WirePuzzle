using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using Random = UnityEngine.Random;

public class Prefabs : GenericSingleton<Prefabs>
{
    public GameObject FireWork;
    public GameObject CoinPF;

    public void CreateFireWork(int item, RowCol type, Grid<GridNode> grid)
    {
        Debug.Log("Firework");
        // Vector3 initialPos;
        // Vector3 tagetPos;
        List<GridNode> DestroyNodes = new();
        if (type == RowCol.Column)
        {
            // initialPos = grid.GetWorldPosition(item, 0).SwitchYZ();
            // tagetPos = grid.GetWorldPosition(item, grid.GetWidth()).SwitchYZ();
            for (int i = 0; i < grid.GetWidth(); i++)
            {
                DestroyNodes.Add(grid.GetGridObject(item, i));
            }
        }
        else
        {
            // initialPos = grid.GetWorldPosition(0, item).SwitchYZ();
            // tagetPos = grid.GetWorldPosition(grid.GetHeight(), item).SwitchYZ();
            for (int i = 0; i < grid.GetHeight(); i++)
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

            DestroyNodes[0].Slot.ScaledDestroy();
            int nodeIndex = 1;
            // Vector3 initial = transform.position;
            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                fireWork.transform.position = Vector3.Lerp(initialPos, tagetPos, t);
                // DestroyNodes[0].Slot.ScaledDestroy();
                // DestroyNodes.RemoveAt(0);

                int targetIndex = Mathf.FloorToInt(t * DestroyNodes.Count);

                while (nodeIndex <= targetIndex && nodeIndex < DestroyNodes.Count)
                {
                    DestroyNodes[nodeIndex].Slot.ScaledDestroy();
                    nodeIndex++;
                }
                yield return null;
            }
            Destroy(fireWork);
        }
    }

}

public enum RowCol
{
    Row, Column
}