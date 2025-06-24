using System;

namespace Noo.Nui
{
    public class NuiDropdownListButton : NuiDropdownList.Item
    {
        public string buttonText;
        NuiIconMat icon;
        NuiButton btn;

        protected override void OnCreate()
        {
            base.OnCreate();

            icon = NuiPool.Rent<NuiIconMat>().WithClass("nui-dropdown-list-button__checkmark").AppendTo(Root);
            icon.Icon = MatIcon.Check;

            btn = NuiPool.Rent<NuiButton>().WithClass("nui-dropdown-list-button").AppendTo(Root);
            btn.text = buttonText;
            btn.clicked += OnClicked;
        }

        private void OnClicked()
        {
            Selected = !Selected;
        }

        protected override void OnDestroy()
        {
            if (icon != null)
            {
                NuiPool.Return(icon.WithoutClass("nui-dropdown-list-button__checkmark"));
                icon = null;
            }

            if (btn != null)
            {
                btn.clicked -= OnClicked;
                NuiPool.Return(btn.WithoutClass("nui-dropdown-list-button"));
                btn = null;
            }

            base.OnDestroy();
        }

        public override bool Filter(string query)
        {
            return buttonText.Contains(query, StringComparison.InvariantCultureIgnoreCase);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();

            buttonText = default;
        }
    }
}
