using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float step;
    [SerializeField] private float defaultValue;
    [SerializeField] private string valueInfo;
    [SerializeField] [Range(0,5)] private int valueRounding;

    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text valueText;

    // Start is called before the first frame update
    void Awake()
    {
        slider.onValueChanged.AddListener(UpdateSlider);
        UpdateSlider(slider.value);
    }

    private void UpdateSlider(float value)
    {
        // Adjust the value to match step precision
        float steppedValue = Mathf.Round(value / step) * step;

        // Update the slider's value (if you need forced snapping)
        slider.value = steppedValue;

        valueText.text = steppedValue.ToString($"F{valueRounding}") + valueInfo;
    }
}
