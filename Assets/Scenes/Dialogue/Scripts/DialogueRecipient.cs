using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueRecipient
{
    public Dictionary<Question, List<string>> Answers = new();

    public List<Question> RemainingQuestions = new();

    public string Name;

    public DialogueRecipient()
    {
        // Populate questions list with all question types
        foreach (Question questionType in Enum.GetValues(typeof(Question)))
            RemainingQuestions.Add(questionType);
    }

    public List<string> GetAnswer(Question question) => Answers[question];
}
