using System;
using UnityEngine;

namespace Viking.ReSize
{
    /// <summary>
    /// Category for ReSize.
    /// </summary>
    [Serializable]
    public class Category
    {
        /// <summary>
        /// Title of the category. ie. Audio
        /// </summary>
        public string title;

        /// <summary>
        /// Extensions to use. ie. mp3 wav ogg
        /// </summary>
        public string extensions;

        /// <summary>
        /// Color to display the category as.
        /// </summary>
        public Color32 color = new Color32(220, 220, 220, 255);

        /// <summary>
        /// Size of the files in the category.
        /// </summary>
        public double amount;

        /// <summary>
        /// Flag for displaying the category or not.
        /// </summary>
        public bool show = true;
    }
}
