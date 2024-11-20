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
using UnityEditor.Compilation;

public class CharacterInstanceEditTest
{
    private List<CharacterInstance> characters;
    private CharacterData           mainCharacterData;
    private CharacterInstance       mainCharacter;
    
    [OneTimeSetUp]
    public void Setup()
    {
        // Get some random characters to set up the tests
        CharacterData c1 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/0_Fatima_Data.asset", typeof(CharacterData)); // This will be the "main" character during the tests
        mainCharacterData = c1;
        CharacterData c2 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/1_Guilietta_Data.asset", typeof(CharacterData)); // This will be the culprit
        CharacterData c3 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/2_Willow_Data.asset", typeof(CharacterData)); // This will be the chosen culprit
        CharacterData c4 = (CharacterData) AssetDatabase.LoadAssetAtPath("Assets/Data/Character Data/3_Olivier_Data.asset", typeof(CharacterData));
        
        characters = new List<CharacterInstance>();
        mainCharacter = new CharacterInstance(c1);
        characters.Add(mainCharacter);
        
        // Other dummy characters
        characters.Add(new CharacterInstance(c2));
        characters.Add(new CharacterInstance(c3));
        characters.Add(new CharacterInstance(c4));
        
        // Load "Loading scene" and find GameManager to set it up
        EditorSceneManager.OpenScene("Assets/Scenes/Loading/Loading.unity");
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.currentCharacters = new List<CharacterInstance>();
        
        foreach (CharacterInstance c in characters)
            gm.currentCharacters.Add(c);

        // Set culprit and chosen culprit
        gm.currentCharacters[1].isCulprit = true;
        gm.FinalChosenCuplrit = gm.currentCharacters[2];
        
        GameManager.gm = gm;
    }

    /// <summary>
    /// Tests if the constructor for CharacterInstance behaves correctly
    /// </summary>
    [Test]
    public void CharacterInstanceInitTest()
    {
        // Set CharacterInstance fields
        Assert.AreEqual(mainCharacter.data, mainCharacterData);
        Assert.AreEqual(mainCharacter.characterName, mainCharacterData.characterName);
        Assert.AreEqual(mainCharacter.id, mainCharacterData.id);
        Assert.AreEqual(mainCharacter.avatar, mainCharacterData.avatar);
        Assert.AreEqual(mainCharacter.pitch, mainCharacterData.voicePitch);

        // Check if InitializeQuestions() goes correctly
        foreach (var kvp in mainCharacterData.answers)
        {
            Assert.AreEqual(mainCharacter.Answers[kvp.question], kvp.answer);
            Assert.AreEqual(mainCharacter.Traits[kvp.question], kvp.trait);
            Assert.IsTrue(mainCharacter.RemainingQuestions.Contains(kvp.question));
        }
    }

    /// <summary>
    /// Tests if the GetGreeing method behaves correctly
    /// </summary>
    /// <param name="greetings">We should also test if the data.greetings list is empty / null</param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GetGreetingTest(bool greetings)
    {
        // Test if character has no greetings
        if (!greetings)
        {
            CharacterData data = mainCharacterData;
            mainCharacter.data.greetings = new DialogueLines[] { }; // Create data without lines
            var lines = mainCharacter.GetGreeting();
            
            // The only thing returned should be "Hello"
            Assert.AreEqual(lines.Count, 1);
            Assert.AreEqual(lines[0], "Hello");
            
            mainCharacter.data = data; // Put data with lines back for other tests
        }
        else
        {
            var lines = mainCharacter.GetGreeting();
            Assert.IsNotNull(lines); // Test if some lines get returned
        }
    }

    /// <summary>
    /// Tests if the correct epilogue dialouge gets returned, based on win status
    /// </summary>
    /// <param name="hasWon">Whether or not the player has won</param>
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GetEpilogueDialogueTest(bool hasWon)
    {
        List<List<string>> epilogueDialogue = mainCharacter.GetEpilogueDialogue(hasWon);

        if (hasWon)
        {
            List<string> speakingText1 = new List<string>()
            {
                "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName + ".",
                "I was indeed the one who kept sending you messages.",
                "and in fact, I knew that you did not know who was sending the messages.",
                "You managed to guess correctly, and so i wanted to ask you the following:",
                "What made you think it was me sending the messages?"
            };
            List<string> speakingText2 = new List<string>()
            {
                "Okay, thats very interesting!",
                "Now I have another question for you:",
                "Have you found something about me that you can relate to?"
            };
            List<string> speakingText3 = new List<string>()
            {
                "Alright very cool.",
                "I have to go now.",
                "I do not want to miss the bus.",
                "Goodbye."
            };
            // List of lists, where in between each list an OpenResponseDialogueObject will be called.
            List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3};
            
            Assert.AreEqual(retval, epilogueDialogue);
        }
        else
        {
            List<string> speakingText1 = new List<string>()
            { 
                "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName,
                "You are asking me if I was sending you messages?",
                "I am sorry but I do not know what you are talking about.",
                "I have to go now, bye."
            };
            List<string> speakingText2 = new List<string>()
            { 
                "Well that was pretty awkward, wasn't it?",
                "I'm " + GameManager.gm.GetCulprit().characterName,
                "I am the one who kept sending you messages.",
                "and in fact, I knew that you did not know who",
                "was sending the messages.",
                "You did not guess correctly unfortunately.",
                "Despite that, I still wanted to to ask you the following:",
                "What made you think it was me sending the messages?"
            };
            List<string> speakingText3 = new List<string>()
            { 
                "Okay, thats very interesting!",
                "Now I have another question for you:",
                "Have you found something about me that you can relate to?"
            };
            List<string> speakingText4 = new List<string>()
            { 
                "Alright very cool.",
                "I have to go now.",
                "I do not want to miss the bus.",
                "Goodbye." 
            };
            // List of lists, where in between each list an OpenResponseDialogueObject will be called.
            List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3, speakingText4};
            
            Assert.AreEqual(retval, epilogueDialogue);
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GetRandomTraitTest(bool anyQuestions)
    {
        if (anyQuestions)
        {
            var retval = mainCharacter.GetRandomTrait();
            
            Assert.IsNotNull(retval);
            
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

            var question = mainCharacter.Traits.FirstOrDefault(x => x.Value == retval).Key;

            foreach (CharacterInstance c in GameManager.gm.currentCharacters)
            {
                Assert.IsFalse(c.RemainingQuestions.Contains(question));
            }
        }
        else
        {
            mainCharacter.RemainingQuestions = new List<Question>();

            var retval = mainCharacter.GetRandomTrait();
            
            LogAssert.Expect(LogType.Error, "GetRandomTrait(), but there are no more traits remaining");
            Assert.IsNull(retval);
        }
    }
}