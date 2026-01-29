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

// Color Matching: Perhaps a blue battery can only power a blue bulb, or paths can cross if they are different colors.

// Logic Gates: Add "AND/OR" blocks. For example, a bulb that only lights up if two batteries are connected to it.

// Transformers/Splitters: Blocks that take one signal and split it into two, allowing you to power multiple bulbs at once.

// One-Way Diodes: Blocks that only allow electricity to flow in one direction.