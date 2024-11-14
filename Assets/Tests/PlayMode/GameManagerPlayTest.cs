using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using UnityEditor;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManagerPlayTest
{
    private StoryObject story;
    private GameManager gm;
    /// <summary>
    /// SetUp which is run for every test (currently WaitForSeconds is used, this will probably be changed in the
    /// future. Refer to #testing channel for the correct implementation (sanders dingetje met WaitUntil)).
    /// </summary>
    /*
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load scene
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded); // Wait for scene to load
        yield return new WaitForSeconds(1);
        Button newGameButton = GameObject.Find("NewGameButton").GetComponent<Button>();
        newGameButton.onClick.Invoke();
        yield return new WaitForSeconds(1);
        Button noButton = GameObject.Find("NoButton").GetComponent<Button>();
        noButton.onClick.Invoke();
        yield return new WaitForSeconds(1);
        Button storyAButton = GameObject.Find("StoryA_Button").GetComponent<Button>();
        storyAButton.onClick.Invoke();
        yield return new WaitForSeconds(2);
    }*/

    /// <summary>
    /// Set up the game so that each test starts at the NPCSelectScene with the chosen story.
    /// </summary>
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("Loading");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded); // Wait for scene to load.
        
        // Get a StoryObject.
        StoryObject[] stories = Resources.LoadAll<StoryObject>("Stories");
        story = stories[0];
        
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Start the game with the chosen story.
        gm.StartGame(null, story);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
    }
    
    /// <summary>
    /// Move the toolbox under loading as a child, then remove all scenes. This ensures that the toolbox
    /// gets removed before a new test starts.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneByName("Loading"));
        SceneController.sc.UnloadAdditiveScenes();
    }
    
    /// <summary>
    /// Checks if the character list gets populated.
    /// </summary>
    [UnityTest]
    public IEnumerator PopulateCharactersTest()
    {
        // Set up expected and actual values.
        int expected = gm.currentCharacters.Count;
        // The number of characters at the start of the game.
        int actual = 4;

        // Check if they are equal.
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if the characters all get set to active during populating.
    /// </summary>
    [UnityTest]
    public IEnumerator ActiveCharactersTest()
    {
        // Set up expected and actual values.
        int expected = gm.currentCharacters.Count(c => c.isActive);
        // The number of characters at the start of the game.
        int actual = 4;
        
        // Check if they are equal.
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if one culprit gets chosen during populating.
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseCulpritTest()
    {
        // Set up expected and actual values.
        int expected = gm.currentCharacters.Count(c => c.isCulprit);
        int actual = 1;

        // Check if they are equal.
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    // Input parameters for testing different inputs.
    static bool[] bools = new bool[] { true, false };
    static int[] ints = new int[] { 0, 1, 2 };
    
    /// <summary>
    /// Checks if HasQuestionsLeft returns true when numQuestionsAsked is smaller than numQuestions,
    /// or false when numQuestionsAsked is greater than or equal to numQuestions.
    /// </summary>
    [UnityTest]
    public IEnumerator HasQuestionsLeftTest([ValueSource(nameof(ints))] int values)
    {
        gm.numQuestionsAsked = values;
        gm.story.numQuestions = 1;
        if (values < gm.story.numQuestions)
        {
            // Check if HasQuestionsLeft is true when numQuestionsAsked is smaller than numQuestions.
            Assert.IsTrue(gm.HasQuestionsLeft());
        }
        else
        {
            // Check if HasQuestionsLeft returns false when numQuestionsAsked is greater than or equal to numQuestions.
            Assert.IsFalse(gm.HasQuestionsLeft());
        }
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetCulpritTest" returns the culprit.
    /// </summary>
    [UnityTest]
    public IEnumerator GetCulpritTest()
    {
        // Set up expected and actual values.
        CharacterInstance expected = gm.currentCharacters.Find(c => c.isCulprit);
        CharacterInstance actual = gm.GetCulprit();

        // Check if they are equal.
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "EnoughCharacters" returns true when there are enough characters remaining, else false.
    /// </summary>
    [UnityTest]
    public IEnumerator EnoughCharactersTest([ValueSource(nameof(bools))] bool enoughCharacters)
    {
        if (enoughCharacters)
        {
            bool hasEnoughCharacters = gm.currentCharacters.Count(c => c.isActive) > 2;
            // Check if when there are enough characters, EnoughCharactersRemaining returns true.
            Assert.IsTrue(hasEnoughCharacters);
            Assert.IsTrue(gm.EnoughCharactersRemaining());
        }
        else
        {
            // Keep removing 1 character which is not the culprit, until there are not enough characters remaining.
            while (gm.currentCharacters.Count(c => c.isActive) > 2)
            {
                // Set this bool to true once a character has been removed.
                bool removedCharacter = false;
                foreach (CharacterInstance c in gm.currentCharacters)
                {
                    // Set a character to not active if it is not a culprit, is active and the bool removedCharacter is false.
                    if (!c.isCulprit && c.isActive && !removedCharacter)
                    {
                        c.isActive = false;
                        removedCharacter = true;
                    }
                }
            }
            // Check if EnoughCharactersRemaining returns false.
            Assert.IsFalse(gm.EnoughCharactersRemaining());
        }
        
        yield return null;
    }

    /*
    /// <summary>
    /// Checks if the "RestartStoryScene" correctly resets the variables.
    /// </summary>
    [UnityTest]
    public IEnumerator RestartStoryTest()
    {
        // Start the game cycle.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Start dialogue.
        gm.StartDialogue(gm.currentCharacters[0]);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Call RestartStoryScene to check if values get reset.
        gm.RestartStoryScene();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Check if we are in the NpcSelect gameState and if only 2 scenes exist,
        // namely NpcSelectScene and Loading.
        // TODO: currently the gameState is not reset correctly.
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        Assert.AreEqual(2, SceneManager.loadedSceneCount);
        Assert.IsTrue(SceneManager.GetSceneByName("Loading").isLoaded);
        Assert.IsTrue(SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        yield return null;
    }
    */
    
    /// <summary>
    /// Checks if the "RetryStoryScene" resets all characters to be active.
    /// </summary>
    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        gm.RetryStoryScene();
        
        // True if all characters are active.
        bool actual = gm.currentCharacters.Count(c => c.isActive) == gm.currentCharacters.Count();

        // Check if all characters are active.
        Assert.IsTrue(actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetRandomVictimNoCulprit" returns a CharacterInstance that is not the culprit.
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseVictimTest()
    {
        // Get victim.
        CharacterInstance victim = gm.GetRandomVictimNoCulprit();

        // Check if it actually returned a victim.
        Assert.IsTrue(victim != null);
        
        yield return null;
    }

    /*
    /// <summary>
    /// Checks if the "EndCycle" method sets 1 character to inactive if there are enough characters remaining,
    /// else check if no characters get set to inactive.
    /// </summary>
    [UnityTest]
    public IEnumerator EndCycleTest([ValueSource(nameof(bools))] bool enoughCharacters)
    {
        // If we do not want to have enough characters.
        if (!enoughCharacters)
        {
            // Keep removing 1 character which is not the culprit, until there are not enough characters remaining.
            while (gm.EnoughCharactersRemaining())
            {
                // Set this bool to true once a character has been removed.
                bool removedCharacter = false;
                foreach (CharacterInstance c in gm.currentCharacters)
                {
                    // Set a character to not active if it is not a culprit, is active and the bool removedCharacter is false.
                    if (!c.isCulprit && c.isActive && !removedCharacter)
                    {
                        c.isActive = false;
                        removedCharacter = true;
                    }
                }
            }
        }
        
        // Start the game cycle.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Variable which counts the number of characters before calling EndCycle.
        int nCharactersPrior = gm.currentCharacters.Count(c => c.isActive);
        
        // Set this to maxValue in order to make sure that no more questions can be asked.
        // This will cause the EndCycle method to be called once the dialogue ends.
        gm.numQuestionsAsked = int.MaxValue;
        
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.

        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the NpcDialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the HintDialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        
        // Use reflection to call BacktoNPCScreen twice, to go from NpcDialogue -> HintDialogue -> NpcSelect.
        Type type = typeof(DialogueManager);
        var fakeDialogueManager = Activator.CreateInstance(type);
        MethodInfo m = type.GetMethod("BacktoNPCScreen",
            BindingFlags.NonPublic | BindingFlags.Instance);

        m.Invoke(fakeDialogueManager, null);
        m.Invoke(fakeDialogueManager, null);
        
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Variable which counts the number of characters after calling EndCycle.
        int nCharactersPosterior = gm.currentCharacters.Count(c => c.isActive);
        
        // We test whether a character disappears when EndCycle is called and there are enough characters.
        // If there are not enough characters, then we test if we transition to the culpritSelect gameState
        // and if a character does not disappear.
        if (enoughCharacters)
        {
            // Check if only 1 character has disappeared.
            Assert.AreEqual(nCharactersPrior - 1, nCharactersPosterior);
            // Check if we go to the HintDialogue gameState.
            Assert.AreEqual(GameManager.GameState.HintDialogue, gm.gameState);
            // TODO: close the scenes after each test (else crash when running all tests :( ).
        }
        else
        {
            // Check if no characters have disappeared.
            Assert.AreEqual(nCharactersPrior, nCharactersPosterior);
            // Check if the gameState transitions to culpritSelect.
            // TODO: In the current version, the gameState "culpritSelect" is never used, which should be used.
            Assert.AreEqual(GameManager.GameState.CulpritSelect, gm.gameState);
        }
        
        yield return null;
    }
*/
    /*
    /// <summary>
    /// Checks if the "StartDialogue" has the correct gameState (NpcDialogue) and checks if the DialogueScene is loaded.
    /// </summary>
    [UnityTest]
    public IEnumerator StartDialogueTest()
    {
        // Get character to start dialogue with.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);

        // Check if the gameState is set to NpcDialogue.
        Assert.AreEqual(GameManager.GameState.NpcDialogue, gm.gameState);
        // Check if we are in the DialogueScene.
        bool inDialogueScene = SceneManager.GetSceneByName("DialogueScene").isLoaded;
        Assert.IsTrue(inDialogueScene);
        
        yield return null;
    }
    */
    /// <summary>
    /// Check whether the transition between culprit selection and epilogue works intended by looking at the following:
    /// - if culprit is chosen:
    /// Check if hasWon is set to true, check if the gameState is epilogue and check if we are currently in the DialogueScene.
    /// - if innocent person is chosen:
    /// Check if hasWon is set to false, check if the gameState is epilogue and check if we are currently in the DialogueScene.
    /// </summary>
    [UnityTest]
    public IEnumerator CulpritSelectEpilogueTransition([ValueSource(nameof(bools))] bool hasChosenCulprit)
    {
        // Keep removing 1 character which is not the culprit, until there are not enough characters remaining.
        while (gm.EnoughCharactersRemaining())
        {
            // Set this bool to true once a character has been removed.
            bool removedCharacter = false;
            foreach (CharacterInstance c in gm.currentCharacters)
            {
                // Set a character to not active if it is not a culprit, is active and the bool removedCharacter is false.
                if (!c.isCulprit && c.isActive && !removedCharacter)
                {
                    c.isActive = false;
                    removedCharacter = true;
                }
            }
        }
        
        // Start the game cycle with not enough characters remaining.
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Set this to maxValue in order to make sure that no more questions can be asked.
        gm.numQuestionsAsked = int.MaxValue;
        
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the dialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        /*
        // Use reflection to call BacktoNPCScreen twice, to go from NpcDialogue -> HintDialogue -> NpcSelect.
        Type type = typeof(DialogueManager);
        var fakeDialogueManager = Activator.CreateInstance(type);
        MethodInfo m = type.GetMethod("BacktoNPCScreen", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        m.Invoke(fakeDialogueManager, null);
        m.Invoke(fakeDialogueManager, null);
        */
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Get "SelectionManager" object.
        SelectionManager sm = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        
        if (hasChosenCulprit)
        {
            bool culpritGameObjectFound = false;
            int counter = 1;
            // Find the GameObject that corresponds with the culprit.
            while (!culpritGameObjectFound)
            {
                string path = "characterspace " + counter;
                GameObject go = GameObject.Find(path);
                CharacterInstance selectedCharacter = go.GetComponentInChildren<SelectOption>().character;
                if (selectedCharacter.characterName == gm.GetCulprit().characterName)
                {
                    culpritGameObjectFound = true;
                    // Simulate choosing the culprit.
                    sm.ButtonClicked(selectedCharacter);
                }
                counter++;
            }
            
            // Check if the hasWon variable is set to true.
            Assert.IsTrue(gm.hasWon);
        }
        else
        {
            bool innocentGameObjectFound = false;
            int counter = 1;
            // Find the GameObject that corresponds with an innocent person.
            while (!innocentGameObjectFound)
            {
                string path = "characterspace " + counter;
                GameObject go = GameObject.Find(path);
                CharacterInstance selectedCharacter = go.GetComponentInChildren<SelectOption>().character;
                // Choose an innocent person that is not dead and is not the culprit.
                if (selectedCharacter.characterName != gm.GetCulprit().characterName && selectedCharacter.isActive)
                {
                    innocentGameObjectFound = true;
                    GameObject innocentObject = go;
                    // Simulate choosing an innocent person.
                    sm.ButtonClicked(selectedCharacter);
                }
                counter++;
            }
            
            // Check if the hasWon variable is set to false.
            Assert.IsFalse(gm.hasWon);
        }
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load
        
        // Check if the gameState is switched to epilogue and if the dialogue scene is loaded.
        Assert.AreEqual(GameManager.GameState.Epilogue, gm.gameState);
        bool inDialogueScene = SceneManager.GetSceneByName("DialogueScene").isLoaded;
        Assert.IsTrue(inDialogueScene);
        
        yield return null;
    }
    
    /// <summary>
    /// Check if the transition from the losing scenario works as intended when the dialogue switches from innocent person to culprit.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueToStartDialogueEpilogueTest()
    {
        // Set the finalChosenCulprit in GameManager.
        CharacterInstance innocentPerson = gm.GetRandomVictimNoCulprit();
        gm.FinalChosenCuplrit = innocentPerson;
        // Start the epilogue dialogue.
        gm.StartEpilogueDialogue(innocentPerson);
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the dialogue and add a speakingObject to the responses list in order to start dialogue with a new person.
        dm.currentObject = new TerminateDialogueObject();
        List<string> text = new List<string>(){"hello"};
        GameObject[] background = new GameObject[1]{gm.story.hintBackground};
        dm.currentObject.Responses.Add(new SpeakingObject(text, background));
        dm.currentObject.Execute();
        
        // Check if the DialogueObjects in the responses list of the currentObject
        Assert.GreaterOrEqual(1, dm.currentObject.Responses.Count);

        // Wait for new dialogue with culprit to unload and load.
        yield return new WaitUntil(() => !SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Get the DialogueManager.
        dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        
        // Check if the dialogue switches to the culprit.
        Assert.AreEqual(gm.GetCulprit().characterName, dm.currentRecipient.characterName);

        yield return null;
    }
    
    /*
    /// <summary>
    /// Check if the correct gameState and scene are loaded after the dialogue of the epilogue ends.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueEpilogueTest([ValueSource(nameof(bools))] bool hasWon)
    {
        // Set the hasWon variable from GameManager to true.
        gm.hasWon = hasWon;
        
        // Check if certain properties hold when hasWon is set to true or false.
        if (gm.hasWon)
        {
            // Set the finalChosenCulprit in GameManager.
            CharacterInstance culprit = gm.GetCulprit();
            gm.FinalChosenCuplrit = culprit;
            // Start the epilogue dialogue.
            gm.StartEpilogueDialogue(culprit);
        
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
            // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
            yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
            
            // Get the DialogueManager.
            var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
            // End the dialogue.
            dm.currentObject = new TerminateDialogueObject();
            dm.currentObject.Execute();
        
            // Check if the DialogueObjects in the responses list of the currentObject
            Assert.AreEqual(0, dm.currentObject.Responses.Count);
            
            yield return new WaitUntil(() => SceneManager.GetSceneByName("GameWinScene").isLoaded); // Wait for scene to load.
            
            // TODO: swap gameState in EndDialogue to be before scene transition.
            // Check if the GameWinScene is loaded.
            bool inGameWinScene = SceneManager.GetSceneByName("GameWinScene").isLoaded;
            Assert.IsTrue(inGameWinScene);
        
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameWon, gm.gameState);
        }
        else
        {
            // Set the finalChosenCulprit in GameManager.
            CharacterInstance innocentPerson = gm.GetRandomVictimNoCulprit();
            gm.FinalChosenCuplrit = innocentPerson;
            // Start the epilogue dialogue.
            gm.StartEpilogueDialogue(innocentPerson);
        
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
            // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
            yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
            
            // Get the DialogueManager.
            var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
            // End the dialogue.
            dm.currentObject = new TerminateDialogueObject();
            dm.currentObject.Execute();
        
            // Check if the DialogueObjects in the responses list of the currentObject
            Assert.AreEqual(0, dm.currentObject.Responses.Count);
            
            yield return new WaitUntil(() => SceneManager.GetSceneByName("GameOverScene").isLoaded); // Wait for scene to load.
            
            // Check if the GameOverScene is loaded.
            bool inGameOverScene = SceneManager.GetSceneByName("GameOverScene").isLoaded;
            Assert.IsTrue(inGameOverScene);
        
            // Check if we are in the correct gameState.
            Assert.AreEqual(GameManager.GameState.GameLoss, gm.gameState);
        }

        yield return null;
    }*/
    
    /// <summary>
    /// Check if EndDialogue returns to the NPCSelect state when there are questions left.
    /// </summary>
    [UnityTest]
    public IEnumerator EndDialogueGameCycleTest()
    {
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("DialogueManager") != null);
        
        // Check if there are questions left. 
        Assert.IsTrue(gm.HasQuestionsLeft());
        
        // Get the DialogueManager.
        var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        // End the dialogue.
        dm.currentObject = new TerminateDialogueObject();
        dm.currentObject.Execute();
        
        /*
        // Use reflection to call BacktoNPCScreen twice, to go from NpcDialogue -> NpcSelect.
        Type type = typeof(DialogueManager);
        var fakeDialogueManager = Activator.CreateInstance(type);
        MethodInfo m = type.GetMethod("BacktoNPCScreen", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        m.Invoke(fakeDialogueManager, null);
        */
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded); // Wait for scene to load.
        
        // Waiting for the DialogueManager to appear, since waiting for the DialogueScene is not enough.
        yield return new WaitUntil(() => GameObject.Find("SelectionManager") != null);
        
        // Check if the NPCSelectScene is loaded.
        bool inNpcSelectScene = SceneManager.GetSceneByName("NPCSelectScene").isLoaded;
        Assert.IsTrue(inNpcSelectScene);
        
        // Check if we are in the correct gameState.
        Assert.AreEqual(GameManager.GameState.NpcSelect, gm.gameState);
        
        yield return null;
    }
    
    
}