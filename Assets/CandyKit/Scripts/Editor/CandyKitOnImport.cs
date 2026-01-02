using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace CandyKitSDK
{
    public class CandyKitOnImport
    {
        private static readonly BuildTargetGroup[] DefineSymbolsBuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        private static readonly string[] CandyKitDefineSymbol =
        {
            "CANDYKIT"
        };

        [InitializeOnLoadMethod]
        public static void OnEditorLoad()
        {
            // if (ShouldDefineSymbols())
            // {
            //     ClearDefineSymbols();
            //     SetDefineSymbols();
            // }
        }

        private static void ClearDefineSymbols()
        {
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
            // var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void SetDefineSymbols()
        {
            foreach (var group in DefineSymbolsBuildTargetGroups)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, CandyKitDefineSymbol);
            }
        }

        private static bool ShouldDefineSymbols()
        {
            return true;
        }
    }
}
