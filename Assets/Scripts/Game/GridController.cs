using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using ZPackage;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GridController : MonoBehaviour
{
    public List<Slot> Slots;
    [SerializeField] GameObject SlotPF;
    public Grid<GridNode> Grid;
    public int X;
    public int Y;
    [SerializeField] float toBottom = 0.3f;

    public void Init(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
        Slots.Clear();
        CreateGrid();
    }

    private void CreateGrid()
    {
        Grid = new(X, Y, 1, transform.position + new Vector3(-X * 0.5f, -Y * toBottom, 0), CreateSlot, transform);
    }

    GridNode CreateSlot(Grid<GridNode> grid, int x, int y)
    {
        // foreach (var item in Slots)
        // {
        //     Destroy(item.gameObject);
        // }

        GridNode insObj = Instantiate(SlotPF, grid.GetWorldPLacement(x, y).SwitchYZ(), Quaternion.identity, transform).GetComponent<GridNode>();
        // insObj.transform.localPosition = new Vector3(x + 0.5f, 0, y + 0.5f);
        Slots.Add(insObj.Slot);
        insObj.X = x;
        insObj.Y = y;
        insObj.OwnGrid = grid;
        return insObj;
    }

    public bool IsPlaceAble(Piece selectedPiece, out List<GridNode> freeSlots)
    {
        freeSlots = new List<GridNode>();
        foreach (var item in selectedPiece.GetNodes())
        {
            // print(item.transform.position);
            GridNode node = Grid.GetGridObject(item.transform.position.SwitchYZ());
            Slot slot = node?.Slot;
            if (node != null && slot.IsFree())
            {
                freeSlots.Add(node);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void Place(Piece selectedPiece, List<GridNode> freeSlots)
    {
        Destroy(selectedPiece.GetSilhoutte());
        List<Node> nodes = selectedPiece.GetNodes();
        for (int i = 0; i < freeSlots.Count; i++)
        {
            freeSlots[i].Slot.SetObj(nodes[i]);
        }

    }
    [ContextMenu("Place Slots")]
    public void PlaceSlots()
    {
        Slots.Clear();
        // Must use Undo in prefab mode
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
        }
        CreateGrid();

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    public List<GridNode> GetNeighbors(GridNode currentNode)
    {
        List<GridNode> neighbors = new List<GridNode>();

        // Define the offsets for left, right, up, and down
        int[] xOffset = { -1, 1, 0, 0 };
        int[] yOffset = { 0, 0, 1, -1 };

        for (int i = 0; i < xOffset.Length; i++)
        {
            int checkX = currentNode.X + xOffset[i];
            int checkY = currentNode.Y + yOffset[i];

            // Check if the neighbor is within the grid bounds
            if (checkX >= 0 && checkX < Grid.GetWidth() && checkY >= 0 && checkY < Grid.GetHeight())
            {
                GridNode neighbor = Grid.GetGridObject(checkX, checkY);

                // Check if the neighbor is traversable
                // if (neighbor != null)
                if (neighbor != null && neighbor.IsTraversable)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    public int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.X - nodeB.X);
        int dstY = Mathf.Abs(nodeA.Y - nodeB.Y);
        return dstX + dstY;
    }

    public List<GridNode> FindPath(GridNode startPos, GridNode targetPos)
    {
        List<GridNode> path = new List<GridNode>();
        // Create lists for open and closed nodes
        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        // Add the start node to the open list
        openList.Add(startPos);

        // Start the A* algorithm
        while (openList.Count > 0)
        {
            // Get the node with the lowest F cost from the open list
            GridNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                {
                    currentNode = openList[i];
                }
            }

            // Remove the current node from the open list and add it to the closed set
            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            // Check if we've reached the target node
            if (currentNode == targetPos)
            {
                // We've found the path, so retrace it and return
                // path = RetracePath(currentNode, targetPos);
                path = RetracePath(startPos, targetPos);
                // print("Found " + path.Count);
                return path;
            }

            // Get the neighboring nodes of the current node
            List<GridNode> neighbors = GetNeighbors(currentNode);
            // Debug.Log(neighbors.Count + " Neighbors Count");
            // Process each neighboring node
            foreach (GridNode neighbor in neighbors)
            {
                // Skip this neighbor if it is not traversable or if it is in the closed set
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the new tentative G cost for this neighbor
                int newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                // If the new G cost is lower than the neighbor's current G cost or if the neighbor is not in the open list
                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    // Update the neighbor's G cost and H cost
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetDistance(neighbor, targetPos);

                    // Set the neighbor's parent to the current node
                    neighbor.Parent = currentNode;

                    // If the neighbor is not in the open list, add it
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // print("Path not found");
        // No path found, return an empty path
        return path;
    }

    private List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;
        path.Add(currentNode);
        while (currentNode != startNode)
        {
            // path.Add(currentNode.Position);
            path.Add(currentNode);
            // print(" Position was " + currentBusStop.Grid.GetWorldPosition(currentNode.X, currentNode.Y));
            currentNode = currentNode.Parent;
        }
        path.Add(startNode);
        path.Reverse();
        return path;
    }

    internal List<Piece> GetNeededPiece(List<Piece> piecesPf)
    {
        List<GridNode> FreeNodes = Slots.Where(s => s.IsFree()).Select(s => s.GetComponent<GridNode>()).ToList();
        List<Piece> neededPieces = new List<Piece>();
        // List<Piece> UnneccessaryPieces = new List<Piece>();
        foreach (var item in piecesPf)
        {
            if (IsPlaceAbleSomeWhere(item, out List<GridNode> placeAbleNodes, FreeNodes))
            {
                neededPieces.Add(item);
            }
        }

        // foreach (var item in piecesPf)
        // {
        //     var itemNodes = item.GetNodesAsOffset();
        //     // print($"Offsets Count: {itemNodes.Count}");
        //     foreach (var freeNode in FreeNodes)
        //     {
        //         Vector2 basePos = new Vector2(freeNode.X, freeNode.Y);
        //         bool canPlace = true;

        //         foreach (var offset in itemNodes)
        //         {
        //             Vector2 checkPos = basePos + offset;
        //             GridNode checkNode = Grid.GetGridObject((int)checkPos.x, (int)checkPos.y);
        //             if (checkNode == null || !checkNode.GetComponent<Slot>().IsFree())
        //             {
        //                 canPlace = false;
        //                 break;
        //             }
        //         }
        //         if (canPlace)
        //         {
        //             print($"Can place at {basePos.x}, {basePos.y} and found {item.name}");
        //             neededPieces.Add(item);
        //             break;
        //         }
        //     }
        // }
        print($"Free Nodes Count: {FreeNodes.Count}");
        print($"Needed Pieces Count: {neededPieces.Count}");
        return neededPieces;
    }

    public bool IsPlaceAbleSomeWhere(Piece piece, out List<GridNode> placeAbleNodes, List<GridNode> CheckNodes = null)
    {
        if (CheckNodes == null)
        {
            CheckNodes = Slots
                .Where(s => s.IsFree())
                .Select(s => s.GetComponent<GridNode>())
                .ToList();
        }

        var offsets = piece.GetNodesAsOffset();
        // offsets.RemoveAt(0);
        placeAbleNodes = new();
        foreach (var node in CheckNodes)
        {
            placeAbleNodes.Clear();
            Vector2 basePos = new Vector2(node.X, node.Y);
            bool canPlaceHere = true;
            placeAbleNodes.Add(node);
            foreach (var offset in offsets)
            {
                Vector2 checkPos = basePos + offset;
                GridNode checkNode = Grid.GetGridObject((int)checkPos.x, (int)checkPos.y);
                placeAbleNodes.Add(checkNode);
                if (checkNode == null || !checkNode.Slot.IsFree())
                {
                    canPlaceHere = false;
                    break;
                }
            }

            if (canPlaceHere)
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator ColumnRowCheck()
    {
        // HashSet<GridNode> destroyedNodes = new HashSet<GridNode>();
        HashSet<int> xLinesToDestroy = new HashSet<int>();
        HashSet<int> yLinesToDestroy = new HashSet<int>();
        for (int x = 0; x < X; x++)
        {
            if (!IsColumnFull(x))
                continue;

            // for (int y = 0; y < Y; y++)
            // {
            //     destroyedNodes.Add(Grid.GetGridObject(x, y));
            // }
            xLinesToDestroy.Add(x);
        }
        for (int y = 0; y < Y; y++)
        {
            if (!IsRowFull(y))
                continue;

            // for (int x = 0; x < X; x++)
            // {
            //     destroyedNodes.Add(Grid.GetGridObject(x, y));
            // }
            yLinesToDestroy.Add(y);
        }
        // foreach (var item in destroyedNodes)
        // {
        //     item.Slot.ScaledDestroy();
        // }
        int Counter = 0;
        foreach (var item in xLinesToDestroy)
        {
            Counter++;
            Prefabs.Instance.CreateFireWork(item, RowCol.Column, Grid, () =>
            {
                Counter--;
            });
        }
        foreach (var item in yLinesToDestroy)
        {
            Counter++;
            Prefabs.Instance.CreateFireWork(item, RowCol.Row, Grid, () =>
            {
                Counter--;
            });
        }
        yield return new WaitUntil(() => Counter == 0);
        // onComplete?.Invoke();
    }
    public IEnumerator ClearRowAndColumns()
    {
        int Counter = 0;
        for (int i = 0; i < Grid.GetHeight(); i++)
        {
            Counter++;
            Prefabs.Instance.CreateFireWork(i, RowCol.Column, Grid, () =>
            {
                Counter--;
            });
        }

        yield return new WaitUntil(() => Counter == 0);
    }
    bool IsColumnFull(int x)
    {
        for (int y = 0; y < Y; y++)
        {
            if (Grid.GetGridObject(x, y).Slot.IsFree())
                return false;
        }
        return true;
    }
    bool IsRowFull(int y)
    {
        for (int x = 0; x < X; x++)
        {
            if (Grid.GetGridObject(x, y).Slot.IsFree())
                return false;
        }
        return true;
    }

    public void ResetSlotTypes()
    {
        foreach (var item in Slots)
        {
            item.SetType(SlotType.Empty);
        }
    }

    internal void AddRow()
    {
        DestroyCurrentGrid();
        Y++;
        CreateGrid();
    }

    private void DestroyCurrentGrid()
    {
        foreach (var item in Slots)
        {
            Destroy(item.gameObject);
        }
        Slots.Clear();
    }

    internal void AddColumn()
    {
        DestroyCurrentGrid();
        X++;
        CreateGrid();
    }

    // private List<Vector3> RetracePath(GridNode startNode, GridNode endNode)
    // {
    //     List<Vector3> path = new List<Vector3>();
    //     GridNode currentNode = endNode;
    //     path.Add(Grid.GetWorldPosition(currentNode.X, currentNode.Y));
    //     while (currentNode != startNode)
    //     {
    //         // path.Add(currentNode.Position);
    //         path.Add(Grid.GetWorldPosition(currentNode.X, currentNode.Y));
    //         // print(" Position was " + currentBusStop.Grid.GetWorldPosition(currentNode.X, currentNode.Y));
    //         currentNode = currentNode.Parent;
    //     }
    //     path.Reverse();
    //     return path;
    // }

}

