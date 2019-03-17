using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Viking.ReSize
{
    /// <summary>
    /// Editor window for ReSize.
    /// </summary>
    public class ReSizeWindow : EditorWindow
    {
        /// <summary>
        /// ReSize window.
        /// </summary>
        private static ReSizeWindow window;

        /// <summary>
        /// Path to project directory.
        /// </summary>
        private static string path;

        /// <summary>
        /// List of categories marked as visible.
        /// </summary>
        private List<Category> visible = new List<Category>();

        /// <summary>
        /// Initialize the window.
        /// </summary>
        [MenuItem("Viking/ReSize")]
        private static void Init()
        {
            // set the project path
            path = Application.dataPath + "/";

            // load the settings; categories
            ReSizeSettings.LoadSettings();

            window = (ReSizeWindow)GetWindow(typeof(ReSizeWindow), true, "Viking ReSize");

            window.Show();
        }

        /// <summary>
        /// When the window is destroyed; Closed.
        /// </summary>
        private void OnDestroy()
        {
            // force close the settings window
            if (ReSizeSettingsWindow.window != null)
            {
                ReSizeSettingsWindow.window.Close();
            }
        }

        /// <summary>
        /// When the window loses focus; Clicked out of.
        /// </summary>
        private void OnLostFocus()
        {
            // save and update the settings
            EditorUtility.SetDirty(ReSizeSettings.settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Renders the window.
        /// </summary>
        private void OnGUI()
        {
            if (window == null)
            {
                return;
            }

            GUILayout.Space(4);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Analyze"))
            {
                Analyze();
            }
            if (GUILayout.Button("Settings", GUILayout.Width((window.position.width * 0.2f) - 8)))
            {
                OpenSettings();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(4, 24, window.position.width - 8, window.position.height - 28), EditorStyles.textArea);

            GUIStyle label = EditorStyles.label;
            label.normal.textColor = Color.white;
            label.richText = true;

            // keep track of categories set as visible
            visible = ReSizeSettings.settings.categories.Where(x => x.show == true).ToList();

            // display visible categories
            foreach (Category category in visible)
            {
                if (category.show)
                {
                    Bar(visible.IndexOf(category), category.amount, category.title, category.color, label);
                }
            }

            // total
            label.normal.textColor = Color.black;
            GUI.backgroundColor = new Color32(41, 47, 59, 190);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            GUILayout.Label("<b>Total:</b>", label);
            GUILayout.Label(ConvertSize(ReSizeSettings.settings.total), label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // reset
            GUI.backgroundColor = Color.white;

            GUILayout.EndArea();

            Repaint();
        }

        /// <summary>
        /// Display a category.
        /// </summary>
        /// <param name="index">Index to be displayed; Determines depth in list.</param>
        /// <param name="amount">The category size in bytes.</param>
        /// <param name="title">The title of the category. ie. Audio</param>
        /// <param name="color">The color to display the category as.</param>
        /// <param name="style">Style to display the category.</param>
        private void Bar(int index, double amount, string title, Color32 color, GUIStyle style)
        {
            // set the foreground color/area (used space in total)
            GUI.backgroundColor = new Color(0.22f, 0.23f, 0.33f);
            GUILayout.BeginArea(new Rect(0, 30 * index, ((int)Percent(amount, ReSizeSettings.settings.total) * (window.position.width - 4)) * 0.01f, 30), EditorStyles.helpBox);
            GUILayout.EndArea();

            GUI.backgroundColor = color;

            // category background
            GUILayout.BeginArea(new Rect(0, 30 * index, window.position.width - 4, 30), EditorStyles.textArea);

            float width = 98; // title width

            // category info
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            style.alignment = TextAnchor.MiddleLeft;
            EditorGUILayout.LabelField("<b>" + title + "</b>", style, GUILayout.Width(width)); // title
            GUILayout.FlexibleSpace();
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(ConvertSize(amount), style, GUILayout.Width(width)); // amount/size
            GUILayout.FlexibleSpace();
            style.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.LabelField("<i>" + Percent(amount, ReSizeSettings.settings.total).ToString("0.00") + "%</i>", style, GUILayout.Width(width)); // percent of total
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndArea();
        }

        /// <summary>
        /// Scan the project and update the category values if the extensions match.
        /// </summary>
        private void Analyze()
        {
            Clear();

            // get all project files
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            // check each file
            foreach (string file in files)
            {
                // get the file info
                FileInfo info = new FileInfo(file);

                // check each category
                foreach (Category category in ReSizeSettings.settings.categories)
                {
                    // skip this category if extensions have not been specified
                    if (category.extensions.Length == 0)
                    {
                        continue;
                    }

                    // parse the category extensions
                    List<string> extentions = category.extensions.Split(' ').ToList();

                    // if the file has an extension specified by the category
                    if (extentions.Contains(info.Extension.Remove(0, 1).ToLower()))
                    {
                        // add the file size
                        category.amount += info.Length;
                    }
                }

                // update the total (even if the file doesn't match a category)
                ReSizeSettings.settings.total += info.Length;
            }
        }

        /// <summary>
        /// Clear/reset the categories.
        /// </summary>
        private void Clear()
        {
            ReSizeSettings.settings.total = 0;

            foreach (Category category in ReSizeSettings.settings.categories)
            {
                category.amount = 0;
            }
        }

        /// <summary>
        /// Determine the percent of a total.
        /// </summary>
        /// <param name="a">Amount</param>
        /// <param name="b">Total</param>
        /// <returns>Percent of total.</returns>
        private double Percent(double a, double b)
        {
            return (a / b) * 100;
        }

        /// <summary>
        /// Convert a file size (bytes) into a readable format.
        /// </summary>
        /// <param name="size">Bytes to convert.</param>
        /// <returns>Readable size; B, KB, MB, GB</returns>
        private string ConvertSize(double size)
        {
            if (size < 1024) // b
            {
                return size + "<color=#FCF960>B</color>";
            }
            else if (size < 1048576) // kb
            {
               return (size / 1024).ToString("0.00") + "<color=#FCF960>KB</color>";
            }
            else if (size < 1073741824) // mb
            {
                return (size / 1048576).ToString("0.00") + "<color=#FFAD3B>MB</color>";
            }
            else // gb
            {
                return (size / 1073741824).ToString("0.00") + "<color=#C93038>GB</color>";
            }
        }

        /// <summary>
        /// Open the ReSize Settings window.
        /// </summary>
        private void OpenSettings()
        {
            // initialize if closed
            if (ReSizeSettingsWindow.window == null)
            {
                ReSizeSettingsWindow.Init();
            }
            else
            {
                // set focus
                ReSizeSettingsWindow.window.Focus();
            }
        }
    }
}
