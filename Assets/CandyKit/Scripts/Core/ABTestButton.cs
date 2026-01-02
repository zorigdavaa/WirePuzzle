using System;
using CandyKitSDK;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ABTestButton : MonoBehaviour
{
    public int ClickCount = 0;
    public GameObject ABTestMenu;
    public GameObject buttonPf;
    private void Start()
    {
        ApplyABTestVariant();
    }
    public void Click()
    {
        ClickCount++;
        Debug.Log($"Button clicked {ClickCount} times.");
        if (ClickCount > 10)
        {
            ClickCount = 0;
            ShowMenu();
        }
    }
    bool instantiaded = false;
    private void ShowMenu()
    {
        ABTestMenu.gameObject.SetActive(true);
        if (instantiaded == false)
        {
            instantiaded = true;


            Transform topMostObj = ABTestMenu.transform.GetChild(0);
            foreach (var item in CandyKit.Settings.ABTestValues)
            {
                Button insObj = Instantiate(buttonPf, topMostObj).GetComponent<Button>();
                insObj.GetComponentInChildren<TMP_Text>().text = item;
                insObj.onClick.AddListener(() =>
                {
                    CandyKit.SetGAABTestingValue(item);
                    ApplyABTestVariant();
                });
            }
            buttonPf.GetComponentInChildren<TMP_Text>().text = "Delete";
            buttonPf.GetComponent<Button>().onClick.AddListener(() =>
            {

                CandyKit.DeleteGAABTestingValue();
            });

        }


    }

    private void ApplyABTestVariant()
    {


    }
}
