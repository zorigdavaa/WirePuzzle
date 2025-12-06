using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities;

public class PieceController : MonoBehaviour
{
    public List<Piece> PiecesPf;
    public List<Transform> pieceSlots;
    public RandomBag<Piece> bag;
    public Dictionary<Transform, Piece> CurrentSlotObj = new Dictionary<Transform, Piece>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bag = new RandomBag<Piece>(PiecesPf.ToArray(), pieceSlots.Count);
        Populate();
    }

    private void Populate()
    {
        var newItems = bag.PopRandomItems(pieceSlots.Count);
        for (int i = 0; i < pieceSlots.Count; i++)
        {
            var newObj = Instantiate(newItems[i], pieceSlots[i].transform.position, Quaternion.identity);
            // newItems[i].transform.position = pieceSlots[i].transform.position;
            CurrentSlotObj[pieceSlots[i]] = newObj;
            // newObj.SetPieceSlot(pieceSlots[i]);
            newObj.StartDrag(false);
        }
    }

    internal void GotoSlot(Piece selectedObject)
    {
        Transform target = null;
        foreach (var item in CurrentSlotObj)
        {
            if (item.Value == selectedObject)
            {
                target = item.Key;
                selectedObject.transform.position = target.transform.position;
                selectedObject.StartDrag(false);
            }
        }
        // IEnumerator LocalCor()
        // {
        //     float t = 0f;
        //     float time = 0f;
        //     float duration = 1.0f;
        //     Vector3 initial = transform.position;
        //     Vector3 target = transform.position + Vector3.forward;

        //     while (time < duration)
        //     {
        //         time += Time.deltaTime;
        //         t = time / duration;
        //         transform.position = Vector3.Lerp(initial, target, t);
        //         yield return null;
        //     }
        // }
    }

    internal bool HasSlot(Piece selectedObject)
    {
        foreach (var item in CurrentSlotObj.Values)
        {
            if (item == selectedObject)
            {
                return true;
            }
        }
        return false;
    }

    public void NotifyPlaced(Piece selectedPiece)
    {
        Transform foundKey = null;
        foreach (var kv in CurrentSlotObj) // assume CurrentSlotObj is Dictionary<Transform, Piece>
        {
            if (kv.Value == selectedPiece)
            {
                foundKey = kv.Key;
                break;
            }
        }

        if (foundKey != null)
        {
            CurrentSlotObj[foundKey] = null;
        }
        if (CurrentSlotObj.Values.Where(x => x == null).Count() == CurrentSlotObj.Keys.Count)
        {
            Populate();
        }

    }


}
