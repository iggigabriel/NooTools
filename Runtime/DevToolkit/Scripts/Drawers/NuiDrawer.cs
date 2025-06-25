using Noo.Nui;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public abstract class NuiDrawer : IComparable<NuiDrawer>
    {
        public int Order { get; set; }
        public virtual bool IsValid => true;

        public VisualElement Root { get; private set; }

        /// <summary>Will be used for default sorting and filtering of drawers</summary>
        protected virtual string TextContent => null;

        protected virtual void OnCreate() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnUpdate() { }

        protected virtual bool OnFilter(string query)
        {
            return TextContent == null || TextContent.Contains(query);
        }

        public bool OnFilter(IReadOnlyList<string> queries)
        {
            if (queries == null) return true;

            for (int i = 0; i < queries.Count; i++)
            {
                if (!OnFilter(queries[i])) return false;
            }

            return true;
        }

        internal void Create()
        {
            if (Root != null) throw new Exception($"Drawer ({GetType().Name}) is already created.");
            Root = NuiPool.Rent<VisualElement>().WithClass("dtk-drawer");
            OnCreate();
        }

        internal void Destroy()
        {
            if (Root == null) throw new Exception("Cannot destroy drawer that was never created.");
            OnDestroy();
            if (Root.childCount > 0) throw new Exception($"Drawer ({GetType().Name}) was not properly destroyed, {Root.childCount} child remaining.");
            NuiPool.Return(Root.WithoutClass("dtk-drawer"));
            Root = null;
        }

        internal void Update()
        {
            if (Root != null) OnUpdate();
        }

        internal bool Filter(string query)
        {
            return OnFilter(query);
        }

        public virtual int CompareTo(NuiDrawer other)
        {
            if (other == null) return 1;

            if (Order == other.Order && TextContent != null && other.TextContent != null)
            {
                return TextContent.CompareTo(other.TextContent);
            }

            return other == null ? 1 : Order.CompareTo(other.Order);
        }
    }
}
