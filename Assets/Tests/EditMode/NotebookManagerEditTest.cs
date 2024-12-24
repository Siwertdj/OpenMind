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
using UnityEngine.TextCore.Text;
using TMPro;

public class NotebookManagerEditTest
{
    private NotebookManager         nm;
    private List<CharacterInstance> characters;
    
    [OneTimeSetUp]
    public void Setup()
    {
        // Get some random characters to make the notebook for
        CharacterData c1 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/0_Fatima_Data.asset", typeof(CharacterData));
        CharacterData c2 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/1_Guilietta_Data.asset", typeof(CharacterData));
        CharacterData c3 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/2_Willow_Data.asset", typeof(CharacterData));
        CharacterData c4 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/3_Olivier_Data.asset", typeof(CharacterData));
        
        characters = new List<CharacterInstance>();
        characters.Add(new CharacterInstance(c1));
        characters.Add(new CharacterInstance(c2));
        characters.Add(new CharacterInstance(c3));
        characters.Add(new CharacterInstance(c4));

        // Load "Loading scene" and find GameManager to set it up
        EditorSceneManager.OpenScene("Assets/Scenes/Loading/Loading.unity");
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.currentCharacters = new List<CharacterInstance>();
        
        foreach (CharacterInstance c in characters)
            gm.currentCharacters.Add(c);
        
        GameManager.gm = gm;

        EditorSceneManager.OpenScene("Assets/Scenes/Notebook/NotebookScene.unity");
        nm = GameObject.Find("NotebookManager").GetComponent<NotebookManager>();
    }

    /// <summary>
    /// Checks if all buttons get correctly initialized.
    /// </summary>
    [Test]
    public void InitializeCharacterButtonsTest()
    {
        nm.InitializeCharacterButtons();
        
        var buttons = nm.Test_GetNameButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            int id = i;
            Button button = buttons[id];
            string s = button.GetComponentInChildren<TMP_Text>().text;
            Assert.AreEqual(GameManager.gm.currentCharacters[i].characterName, s);
        }
    }
    
    /// <summary>
    /// Checks if the ToggleCharacterInfo method works correctly
    /// </summary>
    [Test]
    public void ToggleCharacterInfoTest()
    {
        bool active = nm.Test_CharacterInfoField.activeInHierarchy;
        
        nm.ToggleCharacterInfo();
        
        if (active)
            Assert.IsFalse(nm.Test_CharacterInfoField.activeSelf);
        else Assert.IsTrue(nm.Test_CharacterInfoField.activeSelf);
    }
}