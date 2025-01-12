using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GameSlider))]
public class GameSliderEditor : Editor
{
    #region Serialized Properties
    private SerializedProperty step;
    private SerializedProperty valueRounding;
    private SerializedProperty valueText;
    private SerializedProperty valueInfo;
    private SerializedProperty defaultValueEnabled;
    private SerializedProperty defaultValue;
    private SerializedProperty defaultValueRef;
    private SerializedProperty sliderComponentRef;

    private bool valueTextGroup = false;
    private bool defaultValueGroup = false;
    #endregion

    private void OnEnable()
    {
        step = serializedObject.FindProperty(nameof(step));
        valueText = serializedObject.FindProperty(nameof(valueText));
        valueInfo = serializedObject.FindProperty(nameof(valueInfo));
        valueRounding = serializedObject.FindProperty(nameof(valueRounding));
        defaultValue = serializedObject.FindProperty(nameof(defaultValue));
        defaultValueEnabled = serializedObject.FindProperty(nameof(defaultValueEnabled));
        defaultValueRef = serializedObject.FindProperty(nameof(defaultValueRef));
        sliderComponentRef = serializedObject.FindProperty(nameof(sliderComponentRef));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Add a custom field for the GameEvent
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Game Slider Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(step);
        EditorGUILayout.PropertyField(sliderComponentRef);

        valueTextGroup = EditorGUILayout.BeginFoldoutHeaderGroup(valueTextGroup, "Value Text Settings");
        if (valueTextGroup)
        {
            EditorGUILayout.PropertyField(valueText, new GUIContent("Text Object Ref"));
            EditorGUILayout.PropertyField(valueInfo, new GUIContent("Unit Text"));
            EditorGUILayout.IntSlider(valueRounding, 0, 5, new GUIContent("Decimal Rounding"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        defaultValueGroup = EditorGUILayout.BeginFoldoutHeaderGroup(defaultValueGroup, "Default Value Settings");
        if (defaultValueGroup)
        {
            EditorGUILayout.PropertyField(defaultValueEnabled, new GUIContent("Enable Default Value"));
            EditorGUILayout.PropertyField(defaultValueRef, new GUIContent("Default Value Ref"));
            EditorGUILayout.PropertyField(defaultValue, new GUIContent("Default Value Suggestion"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
