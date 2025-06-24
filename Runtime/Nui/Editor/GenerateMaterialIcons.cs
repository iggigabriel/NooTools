#if UNITY_EDITOR && NUI_DEV && ODIN_INSPECTOR_3_3

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Noo.Nui
{
    public class GenerateMaterialIcons : OdinEditorWindow
    {
        public TextAsset sourceCodepoints;

        [Sirenix.OdinInspector.FilePath(RequireExistingPath = false, Extensions = "cs")]
        public string outputFile;

        //[MenuItem("Tools/Noo Nui/Material Icons Script Generator")]
        public static void Open() => GetWindow<GenerateMaterialIcons>();

        [Serializable]
        public class IconInfo : IEquatable<IconInfo>
        {
            public string name;
            public string unicode;

            public override bool Equals(object obj)
            {
                return Equals(obj as IconInfo);
            }

            public bool Equals(IconInfo other)
            {
                return other is not null &&
                       name == other.name;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(name);
            }

            public static bool operator ==(IconInfo left, IconInfo right)
            {
                return EqualityComparer<IconInfo>.Default.Equals(left, right);
            }

            public static bool operator !=(IconInfo left, IconInfo right)
            {
                return !(left == right);
            }
        }

        [Button]
        public void Generate()
        {
            var materialIconInfo = sourceCodepoints.text
                .Split('\n')
                .Where(x => x.Split(" ").Length > 1)
                .Select(x => new IconInfo() { name = x.Split(" ")[0].Trim(), unicode = x.Split(" ")[1].Trim() })
                .Where(x => !string.IsNullOrWhiteSpace(x.name))
                .Distinct()
                .ToList();

            using var sb = new ScriptBuilder(outputFile);

            sb.Comment($"Auto generated script, do not modify!");
            sb.Space();
            sb.Usings("System", "System.Collections.Generic", "Unity.Properties", "UnityEngine.UIElements");

            using (sb.Section($"namespace Noo.Nui"))
            {
                sb.Line("[UxmlElement]");
                using (sb.Section($"public partial class NuiIconMat : NuiIcon"))
                {
                    sb.Line($"private static readonly Dictionary<MatIcon, string> enumToName = new()");
                    sb.Line("{");
                    using (sb.Indent())
                    {
                        foreach (var i in materialIconInfo)
                        {
                            sb.Line($"{{ MatIcon.{EnumFriendlyName(i.name)}, \"{i.name}\" }},");
                        }
                    }
                    sb.Line("};");
                    sb.Space();

                    sb.Line($"private static readonly Dictionary<string, MatIcon> nameToEnum = new()");
                    sb.Line("{");
                    using (sb.Indent())
                    {
                        foreach (var i in materialIconInfo)
                        {
                            sb.Line($"{{ \"{i.name}\", MatIcon.{EnumFriendlyName(i.name)} }},");
                        }
                    }
                    sb.Line("};");
                    sb.Space();

                    sb.Line($"private static readonly Dictionary<MatIcon, string> unicodes = new()");
                    sb.Line("{");
                    using (sb.Indent())
                    {
                        foreach (var i in materialIconInfo)
                        {
                            sb.Line($"{{ MatIcon.{EnumFriendlyName(i.name)}, \"{i.unicode}\" }},");
                        }
                    }
                    sb.Line("};");
                    sb.Space();

                    sb.Line("private MatIcon icon;");
                    sb.Space();

                    sb.Line($"[UxmlAttribute]");
                    sb.Line($"[CreateProperty]");
                    using (sb.Section("public string IconName"))
                    {
                        using (sb.Section("get"))
                        {
                            sb.Line($"return icon != MatIcon.None && enumToName.TryGetValue(icon, out var iconName) ? iconName : string.Empty;");
                        }

                        using (sb.Section("set"))
                        {
                            sb.Line($"Icon = (!string.IsNullOrEmpty(value) && nameToEnum.TryGetValue(value, out var enumValue)) ? enumValue : MatIcon.None;");
                        }
                    }

                    sb.Line($"[UxmlAttribute]");
                    sb.Line($"[CreateProperty]");
                    using (sb.Section("public MatIcon Icon"))
                    {
                        using (sb.Section("get"))
                        {
                            sb.Line($"return icon;");
                        }

                        using (sb.Section("set"))
                        {
                            sb.Line($"icon = value;");
                            sb.Line($"text = unicodes.TryGetValue(value, out string unicode) ? $\"\\\\u{{unicode}}\" : string.Empty;");
                        }
                    }

                    using (sb.Section($"public NuiIconMat() : base()"))
                    {
                        sb.Line("AddToClassList(\"nui-mat-icon\");");
                    }

                    using (sb.Section($"public NuiIconMat(string iconName) : this()"))
                    {
                        sb.Line("IconName = iconName;");
                    }

                    using (sb.Section($"public NuiIconMat(MatIcon icon) : this()"))
                    {
                        sb.Line("Icon = icon;");
                    }

                    using (sb.Section($"public override void OnReturnToPool()"))
                    {
                        sb.Line("base.OnReturnToPool();");
                        sb.Line("icon = MatIcon.None;");
                    }
                }

                using (sb.Section("public enum MatIcon"))
                {
                    sb.Line("None,");
                    sb.Line(string.Join($",{Environment.NewLine}        ", materialIconInfo.Select(x => EnumFriendlyName(x.name))));
                }
            }
        }

        public static string EnumFriendlyName(string text)
        {
            var output = new StringBuilder(text.Length);

            bool inWord = false;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (char.IsLetter(c))
                {
                    output.Append(inWord ? c : char.ToUpperInvariant(c));
                    inWord = true;
                }
                else if (char.IsDigit(c) && i == 0)
                {
                    output.Append("Icon").Append(c);
                    inWord = true;
                }
                else if (char.IsDigit(c))
                {
                    output.Append(c);
                }
                else
                {
                    inWord = false;
                }
            }

            return output.ToString();
        }
    }
}

#endif