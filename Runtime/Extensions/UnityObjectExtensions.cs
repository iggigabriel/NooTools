using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Noo.Tools
{
    public static class UnityObjectExtensions
    {
        [Conditional("UNITY_EDITOR")]
        public static void SaveInEditor(this Object o)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(o);
#endif
        }
    }
}
