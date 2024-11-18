// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Windows;
using Property = NUnit.Framework.PropertyAttribute;

/// <summary>
/// This class tests all filepath related stuff in the saving and loading files. This class:
/// - Saves contents to a file and checks whether no errors are thrown while saving
/// - Loads contents from a file and checks whether no errors are thrown
/// </summary>
public class SavingLoadingTestFilePaths
{
    [OneTimeSetUp]
    public void LoadTestingScene()
    {
        SceneManager.LoadScene("TestingScene");
    }
    
    [OneTimeTearDown]
    public void UnloadTestingScene()
    {
        SceneManager.UnloadSceneAsync("TestingScene");
    }
    
    [UnitySetUp]
    public IEnumerator Initialise()
    {
        int layer = (int)TestContext.CurrentContext.Test.Properties.Get("layer");
        if (layer > 0)
        {
            Debug.Log("hi");
            //create gamemanager without initialising it
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
            yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
        }
        
        if (layer > 1)
        {
            //initialise gamemanager
            StoryObject story = Resources.LoadAll<StoryObject>("Stories")[0];
            GameManager.gm.StartGame(null, story);
            yield return new WaitUntil(() =>
                SceneManager.GetSceneByName("NPCSelectScene").isLoaded);
        }
    }
    
    [UnityTearDown]
    public IEnumerator RemoveGameManager()
    {
        int layer = (int)TestContext.CurrentContext.Test.Properties.Get("layer");
        GameManager.gm = null;
        
        if (layer > 0)
        {
            SceneManager.UnloadSceneAsync("Loading");
            yield return new WaitUntil(() => !SceneManager.GetSceneByName("Loading").isLoaded);
        }
    }
    
    private Save saving  => Save.Saver;
    private Load loading => Load.Loader;
    
    /// <summary>
    /// Tests whether the correct error is thrown when gamemanager is null
    /// </summary>
    [Test]
    [Property("layer", 0)]
    public void TestSavingErrorHandlingGamemanagerIsNull()
    {
        //create a saving instance to test the function on.
        //note: afaik there is no way to attach this saving instance to the testing scene, so it has to be created with new
        Save saving = new Save();
        LogAssert.Expect(LogType.Error, "Cannot save data when the gamemanger is not loaded.\nSaving failed");
        saving.SaveGame();
    }
    
    /// <summary>
    /// Tests whether the correct error is thrown when gamemanager.currentCharacters is null
    /// </summary>
    [Test]
    [Property("layer", 1)]
    public void TestSavingErrorHandlingGamemanagerCurrentCharactersIsNull()
    {
        LogAssert.Expect(LogType.Error, "Cannot save data when gameManager.currentCharacters has not been assigned yet.\nSaving failed");
        saving.SaveGame();
    }
    
    /// <summary>
    /// Tests whether the correct error is thrown when the characters don't all have unique ids, i.e. duplicate characters
    /// </summary>
    [Test]
    [Property("layer", 2)]
    public void TestSavingErrorHandlingDuplicateCharacterIds()
    {
        GameManager.gm.currentCharacters.Add(GameManager.gm.currentCharacters[0]);
        
        LogAssert.Expect(LogType.Error, "Not all character ids were unique, this is going to cause issues when loading characters.\nSaving failed.");
        saving.SaveGame();
    }
    
    /// <summary>
    /// Tests a basic saving operation
    /// </summary>
    [Test]
    [Property("layer", 2)]
    public void TestInitialSave()
    {
        saving.SaveGame();
    }
    
    
    /// <summary>
    /// Deletes the save file and tests whether no errors are thrown after saving
    /// </summary>
    [Test]
    [Property("layer", 2)]
    public void TestSavingNoSaveFile()
    {
        File.Delete(FilePathConstants.GetSaveFileLocation());
        saving.SaveGame();
    }
    
    /// <summary>
    /// Deletes the save folder and tests whether no errors are thrown after saving
    /// </summary>
    [Test]
    [Property("layer", 2)]
    public void TestSavingNoSaveFolder()
    {
        Directory.Delete(FilePathConstants.GetSaveFolderLocation());
        saving.SaveGame();
    }
    
    
    /// <summary>
    /// Tests if no errors are thrown when loading the saveData and checks whether this savedata is not null
    /// </summary>
    [Test]
    [Property("layer", 2)]
    public void TestInitialLoading()
    {
        //add the loading file to the current object
        SaveData saveData = loading.GetSaveData();
        
        Assert.IsNotNull(saveData);
    }
}
