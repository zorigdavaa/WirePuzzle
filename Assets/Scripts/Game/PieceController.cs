using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities;
using ZPackage;
using Random = UnityEngine.Random;

public class PieceController : Mb
{
    public List<Piece> PiecesPf;
    public List<Piece> LevelPiecesPf;
    // public List<Piece> PiecesPfCopy;
    public List<GameObject> SlotsParent;
    public List<Transform> pieceSlots;
    public List<Material> pieceMaterials;
    public GameObject singlePiecePF;
    // public RandomBag<Piece> bag;
    public Dictionary<Transform, Piece> CurrentSlotObj = new Dictionary<Transform, Piece>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        //3-2 == 0
        SlotsParent[Z.CurrentLevel.Data.PieceCount - 3].gameObject.SetActive(true);
        pieceSlots.Clear();
        foreach (Transform item in SlotsParent[Z.CurrentLevel.Data.PieceCount - 3].transform)
        {
            pieceSlots.Add(item);
            item.transform.GetChild(0).gameObject.SetActive(false);
        }
        // foreach (var item in pieceSlots)
        // {

        // }
        //It need to be Instantiated due to check its position matches
        List<Piece> InstantiatedPieces = new List<Piece>();
        foreach (var item in LevelPiecesPf)
        {
            var instObh = Instantiate(item, transform.position, Quaternion.identity, transform);
            instObh.gameObject.SetActive(false);
            InstantiatedPieces.Add(instObh);
        }
        LevelPiecesPf = InstantiatedPieces;
        // bag = new RandomBag<
        // Piece>(PiecesPf.ToArray(), pieceSlots.Count);
        Populate();
    }
    int materialIndex = 0;

    private void Populate()
    {
        // var newItems = bag.PopRandomItems(pieceSlots.Count);
        var newItems = new List<Piece>();
        // List<Piece> neededPieces = Z.LS.CurrentLevel.gridController.GetNeededPiece(PiecesPf);
        List<Piece> neededPieces = Z.GridController.GetNeededPiece(LevelPiecesPf);
        for (int i = 0; i < pieceSlots.Count; i++)
        {
            if (i < 2 && neededPieces.Count > 0)
            {
                newItems.Add(neededPieces[Random.Range(0, neededPieces.Count)]);
            }
            else
            {
                newItems.Add(LevelPiecesPf[Random.Range(0, LevelPiecesPf.Count)]);
            }
        }
        for (int i = 0; i < pieceSlots.Count; i++)
        {
            var newObj = Instantiate(newItems[i], pieceSlots[i].transform.position, Quaternion.identity);
            newObj.gameObject.SetActive(true);
            // newItems[i].transform.position = pieceSlots[i].transform.position;
            CurrentSlotObj[pieceSlots[i]] = newObj;
            // newObj.SetPieceSlot(pieceSlots[i]);
            newObj.SetColor(pieceMaterials[materialIndex % pieceMaterials.Count]);
            newObj.StartDrag(false);
            materialIndex++;
        }
        //This one has bug due to destroyed by rocked which is coroutine
        // if (neededPieces.Count == 0)
        // {
        //     Z.GM.GameOver(this, EventArgs.Empty);
        // }
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
        if (CurrentSlotObj.Values.Count(x => x == null) == CurrentSlotObj.Keys.Count)
        {
            Populate();
        }
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }
        gameOverCoroutine = StartCoroutine(CheckGameOver());
    }
    Coroutine gameOverCoroutine;
    private IEnumerator CheckGameOver()
    {
        yield return new WaitForSeconds(2f);
        var valuesList = CurrentSlotObj.Values.Where(x => x != null).ToList();
        var placeAblePieces = Z.GridController.GetNeededPiece(valuesList);
        if (placeAblePieces.Count == 0)
        {
            if (IsPlaying)
            {
                // if player draggin item it will go to its slot first
                foreach (var item in CurrentSlotObj)
                {
                    if (item.Value != null)
                    {
                        GotoSlot(item.Value);
                    }
                }
                Z.GM.GameOver(this, EventArgs.Empty);
            }
        }
        gameOverCoroutine = null;
    }

    public GameObject GetSinglePiece()
    {
        return singlePiecePF;
    }

    internal void SetLevelPieces(List<Piece> pieces)
    {
        if (pieces.Count == 0)
        {
            LevelPiecesPf = PiecesPf;
        }
        else
        {
            LevelPiecesPf = pieces;
        }
    }

}
