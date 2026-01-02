using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerUpHandler))]
public class PowerUpHandlerEditor : Editor
{
    SerializedProperty powerUps, powerUpUIs, unlockLoseRequirement, unlockLevelRequirement;

    private void OnEnable()
    {
        powerUps = serializedObject.FindProperty("powerUps");
        powerUpUIs = serializedObject.FindProperty("powerUpUIs");
        unlockLoseRequirement = serializedObject.FindProperty("unlockLoseRequirement");
        unlockLevelRequirement = serializedObject.FindProperty("unlockLevelRequirement");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Power-Up Table", EditorStyles.boldLabel);

        // Table Header
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("PowerUp", EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.Label("UI", EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.Label("Lose Req", EditorStyles.boldLabel, GUILayout.Width(60));
        GUILayout.Label("Level Req", EditorStyles.boldLabel, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        int maxCount = Mathf.Max(
            powerUps.arraySize,
            powerUpUIs.arraySize,
            unlockLoseRequirement.arraySize,
            unlockLevelRequirement.arraySize
        );

        for (int i = 0; i < maxCount; i++)
        {
            EditorGUILayout.BeginHorizontal();

            DrawArrayElement(powerUps, i, 100);
            DrawArrayElement(powerUpUIs, i, 100);
            DrawArrayElement(unlockLoseRequirement, i, 60);
            DrawArrayElement(unlockLevelRequirement, i, 60);

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                RemoveEntryAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Entry"))
        {
            AddEntry();
        }
        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All?", "Are you sure you want to remove all entries?", "Yes", "No"))
            {
                powerUps.arraySize = 0;
                powerUpUIs.arraySize = 0;
                unlockLoseRequirement.arraySize = 0;
                unlockLevelRequirement.arraySize = 0;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
        DrawPropertiesExcluding(serializedObject,
            "powerUps", "powerUpUIs", "unlockLoseRequirement", "unlockLevelRequirement");

        serializedObject.ApplyModifiedProperties();
    }

    void DrawArrayElement(SerializedProperty array, int index, float width)
    {
        if (index < array.arraySize)
        {
            var element = array.GetArrayElementAtIndex(index);
            EditorGUILayout.PropertyField(element, GUIContent.none, GUILayout.Width(width));
        }
        else
        {
            GUILayout.Label("-", GUILayout.Width(width));
        }
    }

    void AddEntry()
    {
        powerUps.arraySize++;
        powerUpUIs.arraySize++;
        unlockLoseRequirement.arraySize++;
        unlockLevelRequirement.arraySize++;
    }

    void RemoveEntryAt(int index)
    {
        RemoveFromArray(powerUps, index);
        RemoveFromArray(powerUpUIs, index);
        RemoveFromArray(unlockLoseRequirement, index);
        RemoveFromArray(unlockLevelRequirement, index);
    }

    void RemoveFromArray(SerializedProperty array, int index)
    {
        if (index < array.arraySize)
        {
            array.DeleteArrayElementAtIndex(index);
        }
    }
}
