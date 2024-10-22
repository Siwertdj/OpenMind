using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for loading all the <see cref="SaveData"/> from the save file.
///
/// There are checks for all relevant exceptions. After an exception is thrown, it results in an error message in the debug menu.
/// A check is made for the location of the notebook file. If it does not exist, an error message will appear in the debug menu and the loading process fails.
/// </summary>
public class Loading : MonoBehaviour
{
    public void Load()
    {
        SaveData saveData = GetSaveData();
        if (saveData is null)
            return;
        
        GameManager gameManager = GameManager.gm;
        if (gameManager is null)
        {
            Debug.LogError("Please activate the gamemanager before loading a game");
            return;
        }
        
        gameManager.LoadGame(saveData);
    }

    /// <summary>
    /// Gets a <see cref="SaveData"/> object from the save file contents
    /// </summary>
    public SaveData GetSaveData()
    {
        string saveFileLocation = FilePathConstants.GetSaveFileLocation();
        string saveFileJsonContents = FilePathConstants.GetSafeFileContents(saveFileLocation, "Save Data", "Loading");
        if (saveFileJsonContents is null)
            return null;    
        
        return JsonConvert.DeserializeObject<SaveData>(saveFileJsonContents);
    }
}
