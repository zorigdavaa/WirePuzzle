using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ZPackage;

public class MainMenuUI : MonoBehaviour
{
    public string gameName;
    public TextMeshProUGUI gameNameText;
    public GameObject levelsParent;
    public List<TextMeshProUGUI> levelsText;

    private void Awake()
    {
        levelsText = levelsParent.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }

    private void Start()
    {
        if (gameName != null || gameName != "")
        {
            SetGameName(gameName);
        }

        SetLevels(GameManager.Instance.Level);
    }

    public void SetGameName(string newGameName)
    {
        gameNameText.text = newGameName;
    }

    public void SetLevels(int starter)
    {
        int j = 0;
        for (int i = starter; i < starter + 10; i++)
        {
            if (j < levelsText.Count)
            {
                levelsText[j].text = i.ToString();
                j++;
            }
        }
    }
}
