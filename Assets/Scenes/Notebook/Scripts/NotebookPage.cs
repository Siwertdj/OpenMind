using System.Collections.Generic;
using UnityEngine;

public class NotebookPage : MonoBehaviour
{
    private CharacterInstance character;
    private string notes;
    private Vector2 drawing;
    public List<Question> AnsweredQuestions = new();

    public NotebookPage(CharacterInstance character)
    {
        this.character = character;
        notes = "hi, i am " + character.characterName;
        drawing = Vector2.zero;
        AnsweredQuestions = new List<Question>();
    }
    
    public string GetNotes()
    {
        return notes;
    }

    public void SetNotes(string notes)
    {
        this.notes = notes;
    }
    
    public string QuestionText()
    {
        string output = "\n";

        foreach (Question q in AnsweredQuestions)
        {
            output += GetQuestionText(q).ToUpper() + "\n";
            foreach (string s in character.Answers[q])
            {
                output += s + " ";
            }
            output += "\n \n";
        }
        return output;
    }

    public string Intro()
    {
        if (AnsweredQuestions.Count > 0)
        {
            return "Your info on " + character.characterName + " .\n";
        }
        else
        {
            return "You have not asked " + character.characterName + " any questions.\n";
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
