using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NotebookData
{
    private Dictionary<CharacterInstance, NotebookPage> _pages = 
        new Dictionary<CharacterInstance, NotebookPage>();

    private string _personalNotes;

    public NotebookData()
    {
        // Empty all pages when we create new notebookdata
        _pages = new Dictionary<CharacterInstance, NotebookPage>();
        
        // TODO: create a method that lets us fill it up based on the characters, instead of hiding it in the constructor
        // TODO: Or create checks for this, in case there are no characters yet
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
        {
            NotebookPage page = new NotebookPage(character);
            _pages[character] = page;
        }
        
        _personalNotes = "Write down your thoughts.";
    }

    /// <summary>
    /// Get the notes the player has written about a character.
    /// </summary>
    public string GetCharacterNotes(CharacterInstance character)
    {
        return _pages[character].GetNotes();
    }
    
    /// <summary>
    /// Get the answers the player has obtained from a character.
    /// </summary>
    public string GetAnswers(CharacterInstance character)
    {
        return _pages[character].Intro() + _pages[character].QuestionText();
    }
    
    /// <summary>
    /// Save the text that the player has written about a character to the notebookpage.
    /// </summary>
    public void UpdateCharacterNotes(CharacterInstance character, string notes)
    {
        _pages[character].SetNotes(notes);
    }
    
    /// <summary>
    /// Write the player's personal notes to the notebookdata.
    /// </summary>
    public void UpdatePersonalNotes(string input)
    {
        _personalNotes = input;
    }
    
    /// <summary>
    /// Get the player's written personal notes from the notebookdata.
    /// </summary>
    public string GetPersonalNotes()
    {
        return _personalNotes;
    }
}
