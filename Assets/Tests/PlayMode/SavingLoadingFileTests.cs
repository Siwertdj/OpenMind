using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// This class tests all filepath related stuff in the saving and loading files. This tests:
/// - Tests whether saving and loading returns no errors (all filepaths are correct)
/// </summary>
public class SavingLoadingTests
{
    private string SaveFileContents;
    
    private void InitialiseGameManager()
    {
        SceneManager.LoadScene("Loading");
        StoryObject story = Resources.LoadAll<StoryObject>("Stories")[0];
        GameManager.gm.StartGame(null, story);
    }
    
    
    /// <summary>
    /// Tests whether the save file contents can be correctly saved and no errors are thrown
    /// </summary>
    [Test, Order(0)]
    public void TestSaveFileReading()
    {
        
    }
    
    
    /// <summary>
    /// Tests if an error message is thrown
    /// </summary>
    [Test, Order(1)]
    public void TestSavingGamemanagerIsNullErrors()
    {
        
    }
}
