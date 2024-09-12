using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRecipient
{
    public Dictionary<QuestionType, List<string>> answers = new();

    public List<string> GetAnswer(QuestionType question)
    {
        return answers[question];
    }
}
