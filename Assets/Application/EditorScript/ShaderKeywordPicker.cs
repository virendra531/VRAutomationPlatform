using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ShaderKeywordPicker : PropertyAttribute {
    public enum KeywordType {
        All,
        Color,
        TexEnv,
        Range,
        Float,
        Vector
    }
    public string rendererName;
    public string materialIndexName;
    public string keywordType;
    public ShaderKeywordPicker() {
        this.materialIndexName = null;
        this.keywordType = KeywordType.All.ToString();
    }
    public ShaderKeywordPicker(KeywordType keywordType = KeywordType.All) {
        this.materialIndexName = null;
        this.keywordType = keywordType.ToString();
    }
    public ShaderKeywordPicker(string materialIndexName = null, KeywordType keywordType = KeywordType.All) {
        this.materialIndexName = materialIndexName;
        this.keywordType = keywordType.ToString();
    }
    public ShaderKeywordPicker(string rendererName = null, string materialIndexName = null, KeywordType keywordType = KeywordType.All) {
        this.rendererName = rendererName;
        this.materialIndexName = materialIndexName;
        this.keywordType = keywordType.ToString();
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShaderKeywordPicker))]
public class ShaderKeywordPickerPropertyDrawer : PropertyDrawer {
    private int selectedIndex;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // property.serializedObject.Update();

        ShaderKeywordPicker picker = (ShaderKeywordPicker)attribute;

        string[] validKeywords = GetValidKeyword(property, picker.rendererName, picker.materialIndexName, picker.keywordType);

        // set index of popup list to current position
        string currentKeyword = property.stringValue;
        selectedIndex = (currentKeyword == "") ? 0 : validKeywords.ToList().IndexOf(currentKeyword);

        if (selectedIndex < 0) selectedIndex = 0;

        // Create Popup list
        selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, validKeywords);
        property.stringValue = validKeywords[selectedIndex];

        // property.serializedObject.ApplyModifiedProperties();
    }

    private string[] GetValidKeyword(SerializedProperty property, string rendererName = null, string materialIndexName = null, string keywordType = null) {
        List<string> Keywords = new List<string>();

        Component component = (Component)property.serializedObject.targetObject;

        // Select Material Using Index
        int materialIndex = 0;
        if (materialIndexName == null) {
            materialIndex = 0;
        } else {
            materialIndex = property.serializedObject.FindProperty(materialIndexName).intValue;
        }
        Material material = null;
        if (rendererName == null)
        {
            material = component.GetComponent<Renderer>().sharedMaterials[materialIndex];
        }else{
            Renderer renderer = (Renderer) property.serializedObject.FindProperty(rendererName).objectReferenceValue;
            if(renderer != null) material = renderer.sharedMaterials[materialIndex];
        }

        try {
            // Select Material Keyword using Type
            for (var i = 0; i < ShaderUtil.GetPropertyCount(material.shader); i++) {
                // Debug.Log($"{ShaderUtil.GetPropertyName(material.shader, i)} => {ShaderUtil.GetPropertyType(material.shader, i).ToString()}");
                // Debug.Log($"{ShaderUtil.GetPropertyType(material.shader, i).ToString()}");

                if (keywordType == null || keywordType.Contains(ShaderKeywordPicker.KeywordType.All.ToString())) {
                    Keywords.Add(ShaderUtil.GetPropertyName(material.shader, i));
                    continue;
                }
                if (ShaderUtil.GetPropertyType(material.shader, i).ToString().Contains(keywordType)) {
                    Keywords.Add(ShaderUtil.GetPropertyName(material.shader, i));
                }
            }
            for (var i = 0; i < material.shaderKeywords.Length; i++) {
                if (keywordType == null || keywordType.Contains(ShaderKeywordPicker.KeywordType.All.ToString())) {
                    Keywords.Add(material.shaderKeywords[i]);
                    continue;
                }
                if (ShaderUtil.GetPropertyType(material.shader, i).ToString().Contains(keywordType)) {
                    Keywords.Add(material.shaderKeywords[i]);
                }
            }
        } catch (System.Exception ex) {
            // Debug.LogError(ex.Message);
        }


        if (Keywords.Count == 0) {
            Keywords.Add("No Keyword Found !!!");
            Debug.LogError("No Keyword Found !!!");
        }
        return Keywords.ToArray();
    }
}
#endif
