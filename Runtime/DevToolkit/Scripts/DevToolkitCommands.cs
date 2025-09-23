using Noo.Nui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using static Noo.DevToolkit.DevToolkitUtility;

namespace Noo.DevToolkit
{
    public class DevToolkitCommands
    {
        public static event Action OnInitialize;

        static bool initialized;

        readonly static Dictionary<MemberInfo, CommandInfo> commandInfos = new();
        readonly static Dictionary<string, CommandPage> commandPages = new();
        readonly static HashSet<MemberInfo> implementedMembers = new();
        internal readonly static Dictionary<DevHotkeyAttribute, CommandInfo> hotkeyInfos = new();

        internal readonly DtkCommandsPage rootVisualElement = new();
        readonly List<NuiDrawer> foundDrawers = new(); // Used for search filtering
        readonly NuiDrawer notFoundDrawer = new NuiTextDrawer("Couldn’t find any results", true);

        internal class CommandInfo
        {
            /// <summary>Full path all lowercase</summary>
            public string id;
            public MemberInfo memberInfo;
            public int order;
            public string path;
            public string displayName;
            public string info;
            public bool inline = true;
            public DevHotkeyAttribute hotkey;
        }

        internal class CommandPage
        {
            public string path;
            public string displayName;
            public string parentPath;
            public List<NuiDrawer> drawers = new();
            public bool sorted;
            public NuiDrawer headerDrawer;

            public void AssertSorted()
            {
                if (!sorted)
                {
                    drawers.Sort();
                    sorted = true;
                }
            }
        }

        internal DevToolkitCommands()
        {
            rootVisualElement.OnSearchQuery += OnCommandsSearchQuery;
            rootVisualElement.OnMoreClicked += OnShowMoreClicked;
        }

        public async Awaitable Load()
        {
            await Awaitable.NextFrameAsync();
            Initialize();
        }

        static void Initialize()
        {
            if (initialized) return;

            initialized = true;

            foreach (var type in CommandTypes)
            {
                GenerateCommands(type);
            }

            OnInitialize?.Invoke();
        }

        static void GenerateCommands(Type type)
        {
            var typeAttr = type.GetCustomAttribute<DevCommandsAttribute>();

            if (typeAttr == null) return;

            var autoGenerateMemberBindingFlags = typeAttr.GenerateMemberFlags;

            using var _ = HashSetPool<FieldInfo>.Get(out var autoFields);
            using var __ = HashSetPool<PropertyInfo>.Get(out var autoProperties);
            using var ___ = HashSetPool<MethodInfo>.Get(out var autoMethods);

            if (typeAttr.GenerateMemberTypes.HasFlagNonAlloc(DevMemberType.Field))
            {
                foreach (var x in type.GetFields(autoGenerateMemberBindingFlags)) autoFields.Add(x);
            }

            if (typeAttr.GenerateMemberTypes.HasFlagNonAlloc(DevMemberType.Property))
            {
                foreach (var x in type.GetProperties(autoGenerateMemberBindingFlags)) autoProperties.Add(x);
            }

            if (typeAttr.GenerateMemberTypes.HasFlagNonAlloc(DevMemberType.Method))
            {
                foreach (var x in type.GetMethods(autoGenerateMemberBindingFlags)) autoMethods.Add(x);
            }

            foreach (var field in type.GetFields(BindFlagAll))
            {
                if (implementedMembers.Contains(field)) continue;
                implementedMembers.Add(field);

                if (!NuiProperty.IsValidMemberForProperty(field)) continue;

                var devCommandAttribute = field.GetCustomAttribute<DevCommandAttribute>();

                if (!autoFields.Contains(field) && devCommandAttribute == null) continue;

                var commandInfo = GetCommandInfo(field);

                var page = GetOrCreatePage(commandInfo.path);
                var drawer = NuiDrawerUtility.CreateDrawer(field);
                page.drawers.Add(drawer);
            }

            foreach (var property in type.GetProperties(BindFlagAll))
            {
                if (implementedMembers.Contains(property)) continue;
                implementedMembers.Add(property);

                if (!NuiProperty.IsValidMemberForProperty(property)) continue;

                var devCommandAttribute = property.GetCustomAttribute<DevCommandAttribute>();
                if (!autoProperties.Contains(property) && devCommandAttribute == null) continue;

                var commandInfo = GetCommandInfo(property);

                var page = GetOrCreatePage(commandInfo.path);
                var drawer = NuiDrawerUtility.CreateDrawer(property);
                page.drawers.Add(drawer);
            }

            foreach (var method in type.GetMethods(BindFlagAll))
            {
                if (implementedMembers.Contains(method)) continue;
                implementedMembers.Add(method);

                if (!NuiProperty.IsValidMemberForProperty(method)) continue;

                var devCommandAttribute = method.GetCustomAttribute<DevCommandAttribute>();
                if (!autoMethods.Contains(method) && devCommandAttribute == null) continue;

                var commandInfo = GetCommandInfo(method);

                if (commandInfo.inline || method.GetParameters().Length == 0)
                {
                    var page = GetOrCreatePage(commandInfo.path);
                    page.drawers.Add(NuiDrawerUtility.CreateDrawer(method));
                }
                else
                {
                    var page = GetOrCreatePage(commandInfo.id, "dtk-drawer__method-page-button");
                    page.drawers.Add(NuiDrawerUtility.CreateDrawer(method));
                }
            }
        }

