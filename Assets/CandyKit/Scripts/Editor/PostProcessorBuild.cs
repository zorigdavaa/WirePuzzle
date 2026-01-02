
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
#if UNITY_IOS
using System.Xml;
using UnityEditor.iOS.Xcode;
#endif

public class PostProcessorBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        AddAdService(target, buildPath);
        // AddTenjinEndpoint(target, buildPath);
        // AddFirebaseConsent(target, buildPath);
    }

    private static bool AddFirebaseConsent(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        if (target != BuildTarget.iOS)
            return false;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

        if (!File.Exists(plistPath))
        {
            Debug.LogError("Info.plist not found!");
            return false;
        }

        XmlDocument plist = new XmlDocument();
        plist.Load(plistPath);

        XmlNode dict = plist.SelectSingleNode("plist/dict");
        if (dict == null)
        {
            Debug.LogError("Could not find <dict> node in Info.plist!");
            return false;
        }

        // Helper function to add a key-value pair without duplication
        void AddBooleanKey(XmlNode dictNode, string key)
        {
            // Check if key already exists
            XmlNode existingKey = null;
            foreach (XmlNode node in dictNode.ChildNodes)
            {
                if (node.Name == "key" && node.InnerText == key)
                {
                    existingKey = node;
                    break;
                }
            }

            // Remove old key-value pair if it exists
            if (existingKey != null)
            {
                dictNode.RemoveChild(existingKey.NextSibling); // Remove value
                dictNode.RemoveChild(existingKey); // Remove key
            }

            // Add new key
            XmlElement keyElement = plist.CreateElement("key");
            keyElement.InnerText = key;
            dictNode.AppendChild(keyElement);

            // Add corresponding <true/> value
            XmlElement trueElement = plist.CreateElement("true");
            dictNode.AppendChild(trueElement);
        }

        // Add the required keys
        AddBooleanKey(dict, "google_analytics_default_allow_analytics_storage");
        AddBooleanKey(dict, "google_analytics_default_allow_ad_storage");
        AddBooleanKey(dict, "google_analytics_default_allow_ad_user_data");
        AddBooleanKey(dict, "google_analytics_default_allow_ad_personalization_signals");

        // Save the updated plist
        plist.Save(plistPath);

        Debug.Log("Successfully updated Info.plist with Google Analytics settings.");
#endif
        return true;
    }

    //     <meta-data android:name="google_analytics_default_allow_analytics_storage" android:value="true" />
    // <meta-data android:name="google_analytics_default_allow_ad_storage" android:value="true" />
    // <meta-data android:name="google_analytics_default_allow_ad_user_data" android:value="true" />
    // <meta-data android:name="google_analytics_default_allow_ad_personalization_signals" android:value="true" />


    private static void AddTenjinEndpoint(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            // Path to the Info.plist file
            string plistPath = pathToBuiltProject + "/Info.plist";

            // Load the plist file
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Add the NSAdvertisingAttributionReportEndpoint key if it doesn't exist
            var rootDict = plist.root;
            if (!rootDict.values.ContainsKey("NSAdvertisingAttributionReportEndpoint"))
            {
                rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://tenjin-skan.com");
            }
            // Save the updated plist file
            plist.WriteToFile(plistPath);
        }
#endif
    }

    private static void AddAdService(BuildTarget target, string buildPath)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            // Path to the Xcode project
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            // Get the target GUID (compatible with Unity 2019 and later)
            string targetGuid = project.GetUnityMainTargetGuid();

            // Add AdServices framework in weak link mode
            project.AddFrameworkToProject(targetGuid, "AdServices.framework", weak: true);

            // Save the modified Xcode project
            project.WriteToFile(projectPath);
        }
#endif
    }
}

