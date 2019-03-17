using UnityEditor;
using UnityEngine;

namespace Viking.ReSize
{
    /// <summary>
    /// Settings for ReSize.
    /// </summary>
    public static class ReSizeSettings
    {
        /// <summary>
        /// Path to the settings object.
        /// </summary>
        public static string path = "Assets/Viking/ReSize/Settings.asset";

        /// <summary>
        /// Object containing the data.
        /// </summary>
        public static Settings settings;

        /// <summary>
        /// Load the settings.
        /// </summary>
        public static void LoadSettings()
        {
            // get the settings
            settings = AssetDatabase.LoadAssetAtPath<Settings>(path);

            // if one isn't found
            if (settings == null)
            {
                // create and set
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Settings>(), path);
                settings = AssetDatabase.LoadAssetAtPath<Settings>(path);
            }
        }
    }
}
