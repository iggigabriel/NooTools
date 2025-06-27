using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [UxmlElement]
    public partial class NuiText : TextElement, INuiPoolable
    {
        public NuiText()
        {
            ClearClassList();
            AddToClassList("nui-text");
            pickingMode = PickingMode.Ignore;
        }

        public NuiText(string text) : this()
        {
            this.text = text;
        }

        public virtual void OnRentFromPool()
        {
        }

        public virtual void OnReturnToPool()
        {
            pickingMode = PickingMode.Ignore;
            text = string.Empty;
        }
    }
}
