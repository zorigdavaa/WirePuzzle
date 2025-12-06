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
    public Grid<Slot> Grid;
    public int X;
    public int Y;
    [SerializeField] float toBottom = 0.3f;
    // public
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        Grid = new(X, Y, 1, transform.position + new Vector3(-X * 0.5f, -Y * toBottom, 0), CreateSlot, transform);
    }

    Slot CreateSlot(Grid<Slot> grid, int x, int y)
    {
        GameObject insObj = Instantiate(SlotPF, grid.GetWorldPLacement(x, y).SwitchYZ(), Quaternion.identity, transform);
        // insObj.transform.localPosition = new Vector3(x + 0.5f, 0, y + 0.5f);
        Slots.Add(insObj.GetComponent<Slot>());
        return insObj.GetComponent<Slot>();
    }

    public bool IsPlaceAble(Piece selectedPiece, out List<Slot> freeSlots)
    {
        freeSlots = new List<Slot>();
        foreach (var item in selectedPiece.GetNodes())
        {
            print(item.transform.position);
            Slot slot = Grid.GetGridObject(item.transform.position.SwitchYZ());
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
            freeSlots[i].SetShooter(nodes[i]);
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

        EditorUtility.SetDirty(transform);
        CreateGrid();

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
    // public void SavePrefab()
    // {
    //     // Get path of the prefab the instance belongs to
    //     var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);

    //     if (!string.IsNullOrEmpty(path))
    //     {
    //         // Save changes back to prefab
    //         PrefabUtility.ApplyPrefabInstance(gameObject, InteractionMode.UserAction);
    //         AssetDatabase.SaveAssets();
    //         Debug.Log("Prefab saved: " + path);
    //     }
    // }
}

