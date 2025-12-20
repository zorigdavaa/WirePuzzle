using System;
using UnityEngine;
using ZPackage;

public class LevelEditor : GenericSingleton<LevelEditor>
{
    public Level BaseLevel;
    public Level CurrentLevel;
    public LevelData LevelData;
    public PieceController PieceController;
    public void CreateLevel()
    {
        CurrentLevel = Instantiate(BaseLevel);
        LevelData = ScriptableObject.CreateInstance<LevelData>();
        LevelData.DefaultValues();
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
        throw new NotImplementedException();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
