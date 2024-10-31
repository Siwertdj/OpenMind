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

    [UnityTest]
    public IEnumerator StartNotebookTest()
    {
        Assert.IsFalse(nm.characterInfo.activeSelf);
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        Assert.IsTrue(nm.inputField.activeSelf);
        Assert.AreEqual(nm.notebookData, gm.notebookData);
        Assert.IsFalse(nm.personalButton.interactable);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator InitializeCharacterButtonsTest()
    {
        nm.InitializeCharacterButtons();
        
        var buttons = nm.nameButtons.GetComponentsInChildren<Button>().ToList();

        for (int i = 0; i < buttons.Count; i++)
        {
            int id = i;
            Button button = buttons[id];
            //Assert.AreEqual(gm.currentCharacters[i].characterName, button.GetComponentInChildren<Text>());
            // TODO: make better way to get text on button
        }
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator OpenPersonalNotesTest()
    {
        nm.OpenPersonalNotes();
        
        // TODO: Save notes
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        // TODO: better way to get text on button
        
        // CHANGE BUTTONS
        
        yield return null;
    }
    
    // TODO: CharacterTab test?

    [UnityTest]
    public IEnumerator ToggleCharacterInfoTest()
    {
        bool active = nm.characterInfo.activeInHierarchy;
        
        nm.ToggleCharacterInfo();
        
        if (active)
            Assert.IsFalse(nm.characterInfo.activeSelf);
        else Assert.IsTrue(nm.characterInfo.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SaveNotesTest()
    {
        // TODO: better way to get text on button
        
        yield return null;
    }
    
    // TODO: ChangeButtons test?
}