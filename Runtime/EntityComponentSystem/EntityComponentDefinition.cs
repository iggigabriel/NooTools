#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public class EntityComponentDefinition : ScriptableObject
    {
        [Serializable, HideLabel]
        public class ComponentVariable
        {
            [TabGroup("General")]
            [HideInTables, ShowIf("@$root.structLayout == LayoutKind.Explicit")]
            public int fieldOffset;

            [TabGroup("General")]
            public string type;
            [TabGroup("General")]
            public string name;
            [TabGroup("General")]
            public string defaultValue;

            [TabGroup("Metadata")]
            [TextArea]
            public string tooltip;

            [TabGroup("Metadata")]
            [TextArea]
            public string extraAttributes;
        }

        [OnValueChanged(nameof(UpdateAssetName))]
        public string typeName;

        public LayoutKind structLayout = LayoutKind.Sequential;
        public string structAttributes;

        [Title("Variables"), ListDrawerSettings(ShowPaging = false, DefaultExpandedState = true)]
        public ComponentVariable[] variables;

        void UpdateAssetName()
        {
            name = $"{typeName}";
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [Button(ButtonSizes.Large), GUIColor("#ff9999"), Title("Actions")]
        void Delete()
        {
            if (EditorUtility.DisplayDialog("Are you sure", $"Do you want to delete {name}?", "Ok", "Cancel"))
            {
                var assetPath = AssetDatabase.GetAssetPath(this);
                AssetDatabase.RemoveObjectFromAsset(this);
                DestroyImmediate(this, true);
                EditorUtility.SetDirty(AssetDatabase.LoadMainAssetAtPath(assetPath));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif