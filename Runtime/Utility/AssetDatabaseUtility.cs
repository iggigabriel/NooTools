#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    public static class AssetDatabaseUtility
    {
        public static IEnumerable<T> FindAllAssetsOfType<T>(string query = "", Type filterType = null) where T : UnityEngine.Object
        {
            var type = filterType ?? typeof(T);

            return AssetDatabase.FindAssets(string.Format("t:{0} {1}", type.Name, query))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
                .Where(x => x && x is T)
                .Cast<T>();
        }

        public static T FindAssetOfType<T>(string query, Type filterType = null) where T : UnityEngine.Object
        {
            return FindAllAssetsOfType<T>(query, filterType).FirstOrDefault();
        }

        public static T FindAssetWithName<T>(string name, Type filterType = null) where T : UnityEngine.Object
        {
            var type = filterType ?? typeof(T);

            return AssetDatabase.FindAssets(string.Format("t:{0} {1}", type.Name, name))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
                .Where(x => x && x is T)
                .Cast<T>()
                .Where(x => x.name == name)
                .FirstOrDefault();
        }

        public static List<T> GetPrefabsWithComponent<T>(string filter = "") where T : Component
        {
            return AssetDatabase.FindAssets(string.Format("t:Prefab {0}", filter))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(x))
                .Select(x => x.GetComponent<T>())
                .Where(x => x != null)
                .ToList();
        }

        public static List<GameObject> GetPrefabsWithComponentInChildren<T>(string filter = "", bool includeInactive = false) where T : Component
        {
            return AssetDatabase.FindAssets(string.Format("t:Prefab {0}", filter))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(x))
                .Where(x => x.GetComponentInChildren<T>(includeInactive) != null)
                .ToList();
        }

        public static void SaveAsset<T>(T asset) where T : UnityEngine.Object
        {
            EditorUtility.SetDirty(asset);
        }

        public static IEnumerable<T> SaveAssets<T>(this IEnumerable<T> assets) where T : UnityEngine.Object
        {
            foreach (var asset in assets) EditorUtility.SetDirty(asset);
            return assets;
        }

        public static void DestroyAllSubassetsAtPath(UnityEngine.Object mainAsset)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mainAsset));

            Undo.SetCurrentGroupName($"Destroy All Subassets of ({mainAsset.name})");
            var group = Undo.GetCurrentGroup();

            foreach (var asset in assets)
            {
                if (!AssetDatabase.IsMainAsset(asset)) Undo.DestroyObjectImmediate(asset);
            }

            Undo.CollapseUndoOperations(group);
        }
    }
}
#endif