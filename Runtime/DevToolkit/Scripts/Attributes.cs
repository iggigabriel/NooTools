using System;
using System.Reflection;

[assembly: Noo.DevToolkit.DevAssembly]
namespace Noo.DevToolkit
{
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
        public bool Inline { get; set; } = false;

        /// <summary>Auto generate DevCommands from all members matching these binding flags.</summary>
        public BindingFlags GenerateMemberFlags { get; set; }

        /// <summary>Auto generate DevCommands from all members of this type.</summary>
        public MemberTypes GenerateMemberTypes { get; set; } = MemberTypes.All;

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
        public bool Inline { get; set; } = false;

        /// <summary>Override the relative path name like "CustomGroup/ItemName" or an absolute path like "/CustomCategory/CustomGroup/ItemName". Member name by default.</summary>
        public DevCommandAttribute(string pathName = null, string info = null)
        {
            PathName = pathName;
            Info = info;
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
