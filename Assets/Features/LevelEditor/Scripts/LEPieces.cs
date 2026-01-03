using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using ZPackage;

public class LEPieces : GenericSingleton<LEPieces>
{
    public List<Piece> PiecesPf;
    public List<Piece> InsPieces;
    // public List<Piece> LevelPiecesPf;
    public List<Transform> pieceSlots;
    public List<Material> pieceMaterials;
    public GameObject singlePiecePF;
    public Transform InsParent;
    public GameObject ScrollObjPF;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        if (Z.CurrentLevel.Data.Pieces.Count == 0)
        {
            //It need to be Instantiated due to check its position matches
            foreach (var item in PiecesPf)
            {
                Piece instObh = InstantiatePiece(item);
                instObh.SetColor(pieceMaterials[0]);

            }
        }
        else
        {
            HashSet<Piece> allPieces = new();
            allPieces.AddRange(Z.CurrentLevel.Data.Pieces);
            allPieces.AddRange(PiecesPf);
            foreach (var item in allPieces)
            {
                Piece instObh = InstantiatePiece(item);
                if (Z.CurrentLevel.Data.Pieces.Contains(item))
                {
                    instObh.SetColor(pieceMaterials[1]);
                    instObh.Order = 1;
                }
                else
                {
                    instObh.SetColor(pieceMaterials[0]);
                    instObh.Order = 0;
                }
            }
        }

    }

    private Piece InstantiatePiece(Piece PF)
    {
        var parentObj = Instantiate(ScrollObjPF, transform.position, Quaternion.identity, InsParent);
        parentObj.gameObject.SetActive(true);
        var instObj = Instantiate(PF, transform.position, Quaternion.identity, parentObj.transform);
        var Holder = instObj.gameObject.AddComponent<PrefabRefHolder>();
        Holder.PFRef = PF;
        instObj.transform.localScale = Vector3.one * 0.5f;
        InsPieces.Add(instObj);
        return instObj;
    }

    public void SelectPiece(Piece piece)
    {
        if (piece.Order == 0)
        {
            piece.Order = 1;
            piece.SetColor(pieceMaterials[1]);
        }
        else
        {
            piece.Order = 0;
            piece.SetColor(pieceMaterials[0]);
        }
        OrderPieces();

    }

    private void OrderPieces()
    {
        InsPieces = InsPieces.OrderByDescending(x => x.Order).ToList();
        int index = 0;
        foreach (var item in InsPieces)
        {
            item.transform.parent.SetSiblingIndex(index);
            index++;
        }
    }

    public List<Piece> GetGreenPieces()
    {
        return InsPieces.Where(x => x.Order > 0).Select(x => x.GetComponent<PrefabRefHolder>().PFRef).ToList();
    }
}
