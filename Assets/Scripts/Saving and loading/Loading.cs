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
    // Game Events
    [Header("Events")]
    public GameEvent onGameLoaded;
    
    public void Load()
    {
        SaveData saveData;
        string saveFileLocation = FilePathConstants.GetSaveFileLocation();
        Debug.Log(saveFileLocation);
        string saveFileJsonContents = FilePathConstants.GetSafeFileContents(saveFileLocation, "Save Data", "Loading");
        if (saveFileJsonContents is null)
        {
            Debug.Log("SaveData was null");
            return;    
        }
        
        
        saveData = JsonConvert.DeserializeObject<SaveData>(saveFileJsonContents);

        /*
        //do checks to make sure everything works correctly
        if (!DoChecks(saveFileJsonContents, gameManager, saveData))
            return;

        //actual loading happens here, this is where the game data is modified and all checks should've been done before this point.
        //first unload all scenes
        SceneController.sc.UnloadAdditiveScenes();
        //TODO: fix it so loading the dialoguescene works
        // foreach (var t in saveData.sceneStack)
        // {
        //     Debug.Log(t);
        //     SceneManager.LoadScene(t, LoadSceneMode.Additive);
        // }
        //temp solution:
        SceneManager.LoadScene("NPCSelectScene", LoadSceneMode.Additive);
        if (saveData.sceneStack.Length == 2)
            SceneManager.LoadScene("NotebookScene", LoadSceneMode.Additive);
        */

        //then load all the data
        // TODO: Load 'Loading'-scene, wait for the signal that its done, then raise the event and pass the savedata
        //Note: this is a temporary solution for testing
        
        Debug.Log(saveData);
        
        StartCoroutine(LoadGame(saveData));
    }
    
    private bool DoChecks(string saveFileJsonContents, GameManager gameManager, SaveData saveData)
    {
        if (saveFileJsonContents is null)
            return false;
        
        //check if the gamemanger is loaded
        //otherwise no character data can be assigned
         if (gameManager is null)
        {
            Debug.LogError("Cannot load data when the gamemanger is not loaded.\nLoading failed");
            return false;
        }
        
        //check if all ids in the gamemanager.currentCharacters list are found in the saveData
        //otherwise some characters cannot have a valid isActive value, since they don't belong to the active characters group in the saveData, nor in the inactive characters group
         if (!gameManager.currentCharacters.All(c =>
                saveData.activeCharacterIds.Contains(c.id) || saveData.inactiveCharacterIds.Contains(c.id)))
        {
            Debug.LogError($"Not all ids in the gamemanger.currentCharacters appear in the saveData. There are more characters now in the gamemanger than were in the gamemanager when the game was saved, thus making this save file invalid.\nLoading failed.");
            return false;
        }
        
        //check if all ids in the saveData of active characters are found in gamemanager.currentCharacters
        //otherwise some remaining questions stored in the saveData cannot be assigned to a character
         if (!saveData.activeCharacterIds.All(ac => gameManager.currentCharacters.Any(c => c.id == ac)))
        {
            Debug.LogError($"Some saved active character to not appear in gamemanager.currentCharacters. There are less characters now than were when this file was saved.\nLoading failed.");
            return false;
        }
        
        //check if the id of the culprit exists in gamemanager.currentCharacters
        //otherwise no culprit can be assigned
         if (gameManager.currentCharacters.All(c => saveData.culpritId != c.id))
        {
            Debug.LogError($"The culprit that was saved into the save file does not appear in gamemanager.currentCharacters.\nLoading failed.");
            return false;
        }

        return true;
    }
    
    // TODO: This is duplicate code, also found in TimelineManager. Make this a global thing?
    IEnumerator LoadGame(SaveData saveData)
    {
        // If there is already a 'Loading'-scene,
        // we create a reference BEFORE we load a new one, so that we can destroy it later.
        Scene oldLoadingScene =  SceneManager.GetSceneByName("Loading");
        
        // Start the loadscene-operation
        // TODO: Load scenestack here properly
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        onGameLoaded.Raise(this, saveData);
        
        // TODO: Make sure the old scenestack is completely unloaded
        // TODO: Bugfix this a bit
        // Finally, when the data has been sent, we then unload all our scenes, except the last one (as decided by c-1)
        
        int c = SceneManager.sceneCount;
        for (int i = 0; i < c; i++) {
            Scene scene = SceneManager.GetSceneAt (i);
            print (scene.name);
            if (scene.name != "Loading") {
                SceneManager.UnloadSceneAsync (scene);
            }
            else if (oldLoadingScene.isLoaded)
            {
                // destroy the old loading scene
                SceneManager.UnloadSceneAsync(oldLoadingScene);
                // delete old Toolbox
            }
        }
    
    }
}
