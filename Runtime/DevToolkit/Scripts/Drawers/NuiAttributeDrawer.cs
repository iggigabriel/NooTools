namespace Noo.DevToolkit
{
    public abstract class NuiAttributeDrawer : NuiPropertyDrawer
    {
        public DrawerAttribute DrawerAttribute { get; internal set; }
    }

    public abstract class NuiAttributeDrawer<T> : NuiAttributeDrawer where T : DrawerAttribute
    {
        public new T DrawerAttribute => base.DrawerAttribute as T;
    }
}