        readonly string[] moreButtons = new string[] { "Show Hidden Pages" };

        private void OnShowMoreClicked(Button button)
        {
            var dropdown = DropdownUtility.CreateSimpleMenu(moreButtons, (x) =>
            {
                switch (x)
                {
                    case "Show Hidden Pages":
                        ShowPage("Hidden");
                        break;

                    default:
                        break;
                }
            });

            dropdown.Show(button);
        }

        public static void AddCommands(string pagePath, IReadOnlyList<NuiDrawer> drawers)
        {
            pagePath = pagePath.Trim('/');

            if (drawers != null && drawers.Count > 0)
            {
                var page = GetOrCreatePage(pagePath);

                for (int i = 0; i < drawers.Count; i++)
                {
                    page.drawers.Add(drawers[i]);
                }
            }
        }

        private void OnCommandsSearchQuery(IReadOnlyList<string> queries)
        {
            foundDrawers.Clear();

            if (queries.Count > 0)
            {
                foreach (var (pagePath, page) in commandPages.OrderBy(x => x.Key))
                {
                    if (page.path.StartsWith("hidden")) continue;

                    page.AssertSorted();

                    using var _ = ListPool<NuiDrawer>.Get(out var pageDrawers);

                    foreach (var drawer in page.drawers)
                    {
                        if (drawer is NuiPageButtonDrawer) continue;

                        if (drawer.OnFilter(queries))
                        {
                            pageDrawers.Add(drawer);
                        }
                    }

                    if (pageDrawers.Count > 0)
                    {
                        foundDrawers.Add(page.headerDrawer);
                        foundDrawers.AddRange(pageDrawers);
                    }
                }
            }

            if (queries.Count > 0 && foundDrawers.Count == 0)
            {
                foundDrawers.Add(notFoundDrawer);
            }

            rootVisualElement.ShowFoundCommands(foundDrawers);
        }

        internal bool TryGetPage(string path, out CommandPage page)
        {
            return commandPages.TryGetValue(path.ToLowerInvariant(), out page);
        }

