using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScripableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public List<LevelConnectData> LevelConnectDatas;
    public int X;
    public int Y;
    public List<Piece> Pieces;
}

