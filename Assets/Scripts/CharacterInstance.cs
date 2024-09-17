using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance
{
    private CharacterData data;

    public Dictionary<Question, List<string>> Answers = new();
    public List<Question> RemainingQuestions = new();

    public CharacterInstance(CharacterData data)
    {
        this.data = data;

        Debug.Log($"Creating character {data.characterName}");

        foreach (var kvp in data.answers)
        {
            Answers[kvp.question] = kvp.answer;
            RemainingQuestions.Add(kvp.question);
        }
    }
}
