using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;

public class Level : MonoBehaviour
{
    public LevelData Data;
    public GridController gridController;
    public List<Slot> ChargerPoses;
    public List<Slot> ConnectPoses;
    // public List<Slot> Blocked;
    public bool isInitialized = false;
    public int DataIndex = 0;

    // public slot
    void Start()
    {

    }

    private void SetGridByData()
    {
        SetGridByData(DataIndex);
        DataIndex++;
    }
    public void SetGridByData(int Index)
    {
        if (Index >= Data.LevelConnectDatas.Count)
        {
            if (Z.IsPlaying)
            {
                Z.GM.LevelComplete(this, 0);
            }
            return;
        }
        gridController.ResetSlotTypes();
        ConnectPoses.Clear();
        ChargerPoses.Clear();
        // Blocked.Clear();
        foreach (var item in Data.LevelConnectDatas[Index].cellDatas)
        {
            GridNode node = gridController.Grid.GetGridObject(item.Position.x, item.Position.y);
            node.GetComponent<Slot>().SetType(item.Type);
            if (item.Type == SlotType.Power)
            {
                ChargerPoses.Add(node.GetComponent<Slot>());
            }
            else if (item.Type == SlotType.Light)
            {
                ConnectPoses.Add(node.GetComponent<Slot>());
            }
            // ChargerPoses.Add(node.GetComponent<Slot>());
        }
        // foreach (var item in Data.LevelConnectDatas[Index].ChargerPoses)
        // {
        //     GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
        //     node.GetComponent<Slot>().SetType(SlotType.Power);
        //     ChargerPoses.Add(node.GetComponent<Slot>());
        // }
        // foreach (var item in Data.LevelConnectDatas[Index].ConnectPoses)
        // {
        //     GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
        //     node.GetComponent<Slot>().SetType(SlotType.Light);
        //     ConnectPoses.Add(node.GetComponent<Slot>());
        // }
        // foreach (var item in Data.LevelConnectDatas[Index].Blocked)
        // {
        //     GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
        //     node.GetComponent<Slot>().SetType(SlotType.Blocked);
        //     Blocked.Add(node.GetComponent<Slot>());
        // }
        ClearCurrentPath();

    }
    public void CheckColumnsRows()
    {
        StartCoroutine(NewMethod());

        IEnumerator NewMethod()
        {
            yield return gridController.ColumnRowCheck();
            CheckConnected();
        }
    }
    public bool CheckColumnsRows(List<GridNode> placeAbleNodes)
    {
        gridController.ColumnRowCheck(placeAbleNodes);
        return true;
    }

    public void CheckConnected()
    {
        List<List<GridNode>> paths = FindConnection();
        // print($"found {paths.Count}");
        foreach (var path in paths)
        {
            foreach (var node in path)
            {
                if (node.Slot.type == SlotType.Light)
                {
                    ConnectPoses.Remove(node.Slot);
                }
                node.GetComponent<Slot>().DestoyObjWithShine();

            }
        }

        if (paths.Count > 0 && ConnectPoses.Count == 0)
        {
            Invoke(nameof(SetGridByData), 1.5f);
            // SetGridByData();
        }
    }
    public void ClearCurrentPath()
    {
        gridController.ColumnRowCheck();
        List<List<GridNode>> paths = FindConnection();
        // print($"found {paths.Count}");
        foreach (var path in paths)
        {
            foreach (var node in path)
            {
                node.Slot.DestroyWithNoCoin();
            }
        }
    }

    private List<List<GridNode>> FindConnection()
    {
        List<List<GridNode>> paths = new List<List<GridNode>>();
        foreach (var Light in ConnectPoses)
        {
            foreach (var charger in ChargerPoses)
            {
                GridNode start = Light.GetComponent<GridNode>();
                GridNode end = charger.GetComponent<GridNode>();
                if (start.IsTraversable && end.IsTraversable)
                {
                    // print($"Searching {Light.GetComponent<GridNode>().X} and {Light.GetComponent<GridNode>().Y} to {charger.GetComponent<GridNode>().X} {charger.GetComponent<GridNode>().Y}");
                    var foundPaht = gridController.FindPath(Light.GetComponent<GridNode>(), charger.GetComponent<GridNode>());
                    if (foundPaht.Count > 0)
                    {
                        paths.Add(foundPaht);
                    }
                }
            }
        }

        return paths;
    }

    internal void FillSlot(Vector3 position)
    {
        GridNode node = gridController.Grid.GetGridObject(position);
        if (node && node.Slot.IsFree())
        {
            var instPiece = Instantiate(Z.LS.PieceController.GetSinglePiece(), node.transform.position, Quaternion.identity);
            gridController.Place(instPiece.GetComponent<Piece>(), new List<GridNode> { node });
        }
    }

    internal void Block(Vector3 worldMouse)
    {
        GridNode node = gridController.Grid.GetGridObject(worldMouse);
        if (node)
        {
            node.Slot.SetType(SlotType.Blocked);
        }
    }

    public void SetData(LevelData levelData)
    {
        Data = levelData;
        gridController = transform.GetComponentInChildren<GridController>();
        gridController.Init(Data.X, Data.Y);
        SetGridByData();
        if (GameManager.Instance)
        {
            // Z.PieceController.SetLevelPieces(levelData.Pieces);
            Z.PieceController.Init();
        }
        isInitialized = true;
    }
    public LevelConnectData GetConnectData()
    {
        LevelConnectData levelConnectData = new LevelConnectData(new());
        foreach (var item in gridController.Slots)
        {
            GridNode node = item.GetComponent<GridNode>();
            if (item.type != SlotType.Empty)
            {
                CellData data = new CellData
                {
                    Position = new Vector2Int(node.X, node.Y),
                    Type = item.type
                };

                levelConnectData.cellDatas.Add(data);
            }
            // if (item.type == SlotType.Light)
            // {
            //     levelConnectData.ConnectPoses.Add(new Vector2(node.X, node.Y));
            // }
            // else if (item.type == SlotType.Power)
            // {
            //     levelConnectData.ChargerPoses.Add(new Vector2(node.X, node.Y));
            // }
            // else if (item.type == SlotType.Blocked)
            // {
            //     levelConnectData.Blocked.Add(new Vector2(node.X, node.Y));
            // }
        }
        return levelConnectData;
    }

    public void ClearConnects()
    {
        foreach (var item in gridController.Slots)
        {
            GridNode node = item.GetComponent<GridNode>();
            node.Slot.SetType(SlotType.Empty);
        }
    }

    internal void TestGrid(Vector3 worldMouse)
    {
        GridNode node = gridController.Grid.GetGridObject(worldMouse);
        if (node)
        {
            Prefabs.Instance.ShortCircuit(node.Y, RowCol.Column, gridController.Grid);
            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // Destroy(cube, 3);
            // Vector3 position = gridController.Grid.GetWorldPosition(node.X, node.Y).SwitchYZ();
            // cube.transform.position = position;
        }
    }
}
