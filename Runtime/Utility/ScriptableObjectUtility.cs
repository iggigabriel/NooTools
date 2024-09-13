#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateSubAsset<T>(Object parentAsset) where T : ScriptableObject
        {
            var assetPath = AssetDatabase.GetAssetPath(parentAsset);

            if (string.IsNullOrEmpty(assetPath)) throw new System.Exception("Parent asset is invalid.");

            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.AddObjectToAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAssetWithFileDialog<T>(bool selectAsset = true) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = EditorUtility.SaveFilePanel("Save location", "Assets", typeof(T).Name.ToString (), "asset"); 

            if ( string.IsNullOrEmpty(path) )
                return null;

            //Get project relative path and ensure path is within project
            var projectRelative = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(projectRelative))
            {
                EditorUtility.DisplayDialog("Error", "Please select somewhere within your assets folder.", "OK");
                return null;
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(projectRelative);

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if(selectAsset) {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

            return asset;
        }
        
        [MenuItem("Assets/Create/Noo.Tools/Asset From Selected Script", priority = -10000)]
        public static void CreateAssetFromSelectedScript()
        {
            MonoScript ms = Selection.objects[0] as MonoScript;

            var targetDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
            
            string path = EditorUtility.SaveFilePanel("Save location", targetDir, ms.name, "asset"); 
            
            if ( string.IsNullOrEmpty(path) )
                return;
            
            //Get project relative path and ensure path is within project
            var projectRelative = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(projectRelative))
            {
                EditorUtility.DisplayDialog("Error", "Please select somewhere within your assets folder.", "OK");
                return;
            }
            
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(projectRelative);
            
            ScriptableObject scriptableObject = ScriptableObject.CreateInstance(ms.GetClass());
            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scriptableObject;
        }
        
        [MenuItem("Assets/Create/Noo.Tools/Asset From Selected Script", true)]
        public static bool CreateAssetFromSelectedScript_Validator()
        {
            if ( Selection.objects != null &&
                Selection.objects.Length == 1 &&
                Selection.objects[0] is MonoScript &&
                (Selection.objects[0] as MonoScript).GetClass() != null &&
                (Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject))&&
                !(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif