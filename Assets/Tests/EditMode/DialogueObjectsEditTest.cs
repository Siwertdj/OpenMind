using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WaitUntil = UnityEngine.WaitUntil;
using UnityEditor.SceneManagement;
using UnityEditor;

public class DialogueObjectsEditTest
{
    private GameManager gm;
    private DialogueManager dm;
    private StoryObject story;

    [OneTimeSetUp]
    public void Setup()
    {
        // Load "Loading scene" and find GameManager to set it up
        EditorSceneManager.OpenScene("Assets/Scenes/Loading/Loading.unity");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        story = Resources.LoadAll<StoryObject>("Stories")[0];
    }

    /// <summary>
    /// Tests if the default response to a speaking object is a TerminateDialogueObject
    /// </summary>
    [Test]
    public void SpeakingObjectDefaultResponseTest()
    {
        var exampleLine = new List<string> { "Hello" };
        var exampleBackground = new GameObject[] { story.dialogueBackground };
        var speakingObject = new SpeakingObject(exampleLine, exampleBackground);

        speakingObject.Execute();

        Assert.AreEqual(new TerminateDialogueObject(), speakingObject.Responses[0]);
    }

    [Test]
    public void GetQuestionResponseTest()
    {
        var data = AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/Data/Character Data/0_Fatima_Data.asset");
        var character = new CharacterInstance(data);


        var exampleLine = new List<string> { "Hello" };
        var exampleBackground = new GameObject[] { story.dialogueBackground };
        var speakingObject = new SpeakingObject(exampleLine, exampleBackground);
    }
}
