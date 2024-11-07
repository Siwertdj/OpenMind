using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Random = System.Random;

/// <summary>
/// This class tests all read and write related stuff while the game is running. This class:
/// - Assigns specific values for each variable in the SaveData class & saves these contents
/// - Then checks whether the loaded contents are the same
/// - Then checks whether every variable is assigned correctly
/// - Then saves gain and loads again and checks whether every variable is assigned correctly, this tests whether saving is correct
/// </summary>
public class SavingLoadingTestValueReadAndWrite
{
    Random          random = new Random();
    private Saving  saving  => GameManager.FindObjectOfType<Saving>();
    private Loading loading => GameManager.FindObjectOfType<Loading>();
    
    [UnitySetUp]
    private IEnumerator Initialise()
    {
        SceneManager.LoadScene("TestingScene");
        yield return new WaitUntil(() => SceneManager.GetSceneByName("TestingScene").isLoaded);
        
        //create gamemanager without initialising it
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        
        //initialise gamemanager
        StoryObject story = Resources.LoadAll<StoryObject>("Stories")[0];
        GameManager.gm.StartGame(null, story);
        yield return new WaitUntil(() => SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
    }
    
    [UnityTearDown]
    public IEnumerator RemoveGameManager()
    {
        SceneManager.LoadScene("TestingScene");
        yield return new WaitUntil(() => SceneManager.loadedSceneCount == 1);
    }
    
    private bool RandB() => RandI(2) == 0;
    private int RandI(int max) => (int)(random.NextDouble() * max);
    private T RandL<T>(IList<T> list) => list[RandI(list.Count)];
    private char RandC() => (char)RandI(128);
    private string RandS(int length) => length <= 1 ? RandC().ToString(): RandC() + RandS(length - 1); 
    
    /// <summary>
    /// Creates a test variant of the savedata with randomly assigned values
    /// </summary>
    private SaveData CreateSaveData()
    {
        StoryObject[] stories = Resources.LoadAll<StoryObject>("Stories");
        int storyId = RandI(stories.Length);
        
        int[] possibleCharacterIds =  
            (typeof(GameManager)
                .GetField("characters", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(GameManager.gm) as List<CharacterData>).Select(cd => cd.id).ToArray();
        
        RandList<int> possibleCharacterIdsRandom = new RandList<int>(random, possibleCharacterIds);
        int totalCharacters = stories[storyId].numberOfCharacters;
        int activeCharacterCount = RandI(totalCharacters) + 1;
        int inactiveCharacterCount = totalCharacters - activeCharacterCount;
        
        int[] activeCharacterIds = new int[activeCharacterCount];
        for (int i = 0; i < activeCharacterCount; i++)
            activeCharacterIds[i] = possibleCharacterIdsRandom.GetNext();
        
        int[] inactiveCharacterIds = new int[inactiveCharacterCount];
        for (int i = 0; i < inactiveCharacterCount; i++)
            inactiveCharacterIds[i] = possibleCharacterIdsRandom.GetNext();
        
        int culpritId = RandL(possibleCharacterIds);
        
        Question[] possibleQuestions = Enum.GetValues(typeof(Question)).Cast<Question>().ToArray();
        
        int numQuestionsAsked = 0;
        
        List<(int, List<Question>)> askedQuestions = new List<(int, List<Question>)>();
        List<(int, List<Question>)> remainingQuestions = new List<(int, List<Question>)>();
        List<(int, string)> characterNotes = new List<(int, string)>();
        
        foreach (int characterId in possibleCharacterIds)
        {
            if (!activeCharacterIds.Contains(characterId) && !inactiveCharacterIds.Contains(characterId))
                continue;
            
            int totalQuestions = stories[storyId].numQuestions;
            Question[] questions = new Question[totalQuestions];
            for (var i = 0; i < questions.Length; i++)
                questions[i] = RandL(possibleQuestions);
            
            List<Question> asked = new List<Question>();
            List<Question> remaining = new List<Question>();
            
            for (int i = 0; i < totalQuestions; i++)
                if (RandB())
                    asked.Add(questions[i]);
                else
                    remaining.Add(questions[i]);
            
            askedQuestions.Add((characterId, asked));
            remainingQuestions.Add((characterId, remaining));
            characterNotes.Add((characterId, RandS(128)));
            
            numQuestionsAsked += asked.Count;
        }
        
        SaveData saveData = new SaveData
        {
            storyId = storyId,
            activeCharacterIds = activeCharacterIds,
            inactiveCharacterIds = inactiveCharacterIds,
            culpritId = culpritId,
            remainingQuestions = remainingQuestions.ToArray(),
            askedQuestionsPerCharacter = askedQuestions.ToArray(),
            personalNotes = RandS(128),
            numQuestionsAsked = numQuestionsAsked,
            characterNotes = characterNotes.ToArray()
        };
        
        return saveData;
    }
    
    private void ListEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2, string msg, Action<T, T, string> comparer = null)
    {
        List<T> l1 = list1.ToList();
        List<T> l2 = list2.ToList();
        Assert.AreEqual(l1.Count, l2.Count, msg + ": count");
        
        
        for (int i = 0; i < l1.Count; i++)
        {
            bool found = false;
            
            for (int j = 0; j < l2.Count; j++)
                if (comparer is null)
                    found |= l1[i].Equals(l2[j]);
                else
                    comparer(l1[i], l2[i], msg);
            
            if (comparer is null)
                Assert.IsTrue(found, msg + ": item " + l1[i]);
        }
            
    }
    
    private void CompareQuestionList((int, List<Question>) item1, (int, List<Question>) item2, string msg)
    {
        Assert.AreEqual(item1.Item1, item2.Item1, msg + ": characterID");
        ListEquals(item1.Item2, item2.Item2, msg + ": questionList");
    }
    
    private void CompareStringTuple((int, string) item1, (int, string) item2, string msg)
    {
        Assert.AreEqual(item1.Item1, item2.Item1, msg + ": characterID");
        Assert.AreEqual(item1.Item2, item2.Item2, msg + ": characterNotes");
    }
    
    private void CompareSaveData(SaveData sd1, SaveData sd2, string msg)
    {
        Assert.AreEqual(sd1.storyId, sd2.storyId, msg + ":storyId");
        ListEquals(sd1.activeCharacterIds, sd2.activeCharacterIds, msg + ":activeCharacterIds");
        ListEquals(sd1.inactiveCharacterIds, sd2.inactiveCharacterIds, msg + ":inactiveCharacterIds");
        Assert.AreEqual(sd1.culpritId, sd2.culpritId, msg + ":culpritId");
        ListEquals(sd1.remainingQuestions, sd2.remainingQuestions, msg + ":remainingQuestions",
            CompareQuestionList);
        Assert.AreEqual(sd1.personalNotes, sd2.personalNotes, msg + ":personalNotes");
        ListEquals(sd1.characterNotes, sd2.characterNotes, msg + ":characterNotes", CompareStringTuple);
        ListEquals(sd1.askedQuestionsPerCharacter, sd2.askedQuestionsPerCharacter,
            msg + ":askedQuestionsPerCharacter", CompareQuestionList);
        Assert.AreEqual(sd1.numQuestionsAsked, sd2.numQuestionsAsked, msg + ":numQuestionsAsked");
    }
    
    
    /// <summary>
    /// Tests whether saving and loading a SaveData object returns the same object
    /// </summary>
    [Test]
    public void SavingLoadingDoesNotChangeContents()
    {
        SaveData saveData = CreateSaveData();
        saving.Save(saveData);
        SaveData loaded = loading.GetSaveData();
        
        CompareSaveData(saveData, loaded, "compare change");
    }
    
    /// <summary>
    /// Tests whether loading a SaveData object into the gamemanager returns no errors
    /// </summary>
    [Test]
    public void LoadingIntoGamemanagerReturnsNoErrors()
    {
        SaveData saveData = CreateSaveData();
        GameManager.gm.StartGame(null, saveData);
    }
    
    /// <summary>
    /// Tests whether retrieving a SaveData object from the gamemanager returns no errors
    /// </summary>
    [Test]
    public void RetrievingFromGamemanagerReturnsNoErrors()
    {
        saving.CreateSaveData();
    }
    
    /// <summary>
    /// Tests whether saving and loading repeatedly does nothing to change the savedata or the gamestate
    /// </summary>
    [UnityTest]
    public IEnumerator SavingLoadingDoesNotChangeGameState()
    {
        SaveData saveData = //loading.GetSaveData();
                            CreateSaveData();
        
        for (int i = 0; i < 3; i++)
        {
            saving.Save(saveData);
            SaveData loaded = loading.GetSaveData();
            GameManager.gm.StartGame(null, loaded);
            SaveData retrieved = saving.CreateSaveData();
            
            CompareSaveData(loaded, retrieved, "compare " + i);
            
            SceneManager.UnloadSceneAsync("NPCSelectScene");
            yield return new WaitUntil(
                () => !SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        }
    }
}

/// <summary>
/// A collection of unique random items
/// </summary>
class RandList<T> : IEnumerable<T>
{
    private Random  random;
    private List<T> remaining = new ();
    
    public RandList(Random random, IEnumerable<T> collection)
    {
        this.random = random;
        remaining.AddRange(collection);
    }
    
    public T GetNext()
    {
        int index = random.Next(remaining.Count);
        T item = remaining[index];
        remaining[index] = remaining[^1];
        remaining.RemoveAt(remaining.Count-1);
        return item;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        while (remaining.Count > 0)
            yield return GetNext();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
