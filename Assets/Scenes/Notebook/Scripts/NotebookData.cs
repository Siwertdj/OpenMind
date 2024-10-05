using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class NotebookData
{
    private readonly Dictionary<CharacterInstance, NotebookPage> _pages = 
        new Dictionary<CharacterInstance, NotebookPage>();

    private string _personalNotes;

    public NotebookData()
    {
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
        {
            NotebookPage page = new NotebookPage(character);
            _pages[character] = page;
        }
        
        // Should load save, else
        _personalNotes = "This note is a text";
    }

    public void UpdateQuestions(CharacterInstance character, Question question)
    {
        _pages[character].AddQuestion(question);
    }

    public string GetPage(CharacterInstance character)
    {
        return _pages[character].GetNotes();
    }

    public string GetAnswers(CharacterInstance character)
    {
        return _pages[character].Intro() + _pages[character].QuestionText();
    }

    public void UpdateNotes(CharacterInstance character, string notes)
    {
        _pages[character].SetNotes(notes);
    }

    public void UpdatePersonalNotes(string input)
    {
        this._personalNotes = input;
    }
    
    public string GetPersonalNotes()
    {
        return _personalNotes;
    }
}
