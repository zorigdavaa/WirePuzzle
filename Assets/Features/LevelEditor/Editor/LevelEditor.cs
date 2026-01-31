using System;
using System.Collections.Generic;
using UnityEditor;
// using UnityEditor.VersionControl;
using UnityEngine;
using ZPackage;

public class LevelEditor : GenericSingleton<LevelEditor>
{
    public Level BaseLevel;
    public Level CurrentLevel;
    public LevelData LevelData;
    public PieceController PieceController;
    public LEPieces LEPieces;
    const string folder = "Assets/LevelData";
    const string baseName = "Level_";
    //Current Data Index
    public int CDIndex = 0;
    void Start()
    {

    }
    public void CheckLDCButtons()
    {
        if (LevelData == null || LevelData.LevelConnectDatas.Count == 0)
        {
            LECanvas.Instance.BeforeButton.gameObject.SetActive(false);
            LECanvas.Instance.NextButton.gameObject.SetActive(false);
        }
        else
        {
            if (CDIndex == 0)
            {
                LECanvas.Instance.BeforeButton.gameObject.SetActive(false);
                LECanvas.Instance.NextButton.gameObject.SetActive(true);
            }
            else if (CDIndex == LevelData.LevelConnectDatas.Count - 1)
            {
                LECanvas.Instance.BeforeButton.gameObject.SetActive(true);
                LECanvas.Instance.NextButton.gameObject.SetActive(false);
            }
            else
            {
                LECanvas.Instance.BeforeButton.gameObject.SetActive(true);
                LECanvas.Instance.NextButton.gameObject.SetActive(true);
            }
        }

    }
    public void CreateLevel(int x, int y)
    {
        CurrentLevel = Instantiate(BaseLevel);
        LevelData = ScriptableObject.CreateInstance<LevelData>();
        LevelData.DefaultValues();
        LevelData.X = x;
        LevelData.Y = y;
        CurrentLevel.SetData(LevelData);
        // PieceController.SetLevelPieces(LevelData.Pieces);
        // LEPieces
        // PieceController.Init();
        LEPieces.Instance.Init();
    }

    public void LoadLevel()
    {
        if (LevelData != null)
        {
            CurrentLevel = Instantiate(BaseLevel);
            CurrentLevel.SetData(LevelData);
            LEPieces.Instance.Init();
        }
    }

    public void SaveLevel()
    {
        if (LevelData != null)
        {
            SaveCurrent();
            // LevelData.Pieces = PieceController.LevelPiecesPf;
            // LevelData.Pieces = LEPieces.Instance.GetGreenPieces();
            string path = AssetDatabase.GetAssetPath(LevelData);
            if (string.IsNullOrEmpty(path))
            {
                path = GetNextPath();
                Save(LevelData, path);
                Debug.LogError($"Saved Data to {path}");
            }
            else
            {
                EditorUtility.SetDirty(LevelData);
                AssetDatabase.SaveAssets();
                Debug.LogError($"Updated Data to {path}");
            }
        }
        else
        {
            Debug.LogError("No Level Data to Save");
        }
    }

    public void Before()
    {
        Debug.Log("Before is clicked");
        if (CurrentLevel && LevelData.LevelConnectDatas.Count > 0)
        {
            CDIndex--;
            if (CDIndex < 0)
            {
                CDIndex = 0;
            }
            CurrentLevel.SetGridByData(CDIndex);
        }
        CheckLDCButtons();
    }

    public void Next()
    {
        Debug.Log("Next is clicked");
        if (CurrentLevel && CurrentLevel && CDIndex < LevelData.LevelConnectDatas.Count - 1)
        {
            CDIndex++;
            CurrentLevel.SetGridByData(CDIndex);
        }
        CheckLDCButtons();
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
            AssetDatabase.CreateFolder("Assets", "LevelData");
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

    public void SaveCurrent()
    {
        Debug.Log("Save is clicked");
        if (LevelData && CurrentLevel)
        {
            LevelConnectData connectData = CurrentLevel.GetConnectData();
            if (LevelData.LevelConnectDatas.Count > CDIndex)
            {
                LevelData.LevelConnectDatas[CDIndex] = connectData;
            }
            else
            {
                LevelData.LevelConnectDatas.Add(connectData);
            }

        }
        CheckLDCButtons();
    }

    internal void AddCurrent()
    {
        Debug.Log("Add is clicked");
        SaveCurrent();
        if (CurrentLevel && LevelData)
        {
            CDIndex = LevelData.LevelConnectDatas.Count;
            LevelData.LevelConnectDatas.Add(new());
            CurrentLevel.ClearConnects();
        }
        CheckLDCButtons();
    }
    public void AddRow()
    {
        if (CurrentLevel)
        {
            CurrentLevel.gridController.AddRow();
        }
    }
    public void AddColumn()
    {
        if (CurrentLevel)
        {
            CurrentLevel.gridController.AddColumn();
        }
    }
}
