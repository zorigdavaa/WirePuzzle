using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCompleteUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    private int rewardCoin;

    public void Activate(int level, int coin)
    {
        rewardCoin = coin;
        gameObject.SetActive(true);
        levelText.text = "Level " + level;
        coinText.text = coin.ToString();
    }

    public void OnButtonClick()
    {
        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.AddCoin(rewardCoin);
        }
    }
}
