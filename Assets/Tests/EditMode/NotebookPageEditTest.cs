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

public class NotebookPageEditTest
{
    private NotebookPage      page;
    private CharacterInstance character;
    private string            notes;
    
    [OneTimeSetUp]
    public void Setup()
    {
        CharacterData c = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/0_Fatima_Data.asset", typeof(CharacterData));
        character = new CharacterInstance(c);
        page = new NotebookPage(character);
        notes = "Notes on " + character.characterName + ".\n";
    }
    
    [UnityTest]
    public IEnumerator GetNotesTest()
    {
        Assert.AreEqual(notes, page.GetNotes());
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetNotesTest()
    {
        page.SetNotes("hello");
        
        Assert.AreEqual("hello", page.GetNotes());
        Assert.AreNotEqual(notes, page.GetNotes());

        yield return null;
    }

    [UnityTest]
    public IEnumerator QuestionTextTest()
    {
        Assert.AreEqual("\n", page.QuestionText());

        yield return null;
    }

    [UnityTest]
    public IEnumerator IntroTest()
    {
        var res = page.Intro();
        
        if (character.AskedQuestions.Count > 0)
            Assert.AreEqual("Your info on " + character.characterName + ".\n", res);
        else Assert.AreEqual("You have not asked " + character.characterName + "\nany questions.\n", res);
        
        yield return null;
    }
}
