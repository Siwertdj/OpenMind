using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NotebookManagerPlayTest : MonoBehaviour
{
    private GameManager     gm;
    private NotebookManager nm;
    
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
    }
    
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("NotebookScene"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    #endregion

    /// <summary>
    /// Checks if the notebook gets setupped correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator StartNotebookTest()
    {
        // Check if certain properties hold
        Assert.IsFalse(nm.characterInfo.activeSelf);
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        Assert.IsTrue(nm.inputField.activeSelf);
        Assert.AreEqual(nm.notebookData, gm.notebookData);
        Assert.IsFalse(nm.personalButton.interactable);
        
        yield return null;
    }

    /// <summary>
    /// Checks if all buttons get correctly initialized.
    /// </summary>
    [UnityTest]
    public IEnumerator InitializeCharacterButtonsTest()
    {
        nm.InitializeCharacterButtons();
        
        // Get all buttons
        var buttons = nm.nameButtons.GetComponentsInChildren<Button>().ToList();

        // Check if all buttons have the correct name
        for (int i = 0; i < buttons.Count; i++)
        {
            int id = i;
            Button button = buttons[id];
            //Assert.AreEqual(gm.currentCharacters[i].characterName, button.GetComponentInChildren<TMP_Text>());
            // TODO: make better way to get text on button
        }
        
        yield return null;
    }

    /// <summary>
    /// Checks if the notes are correctly opened
    /// </summary>
    [UnityTest]
    public IEnumerator OpenPersonalNotesTest()
    {
        nm.OpenPersonalNotes();
        
        // TODO: Save notes
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        // TODO: better way to get text on button
        
        // TODO: ChangeButtons method
        
        yield return null;
    }
    
    // TODO: CharacterTab test?

    /// <summary>
    /// Checks if the ToggleCharacterInfo method works correctly
    /// </summary>
    [UnityTest]
    public IEnumerator ToggleCharacterInfoTest()
    {
        bool active = nm.characterInfo.activeInHierarchy;
        
        nm.ToggleCharacterInfo();
        
        // CharacterInfo should be toggled from active to non-active or vice versa
        if (active)
            Assert.IsFalse(nm.characterInfo.activeSelf);
        else Assert.IsTrue(nm.characterInfo.activeSelf);

        yield return null;
    }

    /// <summary>
    /// Checks if the notes get saved correctly
    /// </summary>
    [UnityTest]
    public IEnumerator SaveNotesTest()
    {
        // TODO: better way to get text on button
        
        yield return null;
    }
    
    // TODO: ChangeButtons test?
}