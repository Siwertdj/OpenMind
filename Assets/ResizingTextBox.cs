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

    public void SetText(string text)
    {
        textComponent.text = text;
        AdjustFontSize();
    }

    public void AdjustFontSize() => textComponent.fontSize = SettingsManager.sm.GetFontSize();
}