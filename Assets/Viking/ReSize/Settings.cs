using System.Collections.Generic;
using UnityEngine;

namespace Viking.ReSize
{
    /// <summary>
    /// Settings for ReSize.
    /// </summary>
    public class Settings : ScriptableObject
    {
        [SerializeField]
        public List<Category> categories = new List<Category>();

        // total project size
        public long total;
    }
}
