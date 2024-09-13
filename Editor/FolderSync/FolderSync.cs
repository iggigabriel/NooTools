using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public class FolderSync : ScriptableObject
    {
        [MenuItem("Tools/Sync Folders")]
        static void MenuItem_SyncAll()
        {
            var assets = AssetDatabase.FindAssets("t:FolderSync")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<FolderSync>(x))
                .Where(x => x);

            foreach (var asset in assets) asset.Sync();
        }

        public string Root
        {
            get => EditorPrefs.GetString($"FolderSync_Root_${name}");
            set => EditorPrefs.SetString($"FolderSync_Root_${name}", value);
        }

        [Serializable]
        public class FolderInfo
        {
            public string source;
            public string dest;
        }

        public List<string> files = new() { "*.jpg", "*.png", "*.psd" };

        public List<FolderInfo> folders = new();

        public void Sync()
        {
            if (string.IsNullOrEmpty(Root))
            {
                Debug.LogError($"{name}: Root folder not set!");
                return;
            }

            foreach(var folder in folders)
            {
                var sourceDir = Path.Combine(Root, folder.source).Replace('\\', '/');
                var destDir = Path.Combine(Application.dataPath, folder.dest);

                foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                {
                    string dirToCreate = dir.Replace(sourceDir, destDir);
                    Directory.CreateDirectory(dirToCreate);
                }

                var newFiles = files.SelectMany(x => Directory.GetFiles(sourceDir, x, SearchOption.AllDirectories)).ToList();

                for (int i = 0; i < newFiles.Count; i++)
                {
                    var newPath = newFiles[i];

                    File.Copy(newPath, newPath.Replace(sourceDir, destDir), true);

                    if (EditorUtility.DisplayCancelableProgressBar(name, $"{Path.GetFileName(newPath)}", (float)i / newFiles.Count))
                    {
                        Debug.Log($"{name} sync cancelled");
                        return;
                    }
                }

                EditorUtility.ClearProgressBar();

                AssetDatabase.Refresh();

                Debug.Log($"{name} synced {newFiles.Count} files.");
            }
        }
    }

    [CustomEditor(typeof(FolderSync))]
    public class FolderSyncEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var asset = target as FolderSync;

            asset.Root = EditorGUILayout.TextField("Root", asset.Root);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("files"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("folders"));

            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(asset.Root));
            if (GUILayout.Button("Sync"))
            {
                try
                {
                    asset.Sync();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    EditorUtility.ClearProgressBar();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}