using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScripableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public List<Vector2> ChargerPoses;
    public List<Vector2> ConnectPoses;
    public List<Vector2> Blocked;
}
