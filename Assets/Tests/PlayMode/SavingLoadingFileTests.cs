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
/// - Assigns specific values for each variable in the SaveData class & saves these contents
/// - Then checks whether the loaded contents are the same
/// - Then checks whether every variable is assigned correctly
/// - Then saves gain and loads again and checks whether every variable is assigned correctly, this tests whether saving is correct
/// </summary>
public class SavingLoadingTests
{
    [UnitySetUp]
    private IEnumerator Initialise()
    {
        int layer = (int)TestContext.CurrentContext.Test.Properties.Get("layer");
        switch (layer)
        {
            case 0:
                break;
            
            case 1:
                //create gamemanager without initialising it
                SceneManager.LoadScene("Loading");
                yield return new WaitUntil(() => SceneManager.GetSceneByName("Loading").isLoaded);
                break;
                
            case 2:
                //initialise gamemanager
                StoryObject story = Resources.LoadAll<StoryObject>("Stories")[0];
                GameManager.gm.StartGame(null, story);
                goto case 1;
        }
    }
    
    private Saving saving => GameManager.FindObjectOfType<Saving>();
    private Loading loading => GameManager.FindObjectOfType<Loading>();
    
    /// <summary>
    /// Tests whether the correct error is thrown when gamemanager is null
    /// </summary>
    [Test]
    [Property("layer", 0)]
    public void TestSavingErrorHandlingGamemanagerIsNull()
    {
        //create a saving instance to test the function on.
        //note: afaik there is no way to attach this saving instance to the testing scene, so it has to be created with new
        Saving saving = new Saving();
        LogAssert.Expect(LogType.Error, "Cannot save data when the gamemanger is not loaded.\nSaving failed");
        saving.Save();
    }
    
    /// <summary>
    /// Tests whether the correct error is thrown when gamemanager.currentCharacters is null
    /// </summary>
    [Test]
    [Property("layer", 1)]
    public void TestSavingErrorHandlingGamemanagerCurrentCharactersIsNull()
    {
        LogAssert.Expect(LogType.Error, "Cannot save data when gameManager.currentCharacters has not been assigned yet.\nSaving failed");
        saving.Save();
    }
    
    /// <summary>
    /// Tests whether the correct error is thrown when the characters don't all have unique ids, i.e. duplicate characters
    /// </summary>
    [UnityTest]
    public IEnumerator TestSavingErrorHandlingDuplicateCharacterIds()
    {
        yield return Initialise(1);
        GameManager.gm.currentCharacters.Add(GameManager.gm.currentCharacters[0]);
        
        LogAssert.Expect(LogType.Error, "Not all character ids were unique, this is going to cause issues when loading characters.\nSaving failed.");
        saving.Save();
    }
    
    /// <summary>
    /// Tests a basic saving operation
    /// </summary>
    [UnityTest]
    public IEnumerator TestInitialSave()
    {
        yield return Initialise(1);
        saving.Save();
    }
    
    
    /// <summary>
    /// Deletes the save file and tests whether no errors are thrown after saving
    /// </summary>
    [Test]
    public void TestSavingNoSaveFile()
    {
        File.Delete(FilePathConstants.GetSaveFileLocation());
        saving.Save();
    }
    
    /// <summary>
    /// Deletes the save folder and tests whether no errors are thrown after saving
    /// </summary>
    [Test]
    public void TestSavingNoSaveFolder()
    {
        Directory.Delete(FilePathConstants.GetSaveFolderLocation());
        saving.Save();
    }
    
    
    /// <summary>
    /// Tests if no errors are thrown when loading the saveData and checks whether this savedata is not null
    /// </summary>
    [Test]
    public void TestInitialLoading()
    {
        //add the loading file to the current object
        SaveData saveData = loading.GetSaveData();
        
        Assert.IsNotNull(saveData);
    }
}
