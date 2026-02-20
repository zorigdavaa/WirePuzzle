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

        // foreach (var piece in pieces)
        // {
        //     DrawPiecePreview(piece);
        //     // GUILayout.BeginHorizontal();
        //     GUILayout.Space(8);
        //     // GUILayout.EndHorizontal();
        // }
        int perRow = 4;
        int count = 0;

        foreach (var piece in pieces)
        {
            if (count % perRow == 0)
                GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(80));
            DrawPiecePreview(piece);
            GUILayout.EndVertical();

            count++;

            if (count % perRow == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
        }

        if (count % perRow != 0)
            GUILayout.EndHorizontal();
    }
    void DrawPiecePreview(Piece piece)
    {
        if (!piece)
        {
            return;
        }
        var nodes = piece.GetChilds();
        if (nodes == null) return;

        float cellSize = 10f;
        float padding = 0f;

        // ---- Find bounds so we can center the shape
        Vector2 min = Vector2.one * 999;
        Vector2 max = Vector2.one * -999;

        foreach (var node in nodes)
        {
            Vector2 p = new Vector2(node.localPosition.x, node.localPosition.z);
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 size = max - min + Vector2.one;
        Rect rect = GUILayoutUtility.GetRect(size.x * cellSize, size.y * cellSize);

        // Background
        // EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        // ---- Draw blocks
        foreach (var node in nodes)
        {
            Vector2 p = new Vector2(node.localPosition.x, node.localPosition.z);
            p -= min;               // normalize
            p.y = size.y - p.y - 1; // flip Y for GUI

            Rect r = new Rect(
                rect.x + p.x * cellSize + padding,
                rect.y + p.y * cellSize + padding,
                cellSize - padding * 2,
                cellSize - padding * 2
            );

            // EditorGUI.DrawRect(r, Color.cyan);
            Handles.DrawSolidRectangleWithOutline(r, Color.cyan, Color.black);
        }
        // for (int x = 0; x < size.x; x++)
        //     for (int y = 0; y < size.y; y++)
        //     {
        //         Rect g = new Rect(rect.x + x * cellSize, rect.y + y * cellSize, cellSize, cellSize);
        //         Handles.DrawSolidRectangleWithOutline(g, Color.clear, new Color(0, 0, 0, 0.2f));
        //     }

    }
}