        static CommandPage GetOrCreatePage(string path, string buttonClass = "")
        {
            var id = path.ToLowerInvariant();

            if (!commandPages.TryGetValue(id, out var page))
            {
                var pageName = NiceifyName(GetLastPathPart(path));

                page = new CommandPage
                {
                    path = id,
                    displayName = pageName,
                    headerDrawer = new NuiHeaderDrawer(pageName)
                };

                TryGetParentPath(path, out page.parentPath);

                commandPages[id] = page;

                if (!string.IsNullOrEmpty(id) && id != "hidden")
                {
                    var btnDrawer = new NuiPageButtonDrawer(id, page.displayName)
                    {
                        buttonClass = buttonClass
                    };

                    var parentPage = GetOrCreatePage(page.parentPath);

                    parentPage.drawers.Add(btnDrawer);
                }
            }

            return page;
        }

        internal void ShowPage(string path)
        {
            if (TryGetPage(path, out var page))
            {
                rootVisualElement.ShowCommandPage(page);
            }
        }

        public bool PageHasValidItems(string path)
        {
            if (TryGetPage(path, out var page))
            {
                foreach (var drawer in page.drawers)
                {
                    if (drawer.IsValid) return true;
                }
            }

            return false;
        }

        internal static CommandInfo GetCommandInfo(MemberInfo memberInfo)
        {
            if (!commandInfos.TryGetValue(memberInfo, out CommandInfo commandInfo))
            {
                commandInfo = new CommandInfo
                {
                    memberInfo = memberInfo
                };

                var customPathName = string.Empty;
                var parentCommand = memberInfo.DeclaringType == null ? null : GetCommandInfo(memberInfo.DeclaringType);
                var parentPath = memberInfo.DeclaringType == null ? string.Empty : parentCommand.id;

                commandInfo.hotkey = memberInfo.GetCustomAttribute<DevHotkeyAttribute>();

                if (memberInfo is TypeInfo)
                {
                    var attr = memberInfo.GetCustomAttribute<DevCommandsAttribute>();

                    if (attr != null)
                    {
                        customPathName = attr.PathName;
                        commandInfo.order = attr.Order;
                        commandInfo.inline = attr.Inline;
                    }
                }
                else
                {
                    var attr = memberInfo.GetCustomAttribute<DevCommandAttribute>();

                    if (attr != null)
                    {
                        customPathName = attr.PathName;
                        commandInfo.order = attr.Order;
                        commandInfo.info = attr.Info;
                        commandInfo.inline = attr.Inline;
                    }
                }

                if (string.IsNullOrWhiteSpace(customPathName))
                {
                    (commandInfo.displayName, commandInfo.path) = GetDefaultInfo(memberInfo);
                }
                else
                {
                    if (!customPathName.StartsWith('/') && !string.IsNullOrEmpty(customPathName))
                    {
                        customPathName = $"{parentPath}/{customPathName}";
                    }

                    var pathParts = SplitPath(customPathName);
                    using var validPathParts = new TempList<string>(null);

                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        ref var part = ref pathParts[i];
                        part = NiceifyName(part);
                        if (!string.IsNullOrEmpty(part)) validPathParts.Add(part);
                    }

                    if (validPathParts.Count == 0)
                    {
                        (commandInfo.displayName, commandInfo.path) = GetDefaultInfo(memberInfo);
                    }
                    else
                    {
                        var lastIndex = validPathParts.Count - 1;
                        commandInfo.displayName = validPathParts[lastIndex];
                        validPathParts.RemoveAt(lastIndex);
                        commandInfo.path = string.Join('/', validPathParts);
                    }
                }

                commandInfo.id = TrimPath($"{commandInfo.path}/{commandInfo.displayName}").ToLowerInvariant();

                commandInfos[memberInfo] = commandInfo;

                if (commandInfo.hotkey != null && commandInfo.hotkey.IsMemberTypeValid(commandInfo.memberInfo))
                {
                    hotkeyInfos[commandInfo.hotkey] = commandInfo;
                }
            }

            return commandInfo;

            static (string, string) GetDefaultInfo(MemberInfo memberInfo)
            {
                var name = NiceifyName(memberInfo.Name);
                var path = memberInfo.DeclaringType == null ? string.Empty : GetCommandInfo(memberInfo.DeclaringType).id;
                return (name, path);
            }
        }
    }
}
