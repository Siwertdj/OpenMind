using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    }

    public float GetObjectHeight()
    {
        return 0f;
    }
}
