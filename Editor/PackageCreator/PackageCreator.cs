using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nootools.Editor.PackageCreator
{
    public static class PackageCreator
    {
        
    }

    public class PackageCreatorWizard : ScriptableWizard
    {
        public string packageName;

        [Space]
        public AssemblyDefinitionAsset[] runtimeDependacies;
        public AssemblyDefinitionAsset[] editorDependacies;

        [MenuItem("Assets/Create/Nootools/New Package")]
        static void CreateWizard()
        {
            DisplayWizard<PackageCreatorWizard>("Create New Package", "Create");
        }

        bool IsValid => !string.IsNullOrWhiteSpace(packageName);

        void OnWizardCreate()
        {
            if (!IsValid) return;
            

        }

        
    }
}