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
using TMPro;
using TMPro.Examples;

public class NotebookManagerPlayTest
{
    private GameManager     gm;
    private NotebookManager nm;
    
    #region Setup and Teardown
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load StartScreenScene in order to put the SettingsManager into DDOL
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);

        // Move debugmanager and copyright back to startscreenscene so that 
        SceneManager.MoveGameObjectToScene(GameObject.Find("DebugManager"), SceneManager.GetSceneByName("StartScreenScene"));
        SceneManager.MoveGameObjectToScene(GameObject.Find("Copyright"), SceneManager.GetSceneByName("StartScreenScene"));
        
        // Unload the StartScreenScene
        SceneManager.UnloadSceneAsync("StartScreenScene");
        
        // Load the "Loading" scene in order to get access to the toolbox in DDOL
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Put toolbox as parent of SettingsManager
        GameObject.Find("SettingsManager").transform.SetParent(GameObject.Find("Toolbox").transform);
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        gm.StartGame(null, Resources.LoadAll<StoryObject>("Stories")[0]);

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
    /// Checks if the notebook is set up correctly.
    /// </summary>
    [UnityTest]
    public IEnumerator StartNotebookTest()
    {
        // Check if some basic properties hold
        Assert.IsFalse(nm.characterInfo.activeSelf);
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        Assert.IsTrue(nm.inputField.activeSelf);
        Assert.AreEqual(nm.notebookData, gm.notebookData);
        Assert.IsFalse(nm.personalButton.interactable);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the notes are correctly opened
    /// </summary>
    [UnityTest]
    public IEnumerator OpenPersonalNotesTest()
    {
        // Set up fields
        string textBefore = nm.inputField.GetComponent<TMP_InputField>().text;
        
        string newText = "hello";

        nm.inputField.GetComponent<TMP_InputField>().text = newText;
        
        nm.OpenPersonalNotes();
        
        var textAfter = nm.inputField.GetComponent<TMP_InputField>().text;
        
        // Check if SaveNotes works correctly
        
        // Check if text has changed
        Assert.AreNotEqual(textBefore, textAfter);
        
        bool active = nm.inputField.activeInHierarchy;
        
        // Check if the new text is equal to the dummy text
        if (active)
            Assert.AreEqual(nm.notebookData.GetPersonalNotes(), newText);
        else
        {
            var prop = nm.GetType().GetField("currentCharacter", System.Reflection.BindingFlags.NonPublic
                                                                 | System.Reflection.BindingFlags.Instance);
            prop.SetValue(nm, gm.currentCharacters[0]);
            
            Assert.AreEqual(nm.notebookData.GetCharacterNotes(gm.currentCharacters[0]), newText);
        }
        
        // InputField should not be active
        Assert.IsFalse(nm.inputFieldCharacters.activeSelf);
        
        // Personal notes should be printed on the screen
        Assert.AreEqual(nm.notebookData.GetPersonalNotes(), nm.inputField.GetComponent<TMP_InputField>().text);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the notes get saved correctly
    /// </summary>
    [UnityTest]
    public IEnumerator SaveNotesTest()
    {
        string textBefore = nm.inputField.GetComponent<TMP_InputField>().text;
        
        // Write dummy text to input field
        string newText = "hello";
        nm.inputField.GetComponent<TMP_InputField>().text = newText;
        
        nm.SaveNotes();
        
        var textAfter = nm.inputField.GetComponent<TMP_InputField>().text;
        
        // Check if text has changed
        Assert.AreNotEqual(textBefore, textAfter);

        bool active = nm.inputField.activeInHierarchy;
        
        // Check if the new text is equal to the dummy text
        if (active)
            Assert.AreEqual(nm.notebookData.GetPersonalNotes(), newText);
        else
        {
            var prop = nm.GetType().GetField("currentCharacter", System.Reflection.BindingFlags.NonPublic
                                                  | System.Reflection.BindingFlags.Instance);
            prop.SetValue(nm, gm.currentCharacters[0]);
            
            Assert.AreEqual(nm.notebookData.GetCharacterNotes(gm.currentCharacters[0]), newText);
        }
        
        yield return null;
    }
}