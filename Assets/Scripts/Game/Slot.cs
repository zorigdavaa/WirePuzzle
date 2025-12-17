using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;

public class Slot : MonoBehaviour
{
    public SlotType type;
    public Node Obj; //ISlotObj type
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
        Obj = slotObj.gameObject.GetComponent<Node>();
        if (slotObj != null)
        {
            slotObj.Slot = this;
            Obj.transform.position = transform.position;
            Obj.transform.SetParent(transform);
            // shooter.SetSlot(this);
            // shooter.transform.position = transform.position;
        }
    }
    public Node GetObj()
    {
        return Obj.GetComponent<Node>();
    }

    public bool IsFree()
    {
        return Obj == null && type != SlotType.Blocked;
    }

    public void DestoyObjWithShine()
    {
        if (Obj != null)
        {
            Destroy(Obj.gameObject, 1f);
            Obj.GetComponent<Node>().Shine();
            Obj = null;
            Z.GM.Coin++;
        }
    }

    public void ScaledDestroy()
    {
        if (Obj != null)
        {
            Obj.GetComponent<Node>().Scale();
            Obj = null;
            Z.GM.Coin++;
        }
    }

    internal void DestroyWithNicoin()
    {
        if (Obj != null)
        {

            Destroy(Obj.gameObject);
            Obj = null;
            Z.GM.Coin++;
        }
    }
}

public enum SlotType
{
    None, Power, Light, Blocked
}
