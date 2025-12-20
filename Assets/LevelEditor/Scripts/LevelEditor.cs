using System;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public Level BaseLevel;
    public Level CurrentLevel;
    public void CreateLevel()
    {
        CurrentLevel = Instantiate(BaseLevel);
    }

    public void LoadLevel()
    {
        throw new NotImplementedException();
    }

    public void SaveLevel()
    {
        throw new NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
