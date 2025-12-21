using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using ZPackage;

public class LevelEditor : GenericSingleton<LevelEditor>
{
    public Level BaseLevel;
    public Level CurrentLevel;
    public LevelData LevelData;
    public PieceController PieceController;
    const string folder = "Assets/Levels";
    const string baseName = "Level_";
    public void CreateLevel(int x, int y)
    {
        CurrentLevel = Instantiate(BaseLevel);
        LevelData = ScriptableObject.CreateInstance<LevelData>();
        LevelData.DefaultValues();
        LevelData.X = x;
        LevelData.Y = y;
        CurrentLevel.SetData(LevelData);
        PieceController.SetLevelPieces(LevelData.Pieces);
        // PieceController.Init();
    }

    public void LoadLevel()
    {
        throw new NotImplementedException();
    }

    public void SaveLevel()
    {
        if (LevelData != null)
        {
            List<LevelConnectData> connectDatas = new List<LevelConnectData>();
            
            LevelData.Pieces = PieceController.LevelPiecesPf;
            string path = AssetDatabase.GetAssetPath(LevelData);
            if (string.IsNullOrEmpty(path))
            {
                path = GetNextPath();
                Save(LevelData, path);
            }
            else
            {
                EditorUtility.SetDirty(LevelData);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            Debug.LogError("No Level Data to Save");
        }
    }
    public int PositionIndex = 0;
    public void Before()
    {
        if (CurrentLevel && CurrentLevel.ConnectPoses.Count > 0)
        {
            PositionIndex--;
            if (PositionIndex < 0)
            {
                PositionIndex = 0;
            }
        }
    }

    public void Next()
    {
        if (CurrentLevel && CurrentLevel.ConnectPoses.Count < PositionIndex)
        {
            PositionIndex++;

        }
    }

    public static void Save(LevelData data, string path)
    {
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static string GetNextPath()
    {


        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
        }

        int index = 1;
        string path;

        do
        {
            path = $"{folder}/{baseName}{index:D3}.asset";
            index++;
        }
        while (AssetDatabase.AssetPathExists(path));

        return path;
    }
}
