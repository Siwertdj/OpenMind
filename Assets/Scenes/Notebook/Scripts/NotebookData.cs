using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class NotebookData
{
    private Dictionary<CharacterInstance, NotebookPage> _pages = 
        new Dictionary<CharacterInstance, NotebookPage>();

    private string _personalNotes;

    public NotebookData()
    {
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
        {
            NotebookPage page = new NotebookPage(character);
            _pages[character] = page;
        }
        
        _personalNotes = "Write down your thoughts.";
    }

    public string GetCharacterNotes(CharacterInstance character)
    {
        return _pages[character].GetNotes();
    }

    public string GetAnswers(CharacterInstance character)
    {
        return _pages[character].Intro() + _pages[character].QuestionText();
    }

    public void UpdateCharacterNotes(CharacterInstance character, string notes)
    {
        _pages[character].SetNotes(notes);
    }

    public void UpdatePersonalNotes(string input)
    {
        _personalNotes = input;
    }
    
    public string GetPersonalNotes()
    {
        return _personalNotes;
    }
}
