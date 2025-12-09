using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;

public class Level : MonoBehaviour
{
    public LevelData Data;
    GridController gridController;
    public List<Slot> ChargerPoses;
    public List<Slot> ConnectPoses;
    public List<Slot> Blocked;

    // public slot
    void Start()
    {
        gridController = transform.GetComponentInChildren<GridController>();
        gridController.Init();
        foreach (var item in Data.ChargerPoses)
        {
            GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
            node.GetComponent<Slot>().SetType(SlotType.Power);
            ChargerPoses.Add(node.GetComponent<Slot>());
        }
        foreach (var item in Data.ConnectPoses)
        {
            GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
            node.GetComponent<Slot>().SetType(SlotType.Light);
            ConnectPoses.Add(node.GetComponent<Slot>());
        }
        foreach (var item in Data.Blocked)
        {
            GridNode node = gridController.Grid.GetGridObject((int)item.x, (int)item.y);
            node.GetComponent<Slot>().SetType(SlotType.Blocked);
            Blocked.Add(node.GetComponent<Slot>());
        }
    }
    public void CheckConnected()
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
                    print($"Searching {Light.GetComponent<GridNode>().X} and {Light.GetComponent<GridNode>().Y} to {charger.GetComponent<GridNode>().X} {charger.GetComponent<GridNode>().Y}");
                    var foundPaht = gridController.FindPath(Light.GetComponent<GridNode>(), charger.GetComponent<GridNode>());
                    if (foundPaht.Count > 0)
                    {
                        paths.Add(foundPaht);
                    }
                }
            }
        }
        print($"found {paths.Count}");
        foreach (var path in paths)
        {
            foreach (var node in path)
            {

                node.GetComponent<Slot>().DestoyObj();

            }
        }
    }
}
