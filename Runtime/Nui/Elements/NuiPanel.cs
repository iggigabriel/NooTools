using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    public partial class NuiPanel : VisualElement
    {
        public NuiPanel() : base() 
        {
            AddToClassList("nui-panel");
            pickingMode = PickingMode.Ignore;
        }
    }
}
