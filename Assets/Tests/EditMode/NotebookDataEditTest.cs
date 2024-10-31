using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class NotebookPageDataTest
{
    private NotebookData      data;
    private CharacterInstance character;
    
    [OneTimeSetUp]
    public void Setup()
    {
        // Get some random character to make the page for
        CharacterData c = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/0_Fatima_Data.asset", typeof(CharacterData));
        
        character = new CharacterInstance(c);

        // Load "Loading scene" and find GameManager to set it up
        EditorSceneManager.OpenScene("Assets/Scenes/Loading/Loading.unity");
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.currentCharacters = new List<CharacterInstance>();
        gm.currentCharacters.Add(character);
        GameManager.gm = gm;
        
        data = new NotebookData();
    }

    /// <summary>
    /// Checks if the character notes get retrieved correctly
    /// </summary>
    [Test]
    public void GetCharacterNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdateCharacterNotes(character, newNotes);
        
        var notes = data.GetCharacterNotes(character);
        
        Assert.AreEqual(newNotes, notes);
    }
    
    /// <summary>
    /// Checks if the answers get retrieved correctly
    /// </summary>
    [Test]
    public void GetAnswersTest()
    {
        var actual = data.GetAnswers(character);

        var expected = "";
        
        if (character.AskedQuestions.Count > 0)
        {
            expected += "Your info on " + character.characterName + ".\n";
        }
        else
        {
            expected += "You have not asked " + character.characterName + "\nany questions.\n";
        }

        expected += "\n";

        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// Checks if the character notes get updated correctly
    /// </summary>
    [Test]
    public void UpdateCharacterNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdateCharacterNotes(character, newNotes);
        
        Assert.AreEqual(newNotes, data.GetCharacterNotes(character));
    }

    /// <summary>
    /// Checks if the personal notes get updated correctly
    /// </summary>
    [Test]
    public void UpdatePersonalNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdatePersonalNotes(newNotes);
        
        Assert.AreEqual(newNotes, data.GetPersonalNotes());
    }

    /// <summary>
    /// Checks if the personal notes get retrieved correctly
    /// </summary>
    [Test]
    public void GetPersonalNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdatePersonalNotes(newNotes);
        
        var notes = data.GetPersonalNotes();
        
        Assert.AreEqual(newNotes, data.GetPersonalNotes());
    }
}
