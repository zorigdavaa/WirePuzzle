using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 0)]
public class GameConfig : ScriptableObject
{
    private static GameConfig _instance;
    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameConfig>("GameConfig/GameConfig");

                if (_instance == null)
                    Debug.LogError("GameConfig not found! Put it inside Resources folder.");
            }
            return _instance;
        }
    }

    public PiecesPreset AllPiecesPreset;
}
