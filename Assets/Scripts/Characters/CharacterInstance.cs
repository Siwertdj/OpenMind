// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An instance of a character. Takes a set of <see cref="CharacterData"/>
/// Each character in the game is a different a separate instance of this class. With its own <see cref="CharacterData"/>.
/// </summary>
public class CharacterInstance
{
    public CharacterData data;

    public Dictionary<Question, List<string>> Answers = new();
    public Dictionary<Question, List<string>> Traits = new();
    public List<Question> RemainingQuestions = new();
    public List<Question> AskedQuestions = new();

    public string characterName;
    public int id;
    public Sprite avatar;
    public float pitch;

    public bool isCulprit;      // This character is the culprit and a random characteristic is revealed every cycle
    public bool isActive;       // If they havent yet been the victim, should be true. Use this to track who is "alive" and you can talk to, and who can be removed by the culprit
    public bool TalkedTo;       // If the player has already talked to this NPC in the current cycle, should be false at the start of every cycle and set to true once the player has talked with them
    
    /// <summary>
    /// The constructor for <see cref="CharacterInstance"/>.
    /// Sets this instances variables to the information from <see cref="data"/> 
    /// </summary>
    /// <param name="data">A set of <see cref="CharacterData"/></param>
    public CharacterInstance(CharacterData data)
    {
        this.data = data;
        characterName = data.characterName;
        id = data.id;
        avatar = data.avatar;
        pitch = data.voicePitch;

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
    /// Get the epilogue dialogue depending on the players choice at the end of the game.
    /// </summary>
    /// <param name="hasWon">Whether the player has chosen the correct character at the end</param>
    /// <returns> Returns a list of list with type string, where after every list with type string an open question will be asked. </returns>
    public List<List<string>> GetEpilogueDialogue(bool hasWon)
    {
        if (hasWon)
            return EpilogueWinScenario();
        else
            return EpilogueLoseScenario();
    }

    /// <summary>
    /// Helper function for <see cref="GetEpilogueDialogue"/> which gives the scenario for when the player guesses the culprit.
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
    /// Helper function for <see cref="GetEpilogueDialogue"/> which gives the scenario for when the player does not guess the correct culprit
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
    /// <returns>A list of type string containing all traits of this character</returns>
    private List<string>[] GetAllTraits()
    {
        return Traits.Values.ToArray();
    }

    /// <summary>
    /// Helper function for the constructor.
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
            
            // Remove question from all characters so that it can not be asked to anyone, if RemainingQuestions contains it.
            foreach (CharacterInstance character in GameManager.gm.currentCharacters)
            {
                if (character.RemainingQuestions.Contains(question))
                {
                    character.RemainingQuestions.Remove(question);
                }
            }

            // Return the answer to the question in trait form
            return Traits[question];
        }
        else
        {
            // In a normal game loop, this should never occur
            Debug.LogError("GetRandomTrait(), but there are no more traits remaining. " +
                           "Game lasted to long/ character needs more questions");
            return null;
        }
    }
}
