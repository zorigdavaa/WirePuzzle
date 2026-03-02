using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using CandyKitSDK;
using UnityEngine;

public static class CKCV
{
    static List<(float minThreshold, float maxThreshold, int CV, string coarse)> CVMAP = new()
    {
        (0f,0.01f,1,"Low"),
        (0.01f,0.02f,2,"Low"),
        (0.02f,0.03f,3,"Low"),
        (0.03f,0.04f,4,"Low"),
        (0.04f,0.05f,5,"Low"),
        (0.05f,0.06f,6,"Low"),
        (0.06f,0.07f,7,"Low"),
        (0.07f,0.08f,8,"Low"),
        (0.08f,0.09f,9,"Low"),
        (0.09f,0.1f,10,"Low"),
        (0.1f,0.11f,11,"Low"),
        (0.11f,0.12f,12,"Low"),
        (0.12f,0.13f,13,"Low"),
        (0.13f,0.14f,14,"Low"),
        (0.14f,0.15f,15,"Low"),
        (0.15f,0.16f,16,"Medium"),
        (0.16f,0.17f,17,"Medium"),
        (0.17f,0.18f,18,"Medium"),
        (0.18f,0.19f,19,"Medium"),
        (0.19f,0.2f,20,"Medium"),
        (0.2f,0.21f,21,"Medium"),
        (0.21f,0.22f,22,"Medium"),
        (0.22f,0.23f,23,"Medium"),
        (0.23f,0.24f,24,"Medium"),
        (0.24f,0.25f,25,"Medium"),
        (0.25f,0.26f,26,"Medium"),
        (0.26f,0.27f,27,"Medium"),
        (0.27f,0.28f,28,"Medium"),
        (0.28f,0.29f,29,"Medium"),
        (0.29f,0.3f,30,"Medium"),
        (0.3f,0.31f,31,"High"),
        (0.31f,0.32f,32,"High"),
        (0.32f,0.33f,33,"High"),
        (0.33f,0.34f,34,"High"),
        (0.34f,0.35f,35,"High"),
        (0.35f,0.36f,36,"High"),
        (0.36f,0.37f,37,"High"),
        (0.37f,0.38f,38,"High"),
        (0.38f,0.39f,39,"High"),
        (0.39f,0.4f,40,"High"),
        (0.4f,0.41f,41,"High"),
        (0.41f,0.42f,42,"High"),
        (0.42f,0.43f,43,"High"),
        (0.43f,0.44f,44,"High"),
        (0.44f,0.45f,45,"High"),
        (0.45f,0.46f,46,"High"),
        (0.46f,0.47f,47,"High"),
        (0.47f,0.48f,48,"High"),
        (0.48f,0.49f,49,"High"),
        (0.49f,1f,50,"High"),
        (1f,2f,51,"High"),
        (2f,3f,52,"High"),
        (3f,4f,53,"High"),
        (4f,5f,54,"High"),
        (5f,6f,55,"High"),
        (6f,8f,56,"High"),
        (8f,10f,57,"High"),
        (10f,12f,58,"High"),
        (12f,15f,59,"High"),
        (15f,20f,60,"High"),
        (20f,30f,61,"High"),
        (30f,50f,62,"High"),
        (50f,float.MaxValue,63,"High")
    };
    static List<(float minThreshold, string eventName)> RevenueMAP = new()
    {
        (0.2f,"revenue_reach_20"),
        (0.25f,"revenue_reach_25"),
        (0.30f,"revenue_reach_30"),
        (0.35f,"revenue_reach_35"),
        (0.40f,"revenue_reach_40"),
        (0.45f,"revenue_reach_45"),
        (0.50f,"revenue_reach_50"),
        (0.55f,"revenue_reach_55"),
        (0.60f,"revenue_reach_60"),
        (0.65f,"revenue_reach_65"),
        (0.70f,"revenue_reach_70"),
        (0.75f,"revenue_reach_75"),
        (0.80f,"revenue_reach_80"),
        (0.85f,"revenue_reach_85"),
        (0.90f,"revenue_reach_90"),
        (0.95f,"revenue_reach_95"),
        (1f,"revenue_reach_100"),
        (1.05f,"revenue_reach_105"),
        (1.1f,"revenue_reach_110"),
        (1.15f,"revenue_reach_115")
    };


    static List<(int minThreshold, string eventName)> ScoreMAP = new()
    {
        (15,"revenue_score_15"),
        (20,"revenue_score_20"),
        (25,"revenue_score_25"),
        (30,"revenue_score_30"),
        (35,"revenue_score_35"),
        (40,"revenue_score_40"),
        (45,"revenue_score_45"),
        (50,"revenue_score_50"),
        (55,"revenue_score_55"),
        (60,"revenue_score_60"),

    };


