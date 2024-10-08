using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInstance
{
    private CharacterData data;

    public Dictionary<Question, List<string>> Answers = new();
    public Dictionary<Question, List<string>> Traits = new();
    public List<Question> RemainingQuestions = new();

    public List<string>[] greetings;

    public string characterName;
    public int id;
    public Sprite avatar;
    public float pitch;

    public bool isCulprit;      // This character's random characteristic is revealed every cycle
    public bool isActive;       // If they havent yet been the victim, should be true. Use this to track who is "alive" and you can talk to, and who can be removed by the culprit
    public bool TalkedTo;       // If the player has already talked to this NPC in the current cycle, should be false at the start of every cycle and set to true once the player has talked with them

    public CharacterInstance(CharacterData data)
    {
        Debug.Log($"Creating character {data.characterName}");

        this.data = data;

        characterName = data.characterName;
        id = data.id;
        avatar = data.avatar;
        pitch = data.voicePitch;

        //Debug.Log($"Creating character {data.characterName}");

        InitializeQuestions();
    }

    /// <summary>
    /// Get a random greeting from the character's list of greetings.
    /// </summary>
    /// <returns>A greeting in the form of dialogue lines.</returns>
    public List<string> GetGreeting()
    {
        // Pick random greeting from data list
        if (data.greetings != null && data.greetings.Length > 0)
        {
            int randomInt = new System.Random().Next(data.greetings.Length);
            return data.greetings[randomInt].lines;
        }

        // If no greeting was found, return default greeting
        return new() { "Hello" };
    }

    /// <summary>
    /// Get the epilogue dialogue depending on whether the player has chosen the correct character or not.
    /// </summary>
    /// <param name="hasWon"></param>
    /// <returns></returns>
    public List<List<string>> GetEpilogueDialogue(bool hasWon)
    {
        if (hasWon)
            return EpilogueWinScenario();
        else
            return EpilogueLoseScenario();
    }

    /// <summary>
    /// Scenario for when the player guesses the culprit.
    /// </summary>
    /// <returns> Returns a list of list with type string, where after every list with type string an open question will be asked. </returns>
    private List<List<string>> EpilogueWinScenario()
    {
        List<string> speakingText1 = new List<string>()
        {
            "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName,
            "I was indeed the one who kept sending u messages.",
            "and in fact, I knew that you did not know who",
            "was sending the messages.",
            "You managed to guess correctly, and so i wanted to ask you the following:",
            "Why did u think I was the one who kept sending u messages?"
        };
        List<string> speakingText2 = new List<string>()
        {
            "Okay, thats very interesting!",
            "Now I have another question for you:",
            "Which characteristics of the character resemble with urself?"
        };
        List<string> speakingText3 = new List<string>()
        {
            "alright very cool.",
            "I have to go now.",
            "I do not want to miss the bus.",
            "Goodbye."
        };
        // List of lists, where in between each list an OpenResponseObject will be called.
        List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3};
        return retval;
    }

    /// <summary>
    /// Scenario for when the player does not guess the correct culprit
    /// </summary>
    /// <returns> Returns a list of list with type string, where after every list with type string an open question will be asked. </returns>
    private List<List<string>> EpilogueLoseScenario()
    {
        List<string> speakingText1 = new List<string>()
        { 
            "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName,
            "You are asking me if I was sending u messages?",
            "I am sorry but i do not know what u are talking about.",
            "I have to go now, bye."
        };
        List<string> speakingText2 = new List<string>()
        { 
            "Well that was pretty awkward, wasn't it?",
            "I'm " + GameManager.gm.GetCulprit().characterName,
            "I am the one who kept sending u messages.",
            "and in fact, I knew that you did not know who",
            "was sending the messages.",
            "You did not guess correctly unfortunately.",
            "Despite that, I still wanted to to ask you the following:",
            "Why did u think I was the one who kept sending u messages?"
        };
        List<string> speakingText3 = new List<string>()
        { 
            "Okay, thats very interesting!",
            "Now I have another question for you:",
            "Which characteristics of the character resemble with urself?"
        };
        List<string> speakingText4 = new List<string>()
        { 
            "alright very cool.",
            "I have to go now.",
            "I do not want to miss the bus.",
            "Goodbye." 
        };
        // List of lists, where in between each list an OpenResponseObject will be called.
        List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3, speakingText4};
        return retval;
    }

    /// <summary>
    /// Gets all traits of this character, can be modified later if traits are stored differently
    /// </summary>
    private List<string>[] GetAllTraits()
    {
        return Traits.Values.ToArray();
    }

    /// <summary>
    /// Places character data (answers & traits) in their respective dictionaries.
    /// </summary>
    public void InitializeQuestions()
    {
        foreach (var kvp in data.answers)
        {
            Answers[kvp.question] = kvp.answer;
            Traits[kvp.question] = kvp.trait;
            RemainingQuestions.Add(kvp.question);
        }
    }
    
    /// <summary>
    /// The logic for obtaining a random trait and removing it from the list of available questions for all characters.
    /// If the random variable is left null, it will be obtained from gameManager, but it can be provided for slight optimization.
    /// This method is used for obtaining hints about the victim and the culprit at the start of each cycle.
    /// </summary>
    /// <returns>A List of strings containing a random trait of this character.</returns>
    public List<string> GetRandomTrait()
    {
        // If there are any questions remaining
        if (RemainingQuestions.Count > 0)
        {
            // Find a random question
            int randomInt = GameManager.gm.random.Next(RemainingQuestions.Count);
            Question question = RemainingQuestions[randomInt];

            // Remove question from all characters so that it can not be asked to anyone
            foreach (CharacterInstance character in GameManager.gm.currentCharacters)
                character.RemainingQuestions.Remove(question);

            // Return the answer to the question in trait form
            return Traits[question];
        }
        else
        {
            // In a normal game loop, this should never occur
            Debug.LogError("GetRandomTrait(), but there are no more traits remaining");

            return null;
        }
    }
}
