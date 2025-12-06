using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public SlotType type;
    public ISlotObj Obj;
    public List<GameObject> TypeModels;
    // Start is called before the first frame update
    void Start()
    {
        TypeModels[(int)type].gameObject.SetActive(true);
    }

    public void SetShooter(ISlotObj slotObj)
    {
        Obj = slotObj;
        if (slotObj != null)
        {
            Obj.Slot = this;
            Obj.transform.position = transform.position;
            Obj.transform.SetParent(transform);
            // shooter.SetSlot(this);
            // shooter.transform.position = transform.position;
        }
    }

    public bool IsFree()
    {
        return Obj == null;
    }
}

public enum SlotType
{
    None, Power, Light, Blocked
}
