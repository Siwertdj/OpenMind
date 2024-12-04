using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Slider sliderComponentRef;

    public Slider slider { get { return sliderComponentRef; } }

    private void Awake()
    {
        //slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(UpdateSlider);
        UpdateSlider(slider.value);

        // Set the default value sample to the correct position
        var rectTransform = (RectTransform)defaultValueRef.transform.parent;
        Debug.Log($"{rectTransform.gameObject.name} is {rectTransform.rect.width}");
        defaultValueRef.localPosition = new Vector2(
            -rectTransform.rect.width * (defaultValue / (slider.maxValue + slider.minValue)),
            defaultValueRef.localPosition.y);
    }

    private void UpdateSlider(float value)
    {
        // Adjust the value to match step precision
        float steppedValue = Mathf.Round(value / step) * step;

        // Update the slider's value (if you need forced snapping)
        slider.value = steppedValue;

        var rectTransform = (RectTransform)defaultValueRef.transform.parent;

        valueText.text = steppedValue.ToString($"F{valueRounding}") + valueInfo;
    }
}