    public static void SendConversionValue()
    {
        // if (CandyKit.m_Tenjin)
        // {
        //     // int adValue = m_CkAdHandler.GetAdConversionValue();
        //     (int cv, string coarse) cv = GetConversionValue();
        //     // int iapValue = m_IAPManager.GetIAPConversionValue();
        //     Debug.Log("CK--> tenjin sending " + cv.cv + " and course" + cv.coarse);
        //     CandyKit.m_Tenjin.SendConversionValue(cv.cv, cv.coarse);
        //     float Score = GetScore();
        //     Debug.Log("Score is " + Score);
        //     TenjinRevenueScore(Score);
        // }
        // else
        // {
        //     Debug.LogError("Tenjin SKAD not Initialized");
        // }
    }
    public static float GetRevenue()
    {
        return PlayerPrefs.GetFloat(CkConstants.CVRevenue, 0);
    }
    public static float GetScore()
    {
        int rvCount = PlayerPrefs.GetInt(CkConstants.RVWatchCount, 0);
        int InterCount = PlayerPrefs.GetInt(CkConstants.InterWatchCount, 0);
        int IAPCount = PlayerPrefs.GetInt(CkConstants.IAPCount, 0);

        float scoreRv = rvCount * 1.12f;
        float scoreInter = InterCount * 1.0f;
        float IAPScore = IAPCount * 40f;
        float totalScore = scoreRv + scoreInter + IAPScore;
        return totalScore;
    }
    public static void IncreaseRevenue(float revenue)
    {
        float savedRev = GetRevenue();
        savedRev += revenue;
        PlayerPrefs.SetFloat(CkConstants.CVRevenue, savedRev);
    }

    public static (int, string) GetConversionValue()
    {
        float revenue = GetRevenue();
        // float step = 0.05f;
        // int steppedCV = Mathf.RoundToInt(revenue / step);
        SendRevenueReachEvent(revenue);
        // TenjinRevenueStep(steppedCV);
        foreach (var item in CVMAP)
        {
            float min = item.minThreshold;
            float max = item.maxThreshold;
            if (min < revenue && revenue <= max)
            {
                return (item.CV, item.coarse);
            }
        }
        return (0, "Low");
    }
    public static void TenjinRevenueStep(int steppedConversion)
    {
        // int oldStepValue = PlayerPrefs.GetInt(CkConstants.RevenueStep, 0);
        // if (steppedConversion > oldStepValue)
        // {
        //     int valueDif = steppedConversion - oldStepValue;
        //     CandyKit.m_Tenjin?.SentStepEvent(valueDif);
        //     Debug.Log("CK--> Sent event " + valueDif);
        //     PlayerPrefs.SetInt(CkConstants.RevenueStep, steppedConversion);
        //     // m_Tenjin.SendConversionValue(conversionValue);
        // }
    }

    public static void TenjinRevenueScore(float score)
    {
#if UNITY_IOS
        float minfromList = ScoreMAP[0].minThreshold;
        if (score < minfromList)
        {
            return;
        }
        float oldScore = PlayerPrefs.GetFloat(CkConstants.RevenueScore, 0);

        List<(float minThreshold, string eventName)> bestEvent = new List<(float minThreshold, string eventName)>();

        foreach (var item in ScoreMAP)
        {
            if (score >= item.minThreshold && item.minThreshold > oldScore)
            {
                // bestEvent = item;
                bestEvent.Add(item);
            }
        }
        if (bestEvent.Count > 0)
        {
            foreach (var item in bestEvent)
            {
                CandyKit.m_Tenjin?.SentCustomEvent(item.eventName);
                Debug.LogError("CK--> Sent Score event " + item.eventName);
            }
            PlayerPrefs.SetFloat(CkConstants.RevenueScore, score);
        }
#endif
    }

    public static void SendRevenueReachEvent(float rev)
    {
#if UNITY_IOS
        float minfromList = RevenueMAP[0].minThreshold;
        if (rev < minfromList)
        {
            return;
        }

        float lastThresholdSent = PlayerPrefs.GetFloat(CkConstants.RevenueReach, 0f);
        // (float minThreshold, string eventName)? bestEvent = null;
        List<(float minThreshold, string eventName)> bestEvent = new List<(float minThreshold, string eventName)>();

        foreach (var item in RevenueMAP)
        {
            if (rev >= item.minThreshold && item.minThreshold > lastThresholdSent)
            {
                // bestEvent = item;
                bestEvent.Add(item);
            }
        }

        if (bestEvent.Count > 0)
        {
            foreach (var item in bestEvent)
            {
                CandyKit.m_Tenjin?.SentCustomEvent(item.eventName);
                Debug.LogError("CK--> Sent event " + item.eventName);
                PlayerPrefs.SetFloat(CkConstants.RevenueReach, item.minThreshold);
            }
        }
#endif
    }
    internal static void TestScoreIncrease()
    {
        int rvCount = PlayerPrefs.GetInt(CkConstants.RVWatchCount, 0);
        rvCount++;
        PlayerPrefs.SetInt(CkConstants.RVWatchCount, rvCount);
        int InterCount = PlayerPrefs.GetInt(CkConstants.InterWatchCount, 0);
        InterCount++;
        PlayerPrefs.SetInt(CkConstants.InterWatchCount, InterCount);
        Debug.Log("Increase to " + InterCount);
    }
}
