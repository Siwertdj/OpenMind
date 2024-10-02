using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInstance
{
    public CharacterData data;

    public Dictionary<Question, List<string>> Answers = new();
    public List<Question> RemainingQuestions = new();
    public List<Question> AnsweredQuestions = new();
    
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
            Debug.Log("THis code yeeeeh");
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
