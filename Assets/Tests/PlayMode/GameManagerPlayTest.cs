using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameManagerPlayTest
{
    /// <summary>
    /// Checks if the character list gets populated.
    /// </summary>
    [UnityTest]
    public IEnumerator PopulateCharactersTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for scene to load

        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Count;
        var actual = gm.numberOfCharacters;

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if the characters all get set to active during populating
    /// </summary>
    [UnityTest]
    public IEnumerator ActiveCharactersTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for scene to load

        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Count(c => c.isActive);
        var actual = gm.numberOfCharacters;

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if one culprit gets chosen during populating
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseCulpritTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for scene to load

        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Count(c => c.isCulprit);
        var actual = 1;

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }
    
    /// <summary>
    /// Checks if the "HasQuestionsLeft" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator HasQuestionsLeftTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");

        // Get GameManager object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.AmountOfQuestionsRemaining() >= 1;
        var actual = gm.HasQuestionsLeft();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetCulpritTest" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator GetCulpritTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Find(c => c.isCulprit);
        var actual = gm.GetCulprit();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "EnoughCharacters" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator EnoughCharactersTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Set up expected and actual values
        var expected = gm.currentCharacters.Count(c => c.isActive) > 2;
        var actual = gm.EnoughCharactersRemaining();

        // Check if they are equal
        Assert.AreEqual(expected, actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "RetryStoryScene" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator RetryStoryTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
        gm.RetryStoryScene();
        
        // Set up actual value
        var actual = gm.currentCharacters.Count(c => c.isActive) == gm.currentCharacters.Count();

        // Check if it holds
        Assert.IsTrue(actual);
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "GetRandomVictimNoCulprit" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator ChooseVictimTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Get victim
        var victim = gm.GetRandomVictimNoCulprit();

        // Check if it actually returned a victim
        Assert.IsTrue(victim != null);
        
        yield return null;
    }

    static bool[] bools = new bool[] { true, false };

    /// <summary>
    /// Checks if the "EndCycle" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator EndCycleTest([ValueSource(nameof(bools))] bool enoughCharacters)
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

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
        
        // Variable which counts the number of characters before calling EndCycle.
        int nCharactersPrior = gm.currentCharacters.Count;
        
        // End cycle
        gm.EndCycle();
        
        // Variable which counts the number of characters after calling EndCycle.
        int nCharactersPosterior = gm.currentCharacters.Count;
        
        // We test whether a character disappears when EndCycle is called and there are enough characters.
        // If there are not enough characters, then we test if we transition to the culpritSelect gameState
        // and if a character does not disappear.
        // TODO: check if we are in the correct scene (NpcSelect).
        if (enoughCharacters)
        {
            // Check if only 1 character has disappeared.
            Assert.AreEqual(nCharactersPrior - 1, nCharactersPosterior);
            // TODO: close the scenes after each test (else crash :( ).
        }
        else
        {
            // Check if no characters have disappeared.
            Assert.AreEqual(nCharactersPrior, nCharactersPosterior);
            // Check if the gameState transitions to culpritSelect.
            // TODO: In the current version, the gameState "culpritSelect" is never used, which should be used.
            //Assert.AreEqual(gm.gameState, GameManager.GameState.CulpritSelect);
        }
        
        // Get current scene
        //var scene = SceneManager.GetActiveScene().name;
        
        //Debug.Log(scene);

        // See if it's still equal to the "main" scene of the game
        // No scene should be switched, because it's an additive scene
        //Assert.AreEqual("Loading", scene);
        
        
        
        
        
        
        yield return null;
    }

    /// <summary>
    /// Checks if the "StartDialogue" method works.
    /// </summary>
    [UnityTest]
    public IEnumerator StartDialogueTest()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(3); // Wait for it to load

        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();

        // Get character to start dialogue with
        var character = gm.currentCharacters[0];
        gm.StartDialogue(character);

        // Get current scene
        var scene = SceneManager.GetActiveScene().name;

        // See if it's still equal to the "main" scene of the game
        // No scene should be switched, because it's an additive scene
        Assert.AreEqual("Loading", scene);
        
        yield return null;
    }

    /// <summary>
    /// Check whether the transition between culprit selection and epilogue works as intended.
    /// TODO: check if we are in the dialogue scene.
    /// </summary>
    [UnityTest]
    public IEnumerator CulpritEpilogueTransition()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
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
        gm.StartGame();
        yield return new WaitForSeconds(2); // Wait for it to load
        // Set this to maxValue in order to make sure that no more questions can be asked.
        gm.numQuestionsAsked = int.MaxValue;
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply
        // the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "DialogueManager" object
        var d = GameObject.Find("DialogueManager");
        var dm = d.GetComponent<DialogueManager>();
        dm.BacktoNPCScreen();
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "SelectionManager" object.
        var s = GameObject.Find("SelectionManager");
        var sm = s.GetComponent<SelectionManager>();
        
        bool culpritGameObjectFound = false;
        int counter = 1;
        // Find the GameObject that corresponds with the culprit.
        while (!culpritGameObjectFound)
        {
            string path = "characterspace " + counter;
            GameObject go = GameObject.Find(path);
            SelectOption selectOption = go.GetComponentInChildren<SelectOption>();
            if (selectOption.character.characterName == gm.GetCulprit().characterName)
            {
                culpritGameObjectFound = true;
                GameObject culpritObject = go;
                // Simulate choosing the culprit.
                sm.ButtonClicked(culpritObject);
            }
            counter++;
        }
        
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Check whether the hasWon variable is set to true and if the gameState is switched to epilogue.
        Assert.IsTrue(gm.hasWon);
        Assert.AreEqual(gm.gameState, GameManager.GameState.Epilogue);
        
        yield return null;
    }
    
    /// <summary>
    /// Check whether the transition between culprit selection and epilogue works as intended.
    /// TODO: check if we are in the dialogue scene.
    /// </summary>
    [UnityTest]
    public IEnumerator InnocentEpilogueTransition()
    {
        // Load scene
        SceneManager.LoadScene("Loading");
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "GameManager" object
        var g = GameObject.Find("GameManager");
        var gm = g.GetComponent<GameManager>();
        
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
        gm.StartGame();
        yield return new WaitForSeconds(2); // Wait for it to load
        // Set this to maxValue in order to make sure that no more questions can be asked.
        gm.numQuestionsAsked = int.MaxValue;
        // Start dialogue with a character, then go back to NpcSelect scene in order to apply
        // the changes of the variables.
        CharacterInstance character = gm.currentCharacters[0];
        gm.StartDialogue(character);
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "DialogueManager" object
        var d = GameObject.Find("DialogueManager");
        var dm = d.GetComponent<DialogueManager>();
        dm.BacktoNPCScreen();
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Get "SelectionManager" object.
        var s = GameObject.Find("SelectionManager");
        var sm = s.GetComponent<SelectionManager>();
        
        bool innocentGameObjectFound = false;
        int counter = 1;
        // Find the GameObject that corresponds with an innocent person.
        while (!innocentGameObjectFound)
        {
            string path = "characterspace " + counter;
            GameObject go = GameObject.Find(path);
            SelectOption selectOption = go.GetComponentInChildren<SelectOption>();
            // Choose an innocent person that is not dead and is not the culprit.
            if (selectOption.character.characterName != gm.GetCulprit().characterName && selectOption.character.isActive)
            {
                innocentGameObjectFound = true;
                GameObject innocentObject = go;
                // Simulate choosing the culprit.
                sm.ButtonClicked(innocentObject);
            }
            counter++;
        }
        
        yield return new WaitForSeconds(2); // Wait for it to load
        
        // Check whether the hasWon variable is set to true and if the gameState is switched to epilogue.
        Assert.IsFalse(gm.hasWon);
        Assert.AreEqual(gm.gameState, GameManager.GameState.Epilogue);
        
        yield return null;
    }
    
}
