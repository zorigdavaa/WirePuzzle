using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelData levelData = (LevelData)target;
        // foreach (var item in PieceController.StringToPieces(levelData.SequencePreset))
        // {
        //     foreach (var node in item.GetChilds())
        //     {
        //         Vector3 pos = node.position;
        //     }
        // }

        var pieces = PieceController.StringToPieces(levelData.SequencePreset);

        if (pieces == null) return;

        GUILayout.Space(10);
        GUILayout.Label("Sequence Shape Preview", EditorStyles.boldLabel);

        foreach (var piece in pieces)
        {
            DrawPiecePreview(piece);
            GUILayout.Space(8);
        }
    }
    void DrawPiecePreview(Piece piece)
    {
        var nodes = piece.GetChilds();
        if (nodes == null) return;

        float cellSize = 16f;
        float padding = 4f;

        // ---- Find bounds so we can center the shape
        Vector2 min = Vector2.one * 999;
        Vector2 max = Vector2.one * -999;

        foreach (var node in nodes)
        {
            Vector2 p = new Vector2(node.localPosition.x, node.localPosition.y);
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 size = max - min + Vector2.one;
        Rect rect = GUILayoutUtility.GetRect(size.x * cellSize, size.y * cellSize);

        // Background
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        // ---- Draw blocks
        foreach (var node in nodes)
        {
            Vector2 p = new Vector2(node.localPosition.x, node.localPosition.y);
            p -= min;               // normalize
            p.y = size.y - p.y - 1; // flip Y for GUI

            Rect r = new Rect(
                rect.x + p.x * cellSize + padding,
                rect.y + p.y * cellSize + padding,
                cellSize - padding * 2,
                cellSize - padding * 2
            );

            EditorGUI.DrawRect(r, Color.cyan);
            Handles.DrawSolidRectangleWithOutline(r, Color.clear, Color.black);
        }
    }
}
