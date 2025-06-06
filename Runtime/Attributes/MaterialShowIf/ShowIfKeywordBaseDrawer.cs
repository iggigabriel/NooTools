#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Usage: [ShowIfKeywordEnabled(_KEYWORD)]
/// Pass one or more shader keywords. The property is only drawn when the material has at least one of the 
/// specified keywords enabled. Can take up to 4 different keywords. 
/// </summary>
public abstract class ShowIfKeywordBaseDrawer : MaterialPropertyDrawer
{
    
    protected bool doesMaterialHaveArgumentKeyword;
    protected string[] keywordArguments;

    private GUIContent guiContent;
    private MethodInfo internalMethod;
    private Type[] methodArgumentTypes;
    private object[] methodArguments;

    public ShowIfKeywordBaseDrawer()
    {
        Debug.LogWarning($"{this.GetType()} in material was created without a keyword parameter, it will have no effect");
        Initiallize(new string[0]);
    }

    public ShowIfKeywordBaseDrawer(string keyword1)
    {
        Initiallize(keyword1);
    }

    public ShowIfKeywordBaseDrawer(string keyword1, string keyword2)
    {
        Initiallize(keyword1, keyword2);
    }

    public ShowIfKeywordBaseDrawer(string keyword1, string keyword2, string keyword3)
    {
        Initiallize(keyword1, keyword2, keyword3);
    }

    public ShowIfKeywordBaseDrawer(string keyword1, string keyword2, string keyword3, string keyword4)
    {
        Initiallize(keyword1, keyword2, keyword3, keyword4);
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        ValidateHasArgumentKeyword(editor);
        // editor.GetPropertyHeight(prop) crashes the inspector, maybe because it tries to invoke this method, 
        // resulting in an recursive method call error
        return ShouldBeDrawn() ? MaterialEditor.GetDefaultPropertyHeight(prop) : 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        ValidateHasArgumentKeyword(editor);
        if (ShouldBeDrawn())
        {
            // base.OnGUI(position, prop, label, editor); doesn't seem to work for drawing the default OnGUI of the property type
            if (internalMethod != null)
            {
                guiContent.text = label;
                methodArguments[0] = position;
                methodArguments[1] = prop;
                methodArguments[2] = guiContent;

                internalMethod.Invoke(editor, methodArguments);
            }
        }
    }

    protected abstract bool ShouldBeDrawn();

    private void Initiallize(params string[] keywords)
    {
        keywordArguments = keywords;

        guiContent = new GUIContent(string.Empty);

        methodArgumentTypes = new[] { typeof(Rect), typeof(MaterialProperty), typeof(GUIContent) };
        methodArguments = new object[3];

        internalMethod = typeof(MaterialEditor)
            .GetMethod("DefaultShaderPropertyInternal", BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            methodArgumentTypes,
            null);
    }

    protected void ValidateHasArgumentKeyword(MaterialEditor editor)
    {
        // Drawer is recreated when the material keywords change, so we have to update 
        // doesMaterialHaveArgumentKeyword on every GUI event
        doesMaterialHaveArgumentKeyword = HasRequiredKeyword(editor);
    }

    private bool HasRequiredKeyword(MaterialEditor editor)
    {
        if (keywordArguments.Length == 0)
            return true;

        Material targetMaterial = editor.target as Material;
        if (targetMaterial != null)
        {
            foreach (var keyword in keywordArguments)
            {
                if (targetMaterial.IsKeywordEnabled(keyword))
                    return true;
            }
        }
        return false;
    }
}
#endif