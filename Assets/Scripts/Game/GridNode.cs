using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GridNode Parent;
    public int X;
    public int Y;
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get { return GCost + HCost; } }
    public Vector3 Position { get { return new Vector3(X, 0, Y); } }
    public bool IsTraversable { get { return Slot.IsFilled(); } }
    public bool IsPermanentBlocked { get { return Slot.IsPermanentBlocked(); } }
    private Slot _slot;
    public Slot Slot
    {
        get
        {
            if (_slot == null)
            {
                _slot = GetComponent<Slot>();
            }
            return _slot;
        }
    }


    public Grid<GridNode> OwnGrid { get; internal set; }
}
