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
        if (type == SlotType.Blocked)
        {
            DestroyWithNoCoin();
        }
        this.type = type;
        GetModel().gameObject.SetActive(true);
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
    public bool IsFilled()
    {
        return Obj != null;
    }

    public void DestoyObjWithShine()
    {
        if (Obj != null)
        {
            if (type == SlotType.Light)
            {
                // GetModel().GetComponent<Bulb>().TurnOn();
                StartCoroutine(LightCor());
            }
            Destroy(Obj.gameObject, 1f);
            Obj.GetComponent<Node>().Shine();
            Obj = null;
        }
        Debug.Log(GetComponent<GridNode>().X + " " + GetComponent<GridNode>().Y + " destroyed");
    }

    IEnumerator LightCor()
    {
        Bulb bulb = GetModel().GetComponent<Bulb>();
        bulb.TurnOn(true);
        CoinManager.Instance.GetCoin(transform.position);
        yield return new WaitForSeconds(3);
        bulb.TurnOn(false);
        SetType(SlotType.None);
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
    public GameObject GetModel()
    {
        return TypeModels[(int)type];
    }
    internal void DestroyWithNoCoin()
    {
        if (Obj != null)
        {

            Destroy(Obj.gameObject);
            Obj = null;
            // Z.GM.Coin++;
        }
    }

    public void ChangeTypeToNext()
    {
        int nextType = ((int)type + 1) % Enum.GetNames(typeof(SlotType)).Length;
        SetType((SlotType)nextType);
    }
}

public enum SlotType
{
    None, Power, Light, Blocked, Box
}
