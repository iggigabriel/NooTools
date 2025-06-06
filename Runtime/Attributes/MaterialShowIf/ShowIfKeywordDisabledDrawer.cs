#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Usage: [ShowIfKeywordDisabled(_KEYWORD)] or [ShowIfKeywordDisabled(_KEYWORD1, _KEYWORD2)]
/// Pass one or more shader keywords. The property is only drawn when the material has none of the
/// specified keywords enabled (all keywords have to be disabled). Can take up to 4 different keywords. 
/// </summary>
public class ShowIfKeywordDisabledDrawer : ShowIfKeywordBaseDrawer
{
    public ShowIfKeywordDisabledDrawer() : base()
    {
    }

    public ShowIfKeywordDisabledDrawer(string keyword1) : base(keyword1)
    {
    }

    public ShowIfKeywordDisabledDrawer(string keyword1, string keyword2) : base(keyword1, keyword2)
    {
    }

    public ShowIfKeywordDisabledDrawer(string keyword1, string keyword2, string keyword3) : base(keyword1, keyword2, keyword3)
    {
    }

    public ShowIfKeywordDisabledDrawer(string keyword1, string keyword2, string keyword3, string keyword4) : base(keyword1, keyword2, keyword3, keyword4)
    {
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return base.GetPropertyHeight(prop, label, editor);
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        base.OnGUI(position, prop, label, editor);
    }

    // Also draw if the user has accidentally no arguments passed
    protected override bool ShouldBeDrawn()
    {
        return keywordArguments.Length == 0 || !doesMaterialHaveArgumentKeyword;
    }
}
#endif