using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Pieces/PiecePreset")]
public class PiecesPreset : ScriptableObject
{
    public Piece[] Pieces;

    [ContextMenu("Log Pieces")]
    public void LogAllPieces()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("ShapeID\tLocalX\tLocalY");

        foreach (Piece piece in Pieces)
        {
            string shapeID = piece.ID.ToString();

            foreach (Transform child in piece.transform)
            {
                Vector3 p = child.localPosition;

                sb.AppendLine($"{shapeID}\t{p.x}\t{p.z}");
            }
        }

        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log(sb);
        Debug.Log("Copied to clipboard!");
    }
}
