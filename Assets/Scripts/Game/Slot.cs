using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.Android.Gradle;
using UnityEngine;
using ZPackage;

public class Slot : MonoBehaviour
{
    public SlotType type;
    public Node Obj; //ISlotObj type
    public List<GameObject> TypeModels;
    public List<GameObject> ObjModels;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in TypeModels)
        {
            if (item.TryGetComponent<PuzzleElement>(out var puzzleElement))
            {
                puzzleElement.OnDestroyed += OnElementDestroyed;
            }

        }
    }

    private void OnElementDestroyed(object sender, PuzzleElement e)
    {
        SetType(SlotType.Empty);
    }

    public void SetType(SlotType type)
    {
        if (type == SlotType.Hidden)
        {
            foreach (var item in ObjModels)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in ObjModels)
            {
                item.SetActive(true);
            }
        }
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
        if (type == SlotType.Filled)
        {
            var inst = Instantiate(GameConfig.Instance.SingleNodePF);
            SetObj(inst);
            this.type = SlotType.Empty;
            // GameConfig.Instance.SinglePiecePF.SetInSlot(this);
        }
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
    private static readonly HashSet<SlotType> FreeTypes = new()
    {
        SlotType.Empty,
        SlotType.Light,
        SlotType.Power
    };
    private static readonly HashSet<SlotType> BlockedTypes = new()
    {
        SlotType.Blocked, SlotType.Hidden
    };
    public bool IsFree()
    {
        return Obj == null && FreeTypes.Contains(type);
    }
    public bool IsFilled()
    {
        return Obj != null;
    }
    public bool IsPermanentBlocked()
    {
        return BlockedTypes.Contains(type);
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
            Obj.GetComponent<Node>().Shine();
            Destroy(Obj.gameObject, 1f);
            Obj = null;
            Debug.Log(GetComponent<GridNode>().X + " " + GetComponent<GridNode>().Y + " destroyed");
        }
        else
        {

            Debug.Log(GetComponent<GridNode>().X + " " + GetComponent<GridNode>().Y + " Destroyed but OBj null");
        }
    }

    IEnumerator LightCor()
    {
        Bulb bulb = GetModel().GetComponent<Bulb>();
        bulb.TurnOn(true);
        CoinManager.Instance.GetCoin(transform.position);
        yield return new WaitForSeconds(3);
        bulb.TurnOn(false);
        SetType(SlotType.Empty);
    }

    public void ScaledDestroy()
    {
        if (Obj != null)
        {
            Obj.GetComponent<Node>().Scale();
            Obj = null;
            Z.GM.Coin++;
        }
        else if (type == SlotType.Box)
        {
            SetType(SlotType.Empty);
        }
        else if (GetModel().TryGetComponent<PuzzleElement>(out var puzzleElement))
        {
            puzzleElement.TakeDamage();
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
        //SKip Hidden type  
        // if (nextType == (int)SlotType.Hidden)
        // {
        //     nextType = (nextType + 1) % Enum.GetNames(typeof(SlotType)).Length;
        // }
        SetType((SlotType)nextType);
    }

    public void Toggle()
    {
        if (type == SlotType.Hidden)
        {
            SetType(SlotType.Empty);
        }
        else
        {
            SetType(SlotType.Hidden);
        }
    }
}

public enum SlotType
{
    Empty, Power, Light, Blocked, Box, Ice, Hidden, Filled
}
