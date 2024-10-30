using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class NotebookPagePlayTest
{
    private GameManager       gm;
    private NotebookPage      page;
    private CharacterInstance character;
    private string            notes;
    
    #region Setup and Teardown
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);
        
        character = gm.currentCharacters[0];
        notes = "test";
        page = new NotebookPage(notes, character);
        
        yield return null;
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion
    
    /// <summary>
    /// Tessts if the GetNotes method returns the correct notes
    /// </summary>
    [UnityTest]
    public IEnumerator GetNotesTest()
    {
        Assert.AreEqual(notes, page.GetNotes());
        
        yield return null;
    }

    /// <summary>
    /// Checks if the SetNotes method correctly adjusts the notes
    /// </summary>
    [UnityTest]
    public IEnumerator SetNotesTest()
    {
        page.SetNotes("hello");
        
        Assert.AreEqual("hello", page.GetNotes());
        Assert.AreNotEqual(notes, page.GetNotes());

        yield return null;
    }

    /// <summary>
    /// Checks if the QuestionText method correctly returns the questions.
    /// </summary>
    // TODO: write test for when questions have been asked?
    [UnityTest]
    public IEnumerator QuestionTextTest()
    {
        Assert.AreEqual("\n", page.QuestionText());

        yield return null;
    }

    /// <summary>
    /// Checks if the IntroTest method works correctly and returns the correct string.
    /// </summary>
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