using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

public class GameSlider : MonoBehaviour
{
    [SerializeField] private float step = 1;
    [SerializeField] private string valueInfo;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private int valueRounding;
    [SerializeField] private float defaultValue;
    [SerializeField] private RectTransform defaultValueRef;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(UpdateSlider);
        UpdateSlider(slider.value);
        Debug.Log($"Default: {defaultValue}, Max: {slider.maxValue}");

        StartCoroutine(SetDefaultValuePos());
    }

    private IEnumerator SetDefaultValuePos()
    {
        yield return new WaitForEndOfFrame();

        var rectTransform = (RectTransform)defaultValueRef.transform.parent;
        Debug.Log($"{rectTransform.gameObject.name} is {rectTransform.rect.width}");
        defaultValueRef.localPosition = new Vector2(
            defaultValue / (slider.maxValue + slider.minValue) * rectTransform.rect.width - rectTransform.rect.width * rectTransform.pivot.x,
            defaultValueRef.localPosition.y);
    }

    private void UpdateSlider(float value)
    {
        // Adjust the value to match step precision
        float steppedValue = Mathf.Round(value / step) * step;

        // Update the slider's value (if you need forced snapping)
        slider.value = steppedValue;

        var rectTransform = (RectTransform)defaultValueRef.transform.parent;
        Debug.Log(rectTransform.gameObject.name + " " + rectTransform.rect.width);

        valueText.text = steppedValue.ToString($"F{valueRounding}") + valueInfo;
    }
}