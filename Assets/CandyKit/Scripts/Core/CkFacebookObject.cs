using UnityEngine;

#if CANDYKIT 
using Facebook.Unity;
#endif

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace CandyKitSDK
{
    public class CkFacebookObject : MonoBehaviour
    {
#if CANDYKIT
        void Awake()
        {
            if (!FB.IsInitialized) {
                FB.Init(InitCallback, OnHideUnity);
            } else {
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }

#if UNITY_IOS
        Invoke(nameof(RegisterAppForNetworkAttribution), 1);
#endif
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }

#if UNITY_IOS
        private void RegisterAppForNetworkAttribution()
        {
            SkAdNetworkBinding.SkAdNetworkRegisterAppForNetworkAttribution();
        }
#endif

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) {
                if (FB.IsInitialized) {
                    FB.ActivateApp();
                } else {
                    FB.Init(() => {
                        FB.ActivateApp();
                    });
                }
            }
        }
#endif
    }
}
