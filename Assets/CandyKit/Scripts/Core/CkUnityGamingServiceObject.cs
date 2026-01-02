using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;

namespace CandyKitSDK
{
    public class CkUnityGamingServiceObject : MonoBehaviour
    {
        private readonly string environment = "production";

        public async void Initialize(Action onSuccess, Action<string> onError)
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("CK--> UGS before try");
            try
            {
                InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);
                onSuccess();
                Debug.Log("CK--> UGS init asybc try");
            }
            catch (Exception e)
            {
                // Debug.Log("UGS-> Failed : " + e.Message);
                Debug.Log("CK--> UGS init fail " + e.Message);
                onError(e.Message);
            }
        }
    }
}
