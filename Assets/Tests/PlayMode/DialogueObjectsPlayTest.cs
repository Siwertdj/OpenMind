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
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;

public class DialogueObjectsPlayTest
{
    private GameManager gm;
    private StoryObject story;
    private DialogueManager dm;
    private CharacterInstance character;

    private GameObject[] background = { new GameObject() };

    #region Setup & Teardown
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load "Loading" scene
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);

        // Set global variables
        story = Resources.LoadAll<StoryObject>("Stories")[0];
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Start NPC select scene
        gm.StartGame(null, story);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("NPCSelectScene"));
    }

    [UnityTearDown]
    public void TearDown()
    {

    }
    #endregion

    [UnityTest]
    public IEnumerator SpeakingObjectDefaultResponseTest()
    {
        List<string> text = new List<string>{ "Hello, world!" };
        DialogueObject obj = new SpeakingObject(text.ToList(), background);

        gm.StartDialogue(obj);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

        Assert.IsTrue(obj.Responses[0] is TerminateDialogueObject);
    }
}
