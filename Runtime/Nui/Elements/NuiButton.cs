using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using static Noo.Nui.NuiUtility;

namespace Noo.Nui
{
    [UxmlElement]
    public partial class NuiButton : Button, INuiPoolable
    {
        const string classBase = "nui-btn";
        const string classIconLeft = "icon-left";
        const string classIconRight = "icon-right";
        const string classText = "text";
        const string classBackground = "bg";
        const string classMultiline = "-multiline";
        const string classWith = "with";

        public readonly VisualElement background;
        NuiIconMat iconLeftElement;
        NuiIconMat iconRightElement;
        NuiText textElement;

        [UxmlAttribute]
        [CreateProperty]
        public bool Multiline
        {
            get
            {
                return ClassListContains(ClassCombine(classBase, classMultiline));
            }
            set
            {
                EnableInClassList(ClassCombine(classBase, classMultiline), value);
            }
        }

        private MatIcon iconLeft;
        private MatIcon iconRight;

        public override string text { get => ButtonText; set => ButtonText = value; }

        [UxmlAttribute]
        [CreateProperty]
        public MatIcon IconLeft
        {
            get => iconLeft;
            set
            {
                if (value == MatIcon.None)
                {
                    if (iconLeftElement != null)
                    {
                        iconLeftElement.RemoveFromClassList(ClassCombine(classBase, classIconLeft));
                        iconLeftElement.pickingMode = PickingMode.Position;
                        NuiPool.Return(iconLeftElement);
                        iconLeftElement = null;
                        RemoveFromClassList(ClassCombine(classBase, classWith, classIconLeft));
                    }
                }
                else
                {
                    if (iconLeftElement == null)
                    {
                        iconLeftElement = NuiPool.Rent<NuiIconMat>().WithClass(ClassCombine(classBase, classIconLeft));
                        iconLeftElement.pickingMode = PickingMode.Ignore;
                        AddToClassList(ClassCombine(classBase, classWith, classIconLeft));
                        background.Add(iconLeftElement);
                        iconLeftElement.SendToBack();
                    }
                    iconLeft = value;
                    iconLeftElement.Icon = value;
                }
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        public MatIcon IconRight
        {
            get => iconRight;
            set
            {
                if (value == MatIcon.None)
                {
                    if (iconRightElement != null)
                    {
                        iconRightElement.RemoveFromClassList(ClassCombine(classBase, classIconRight));
                        iconRightElement.pickingMode = PickingMode.Position;
                        NuiPool.Return(iconRightElement);
                        iconRightElement = null;
                        RemoveFromClassList(ClassCombine(classBase, classWith, classIconRight));
                    }
                }
                else
                {
                    if (iconRightElement == null)
                    {
                        iconRightElement = NuiPool.Rent<NuiIconMat>().WithClass(ClassCombine(classBase, classIconRight));
                        iconRightElement.pickingMode = PickingMode.Ignore;
                        AddToClassList(ClassCombine(classBase, classWith, classIconRight));
                        background.Add(iconRightElement);
                    }
                    iconRight = value;
                    iconRightElement.Icon = value;
                }
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        public string ButtonText
        {
            get => textElement?.text ?? "";
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (textElement != null)
                    {
                        textElement.RemoveFromClassList(ClassCombine(classBase, classText));
                        textElement.pickingMode = PickingMode.Position;
                        NuiPool.Return(textElement);
                        textElement = null;
                        RemoveFromClassList(ClassCombine(classBase, classWith, classText));
                    }
                }
                else
                {
                    if (textElement == null)
                    {
                        textElement = NuiPool.Rent<NuiText>().WithClass(ClassCombine(classBase, classText));
                        textElement.pickingMode = PickingMode.Ignore;
                        AddToClassList(ClassCombine(classBase, classWith, classText));
                        background.Add(textElement);
                        if (iconLeftElement != null) textElement.PlaceInFront(iconLeftElement);
                        else textElement.SendToBack();
                    }
                    textElement.text = value;
                }
            }
        }

        public NuiButton() 
        {
            ClearClassList();
            AddToClassList(classBase);

            background = new VisualElement().WithClass(ClassCombine(classBase, classBackground)).AppendToHierarchy(this);
            background.usageHints = UsageHints.DynamicColor;
        }

        public NuiButton(string text, MatIcon iconLeft = MatIcon.None, MatIcon iconRight = MatIcon.None) : this()
        {
            ButtonText = text;
            if (iconLeft != MatIcon.None) IconLeft = iconLeft;
            if (iconRight != MatIcon.None) IconRight = iconRight;
        }

        public NuiButton(MatIcon icon) : this(string.Empty, icon)
        {
        }

        public void OnRentFromPool()
        {
            
        }

        public void OnReturnToPool()
        {
            ButtonText = string.Empty;
            IconLeft = MatIcon.None;
            IconRight = MatIcon.None;
            Multiline = false;
            clickable = default;
        }
    }
}
