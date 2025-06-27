using Unity.Properties;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [UxmlElement]
    public partial class NuiCol : VisualElement
    {
        [UxmlAttribute]
        [CreateProperty]
        public bool Reverse
        {
            get => ClassListContains("nui--reverse");
            set => EnableInClassList("nui--reverse", value);
        }

        public NuiCol() : base()
        {
            AddToClassList("nui-col");
        }

        public NuiCol(bool reverse) : this()
        {
            Reverse = reverse;
        }
    }
}
