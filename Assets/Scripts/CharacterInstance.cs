using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInstance
{
    private CharacterData data;

    public Dictionary<Question, List<string>> Answers = new();
    public List<Question> RemainingQuestions = new();

    public string characterName;
    public int id;
    public Sprite avatar;
    public float pitch;

    public bool isCulprit;      // This character's random characteristic is revealed every cycle
    public bool isActive;       // If they havent yet been the victim, should be true. Use this to track who is "alive" and you can talk to, and who can be removed by the culprit
    public bool TalkedTo;       // If the player has already talked to this NPC in the current cycle, should be false at the start of every cycle and set to true once the player has talked with them

    public CharacterInstance(CharacterData data)
    {
        this.data = data;

        characterName = data.characterName;
        id = data.id;
        avatar = data.avatar;
        pitch = data.voicePitch;

        Debug.Log($"Creating character {data.characterName}");

        InitializeQuestions();
    }

    public List<string> GetGreeting()
    {
        return new() { "Hello" };
    }

    /// <summary>
    /// Scenario for when the player guesses the culprit.
    /// </summary>
    /// <returns> Returns a list of list with type string, where after every list with type string an open question will be asked. </returns>
    public List<List<string>> EpilogueWinScenario()
    {
        // TODO: change the 6th sentence based on if their intermediate and final choice are different.
        List<string> speakingText1 = new List<string>()
        {
            "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName,
            "I was indeed the one who kept sending u messages.",
            "and in fact, I knew that you did not know who",
            "was sending the messages. I also got hold of",
            "the results of the intermediate test. You managed",
            "to guess correctly, and so i wanted to ask you the following:", // this one is the 6th sentence
            "Why did u change ur choice compared to the intermediate decision?"
        };
        List<string> speakingText2 = new List<string>()
        {
            "Okay, thats very interesting!",
            "Seems like getting more information really does change ur choice.",
            "bibidibop."
        };
        List<string> speakingText3 = new List<string>()
        {
            "alright very cool.",
            "I have to go now.",
            "I do not want to miss the bus.",
            "Goodbye."
        };
        // List of lists, where in between each list a DialogueObject can be called.
        List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3};
        return retval;
    }

    /// <summary>
    /// Scenario for when the player does not guess the correct culprit
    /// </summary>
    /// <returns> Returns a list of list with type string, where after every list with type string an open question will be asked. </returns>
    public List<List<string>> EpilogueLoseScenario()
    {
        List<string> speakingText1 = new List<string>()
        { 
            "Hi I'm " + GameManager.gm.FinalChosenCuplrit.characterName,
            "I was indeed the one who kept sending u messages.",
            "and in fact, I knew that you did not know who",
            "was sending the messages. I also got hold of",
            "the results of the intermediate test. You managed",
            "to guess correctly, and so i wanted to ask you the following:", // this one is the 6th sentence
            "Why did u change ur choice compared to the intermediate decision?" 
        };
        List<string> speakingText2 = new List<string>()
        { 
            "Okay, thats very interesting!",
            "Seems like getting more information really does change ur choice.",
            "bibidibop." 
        };
        List<string> speakingText3 = new List<string>()
        { 
            "alright very cool.",
            "I have to go now.",
            "I do not want to miss the bus.",
            "Goodbye." 
        };
        // List of lists, where in between each list a DialogueObject can be called.
        List<List<string>> retval = new List<List<string>>(){speakingText1, speakingText2, speakingText3};
        return retval;
    }

    /// <summary>
    /// Gets all traits of this character, can be modified later if traits are stored differently
    /// </summary>
    private List<List<string>> GetAllTraits()
    {
        return Answers.Values.ToList();
    }

    public void InitializeQuestions()
    {
        foreach (var kvp in data.answers)
        {
            Answers[kvp.question] = kvp.answer;
            RemainingQuestions.Add(kvp.question);
        }
    }
    
    /// <summary>
    /// The logic for obtaining a random trait and removing it from the list of available question of that character.
    /// If the random variable is left null, it will be obtained from gameManager, but it can be provided for slight optimization.
    /// THis method is used for obtaining hints about the victim and the culprit at the start of each cycle.
    /// </summary>
    public List<string> GetRandomTrait()
    {  
        // NOTE: Sander idk hoe je code werkt maar ik heb het zo voor nu <3
        //if (random == null)
        //    random = FindObjectOfType<GameManager>().random;

        //return allTraits[random.Next(allTraits.Count)];

        if (RemainingQuestions.Count > 0)
        {

            int randomInt = new System.Random().Next(RemainingQuestions.Count);
            Question question = RemainingQuestions[randomInt];
            RemainingQuestions.RemoveAt(randomInt);

            // TODO: add question-text to the answer that is returned
            return (Answers[question]);
        }
        else
        {
            List<string> output = new List<string>();
            output.Add("No clues were found..");
            return (output);
        }
    }
}
