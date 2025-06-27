using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Noo.Nui
{
    [UxmlElement]
    /// <summary>Copies width of parent and sets height automatically. Works best in columns.</summary>
    public partial class NuiAspectFitter : VisualElement
    {
        float aspectRatio;

        public override VisualElement contentContainer => innerContainer;

        readonly VisualElement innerContainer;

        [CreateProperty]
        [UxmlAttribute]
        public float AspectRatio
        {
            get => aspectRatio;
            set
            {
                aspectRatio = value;
                var padding = value < float.Epsilon ? 0f : (100f / value);
                style.paddingTop = new Length(padding, LengthUnit.Percent);
            }
        }

        public NuiAspectFitter(float aspectRatio) : this()
        {
            AspectRatio = aspectRatio;
        }

        public NuiAspectFitter() : base() 
        {
            AddToClassList("nui-aspect-fitter");
            innerContainer = new VisualElement().WithClass("nui-aspect-fitter__container").AppendToHierarchy(this);
            AspectRatio = 1f;
        }
    }
}
