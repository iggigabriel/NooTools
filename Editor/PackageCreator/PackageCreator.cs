using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace Noo.Tools.Editor
{
    public static class PackageCreator
    {
        
    }

    public class PackageCreatorWizard : ScriptableWizard
    {
        public string rootNamespace = "";

        public string packageName;

        public AssemblyDefinitionAsset[] runtimeDependacies;
        public AssemblyDefinitionAsset[] editorDependacies;

        [MenuItem("Assets/Create/Noo.Tools/New Package", priority = -10000)]
        static void CreateWizard()
        {
            DisplayWizard<PackageCreatorWizard>("Create New Package", "Create").Init();
        }

        private void Init()
        {
            runtimeDependacies = AssetDatabase
                .FindAssets("t:AssemblyDefinitionAsset Nootools")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(x))
                .Where(x => x.name == "Nootools")
                .ToArray();

            editorDependacies = AssetDatabase
                .FindAssets("t:AssemblyDefinitionAsset Nootools")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(x))
                .ToArray();
        }

        private void OnWizardCreate()
        {
            var activeFolder = GetActiveFolder();

            var packageFolder = Path.Combine(activeFolder, packageName);

            var runtimeFolder = Path.Combine(packageFolder, "Runtime");
            var editorFolder = Path.Combine(packageFolder, "Editor");

            var runtimeAssemblyFile = Path.Combine(runtimeFolder, $"{packageName}.asmdef");
            var editorAssemblyFile = Path.Combine(editorFolder, $"{packageName}.Editor.asmdef");

            Directory.CreateDirectory(runtimeFolder);
            Directory.CreateDirectory(editorFolder);

            var runtimeDependacyList = runtimeDependacies.Select(x => x.name).ToArray();
            var editorDependacyList = editorDependacies.Select(x => x.name).Append(packageName).ToArray();

            File.WriteAllText(runtimeAssemblyFile, GetAssemblyDefinition(packageName, runtimeDependacyList));
            File.WriteAllText(editorAssemblyFile, GetAssemblyDefinition($"{packageName}.Editor", editorDependacyList));

            AssetDatabase.Refresh();
        }

        private void OnWizardUpdate()
        {
            isValid = !string.IsNullOrWhiteSpace(packageName);
        }

        private string GetAssemblyDefinition(string packageName, string[] references = null)
        {
            var rootNamespace = string.IsNullOrWhiteSpace(this.rootNamespace) ? packageName : $"{this.rootNamespace}.{packageName}";

            references ??= new string[0];

            return @$"{{
    ""name"": ""{packageName}"",
    ""rootNamespace"": ""{rootNamespace}"",
    ""references"": [{string.Join(",", references.Select(x => $@"""{x}"""))}],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
        }

        static string GetActiveFolder()
        {
            const string DEFAULT_FOLDER = "Assets";

            var obj = Selection.activeObject;

            if (obj == null) return DEFAULT_FOLDER;

            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (string.IsNullOrEmpty(path)) return DEFAULT_FOLDER;

            if (Directory.Exists(path))
            {
                return path;
            }
            else
            {
                return Path.GetDirectoryName(path);
            }
        }

    }
}