using Noo.Nui;

namespace Noo.DevToolkit
{
    public abstract partial class DtkPage : NuiCol
    {
        public abstract string Title { get; }
        public abstract MatIcon Icon { get; }

        public DtkPage() : base()
        {
            AddToClassList("dtk-toolkit-page");
            AddToClassList("flex-grow");
        }

        internal virtual void OnEnable() { }
        internal virtual void OnDisable() { }
    }
}
