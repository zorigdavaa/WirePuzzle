using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScripableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public List<LevelConnectData> LevelConnectDatas;
    public int X;
    public int Y;
    public List<Piece> Pieces;
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
}

