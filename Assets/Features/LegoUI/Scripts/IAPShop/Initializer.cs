using System.Collections;
using System.Collections.Generic;
using CandyKitSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
// using CandyKitSDK;

// #if SW_STAGE_STAGE10_OR_ABOVE
// using SupersonicWisdomSDK;
// #endif

public class Initializer : MonoBehaviour
{
    const float SecondsToWait = 3f;

    // float timer = 0f;
    bool mainSceneLoaded = false;

    void Awake()
    {
        CandyKit.Initialize(LoadMainScene);
    }



    void LoadMainScene()
    {
        if (mainSceneLoaded) return;
        SceneManager.LoadScene("Main");

        mainSceneLoaded = true;
        enabled = false;
    }

}

//Power Up Idead
//Reset pieces
// Random change Piece
// Destroy X
// Remove All Obj in Grid