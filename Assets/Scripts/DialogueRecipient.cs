using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueRecipient
{
    public Dictionary<QuestionType, List<string>> answers = new();

    public List<QuestionType> remainingQuestions = new();

    public DialogueRecipient()
    {
        // Populate questions list with all question types
        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
            remainingQuestions.Add(questionType);
    }

    public List<string> GetAnswer(QuestionType question) => answers[question];
}
