using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScripableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    [TextArea(5, 20)]
    public string LevelString;
    public List<LevelConnectData> LevelConnectDatas;
    public int X;
    public int Y;
    public PiecesPreset Pieces;
    public float CamDistance;
    public float PieceScale;
    public int PieceCount;

    public void DefaultValues()
    {
        LevelConnectDatas = new();
        // LevelConnectDatas = new List<LevelConnectData>()
        // {
        //     new LevelConnectData()
        //     {
        //         ChargerPoses = new List<Vector2>()
        //         {
        //             new Vector2(0,0)
        //         },
        //         ConnectPoses = new List<Vector2>()
        //         {
        //             new Vector2(1,0)
        //         },
        //         Blocked = new List<Vector2>()
        //     }
        // };
        X = 3;
        Y = 3;
        Pieces = new();
        CamDistance = 10;
        PieceScale = 0.5f;
        PieceCount = 3;
    }
    [ContextMenu("Text To Level")]
    public void TextToLevel()
    {
        if (LevelString == String.Empty || LevelString.Length == 0)
        {
            Debug.LogError("No Text");
            return;
        }
        string[] rows = LevelString.Split('\n');
        int rowNumber = rows.Length;
        int colNumber = rows[0].Length;
        X = colNumber;
        Y = rowNumber;
        LevelConnectDatas.Clear();
        LevelConnectData lcd = new LevelConnectData(new List<CellData>());
        for (int y = 0; y < rowNumber; y++)
        {
            string row = rows[y];
            for (int x = 0; x < colNumber; x++)
            {
                char c = row[x];
                SlotType type = SlotType.Empty;
                if (c == 'P')
                {
                    type = SlotType.Power;
                }
                else if (c == 'L')
                {
                    type = SlotType.Light;
                }
                else if (c == 'X')
                {
                    type = SlotType.Blocked;
                }
                else if (c == 'I')
                {
                    type = SlotType.Ice;
                }
                else if (c == 'H')
                {
                    type = SlotType.Hidden;
                }
                else if (c == 'O')
                {
                    type = SlotType.Box;
                }
                if (type != SlotType.Empty)
                {
                    CellData cellData = new CellData();
                    cellData.Position = new Vector2Int(x, rowNumber - y - 1);
                    cellData.Type = type;
                    // LevelConnectData lcd = new LevelConnectData();
                    lcd.cellDatas.Add(cellData);
                }
            }
        }
        LevelConnectDatas.Add(lcd);

    }
}

