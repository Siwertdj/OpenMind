using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotebookTitleObject : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] private TMP_Text titleText;

    public void SetInfo(string title)
    {
        titleText.text = title;

        if (SettingsManager.sm != null)
            titleText.fontSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_LARGE_TEXT;
    }
}
