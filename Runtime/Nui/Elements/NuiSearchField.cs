using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [UxmlElement]
    public partial class NuiSearchField : TextField
    {
        readonly NuiButton clearButton;
        readonly NuiIconMat searchIcon;

        public NuiSearchField() : base()
        {
            AddToClassList("nui-search-field");

            searchIcon = new NuiIconMat(MatIcon.Search).WithClass("nui-search-field__icon").AppendToHierarchy(this);
            searchIcon.pickingMode = PickingMode.Ignore;
            searchIcon.SendToBack();

            clearButton = new NuiButton(MatIcon.Close).WithClass("nui-search-field-clear__btn", "nui-btn-circle").AppendToHierarchy(this);
            clearButton.clicked += () => { value = ""; };
            clearButton.tooltip = "Clear search";
            clearButton.tabIndex = -1;
            UpdateClearButtonVisibility(null);

            new VisualElement().WithClass("nui-search-field__background").PrependToToHierarchy(this);

            this.RegisterValueChangedCallback(UpdateClearButtonVisibility);
        }

        void UpdateClearButtonVisibility(ChangeEvent<string> evt)
        {
            clearButton.style.visibility = evt == null || string.IsNullOrEmpty(evt.newValue) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
