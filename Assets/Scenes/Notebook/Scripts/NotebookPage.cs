using System.Collections.Generic;
using UnityEngine;

public class NotebookPage
{
    private readonly CharacterInstance _character;
    private string _notes;
    //private Vector2 drawing;
    private List<Question> _answeredQuestions;

    public NotebookPage(CharacterInstance character)
    {
        this._character = character;
        _notes = "Notes on " + character.characterName + ".\n";
        //drawing = Vector2.zero;
        _answeredQuestions = new List<Question>();
    }

    public void AddQuestion(Question question)
    {
        _answeredQuestions.Add(question);
    }
    
    public string GetNotes()
    {
        return _notes;
    }

    public void SetNotes(string input)
    {
        this._notes = input;
    }
    
    public string QuestionText()
    {
        string output = "\n";

        foreach (Question q in _answeredQuestions)
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

    public string Intro()
    {
        if (_answeredQuestions.Count > 0)
        {
            return "Your info on " + _character.characterName + ".\n";
        }
        else
        {
            return "You have not asked " + _character.characterName + " any questions.\n";
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
