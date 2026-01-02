using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System.IO;
using System.Collections.Generic;
using GameAnalyticsSDK;
using Facebook.Unity.Settings;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CandyKitSDK
{
    [CanEditMultipleObjects]
    public class CandyKitSettings : EditorWindow
    {
        public const string path = "Assets/Resources/CandyKit/CandyKitSettings.asset";

        [MenuItem("CandyKit/Settings")]
        private static void ShowSettings()
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);

            if (asset)
            {
                AssetDatabase.OpenAsset(asset);
            }
            else
            {
                if (!Directory.Exists("Assets/Resources"))
                {
                    Directory.CreateDirectory("Assets/Resources");
                }
                Directory.CreateDirectory("Assets/Resources/CandyKit");
                CandyKitSettingsScriptableObject settingsAsset = ScriptableObject.CreateInstance<CandyKitSettingsScriptableObject>();

                AssetDatabase.CreateAsset(settingsAsset, "Assets/Resources/CandyKit/CandyKitSettings.asset");
                AssetDatabase.SaveAssets();

                AssetDatabase.OpenAsset(settingsAsset);
            }
        }

        [MenuItem("CandyKit/Setup Credentials")]
        private static void SetupConfigurations()
        {
            CandyKitSettingsScriptableObject settings = Resources.Load<CandyKitSettingsScriptableObject>("CandyKit/CandyKitSettings");

            // GameAnalytics
            GameAnalytics.SettingsGA.UsePlayerSettingsBuildNumber = true;
            while (GameAnalytics.SettingsGA.Platforms.Count > 0)
            {
                GameAnalytics.SettingsGA.RemovePlatformAtIndex(0);
            }
            GameAnalytics.SettingsGA.AddPlatform(RuntimePlatform.Android);
            GameAnalytics.SettingsGA.AddPlatform(RuntimePlatform.IPhonePlayer);

            GameAnalytics.SettingsGA.UpdateGameKey(0, settings.Android.GameAnalyticsGameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(0, settings.Android.GameAnalyticsGameSecret);
            GameAnalytics.SettingsGA.UpdateGameKey(1, settings.iOS.GameAnalyticsGameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(1, settings.iOS.GameAnalyticsGameSecret);

            // Facebook
            FacebookSettings.AppLabels = new List<string> { Application.productName };
            FacebookSettings.AppIds = new List<string> { settings.FacebookAppID };
            FacebookSettings.ClientTokens = new List<string> { settings.FacebookAppClientToken };
        }

        [MenuItem("CandyKit/Add Scoped Package")]
        public static void ShowWindow()
        {
            if (EditorUtility.DisplayDialog("Add Scoped Registry Package", "This will add a scoped registry and a package to your manifest.json. Continue?", "Yes", "Cancel"))
            {
                string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

                if (!File.Exists(manifestPath))
                {
                    Debug.LogError("manifest.json not found!");
                    return;
                }

                var json = File.ReadAllText(manifestPath);
                var manifest = JObject.Parse(json);

                var scopedRegistries = manifest["scopedRegistries"] as JArray ?? new JArray();
                var hasAppLovin = scopedRegistries.Any(r => r["name"]?.ToString() == "AppLovin MAX Unity");
                if (!hasAppLovin)
                {
                    scopedRegistries.Add(new JObject
                    {
                        ["name"] = "AppLovin MAX Unity",
                        ["url"] = "https://unity.packages.applovin.com/",
                        ["scopes"] = new JArray(
                            "com.applovin.mediation.ads",
                            "com.applovin.mediation.adapters",
                            "com.applovin.mediation.dsp")
                    });
                    Debug.Log("Added AppLovin scoped registry.");
                }

                // Add OpenUPM registry if it doesn't exist
                var hasOpenUPM = scopedRegistries.Any(r => r["name"]?.ToString() == "package.openupm.com");
                if (!hasOpenUPM)
                {
                    scopedRegistries.Add(new JObject
                    {
                        ["name"] = "package.openupm.com",
                        ["url"] = "https://package.openupm.com",
                        ["scopes"] = new JArray(
                            "com.google.external-dependency-manager",
                            "com.gameanalytics")
                    });
                    Debug.Log("Added OpenUPM scoped registry.");
                }

                manifest["scopedRegistries"] = scopedRegistries;

                // // Add package
                // var dependencies = manifest["dependencies"] as JObject;
                // if (dependencies != null && dependencies["com.mycompany.mypackage"] == null)
                // {
                //     dependencies["com.mycompany.mypackage"] = "1.2.3";
                //     Debug.Log("Added package dependency.");
                // }

                // Save changes
                File.WriteAllText(manifestPath, manifest.ToString());
                AssetDatabase.Refresh();
                Debug.Log("manifest.json updated. Unity will reload packages.");
            }
        }
    }
}
