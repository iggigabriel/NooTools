using Unity.Properties;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [UxmlElement]
    public partial class NuiRow : VisualElement
    {
        [UxmlAttribute]
        [CreateProperty]
        public bool Reverse
        {
            get => ClassListContains("nui--reverse");
            set => EnableInClassList("nui--reverse", value);
        }

        public NuiRow() : base()
        {
            AddToClassList("nui-row");
        }

        public NuiRow(bool reverse) : this()
        {
            Reverse = reverse;
        }
    }
}
