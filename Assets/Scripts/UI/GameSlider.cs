using System.Collections;
using System.Collections.Concurrent;
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
    [SerializeField] private bool defaultValueEnabled;
    [SerializeField] private RectTransform defaultValueRef;
    [SerializeField] private Slider sliderComponentRef;
    [SerializeField] private float partitionSpace;
    [SerializeField] private GameObject partitionLinePrefab;

    public Slider slider { get { return sliderComponentRef; } }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateSlider);
        UpdateSlider(slider.value);

        // Set default value
        if (defaultValueEnabled)
        {
            // Set the default value sample to the correct position
            defaultValueRef.transform.localPosition = new Vector2(
                GetValuePositionOnSlider(defaultValue),
                defaultValueRef.transform.localPosition.y);
        }
        else
        {
            defaultValueRef.gameObject.SetActive(false);
        }

        // Add partition lines
        if (partitionSpace > 0)
        {
            var parent = (RectTransform)defaultValueRef.transform.parent;
            for (float v = slider.minValue; v < slider.maxValue; v += partitionSpace)
            {
                Debug.Log(v);
                // Partitions should not be placed outside of slider bounds
                if (v <= slider.minValue || v >= slider.maxValue)
                    continue;

                // Partitions should not be placed over the default value
                if (defaultValueEnabled && v == defaultValue)
                    continue;

                // Place partition line
                var line = Instantiate(partitionLinePrefab, parent);
                line.transform.localPosition = new Vector2(
                    GetValuePositionOnSlider(v), 
                    line.transform.localPosition.y);
            }
        }
    }

    private float GetValuePositionOnSlider(float value)
    {
        var fillRect = (RectTransform)defaultValueRef.transform.parent;
        float totalValue = slider.maxValue - slider.minValue;
        float f = value / totalValue - slider.minValue / totalValue; // The factor of the current value
        return fillRect.rect.width * f - fillRect.rect.width / 2; // The new local x position
    }

    public void UpdateSlider(float value)
    {
        // Adjust the value to match step precision
        float steppedValue = Mathf.Round(value / step) * step;

        // Update the slider's value (if you need forced snapping)
        slider.value = steppedValue;

        valueText.text = steppedValue.ToString($"F{valueRounding}") + valueInfo;
    }
}