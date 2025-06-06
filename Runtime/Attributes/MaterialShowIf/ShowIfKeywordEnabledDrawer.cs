#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Usage: [ShowIfKeywordEnabled(_KEYWORD)] or [ShowIfKeywordEnabled(_KEYWORD1, _KEYWORD2)]
/// Pass one or more shader keywords. The property is only drawn when the material has at least one of the 
/// specified keywords enabled. Can take up to 4 different keywords. 
/// </summary>
public class ShowIfKeywordEnabledDrawer : ShowIfKeywordBaseDrawer
{
    public ShowIfKeywordEnabledDrawer() : base() 
    { 
    }

    public ShowIfKeywordEnabledDrawer(string keyword1) : base(keyword1)
    {
    }

    public ShowIfKeywordEnabledDrawer(string keyword1, string keyword2) : base(keyword1, keyword2)
    {
    }

    public ShowIfKeywordEnabledDrawer(string keyword1, string keyword2, string keyword3) : base(keyword1, keyword2, keyword3)
    {
    }

    public ShowIfKeywordEnabledDrawer(string keyword1, string keyword2, string keyword3, string keyword4) : base(keyword1, keyword2, keyword3, keyword4)
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

    protected override bool ShouldBeDrawn()
    {
        return doesMaterialHaveArgumentKeyword;
    }
}
#endif