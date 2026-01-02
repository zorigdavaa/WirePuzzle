using System.Collections;
using System.Collections.Generic;
using CandyKitSDK;
using UnityEngine;

public class CKTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CandyKit.Initialize(null);
        // CKCV.IncreaseRevenue(1);
        CKCV.TestScoreIncrease();

        CKCV.SendConversionValue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CKCV.TestScoreIncrease();
            // CKCV.IncreaseRevenue(1);

            CKCV.SendConversionValue();
        }
    }
}
