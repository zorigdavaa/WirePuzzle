using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using ZPackage.Utility;

namespace ZPackage
{
    public class LevelSpawner : GenericSingleton<LevelSpawner>
    {
        [SerializeField] List<GameObject> Levels;
        [SerializeField] List<LevelData> LevelDatas;
        public PieceController PieceController;
        public Level BaseLevel;
        public Level CurrentLevel;

        public void InitializeLevel()
        {
            PieceController = FindAnyObjectByType<PieceController>();
            int levelIndex = (GameManager.Instance.Level - 1) % Levels.Count;
            GameObject level = Levels[levelIndex];
            // CurrentLevel = Instantiate(level, transform.position, Quaternion.identity, transform).GetComponent<Level>();
            CurrentLevel = Instantiate(BaseLevel, transform.position, Quaternion.identity, transform).GetComponent<Level>();
        }
        public void InitializeLevelWithData()
        {
            // LevelDatas = Resources.LoadAll<LevelData>("LevelData").ToList();
            PieceController = FindAnyObjectByType<PieceController>();
            int levelIndex = (GameManager.Instance.Level - 1) % LevelDatas.Count;
            LevelData levelData = LevelDatas[levelIndex];
            // CurrentLevel = Instantiate(level, transform.position, Quaternion.identity, transform).GetComponent<Level>();
            CurrentLevel = Instantiate(BaseLevel, transform.position, Quaternion.identity, transform).GetComponent<Level>();
            PieceController.SetLevelPieces(levelData.Pieces);
            PieceController.SetPiecesSequence(levelData.SequencePreset);
            CurrentLevel.SetData(levelData);
            Camera.main.transform.position = new Vector3(0, levelData.CamDistance, 0);
        }
    }
}

