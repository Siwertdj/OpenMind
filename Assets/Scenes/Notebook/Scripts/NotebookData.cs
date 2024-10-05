using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class NotebookData : MonoBehaviour
{
    private Dictionary<CharacterInstance, NotebookPage> _pages = 
        new Dictionary<CharacterInstance, NotebookPage>();

    public NotebookData(List<CharacterInstance> characters)
    {
        foreach (CharacterInstance character in characters)
        {
            NotebookPage page = new NotebookPage(character);
            _pages[character] = page;
        }
    }

    public void UpdateQuestions(CharacterInstance character, Question question)
    {
        _pages[character].AnsweredQuestions.Add(question);
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
}
