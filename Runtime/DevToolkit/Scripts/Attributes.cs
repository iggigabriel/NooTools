using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: Noo.DevToolkit.DevAssembly]
namespace Noo.DevToolkit
{
    public enum DevMemberType
    {
        None = 0,
        Field = 1 << 0,
        Property = 1 << 1,
        Method = 1 << 2,
        All = Field | Property | Method
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DevAssemblyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DevCommandsAttribute : Attribute
    {
        /// <summary>Override the full path name, for example "Category/Subcategory/ItemName". Class name by default.</summary>
        public string PathName { get; private set; }

        /// <summary>Drawing order of the command.</summary>
        public int Order { get; set; }

        /// <summary>Only used by Method drawers to draw method inline.</summary>
        public bool Inline { get; set; } = true;

        /// <summary>Auto generate DevCommands from all types defined.</summary>
        public DevMemberType GenerateMemberTypes { get; set; } = DevMemberType.None;

        /// <summary>Auto generate DevCommands from all members with this flag set.</summary>
        public BindingFlags GenerateMemberFlags { get; set; } = BindingFlags.Public | BindingFlags.Static;

        /// <summary>Override the full path name, for example "Category/Subcategory/ItemName". Class name by default.</summary>
        public DevCommandsAttribute(string pathName = null)
        {
            PathName = pathName;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DevCommandAttribute : Attribute
    {
        /// <summary>Override the relative path name like "CustomGroup/ItemName" or an absolute path like "/CustomCategory/CustomGroup/ItemName". Member name by default.</summary>
        public string PathName { get; set; }

        /// <summary>Drawing order of the command.</summary>
        public int Order { get; set; }

        /// <summary>Additional info for this command displayed in the inspector.</summary>
        public string Info { get; set; }

        /// <summary>Only used by Method drawers to draw method inline.</summary>
        public bool Inline { get; set; } = true;

        /// <summary>Override the relative path name like "CustomGroup/ItemName" or an absolute path like "/CustomCategory/CustomGroup/ItemName". Member name by default.</summary>
        public DevCommandAttribute(string pathName = null, string info = null)
        {
            PathName = pathName;
            Info = info;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class DevHotkeyAttribute : Attribute
    {
        readonly Key[] keys = Array.Empty<Key>();
        readonly Key? mainKey;

        public bool Log { get; set; }

        public DevHotkeyAttribute(params Key[] keys)
        {
            this.keys = keys;
            if (keys.Length > 0) mainKey = keys[^1];
        }

        public bool IsPressed
        {
            get
            {
                if (Keyboard.current == null) return false;

                for (int i = 0; i < keys.Length - 1; i++)
                {
                    if (!Keyboard.current[keys[i]].isPressed) return false;
                }

                if (mainKey.HasValue && Keyboard.current[mainKey.Value].wasPressedThisFrame) return true;

                return false;
            }
        }

        public bool IsMemberTypeValid(MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo method && method.IsStatic) return true;

            return false;
        }

        public void Execute(MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo method && method.IsStatic)
            {
                var parameters = method.GetParameters().Select(x => default(object)).ToArray();
                method.Invoke(null, parameters);

                if (Log)
                {
                    Debug.Log($"[DevConsole] Executed: {method.DeclaringType.FullName}.{method.Name}()");
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class DrawerAttribute : Attribute
    {
        /// <summary>Use this to target parameters when put on method.</summary>
        public string Parameter { get; set; }
    }

    public class DevDropdownAttribute : DrawerAttribute
    {
        public string Items { get; private set; }

        public DevDropdownAttribute(string items)
        {
            Items = items;
        }
    }

    public class DevSliderAttribute : DrawerAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public DevSliderAttribute(int min, int max)
        {
            Min = ((min < max) ? min : max);
            Max = ((max > min) ? max : min);
        }

        public DevSliderAttribute(float min, float max)
        {
            Min = ((min < max) ? min : max);
            Max = ((max > min) ? max : min);
        }
    }
}
