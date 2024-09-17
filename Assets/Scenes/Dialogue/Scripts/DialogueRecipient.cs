using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueRecipient
{
    public Dictionary<QuestionType, List<string>> Answers = new();

    public List<QuestionType> RemainingQuestions = new();

    public string Name;

    public DialogueRecipient()
    {
        // Populate questions list with all question types
        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
            RemainingQuestions.Add(questionType);
    }

    public List<string> GetAnswer(QuestionType question) => Answers[question];
}
