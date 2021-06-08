using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Nootools
{
    public static class UnityObjectExtensions
    {
        [Conditional("UNITY_EDITOR")]
        public static void SaveInEditor(this Object o)
        {
            UnityEditor.EditorUtility.SetDirty(o);
            UnityEditor.Undo.RecordObject(o, o.name);
        }
    }
}
