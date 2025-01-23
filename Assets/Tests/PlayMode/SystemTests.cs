// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using UnityEditor;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

public class SystemTests
{
    private List<bool> greeted = new();
    
    /// <summary>
    /// Set up for each of the system level tests.
    /// </summary>
    /// <returns></returns>
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load the StartScreenScene
        SceneManager.LoadScene("StartScreenScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StartScreenScene").isLoaded);
        
        // Move DebugManager and copyright back to StartScreenScene so that 
        //SceneManager.MoveGameObjectToScene(GameObject.Find("DebugManager"), SceneManager.GetSceneByName("StartScreenScene"));
        //SceneManager.MoveGameObjectToScene(GameObject.Find("Copyright"), SceneManager.GetSceneByName("StartScreenScene"));
                
        yield return null;
    }

    /// <summary>
    /// Tear down for each of the system level tests. Move the toolbox under loading as a child,
    /// then remove all scenes. This ensures that the toolbox gets removed before a new test starts.
    /// </summary>
    
    [TearDown]
    public void TearDown()
    {
        // TODO: perhaps check if there is anything under ddol, then move the objects if so.
        // Move the Toolbox and the SettingsManager to be not in ddol
        if(GameObject.Find("Toolbox"))
            SceneManager.MoveGameObjectToScene(GameObject.Find("Toolbox"), SceneManager.GetSceneAt(0));    
        
        //if(GameObject.Find("SettingsManager"))
        //    SceneManager.MoveGameObjectToScene(GameObject.Find("SettingsManager"), SceneManager.GetSceneAt(0));
        
        // Unload additive scenes.
        if(SceneController.sc != null)
            SceneController.sc.UnloadAdditiveScenes();
        
        greeted.Clear();
        Debug.Log(SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
    }
    
    static int[] stories = new int[] { 0 };
    
    [UnityTest, Timeout(100000000)]
    public IEnumerator PlayTheGame([ValueSource(nameof(stories))] int storyID)
    {
        yield return PlayUntilStorySelect();
        yield return SelectStoryAndPlayUntilNPCSelect(storyID);

        // Number of characters in the game
        int numCharacters = GameManager.gm.story.numberOfCharacters;
        
        // Number of characters that are left when you have to choose the culprit
        int charactersLeft = GameManager.gm.story.minimumRemaining;

        // List of characters that have no questions left.
        List<CharacterInstance> emptyQuestionCharacters = new List<CharacterInstance>();
        
        // Play the main loop of the game
        for (int i = 0; i <= (numCharacters - charactersLeft); i++)
        {
            yield return new WaitUntil(() =>
                SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

            // Check if we are in the NPC Select scene
            Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"),
                SceneManager.GetSceneAt(1));

            yield return new WaitForSeconds(0.5f);

            yield return PlayTutorial();
            
            // Select a npc that has questions left.
            yield return SelectNpc(emptyQuestionCharacters, GameManager.gm.currentCharacters);
            
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

            // Check if we are in the Dialogue scene
            Assert.IsTrue(SceneManager.GetSceneByName("DialogueScene").isLoaded);

            yield return new WaitForSeconds(1);

            int numQuestions = GameManager.gm.story.numQuestions;

            // Ask a certain number of questions
            for (int j = 0; j < numQuestions; j++)
            {
                // Wait until you can ask a question
                while (GameObject.Find("Questions Field") == null)
                {
                    yield return new WaitForSeconds(1);
                    GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
                }

                yield return new WaitForSeconds(1);

                // Ask a question
                if (GameObject.Find("questionButton").GetComponent<Button>() != null)
                    GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

                yield return new WaitForSeconds(1);

                // Get the DialogueManager
                var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
                
                // If the character has no more questions remaining, add the character to the list of emptyQuestionCharacters.
                if (dm.currentRecipient.RemainingQuestions.Count == 0)
                    emptyQuestionCharacters.Add(dm.currentRecipient);
                
                // The final iteration of the loop should continue to the hint scene
                if (j == numQuestions - 1)
                    // Wait for hint scene to be over
                    while (!SceneManager.GetSceneByName("NPCSelectScene").isLoaded && !SceneManager.GetSceneByName("EpilogueScene").isLoaded)
                    {
                        yield return new WaitForSeconds(1);

                        // Go through hint scene if it's active, else go through dialogue scene
                        if (GameObject.Find("Phone Dialogue Field") != null)
                        {
                            if (GameObject.Find("Next Dialogue Button") != null)
                                GameObject.Find("Next Dialogue Button").GetComponent<Button>().onClick
                                    .Invoke();
                        }
                        else if (GameObject.Find("Skip Dialogue Button") != null)
                            GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick
                                .Invoke();
                    }
            }
        }

        // Check if we have to choose a culprit
        Assert.AreEqual(SceneManager.GetSceneByName("EpilogueScene"), SceneManager.GetSceneAt(1));
        
        // Choose one of the characters
        GameObject.Find("Portrait Button(Clone)").GetComponent<Button>().onClick.Invoke();

        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);
        Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(2));
        
        // Skip dialogue until first reflection moment
        while (GameObject.Find("InputField") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until second reflection moment
        while (GameObject.Find("InputField") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until GameOver
        while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameOverScene"))
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }

        // Check if we are in the GameOver scene
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetSceneAt(1), SceneManager.GetSceneByName("GameOverScene"));

        yield return null;
    }

    /// <summary>
    /// System level test for saving the game.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"> Occurs when there are no active characters in the game. (should never occur)</exception>
    // TODO: check for isLoaded instead of using GetSceneAt() (refactoring).
    [UnityTest, Timeout(100000000), Order(1)]
    public IEnumerator SaveGame()
    {
        yield return PlayUntilStorySelect();
        yield return SelectStoryAndPlayUntilNPCSelect(0);
        yield return PlayTutorial();

        // List of characters that have no questions left.
        List<CharacterInstance> emptyQuestionCharacters = new List<CharacterInstance>();
        
        // Select a npc that has questions left.
        yield return SelectNpc(emptyQuestionCharacters, GameManager.gm.currentCharacters);
        
        yield return FullConversation(GameManager.gm.story.numQuestions, emptyQuestionCharacters);
        
        // Open Notebook
        GameObject.Find("Notebook Button").GetComponent<Button>().onClick.Invoke();

        // Wait until loaded
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NotebookScene").isLoaded);
                
        // Check if we are in the Notebook scene
        Assert.AreEqual(SceneManager.GetSceneByName("NotebookScene"), SceneManager.GetSceneAt(2));
                
        // Open personal notes
        GameObject.Find("PersonalButton").GetComponent<Button>().onClick.Invoke();
        

        //write character notes
        string notebookTextPrior = "hello";
        TMP_InputField personalNotes = GameObject.Find("Personal Notes Input Field").GetComponent<TMP_InputField>();
        personalNotes.text = notebookTextPrior;
        foreach (var button in 
                 GameObject.Find("Buttons Top Row").GetComponentsInChildren<Button>().Union(
                     GameObject.Find("Buttons Bottom Row").GetComponentsInChildren<Button>()))
        {
            if (button.name == "PersonalButton")
                continue;
            
            button.onClick.Invoke();
            yield return null;
            
            yield return new WaitWhile(() => GameObject.Find("Character Info Field") is null);
            yield return new WaitWhile(() =>
                GameObject.Find("Character Info Field").GetComponentInChildren<TMP_InputField>() is null);
            TMP_InputField TMPcharacterNotes = GameObject.Find("Character Info Field").GetComponentInChildren<TMP_InputField>();
            TMPcharacterNotes.text = notebookTextPrior + " " + button.name;
        }
        
        
        // Close notebook
        GameObject.Find("Notebook Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        // Check if we are back in the NPC Select scene
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
        // Open the menu
        GameObject.Find("Menu Button").GetComponent<Button>().onClick.Invoke();
        
        // Wait until loaded
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("GameMenuScene").isLoaded);
        
        // Save the game
        GameObject.Find("SaveButton").GetComponent<Button>().onClick.Invoke();
        
        bool saveFileExists = FilePathConstants.DoesSaveFileLocationExist();
        Assert.IsTrue(saveFileExists);
        
        // Retrieve the data from the save file
        SaveData saveData = Load.Loader.GetSaveData();
        
        // Check if the following variables are equal to the saved data
        var gm = GameManager.gm;
        
        // Check if storyID is equal
        Assert.AreEqual(gm.story.storyID, saveData.storyId);
        
        // Check if the activeCharacterIds are equal by checking the following 2 properties:
        // 1: Check if the array of activeCharacterIds has the same length as the array of
        // activeCharacterIds from the saveData.
        // 2: Check if both arrays contain the same elements.
        int[] activeCharIdArray = gm.currentCharacters.FindAll(c => c.isActive).ToArray().Select(c => c.id).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(activeCharIdArray.Length, saveData.activeCharacterIds.Length);
        // Check if both arrays contain the same elements
        Assert.IsTrue(activeCharIdArray.All(saveData.activeCharacterIds.Contains));
        
        // Check if the inactiveCharacterIds are equal by checking the following 2 properties:
        // 1: Check if the array of inactiveCharacterIds has the same length as the array of
        // inactiveCharacterIds from the saveData.
        // 2: Check if both arrays contain the same elements.
        int[] inactiveCharIdArray = gm.currentCharacters.FindAll(c => !c.isActive).ToArray().Select(c => c.id).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(inactiveCharIdArray.Length, saveData.inactiveCharacterIds.Length);
        // Check if both arrays contain the same elements
        Assert.IsTrue(inactiveCharIdArray.All(saveData.inactiveCharacterIds.Contains));
        
        // Check if the culpritId is equal
        Assert.AreEqual(gm.GetCulprit().id, saveData.culpritId);
        
        // Check if the remainingQuestions are equal by checking the following 2 properties:
        // 1: Check if the array of remainingQuestions has the same length as the array of
        // remainingQuestions from the saveData.
        // 2: Check if both arrays contain the same elements.
        (int, List<Question>)[] remainingQuestionsArray = gm.currentCharacters.Select(a => (a.id, a.RemainingQuestions)).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(remainingQuestionsArray.Length, saveData.remainingQuestions.Length);
        // Check if both arrays contain the same elements
        for (int i = 0; i < remainingQuestionsArray.Length; i++)
        {
            // Check if the first elements of the pairs are equal
            Assert.AreEqual(remainingQuestionsArray[i].Item1, saveData.remainingQuestions[i].Item1);
            // Check if the second elements of the pairs (question list) are equal
            Assert.IsTrue(remainingQuestionsArray[i].Item2.All(saveData.remainingQuestions[i].Item2.Contains));
        }
        
        // Check if the personalNotes are equal
        Assert.AreEqual(gm.notebookData.GetPersonalNotes(), saveData.personalNotes);
        
        // Check if the characterNotes are equal by checking the following 2 properties:
        // 1: Check if the array of characterNotes has the same length as the array of
        // characterNotes from the saveData.
        // 2: Check if both arrays contain the same elements.
        (int, string)[] characterNotes = gm.currentCharacters
            .Select(c => (c.id, gm.notebookData.GetCharacterNotes(c))).ToArray();
        // Check if the arrays have the same length
        Assert.AreEqual(characterNotes.Length, saveData.characterNotes.Length);
        // Check if both arrays contain the same elements
        for (int i = 0; i < characterNotes.Length; i++)
        {
            // Check if the first elements of the pairs are equal
            Assert.AreEqual(characterNotes[i].Item1, saveData.characterNotes[i].Item1);
            // Check if the second elements of the pairs are equal
            Assert.AreEqual(characterNotes[i].Item2, saveData.characterNotes[i].Item2);
        }
        
        // Check if the numQuestionsAsked is equal
        Assert.AreEqual(gm.numQuestionsAsked, saveData.numQuestionsAsked);
        
        // Close the menu
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
    }

    /// <summary>
    /// System level test for loading the game.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"> Occurs when no save file exists. </exception>
    /// // TODO: check for isLoaded instead of using GetSceneAt() (refactoring).
    [UnityTest, Timeout(100000000), Order(2)]
    public IEnumerator LoadGame()
    {
        if (!FilePathConstants.DoesSaveFileLocationExist())
            throw new Exception("No save file exists");
        
        // Find the New Game button and click it
        GameObject.Find("ContinueButton").GetComponent<Button>().onClick.Invoke();
        
        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        
        // Retrieve the data from the save file
        SaveData saveData = Load.Loader.GetSaveData();
        
        // Number of active characters in the game
        int numActiveCharacters = saveData.activeCharacterIds.Length;
        
        // Number of characters that are left when you have to choose the culprit
        int charactersLeft = GameManager.gm.story.minimumRemaining;
        
        // List of characters that have no questions left.
        List<CharacterInstance> emptyQuestionCharacters = new List<CharacterInstance>();
        
        // Play the main loop of the game
        for (int i = 0; i <= (numActiveCharacters - charactersLeft); i++)
        {
            yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
            
            // Check if we are in the NPC Select scene
            Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
        
            // Select a npc that has questions left.
            yield return SelectNpc(emptyQuestionCharacters, GameManager.gm.currentCharacters);
            
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

            // Check if we are in the Dialogue scene
            Assert.IsTrue(SceneManager.GetSceneByName("DialogueScene").isLoaded);
            yield return new WaitForSeconds(1);
            
            int numQuestions = GameManager.gm.story.numQuestions;

            // Ask a certain number of questions
            for (int j = 0; j < numQuestions; j++)
            {
                // Wait until you can ask a question
                while (GameObject.Find("Questions Field") == null)
                {
                    yield return new WaitForSeconds(1);
                    GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
                }

                yield return new WaitForSeconds(1);

                // Ask a question
                if (GameObject.Find("questionButton").GetComponent<Button>() != null)
                    GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

                yield return new WaitForSeconds(1);

                // Get the DialogueManager
                var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

                CharacterInstance chosenCharacter = dm.currentRecipient;
                
                // The final iteration of the loop should continue to the hint scene
                if (j == numQuestions - 1)
                    // Wait for hint scene to be over
                    while (!SceneManager.GetSceneByName("NPCSelectScene").isLoaded && !SceneManager.GetSceneByName("EpilogueScene").isLoaded)
                    {
                        yield return new WaitForSeconds(1);

                        // Go through hint scene if it's active, else go through dialogue scene
                        if (GameObject.Find("Phone Dialogue Field") != null)
                        {
                            if (GameObject.Find("Next Dialogue Button") != null)
                                GameObject.Find("Next Dialogue Button").GetComponent<Button>().onClick
                                    .Invoke();
                        }
                        else if (GameObject.Find("Skip Dialogue Button") != null)
                            GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick
                                .Invoke();
                    }
                
                // If the character has no more questions remaining, add the character to the list of emptyQuestionCharacters.
                if (chosenCharacter.RemainingQuestions.Count == 0)
                    emptyQuestionCharacters.Add(chosenCharacter);
            }
        }
        
        // Check if we have to choose a culprit
        Assert.AreEqual(SceneManager.GetSceneByName("EpilogueScene"), SceneManager.GetSceneAt(1));
        
        // Choose one of the characters
        GameObject.Find("Portrait Button(Clone)").GetComponent<Button>().onClick.Invoke();

        // Check if we are in the Dialogue scene
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);
        Assert.AreEqual(SceneManager.GetSceneByName("DialogueScene"), SceneManager.GetSceneAt(2));
        
        // Skip dialogue until first reflection moment
        while (GameObject.Find("InputField") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until second reflection moment
        while (GameObject.Find("InputField") == null)
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
        
        // Click on submit input button
        yield return new WaitForSeconds(1);
        GameObject.Find("Submit input Button").GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        // Skip dialogue until GameOver
        while (SceneManager.GetSceneAt(1) != SceneManager.GetSceneByName("GameOverScene"))
        {
            yield return new WaitForSeconds(1);
                
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }

        // Check if we are in the GameOver scene
        yield return new WaitForSeconds(3);
        Assert.AreEqual(SceneManager.GetSceneAt(1), SceneManager.GetSceneByName("GameOverScene"));

        yield return null;
    }

    #region playing through the game
    /// <summary>
    /// Plays the game from the start (clicking new story) until the story select screen
    /// </summary>
    private IEnumerator PlayUntilStorySelect()
    {
        // Find the New Game button and click it
        GameObject.Find("NewGameButton").GetComponent<Button>().onClick.Invoke();

        // Choose to view the prologue
        GameObject.Find("YesButton").GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("PrologueScene").isLoaded);

        // Check if we are in the prologue
        Assert.AreEqual(SceneManager.GetSceneByName("PrologueScene"), SceneManager.GetActiveScene());

        yield return PlayThroughScene("PrologueScene", new List<string>(){"Button parent"});
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("StorySelectScene").isLoaded);
        
        // Check if we are in the StorySelect scene
        Assert.AreEqual(SceneManager.GetSceneByName("StorySelectScene"), SceneManager.GetActiveScene());
        
    }
    
    /// <summary>
    /// Selects a story and plays through the story intro until npcSelect
    /// </summary>
    private IEnumerator SelectStoryAndPlayUntilNPCSelect(int storyID)
    {
        string buttonName = "";

        switch (storyID)
        {
            case 0:
                buttonName = "StoryA_Button";
                break;
            case 1:
                buttonName = "StoryB_Button";
                break;
            case 2:
                buttonName = "StoryC_Button";
                break;
        }

        // Select story
        GameObject.Find(buttonName).GetComponent<Button>().onClick.Invoke();
        yield return new WaitUntil(() => SceneManager.GetSceneByName("IntroStoryScene").isLoaded);
        
        // Check if we are in the story intro
        Assert.AreEqual(SceneManager.GetSceneByName("IntroStoryScene"), SceneManager.GetActiveScene());
        
        yield return PlayThroughScene("IntroStoryScene", new List<string>(){"ContinueButton"});
        
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);

        // Check if we are in the NPC Select scene
        Assert.AreEqual(SceneManager.GetSceneByName("NPCSelectScene"), SceneManager.GetSceneAt(1));
    }

    private IEnumerator PlayTutorial() => PlayThroughScene("TutorialScene",
        new List<string>() { "ContinueButton", "Notebook Button" });

    /// <summary>
    /// plays through a scene by clicking the given buttons
    /// </summary>
    private IEnumerator PlayThroughScene(string sceneName, List<string> buttonsToClick)
    {
        while (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            // Wait a second, otherwise the test crashes
            yield return null;

            for (int i = 0; i < buttonsToClick.Count; i++)
                if (GameObject.Find(buttonsToClick[i]) != null)
                {
                    GameObject.Find(buttonsToClick[i]).GetComponent<Button>().onClick.Invoke();
                    break;
                }
        }
    }
    #endregion
    
    #region dialogue
    /// <summary>
    /// Select a npc that has questions left.
    /// </summary>
    /// <param name="emptyQuestionCharacters"> The list of characters that have no questions left. </param>
    /// <param name="currentCharacters"> The list of currentCharacters. </param>
    public IEnumerator SelectNpc(List<CharacterInstance> emptyQuestionCharacters, List<CharacterInstance> currentCharacters)
    {
        //initialise greeted
        if (greeted.Count == 0)
            greeted.AddRange(currentCharacters.Select(_ => false));
        
        float swipeDuration = GameObject.Find("Scroller").GetComponent<NPCSelectScroller>()
            .scrollDuration;
        // Start at the leftmost character
        while (GameObject.Find("NavLeft"))
        {
            GameObject.Find("NavLeft").GetComponent<Button>().onClick.Invoke(); 
            yield return new WaitForSeconds(swipeDuration);
        }

        yield return new WaitUntil(() =>
            GameObject.Find("Confirm Selection Button") is not null &&
            GameObject.Find("Confirm Selection Button").GetComponent<GameButton>() is not null);
        
        // Find an active character and click to choose them
        foreach (CharacterInstance c in currentCharacters)
        {
            if (c.isActive && emptyQuestionCharacters.All(emptyC => emptyC.characterName != c.characterName))
            {
                GameObject.Find("Confirm Selection Button").GetComponent<GameButton>().onClick.Invoke();
                break;
            }

            if (GameObject.Find("NavRight"))
            {
                GameObject.Find("NavRight").GetComponent<Button>().onClick.Invoke();
                yield return new WaitForSeconds(swipeDuration);
            }
            else
            {
                throw new Exception("There are no active characters");
            }
        }
        
        //yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("DialogueScene").isLoaded);

        // Check if we are in the Dialogue scene
        Assert.IsTrue(SceneManager.GetSceneByName("DialogueScene").isLoaded);
        
        //wait for events to fire and the currentRecipient to be assigned
        yield return new WaitWhile(() =>
            GameObject.Find("DialogueManager").GetComponent<DialogueManager>().currentRecipient is null);

        var rep = GameObject.Find("DialogueManager").GetComponent<DialogueManager>()
            .currentRecipient;
        
        //check if we need to do a greeting
        if (!greeted[currentCharacters.FindIndex(cc => rep.id == cc.id)])
        {
            greeted[currentCharacters.FindIndex(cc => rep.id == cc.id)] = true;
            //do a greeting
            yield return KeepTalking();
            
            yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
            
            //select an npc again
            yield return SelectNpc(emptyQuestionCharacters, currentCharacters);
        }
    }
    
    /// <summary>
    /// In dialogueScene, keep talking 
    /// </summary>
    /// <returns></returns>
    private IEnumerator KeepTalking()
    {
        // Wait until you can ask a question, or the dialogue ends
        while (GameObject.Find("Questions Field") == null && SceneManager.GetSceneByName("DialogueScene").isLoaded)
        {
            yield return null;
            if (GameObject.Find("Skip Dialogue Button") != null)
                GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick.Invoke();
        }
    }

    private IEnumerator FullConversation(int numQuestions, List<CharacterInstance> emptyQuestionCharacters)
    {
        // Ask a certain number of questions
        for (int j = 0; j < numQuestions; j++)
        {
            // Wait until you can ask a question
            yield return KeepTalking();

            //wait for the button to load
            yield return new WaitWhile(() =>
                GameObject.Find("questionButton").GetComponent<Button>() is null);
            
            GameObject.Find("questionButton").GetComponent<Button>().onClick.Invoke();

            // Get the DialogueManager
            var dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
            
            // If the character has no more questions remaining, add the character to the list of emptyQuestionCharacters.
            if (dm.currentRecipient.RemainingQuestions.Count == 0)
                emptyQuestionCharacters.Add(dm.currentRecipient);
            
            // The final iteration of the loop should continue to the hint scene
            if (j == numQuestions - 1)
                // Wait for hint scene to be over
                while (!SceneManager.GetSceneByName("NPCSelectScene").isLoaded && !SceneManager.GetSceneByName("EpilogueScene").isLoaded)
                {
                    yield return null;

                    // Go through hint scene if it's active, else go through dialogue scene
                    if (GameObject.Find("Phone Dialogue Field") != null)
                    {
                        if (GameObject.Find("Next Dialogue Button") != null)
                            GameObject.Find("Next Dialogue Button").GetComponent<Button>().onClick
                                .Invoke();
                    }
                    else if (GameObject.Find("Skip Dialogue Button") != null)
                        GameObject.Find("Skip Dialogue Button").GetComponent<Button>().onClick
                            .Invoke();
                }
        }
    }
    
    
    #endregion
}