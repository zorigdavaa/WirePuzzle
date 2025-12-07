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
    // public
    // Start is called before the first frame update
    void Start()
    {
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
        Slots.Add(insObj.GetComponent<Slot>());
        insObj.GetComponent<GridNode>().X = x;
        insObj.GetComponent<GridNode>().Y = y;
        insObj.GetComponent<GridNode>().OwnGrid = grid;
        return insObj.GetComponent<GridNode>();
    }

    public bool IsPlaceAble(Piece selectedPiece, out List<Slot> freeSlots)
    {
        freeSlots = new List<Slot>();
        foreach (var item in selectedPiece.GetNodes())
        {
            print(item.transform.position);
            Slot slot = Grid.GetGridObject(item.transform.position.SwitchYZ()).GetComponent<Slot>();
            if (slot != null && slot.IsFree())
            {
                freeSlots.Add(slot);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void Place(Piece selectedPiece, List<Slot> freeSlots)
    {
        List<Node> nodes = selectedPiece.GetNodes();
        for (int i = 0; i < freeSlots.Count; i++)
        {
            freeSlots[i].SetObj(nodes[i]);
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

    public List<Vector3> FindPath(GridNode startPos, GridNode targetPos)
    {
        List<Vector3> path = new List<Vector3>();
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

    private List<Vector3> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        GridNode currentNode = endNode;
        path.Add(Grid.GetWorldPosition(currentNode.X, currentNode.Y));
        while (currentNode != startNode)
        {
            // path.Add(currentNode.Position);
            path.Add(Grid.GetWorldPosition(currentNode.X, currentNode.Y));
            // print(" Position was " + currentBusStop.Grid.GetWorldPosition(currentNode.X, currentNode.Y));
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
}

