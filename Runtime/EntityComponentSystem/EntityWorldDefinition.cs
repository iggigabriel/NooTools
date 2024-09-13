#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript, CreateAssetMenu(menuName = "Noo.Tools/ECS World", fileName = "EntityWorld", order = -10000)]
    public class EntityWorldDefinition : ScriptableObject
    {
        [Serializable]
        public class EntityManagerSettings
        {
            public int scriptExecutionOrder = -100;
            public string classAttributes;
            public string baseManagerClass = "MonoBehaviour";
            public bool markAwakeAsOverride = false;
            public bool markOnDisableAsOverride = false;
            public bool markOnValidateAsOverride = false;
        }

        [Serializable]
        public class EntitySettings
        {
            public string classAttributes;
            public string baseEntityClass = "MonoBehaviour";
            public bool markUpdateAsOverride = false;
            public bool markOnEnableAsOverride = false;
            public bool markOnDisableAsOverride = false;
            public bool markOnResetAsOverride = false;
        }

        [Serializable]
        public class EntitySystemSettings
        {
            public int scriptExecutionOrder = -10;
            public string classAttributes;
            public string baseSystemClass = "MonoBehaviour";
            public bool markOnResetAsOverride = false;
        }

        public string typePrefix;
        public string @namespace = "";
        public string outputFolder;

        public readonly string[] defaultUsings = new string[] {
            "System",
            "System.Diagnostics",
            "System.Collections.Generic",
            "System.Linq",
            "System.Runtime.CompilerServices",
            "System.Runtime.InteropServices",
            "Noo.Tools",
            "UnityEngine",
            "UnityEngine.Jobs",
            "Unity.Jobs",
            "UnityEngine.Profiling",
            "UnityEngine.SceneManagement",
            "Unity.Collections",
            "Unity.Mathematics",
            "Sirenix.OdinInspector"
        };

        [PropertySpace]
        public string[] extraUsings;

        [Title("Entity Manager"), HideLabel]
        public EntityManagerSettings managerSettings;

        [Title("Entities"), HideLabel]
        public EntitySettings entitySettings;

        [Title("Entities System"), HideLabel]
        public EntitySystemSettings entitySystemSettings;

        [Button(ButtonSizes.Large, Icon = SdfIconType.FileEarmarkBinaryFill), Title("Actions")]
        void GenerateCs()
        {
            var generator = new EntityWorldScriptGenerator(this, "cs");
            generator.Generate();
            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Large, Icon = SdfIconType.FileEarmarkFontFill)]
        void GenerateTxt()
        {
            var generator = new EntityWorldScriptGenerator(this, "txt");
            generator.Generate();
            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Large, Icon = SdfIconType.FileEarmarkPlusFill)]
        void CreateComponent()
        {
            ScriptableObjectUtility.CreateSubAsset<EntityComponentDefinition>(this);
        }

        [Button(ButtonSizes.Large, Icon = SdfIconType.FileEarmarkPlusFill)]
        void CreateArchetype()
        {
            ScriptableObjectUtility.CreateSubAsset<EntityArchetypeDefinition>(this);
        }

        public string GetOutputFolder()
        {
            var dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            return string.IsNullOrEmpty(outputFolder) ? dir : Path.Combine(dir, outputFolder);
        }

        public IEnumerable<string> ActiveUsings => defaultUsings.Concat(extraUsings).Distinct();

        public IEnumerable<EntityArchetypeDefinition> ActiveArchetypes =>
            AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
            .Where(x => x is EntityArchetypeDefinition)
            .Select(x => x as EntityArchetypeDefinition);

        public IEnumerable<EntityComponentDefinition> ActiveComponents =>
            AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
            .Where(x => x is EntityComponentDefinition)
            .Select(x => x as EntityComponentDefinition);

        [MenuItem(itemName: "Tools/NooEntities/Regenerate All", priority = 10000)]
        private static void RegenerateAll()
        {
            var assets = AssetDatabaseUtility.FindAllAssetsOfType<EntityWorldDefinition>();

            foreach (var asset in assets)
            {
                var generator = new EntityWorldScriptGenerator(asset, "cs");
                generator.Generate();
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif