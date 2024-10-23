using System.Collections.Generic;
using UnityEngine;

public class NotebookPage
{
    private readonly CharacterInstance _character;
    private string _notes;

    public NotebookPage(CharacterInstance character)
    {
        _character = character;
        _notes = "Notes on " + character.characterName + ".\n";
    }

    public NotebookPage(string notes, CharacterInstance character)
    {
        _character = character;
        _notes = notes;
    }
    
    public string GetNotes()
    {
        return _notes;
    }

    public void SetNotes(string input)
    {
        _notes = input;
    }
    
    /// <summary>
    /// Add the title and question text together for each questions that has been asked.
    /// </summary>
    public string QuestionText()
    {
        string output = "\n";

        foreach (Question q in _character.AskedQuestions)
        {
            output += GetQuestionText(q).ToUpper() + "\n";
            foreach (string s in _character.Answers[q])
            {
                output += s + " ";
            }
            output += "\n \n";
        }
        return output;
    }
    
    /// <summary>
    /// Starting text for the character log.
    /// </summary>
    public string Intro()
    {
        if (_character.AskedQuestions.Count > 0)
        {
            return "Your info on " + _character.characterName + ".\n";
        }
        else
        {
            return "You have not asked " + _character.characterName + "\nany questions.\n";
        }
    }
    
    private string GetQuestionText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "Name",
            Question.Age => "Age",
            Question.Wellbeing => "Wellbeing",
            Question.Political => "Political ideology",
            Question.Personality => "Personality",
            Question.Hobby => "Hobbies",
            Question.CulturalBackground => "Cultural background",
            Question.Education => "Education level",
            Question.CoreValues => "Core values",
            Question.ImportantPeople => "Most important people",
            Question.PositiveTrait => "Positive trait",
            Question.NegativeTrait => "Bad trait",
            Question.OddTrait => "Odd trait",
            _ => "",
        };
    }
}
