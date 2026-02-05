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
    // public List<Piece> PiecesPf;
    public PiecesPreset PiecesPf;
    public List<Piece> SequencePreset;
    public List<Piece> LevelPiecesPf;
    // public List<Piece> PiecesPfCopy;
    public List<GameObject> SlotsParent;
    public List<Transform> pieceSlots;
    public List<Material> pieceMaterials;
    public GameObject singlePiecePF;
    public int SeqIndex = 0;
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

    /// <summary>
    /// Populate the next pieces in the slots.
    /// </summary>
    /// <remarks>
    /// This method will populate the next pieces in the slots.
    /// If there is a next piece in the sequence preset, it will be used.
    /// If there are needed pieces (i.e. pieces that can be placed in the grid), they will be used.
    /// Otherwise, a random piece from the level pieces will be used.
    /// </remarks>
    private void Populate()
    {

        //Get the next random pieces from the bag
        //List<Piece> newItems = bag.PopRandomItems(pieceSlots.Count);
        var newItems = new List<Piece>();

        //Get the needed pieces from the grid
        List<Piece> neededPieces = Z.GridController.GetNeededPiece(LevelPiecesPf);

        //Populate the new pieces
        for (int i = 0; i < pieceSlots.Count; i++)
        {
            if (HasNextInSequence())
            {
                //If there is a next piece in the sequence preset, use it
                newItems.Add(SequencePreset[SeqIndex]);
                SeqIndex++;
            }
            else if (i < 2 && neededPieces.Count > 0)
            {
                //If there are needed pieces, use one of them
                newItems.Add(neededPieces[Random.Range(0, neededPieces.Count)]);
            }
            else
            {
                //Otherwise, use a random piece from the level pieces
                newItems.Add(LevelPiecesPf[Random.Range(0, LevelPiecesPf.Count)]);
            }
        }

        //Instantiate the new pieces and set their color
        for (int i = 0; i < pieceSlots.Count; i++)
        {
            var newObj = Instantiate(newItems[i], pieceSlots[i].transform.position, Quaternion.identity);
            newObj.gameObject.SetActive(true);

            //Set the piece slot for the new object
            CurrentSlotObj[pieceSlots[i]] = newObj;

            //Set the color of the new object
            newObj.SetColor(pieceMaterials[materialIndex % pieceMaterials.Count]);

            //Start the drag of the new object
            newObj.StartDrag(false);


        }
        //Increment the material index
        materialIndex++;
        //Check if the game is over
        if (neededPieces.Count == 0)
        {
            //Z.GM.GameOver(this, EventArgs.Empty);
            CheckGameOver();
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
        if (CurrentSlotObj.Values.Count(x => x == null) == CurrentSlotObj.Keys.Count)
        {
            Populate();
        }
        CheckGameOver();
    }

    public void CheckGameOver()
    {
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }
        gameOverCoroutine = StartCoroutine(CheckGameOverCor());
    }

    Coroutine gameOverCoroutine;
    private IEnumerator CheckGameOverCor()
    {
        yield return new WaitForSeconds(3f);
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

    public void SetLevelPieces(PiecesPreset preset)
    {
        if (preset.Pieces.Length == 0)
        {
            LevelPiecesPf = PiecesPf.Pieces.ToList();
        }
        else
        {
            LevelPiecesPf = preset.Pieces.ToList();
        }
    }

    public List<Piece> GetPieces()
    {
        var valuesList = CurrentSlotObj.Values.Where(x => x != null).ToList();
        return valuesList;
        // var placeAblePieces = Z.GridController.GetNeededPiece(valuesList);
        // if (placeAblePieces.Count > 0)
        // {
        //     var pieceToSuggest = placeAblePieces[Random.Range(0, placeAblePieces.Count)];
        //     GotoSlot(pieceToSuggest);
        // }
        // else
        // {
        //     Debug.LogError("Suggest No PlaceAble");
        // }
    }

    public void StringToPiecesSeq(string sequence)
    {
        if (sequence != null && sequence.Length > 0)
        {
            List<Piece> seqList = new List<Piece>();
            string[] ids = sequence.Split(',');
            foreach (string id in ids)
            {
                if (int.TryParse(id, out int pieceIndex))
                {
                    seqList.Add(PiecesPf.Pieces.FirstOrDefault(p => p.ID == pieceIndex));
                }
            }
            // SetPiecesSequence(seqList);
            SequencePreset = seqList;
        }
        // SequencePreset = sequencePreset;
        SeqIndex = 0;
    }
    public bool HasNextInSequence()
    {
        if (SequencePreset == null || SequencePreset.Count == 0)
        {
            return false;
        }
        return SeqIndex < SequencePreset.Count;
    }
}
