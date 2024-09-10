using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "GridConfig", order = 0)]
public class GridConfig : ScriptableObject
{
    public const string asset_path = "Assets/Resources/GridSettings.asset";
    public int2 min_cell;
    public int2 max_cell;
    public float cell_size = 1;
    
    #if UNITY_EDITOR
    internal static GridConfig GetOrCreateSettings()
    {
        GridConfig settings = AssetDatabase.LoadAssetAtPath<GridConfig>(asset_path);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<GridConfig>();
            AssetDatabase.CreateAsset(settings, asset_path);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }
    #endif
}

#if UNITY_EDITOR
static class GridConfigSettingsRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateGridConfigSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/Grid Settings", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "Grid Settings",
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) =>
            {
                var settings = new SerializedObject(GridConfig.GetOrCreateSettings());
                EditorGUILayout.PropertyField(settings.FindProperty("cell_size"));
                settings.ApplyModifiedProperties();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Grid" })
        };

        return provider;
    }
}
#endif