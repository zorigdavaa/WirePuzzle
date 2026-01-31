using System.Collections;
using System.Collections.Generic;
// using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace ZPackage
{
    public static class Z
    {
        public static GameManager GM => GameManager.Instance;
        public static CameraController CamC => CameraController.Instance;
        public static CanvasManager CanM => CanvasManager.Instance;
        public static LevelSpawner LS => LevelSpawner.Instance;
        public static GridController GridController
        {
            get
            {
                if (LevelSpawner.Instance)
                {
                    return LevelSpawner.Instance.CurrentLevel.gridController;
                }
                // return LevelEditor.Instance.CurrentLevel.gridController;
                return GameObject.FindAnyObjectByType<GridController>();
            }
        }
        public static PieceController PieceController
        {
            get
            {
                if (LevelSpawner.Instance)
                {
                    return LevelSpawner.Instance.PieceController;
                }
                // return LevelEditor.Instance.PieceController;
                return GameObject.FindAnyObjectByType<PieceController>();
            }
        }
        public static Level CurrentLevel
        {
            get
            {
                if (LevelSpawner.Instance)
                {
                    return LevelSpawner.Instance.CurrentLevel;
                }
                // return LevelEditor.Instance.CurrentLevel;
                return GameObject.FindAnyObjectByType<Level>();
            }
        }
        private static Player _player;
        public static Player Player
        {
            get
            {
                if (_player == null)
                {
                    _player = Mb.FindAnyObjectByType<Player>();
                }
                return _player;
            }
        }

        public static bool IsPlaying
        {
            get
            {
                if (GM && GM.State == GameState.Playing)
                {
                    return true;

                }
                return false;
            }
        }
    }
}

