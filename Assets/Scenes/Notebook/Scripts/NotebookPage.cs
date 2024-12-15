// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single page in the notebook.
/// </summary>
public class NotebookPage
{
    private readonly CharacterInstance _character;
    private string _notes;

    /// <summary>
    /// Constructor for making a new empty page.
    /// </summary>
    /// <param name="character">The character the page is on.</param>
    public NotebookPage(CharacterInstance character)
    {
        _character = character;
        _notes = "Notes on " + character.characterName + ".\n";
    }

    /// <summary>
    /// Constructor for making a page that already has writing.
    /// </summary>
    /// <param name="notes">The notes that have already been written.</param>
    /// <param name="character">The character the page is on.</param>
    public NotebookPage(string notes, CharacterInstance character)
    {
        _character = character;
        _notes = notes;
    }
    
    /// <summary>
    /// Method which gets the notes contained on this page.
    /// For external use.
    /// </summary>
    /// <returns>The notes written on this page.</returns>
    public string GetNotes()
    {
        return _notes;
    }

    /// <summary>
    /// Method which sets the notes on this page to the input.
    /// For external use.
    /// </summary>
    /// <param name="input">New set of notes.</param>
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
            foreach (string s in _character.Answers[q].GetStrings())
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
    
    /// <summary>
    /// Method which gives a string based on the type of question that was asked.
    /// To use as a prefix to the answer.
    /// </summary>
    /// <param name="questionType">The type of question.</param>
    /// <returns>String relating to the questiontype.</returns>
    private string GetQuestionText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "Name",
            Question.Age => "Age",
            Question.LifeGeneral => "Life",
            Question.Inspiration => "Inspiration",
            Question.Sexuality => "Sexuality",
            Question.Wellbeing => "Wellbeing",
            Question.Political => "Political ideology",
            Question.Personality => "Personality",
            Question.Hobby => "Hobbies",
            Question.CulturalBackground => "Cultural background",
            Question.Religion => "Religion",
            Question.Education => "Education level",
            Question.CoreValues => "Core values",
            Question.ImportantPeople => "Most important people",
            Question.PositiveTrait => "Positive trait",
            Question.NegativeTrait => "Bad trait",
            Question.OddTrait => "Odd trait",
            Question.SocialIssues => "Social Issues",
            Question.EducationSystem => "Dutch education system",
            Question.Lottery => "Lottery",
            Question.Diet => "Diet",
            _ => "",
        };
    }
}
