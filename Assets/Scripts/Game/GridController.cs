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
        if (selectedPiece.GetSilhoutteDirect() != null)
        {
            Destroy(selectedPiece.GetSilhoutteDirect());
        }
        List<Node> nodes = selectedPiece.GetNodes();
        for (int i = 0; i < freeSlots.Count; i++)
        {
            freeSlots[i].Slot.SetObj(nodes[i]);
        }

    }
    [ContextMenu("Place Slots")]
    public void PlaceSlots()
    {
#if UNITY_EDITOR
        Slots.Clear();
        // Must use Undo in prefab mode

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
        }
        CreateGrid();

        EditorUtility.SetDirty(gameObject);
#endif
    }

    public List<GridNode> GetNeighbors(GridNode currentNode, bool ignoreTraversible = false, bool EightDirections = false)
    {
        List<GridNode> neighbors = new List<GridNode>();

        // Define the offsets for left, right, up, and down
        int[] xOffset = { -1, 1, 0, 0 };
        int[] yOffset = { 0, 0, 1, -1 };
        if (EightDirections)
        {
            xOffset = new int[] { -1, 1, 0, 0, -1, -1, 1, 1 };
            yOffset = new int[] { 0, 0, 1, -1, 1, -1, 1, -1 };
        }

        for (int i = 0; i < xOffset.Length; i++)
        {
            int checkX = currentNode.X + xOffset[i];
            int checkY = currentNode.Y + yOffset[i];

            // Check if the neighbor is within the grid bounds
            if (checkX >= 0 && checkX < Grid.GetWidth() && checkY >= 0 && checkY < Grid.GetHeight())
            {
                GridNode neighbor = Grid.GetGridObject(checkX, checkY);

                if (neighbor != null &&
                    ((ignoreTraversible && !neighbor.IsPermanentBlocked && !ThisMakesFull(neighbor.X, neighbor.Y)) ||
                     (!ignoreTraversible && neighbor.IsTraversable)))
                {
                    neighbors.Add(neighbor);
                }

                // Check if the neighbor is traversable
                // if (neighbor != null)
                // {
                //     if (ignoreTraversible && !neighbor.IsPermanentBlocked)
                //     {
                //         neighbors.Add(neighbor);
                //     }
                //     else if (neighbor.IsTraversable)
                //     {
                //         neighbors.Add(neighbor);
                //     }
                // }
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

        if (!startPos.IsTraversable || !targetPos.IsTraversable)
        {
            return path;
        }
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
                if (closedSet.Contains(neighbor))
                    continue;

                int newGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetDistance(neighbor, targetPos);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
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

    public List<GridNode> FindSafePath(GridNode start, GridNode target)
    {
        Queue<PathState> queue = new Queue<PathState>();

        int startIndex = GetIndex(start.X, start.Y);
        ulong startMask = 1UL << startIndex;

        PathState startState = new PathState(start, startMask, null);
        queue.Enqueue(startState);

        // 🚨 THIS is what prevents infinite loop
        HashSet<(int, ulong)> visited = new HashSet<(int, ulong)>();
        visited.Add((startIndex, startMask));

        int iterations = 0;
        const int MAX_ITER = 20000;      // 🔴 Hard safety stop

        while (queue.Count > 0)
        {
            if (++iterations > MAX_ITER)
            {
                Debug.LogWarning("Path search stopped: iteration limit");
                break;
            }
            PathState state = queue.Dequeue();

            if (state.Node == target)
                return RetraceStatePath(state);

            foreach (var neighbor in GetNeighbors(state.Node, true))
            {
                int idx = GetIndex(neighbor.X, neighbor.Y);
                ulong newMask = state.Mask | (1UL << idx);

                if (CreatesFullLine(newMask))
                    continue;

                var key = (idx, newMask);
                if (visited.Contains(key))
                    continue;

                visited.Add(key);
                queue.Enqueue(new PathState(neighbor, newMask, state));
            }
        }

        return new List<GridNode>();
    }

    int GetIndex(int x, int y)
    {
        return y * X + x;
    }

    bool CreatesFullLine(ulong mask)
    {
        // check rows
        for (int y = 0; y < Y; y++)
        {
            bool full = true;

            for (int x = 0; x < X; x++)
            {
                int idx = GetIndex(x, y);
                if ((mask & (1UL << idx)) == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full) return true;
        }

        // check columns
        for (int x = 0; x < X; x++)
        {
            bool full = true;

            for (int y = 0; y < Y; y++)
            {
                int idx = GetIndex(x, y);
                if ((mask & (1UL << idx)) == 0)
                {
                    full = false;
                    break;
                }
            }

            if (full) return true;
        }

        return false;
    }



    List<GridNode> RetraceStatePath(PathState endState)
    {
        List<GridNode> path = new List<GridNode>();

        PathState current = endState;

        while (current != null)
        {
            path.Add(current.Node);
            current = current.Parent;
        }

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

    public bool TryGetPlacementOnPath(Piece piece, IEnumerable<GridNode> preferredPath, out List<GridNode> placeAbleNodes)
    {
        var preferredFreeNodes = preferredPath?
            .Where(node => node != null && node.Slot != null && node.Slot.IsFree())
            .Distinct()
            .ToList();

        if (preferredFreeNodes != null && preferredFreeNodes.Count > 0 &&
            IsPlaceAbleSomeWhere(piece, out placeAbleNodes, preferredFreeNodes))
        {
            return true;
        }

        placeAbleNodes = new List<GridNode>();
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
            Prefabs.Instance.ShortCircuit(item, RowCol.Row, Grid, () =>
            {
                Counter--;
            });
        }
        foreach (var item in yLinesToDestroy)
        {
            Counter++;
            Prefabs.Instance.ShortCircuit(item, RowCol.Column, Grid, () =>
            {
                Counter--;
            });
        }
        yield return new WaitUntil(() => Counter == 0);
        // onComplete?.Invoke();
    }
    public bool WillTheseMakesFullThenDestroy(List<GridNode> placeAbleNodes)
    {
        HashSet<int> xLinesToDestroy = new HashSet<int>();
        HashSet<int> yLinesToDestroy = new HashSet<int>();
        for (int x = 0; x < X; x++)
        {
            if (!IsColumnFullWith(x, placeAbleNodes))
                continue;
            xLinesToDestroy.Add(x);
        }
        for (int y = 0; y < Y; y++)
        {
            if (!IsRowFullWith(y, placeAbleNodes))
                continue;
            yLinesToDestroy.Add(y);
        }
        foreach (var item in xLinesToDestroy)
        {
            Prefabs.Instance.ShortCircuit(item, RowCol.Row, Grid, null, false);
        }
        foreach (var item in yLinesToDestroy)
        {
            Prefabs.Instance.ShortCircuit(item, RowCol.Column, Grid, null, false);
        }
        return xLinesToDestroy.Count > 0 || yLinesToDestroy.Count > 0;
    }
    public IEnumerator ClearRowAndColumns()
    {
        int Counter = 0;
        for (int i = 0; i < Grid.GetHeight(); i++)
        {
            Debug.Log("Clearing Row " + i);
            Counter++;
            Prefabs.Instance.ShortCircuit(i, RowCol.Column, Grid, () =>
            {
                Counter--;
            });
        }

        yield return new WaitUntil(() => Counter == 0);
    }
    bool ThisMakesFull(int x, int y)
    {
        if (!Grid.GetGridObject(x, y).Slot.IsFree())
            return false;
        bool triggerColumnDestroy = true;
        bool triggerRowDestroy = true;
        for (int checkX = 0; checkX < X; checkX++)
        {
            if (checkX != x && Grid.GetGridObject(checkX, y).Slot.IsFree())
            {
                triggerRowDestroy = false;
            }
        }
        if (triggerRowDestroy)
        {
            return true;
        }
        for (int checkY = 0; checkY < Y; checkY++)
        {
            if (checkY != y && Grid.GetGridObject(x, checkY).Slot.IsFree())
            {
                triggerColumnDestroy = false;
            }
        }
        if (triggerColumnDestroy)
        {
            return true;
        }
        return false;
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
    bool IsColumnFullWith(int x, List<GridNode> placeAbleNodes)
    {
        for (int y = 0; y < Y; y++)
        {
            GridNode node = Grid.GetGridObject(x, y);
            if (node.Slot.IsFree())
            {
                if (!placeAbleNodes.Contains(node))
                {

                    return false;
                }
            }

        }
        return true;
    }
    bool IsRowFullWith(int y, List<GridNode> placeAbleNodes)
    {
        for (int x = 0; x < X; x++)
        {
            GridNode node = Grid.GetGridObject(x, y);
            if (node.Slot.IsFree())
            {
                if (!placeAbleNodes.Contains(node))
                {

                    return false;
                }
            }
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

    public void AddRow()
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

    public void AddColumn()
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

