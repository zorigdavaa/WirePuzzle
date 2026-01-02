using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CandyKitSDK;

public class DemoInitializer : MonoBehaviour
{
    void Start()
    {
        CandyKit.Initialize(OnReady);
    }


    void OnReady()
    {
        SceneManager.LoadScene("Demo");
        Debug.Log("On ready");
    }
}
