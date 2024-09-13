using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Noo.Tools.Editor
{
    public static class AddressablesUtility
    {
        public static void SetAddress(string guid, string address, string groupName, string label = null)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.groups.Find(o => o.Name == groupName);
            if (group == null)
            {
                Debug.LogError("Unable to find group " + groupName);
                return;
            }

            var entry = group.GetAssetEntry(guid);
            entry ??= settings.CreateOrMoveEntry(guid, group);

            entry.SetAddress(address);

            if (!string.IsNullOrEmpty(label))
            {
                var labels = settings.GetLabels();
                if (!labels.Contains(label))
                {
                    settings.AddLabel(label);
                }
                entry.SetLabel(label, true);
            }
        }

        public static string GetAddress(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(guid);
            return entry?.address;
        }

        public static void Remove(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            settings.RemoveAssetEntry(guid);
        }
    }
}
