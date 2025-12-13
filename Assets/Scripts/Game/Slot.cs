using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public SlotType type;
    ISlotObj Obj;
    public List<GameObject> TypeModels;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetType(SlotType type)
    {
        foreach (var item in TypeModels)
        {
            item.gameObject.SetActive(false);
        }
        this.type = type;
        TypeModels[(int)type].gameObject.SetActive(true);
    }

    public void SetObj(ISlotObj slotObj)
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
    public ISlotObj GetObj()
    {
        return Obj;
    }

    public bool IsFree()
    {
        return Obj == null;
    }

    public void DestoyObj()
    {
        if (Obj != null)
        {
            Destroy(Obj.gameObject, 1f);
            Debug.Log("destoyed");
            Obj.transform.GetComponent<Node>().Shine();
            Obj = null;
        }
    }
}

public enum SlotType
{
    None, Power, Light, Blocked
}
