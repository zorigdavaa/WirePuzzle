using System;
using System.Collections;
using UnityEngine;
using ZPackage;

public class Prefabs : GenericSingleton<Prefabs>
{
    public GameObject FireWork;
    public GameObject CoinPF;

    public void CreateFireWork(int item, RowCol type, Grid<GridNode> grid)
    {
        Debug.Log("Firework");
        Vector3 initialPos;
        Vector3 tagetPos;
        if (type == RowCol.Row)
        {
            initialPos = grid.GetWorldPosition(item, 0).SwitchYZ();
            tagetPos = grid.GetWorldPosition(item, grid.GetWidth()).SwitchYZ();
        }
        else
        {
            initialPos = grid.GetWorldPosition(0, item).SwitchYZ();
            tagetPos = grid.GetWorldPosition(grid.GetHeight(), 0).SwitchYZ();
        }
        Vector3 direction = tagetPos - initialPos;
        GameObject fireWork = Instantiate(FireWork, initialPos, Quaternion.LookRotation(direction), transform);
        StartCoroutine(LocalCor(fireWork, tagetPos));
    }
    IEnumerator LocalCor(GameObject firework, Vector3 target)
    {
        float t = 0f;
        float time = 0f;
        float duration = 1.0f;
        Vector3 initial = transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            t = time / duration;
            firework.transform.position = Vector3.Lerp(initial, target, t);
            yield return null;
        }
        Destroy(firework);
    }
}

public enum RowCol
{
    Row, Column
}