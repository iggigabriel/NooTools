using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    [InlineProperty]
    public abstract class LinkedValue<T, TSource> where TSource : LinkedValueSource<T>
    {
        public T Value
        {
            get
            {
                if (isLinked) return valueSource ? valueSource.Value : default;
                else return constantValue;
            }
            set
            {
                if (isLinked)
                {
                    if (valueSource) valueSource.Value = value;
                }
                else
                {
                    constantValue = value;
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool isLinked;

        [SerializeField, HideIf(nameof(isLinked)), HorizontalGroup, HideLabel]
#if UNITY_EDITOR
        [InlineButton(nameof(ToggleLinkedState), SdfIconType.Link45deg, Label = "")]
#endif
        private T constantValue;

        [SerializeField, ShowIf(nameof(isLinked)), HorizontalGroup, HideLabel, InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout, Expanded = true)]
#if UNITY_EDITOR
        [InlineButton(nameof(ToggleLinkedState), SdfIconType.Link45deg, Label = "")]
        [InlineButton(nameof(CreateValueAsset), SdfIconType.Plus, Label = "")]
#endif
        private TSource valueSource;

#if UNITY_EDITOR
        private void ToggleLinkedState() => isLinked = !isLinked;
        private void CreateValueAsset()
        {
            valueSource = ScriptableObjectUtility.CreateAssetWithFileDialog<TSource>(false);
            GUIUtility.ExitGUI();
        }
#endif
    }
}