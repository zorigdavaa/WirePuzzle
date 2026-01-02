using UnityEngine;
#if CANDYKIT
using GameAnalyticsSDK;
#endif

public class CkGameAnalyticsInitializer : MonoBehaviour, IGameAnalyticsATTListener
{
    public void Initialize()
    {
        // if (CountryCode.IsInNoTenjinCountries())
        // {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
        // }
        // else
        // {
        //     GameAnalytics.Initialize();
        // }
    }

    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
}
