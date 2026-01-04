using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelConnectData
{
    public List<CellData> cellDatas;
    public LevelConnectData(List<CellData> data)
    {
        cellDatas = data;
    }
    // public List<Vector2> ChargerPoses;
    // public List<Vector2> ConnectPoses;
    // public List<Vector2> Blocked;
    // public LevelConnectData()
    // {
    //     cellDatas = new();
    //     // ChargerPoses = new();
    //     // ConnectPoses = new();
    //     // Blocked = new();
    // }
}
[System.Serializable]
public struct CellData
{
    public SlotType Type;
    public Vector2Int Position;
}
