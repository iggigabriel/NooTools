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

        protected virtual void OnCreate() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnUpdate() { }
        protected virtual bool OnFilter(string query) { return true; }

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

        public int CompareTo(NuiDrawer other) => other == null ? 1 : Order.CompareTo(other.Order);
    }
}
