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
        public PieceController PieceController;
        public Level CurrentLevel;

        public void InitializeLevel()
        {
            PieceController = FindAnyObjectByType<PieceController>();
            int levelIndex = (GameManager.Instance.Level - 1) % Levels.Count;
            GameObject level = Levels[levelIndex];
            CurrentLevel = Instantiate(level, transform.position, Quaternion.identity, transform).GetComponent<Level>();
        }

    }
}

