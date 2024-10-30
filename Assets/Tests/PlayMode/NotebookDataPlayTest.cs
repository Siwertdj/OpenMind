using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotebookDataPlayTest
{
    private GameManager       gm;
    private NotebookManager   nm;
    private NotebookData      data;
    private CharacterInstance character;
    
    #region Setup and Teardown
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        Button notebookButton = GameObject.Find("NotebookButton").GetComponent<Button>();
        notebookButton.onClick.Invoke();

        SceneManager.LoadScene("NotebookScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NotebookScene").isLoaded);
        
        nm = GameObject.Find("NotebookManager").GetComponent<NotebookManager>();
        data = nm.notebookData;
        character = gm.currentCharacters[0];
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("NotebookScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion

    /// <summary>
    /// Checks if the character notes get retrieved correctly
    /// </summary>
    [UnityTest]
    public IEnumerator GetCharacterNotesTest()
    {
        var notes = data.GetCharacterNotes(character);
        
        Assert.AreEqual("Notes on " + character.characterName + ".\n", notes);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the answers get retrieved correctly
    /// </summary>
    [UnityTest]
    public IEnumerator GetAnswersTest()
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

        yield return null;
    }

    /// <summary>
    /// Checks if the character notes get updated correctly
    /// </summary>
    [UnityTest]
    public IEnumerator UpdateCharacterNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdateCharacterNotes(character, newNotes);
        
        Assert.AreEqual(newNotes, data.GetCharacterNotes(character));

        yield return null;
    }

    /// <summary>
    /// Checks if the personal notes get updated correctly
    /// </summary>
    [UnityTest]
    public IEnumerator UpdatePersonalNotesTest()
    {
        string newNotes = "hello";
        
        data.UpdatePersonalNotes(newNotes);
        
        Assert.AreEqual(newNotes, data.GetPersonalNotes());

        yield return null;
    }

    /// <summary>
    /// Checks if the personal notes get retrieved correctly
    /// </summary>
    [UnityTest]
    public IEnumerator GetPersonalNotesTest()
    {
        var notes = data.GetPersonalNotes();
        
        Assert.AreEqual("Write down your thoughts.", data.GetPersonalNotes());

        yield return null;
    }
}