using UnityEditor;
using UnityEngine;

namespace Viking.ReSize
{
    /// <summary>
    /// Editor window for the ReSize Settings.
    /// </summary>
    public class ReSizeSettingsWindow : EditorWindow
    {
        /// <summary>
        /// ReSize Settings window.
        /// </summary>
        public static ReSizeSettingsWindow window;

        /// <summary>
        /// Category scroll position.
        /// </summary>
        private Vector2 scroll = Vector2.zero;

        /// <summary>
        /// Initialize the window.
        /// </summary>
        public static void Init()
        {
            window = (ReSizeSettingsWindow)GetWindow(typeof(ReSizeSettingsWindow), true, "ReSize Settings");
            
            window.Show();
        }

        /// <summary>
        /// When the window loses focus; Clicked out of.
        /// </summary>
        private void OnLostFocus()
        {
            EditorUtility.SetDirty(ReSizeSettings.settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Renders the window.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Category", GUILayout.Width(window.position.width - 28)))
            {
                ReSizeSettings.settings.categories.Add(new Category());
            }

            // help for extensions
            GUI.Box(new Rect(window.position.width - 22, 3, 18, 18), new GUIContent("?", "Extensions need to be seperated by spaces in order to parse correctly."));
            GUI.Label(new Rect(10, 40, 98, 32), GUI.tooltip);
            EditorGUILayout.EndHorizontal();

            Rect frame = new Rect(4, 24, window.position.width - 8, window.position.height - 28);

            GUILayout.BeginArea(frame, EditorStyles.textField);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(frame.width - 4), GUILayout.Height(frame.height - 1));
            EditorGUILayout.BeginVertical();

            // display each category and its settings
            for (int i = 0; i < ReSizeSettings.settings.categories.Count; i++)
            {
                GUI.backgroundColor = new Color32(140, 140, 140, 190);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Title: ", GUILayout.ExpandWidth(false));
                ReSizeSettings.settings.categories[i].title = EditorGUILayout.TextField(ReSizeSettings.settings.categories[i].title, GUILayout.ExpandWidth(true));
                GUILayout.Label("Color: ", GUILayout.ExpandWidth(false));
                ReSizeSettings.settings.categories[i].color = EditorGUILayout.ColorField(ReSizeSettings.settings.categories[i].color, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Extensions: ", GUILayout.ExpandWidth(false));
                ReSizeSettings.settings.categories[i].extensions = EditorGUILayout.TextField(ReSizeSettings.settings.categories[i].extensions, GUILayout.ExpandWidth(true));
                GUILayout.Label("Show: ", GUILayout.ExpandWidth(false));
                ReSizeSettings.settings.categories[i].show = EditorGUILayout.Toggle(ReSizeSettings.settings.categories[i].show);
                EditorGUILayout.EndHorizontal();

                // button to delete the category
                if (GUILayout.Button("Delete"))
                {
                    ReSizeSettings.settings.categories.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
