using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyKitSDK;

public class CKProgressionEvent : MonoBehaviour
{
    bool isInited = false;
    public void Init()
    {
        Debug.LogError("Delete this and uncomment code");
        // if (!isInited)
        // {
        //     GameController.OnGameStateChange += OnGameStateChange;
        //     isInited = true;
        // }
    }

// #if CANDYKIT
//     void OnGameStateChange(GameState state)
//     {
//         if (!CandyKit.IsInitialized())
//         {
//             Debug.Log("Candy not ready");
//             return;
//         }

//         if (state == GameState.Starting)
//         {
//             CandyKit.NotifyLevelZero();
//             Debug.Log("Progression Level Starting " + GameController.Level);
//         }
//         if (state == GameState.Playing)
//         {
//             CandyKit.NotifyLevelStarted(GameController.Level);
//             Debug.Log("Progression Level Start " + GameController.Level);
//         }
//         else if (state == GameState.LevelCompleted)
//         {
//             CandyKit.NotifyLevelCompleted(GameController.Level);
//             Debug.Log("Progression Level Complete " + GameController.Level);
//         }
//         else if (state == GameState.GameOver)
//         {
//             CandyKit.NotifyLevelFailed(GameController.Level);
//             Debug.Log("Progression Game Over " + GameController.Level);
//         }
//         else if (state == GameState.Revive)
//         {
//             CandyKit.NotifyLevelFailed(GameController.Level);
//             Debug.Log("Progression Level Revive " + GameController.Level);
//         }
//     }
// #endif

    // void OnDestroy()
    // {
    //     if (isInited)
    //     {
    //         GameController.OnGameStateChange -= OnGameStateChange;
    //         isInited = false;
    //     }
    // }
}
