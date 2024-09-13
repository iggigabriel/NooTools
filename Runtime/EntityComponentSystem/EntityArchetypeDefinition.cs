#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public class EntityArchetypeDefinition : ScriptableObject
    {
        [Serializable]
        public class ComponentRef
        {
            [HideLabel, HorizontalGroup("Type", Width = 16)]
            public bool customTypeEnabled;

            [ValueDropdown("@$root.WorldComponents"), HorizontalGroup("Type"), HideLabel, HideIf(nameof(customTypeEnabled))]
            public EntityComponentDefinition type;

            [HorizontalGroup("Type"), HideLabel, ShowIf(nameof(customTypeEnabled))]
            public string customType;

            public string name;

            [HideInTables]
            public string description;

            public string TypeName => customTypeEnabled ? customType : type.typeName;

            [HideInTables]
            public string propertyAttributes;

            [HideInTables]
            public bool hideLabel = true;

            [HideInTables]
            public bool hideInInspector;

            [HideInTables, HideIf(nameof(hideInInspector))]
            public string inspectorDrawerAttributes;
        }

        [Serializable]
        public class ComponentBufferRef
        {
            [PropertyOrder(0), TableColumnWidth(90, false)]
            public int maxCapacity = 8;

            [HideLabel, HorizontalGroup("Type", Width = 16)]
            public bool customTypeEnabled;

            [ValueDropdown("@$root.WorldComponents"), HorizontalGroup("Type"), HideLabel, HideIf(nameof(customTypeEnabled))]
            public EntityComponentDefinition type;

            [HorizontalGroup("Type"), HideLabel, ShowIf(nameof(customTypeEnabled))]
            public string customType;

            public string name;

            [HideInTables]
            public string description;

            public string TypeName => customTypeEnabled ? customType : type.typeName;

            [HideInTables]
            public string propertyAttributes;

            [HideInTables]
            public bool hideInInspector;

            [HideInTables, HideIf(nameof(hideInInspector))]
            public string inspectorDrawerAttributes;
        }

        [OnValueChanged(nameof(UpdateAssetName))]
        public string typeName;
        public int initialDataCapacity = 32;
        public bool needsTransformAccess = true;
        public bool deparentTransformOnRegister = true;
        public string classAttributes;

        [Title("Components"), TableList(AlwaysExpanded = true, DrawScrollView = false, ShowPaging = false), LabelText(" ")]
        public ComponentRef[] componentDefinitions;

        [Title("Buffers"), TableList(AlwaysExpanded = true, DrawScrollView = false, ShowPaging = false), LabelText(" ")]
        public ComponentBufferRef[] componentBuffers;

        void UpdateAssetName()
        {
            name = $"{typeName}";
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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

        private IEnumerable WorldComponents =>
            AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
            .Where(x => x is EntityComponentDefinition)
            .Select(x => x as EntityComponentDefinition)
            .Select(x => new ValueDropdownItem(x.typeName, x));
    }
}
#endif