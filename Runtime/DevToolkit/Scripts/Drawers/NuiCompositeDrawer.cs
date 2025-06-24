using Noo.Nui;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public abstract class NuiCompositeDrawer : NuiDrawer
    {
        protected readonly List<NuiDrawer> children = new();
        public NuiDrawer this[int index] => children[index];
        public int ChildCount => children.Count;
        public virtual VisualElement ContentContainer => Root;
        public void AddDrawer(NuiDrawer child) { if (child != null) children.Add(child); }

        protected override void OnCreate()
        {
            Root.WithClass("dtk-composite-drawer");

            for (int i = 0; i < children.Count; i++)
            {
                var drawer = children[i];
                drawer.Create();
                ContentContainer.Add(drawer.Root);
            }
        }

        protected override void OnDestroy()
        {
            Root.WithoutClass("dtk-composite-drawer");

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Destroy();
            }
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Update();
            }
        }

        protected override bool OnFilter(string query)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].Filter(query)) return true;
            }

            return false;
        }
    }
}
