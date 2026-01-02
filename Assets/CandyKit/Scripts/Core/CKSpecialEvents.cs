using System.Collections;
using System.Collections.Generic;
using CandyKitSDK;
using GameAnalyticsSDK;
using UnityEngine;


//this is copy of GaSpecialEvents due to fps is paid feature
public class CKSpecialEvents : MonoBehaviour
{
    private static int _frameCountAvg = 0;
    private static float _lastUpdateAvg = 0f;
    private int _frameCountCrit = 0;
    private float _lastUpdateCrit = 0f;

    private static int _criticalFpsCount = 0;

    private static int _fpsWaitTimeMultiplier = 1;
    private static float _lastPauseStartTime;
    private static float _pauseDurationAvg;
    private static float _pauseDurationCrit;

    public void Start()
    {
        StartCoroutine(SubmitFPSRoutine());
        StartCoroutine(CheckCriticalFPSRoutine());
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (CandyKit.Settings == null
            || !CandyKit.Settings.SubmitFpsAverage && !CandyKit.Settings.SubmitFpsCritical)
        {
            return;
        }

        if (pauseStatus)
        {
            _lastPauseStartTime = Time.realtimeSinceStartup;
        }
        else
        {
            if (CandyKit.Settings.SubmitFpsAverage)
            {
                _pauseDurationAvg += Time.realtimeSinceStartup - _lastPauseStartTime;
            }

            if (CandyKit.Settings.SubmitFpsCritical)
            {
                _pauseDurationCrit += Time.realtimeSinceStartup - _lastPauseStartTime;
            }
        }
    }

    private IEnumerator SubmitFPSRoutine()
    {
        while (Application.isPlaying && CandyKit.Settings != null && CandyKit.Settings.SubmitFpsAverage)
        {
            int waitingTime = 30 * _fpsWaitTimeMultiplier;
            yield return new WaitForSecondsRealtime(waitingTime);
            _fpsWaitTimeMultiplier *= 2;
            SubmitFPS();
        }
    }

    private IEnumerator CheckCriticalFPSRoutine()
    {
        while (Application.isPlaying && CandyKit.Settings != null && CandyKit.Settings.SubmitFpsCritical)
        {
            yield return new WaitForSecondsRealtime(3);
            CheckCriticalFPS();
        }
    }

    public void Update()
    {
        //average FPS
        if (CandyKit.Settings != null && CandyKit.Settings.SubmitFpsAverage)
        {
            _frameCountAvg++;
        }

        //critical FPS
        if (CandyKit.Settings!= null && CandyKit.Settings.SubmitFpsCritical)
        {
            _frameCountCrit++;
        }
    }

    public static void SubmitFPS()
    {
        //average FPS
        if (CandyKit.Settings != null && CandyKit.Settings.SubmitFpsAverage)
        {
            float timeSinceUpdate = Time.unscaledTime - _lastUpdateAvg - _pauseDurationAvg;
            _pauseDurationAvg = 0f;

            if (timeSinceUpdate > 1.0f)
            {
                float fpsSinceUpdate = _frameCountAvg / timeSinceUpdate;
                _lastUpdateAvg = Time.unscaledTime;
                _frameCountAvg = 0;

                if (fpsSinceUpdate > 0)
                {
                    GameAnalytics.NewDesignEvent("AverageFPS", ((int)fpsSinceUpdate));
                }
            }
        }

        if (CandyKit.Settings != null && CandyKit.Settings.SubmitFpsCritical)
        {
            if (_criticalFpsCount > 0)
            {
                GameAnalytics.NewDesignEvent("CriticalFPS", _criticalFpsCount);
                _criticalFpsCount = 0;
            }
        }
    }

    public void CheckCriticalFPS()
    {
        //critical FPS
        if (CandyKit.Settings != null && CandyKit.Settings.SubmitFpsCritical)
        {
            float timeSinceUpdate = Time.unscaledTime - _lastUpdateCrit - _pauseDurationCrit;
            _pauseDurationCrit = 0f;

            if (timeSinceUpdate >= 1.0f)
            {
                float fpsSinceUpdate = _frameCountCrit / timeSinceUpdate;
                _lastUpdateCrit = Time.unscaledTime;
                _frameCountCrit = 0;

                if (fpsSinceUpdate <= CandyKit.Settings.FpsCriticalThreshold)
                {
                    _criticalFpsCount++;
                }
            }
        }
    }
}
