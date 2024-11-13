using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ResizingTextBox : MonoBehaviour
{
    [Header("Text")]
    [TextArea(2, 10)]
    public string textContent;
    public int textSize;

    [Header("Box width")]
    public float minWidth;
    public float maxWidth;

    [Header("Box sprite")]
    public Sprite sprite;

    [Header("Component references")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Image image;

    public void ApplyChanges()
    {
        textComponent.text = textContent;
        textComponent.fontSize = textSize;

        layoutElement.minWidth = minWidth;
        rectTransform.sizeDelta = new Vector2(maxWidth, rectTransform.sizeDelta.y);

        image.sprite = sprite;        
    }
}

[CustomEditor(typeof(ResizingTextBox))]
public class ResizingTextBoxEditor : Editor
{
    private ResizingTextBox component;

    public override void OnInspectorGUI()
    {
        component = (ResizingTextBox)target;

        // Draw the default inspector properties
        DrawDefaultInspector();

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            ExecuteButton();
        if (GUILayout.Button("Apply Changes"))
            ExecuteButton();
    }

    private void ExecuteButton()
    {
        component.ApplyChanges();

        SceneView.RepaintAll();
    }
}
