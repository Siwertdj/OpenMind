using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class NotebookLogObject : MonoBehaviour
{
    [Header("Text Component Refs")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text answerText;

    public void SetText(string question, string answer)
    {
        questionText.text = question;
        answerText.text = answer;

        // Set appropriate font sizes
        if (SettingsManager.sm == null) return;
        questionText.fontSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_SMALL_TEXT;
        answerText.fontSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_SMALL_TEXT;
    }
}