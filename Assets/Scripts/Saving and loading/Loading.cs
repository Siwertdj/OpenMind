// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for loading all the <see cref="SaveData"/> from the save file.
/// There are checks for all relevant exceptions. After an exception is thrown, it results in an error message in the debug menu.
/// A check is made for the location of the notebook file. If it does not exist, an error message will appear in the debug menu and the loading process fails.
/// </summary>
public class Loading : MonoBehaviour
{
    /// <summary>
    /// Loads the game by retrieving savedata, by reloading the game in Gamemanager and passing the savedata.
    /// </summary>
    public void LoadButtonPressed()
    {
        // retrieve savedata, if there is any.
        SaveData saveData = GetSaveData();
        if (saveData is null)
        {
            Debug.LogError("Make sure there is savedata before loading a game");
            return;
        }

        if (GameManager.gm is null)
        {
            Debug.LogError("Please activate the gamemanager before loading a game");
            return;
        }
        
        GameManager.gm.LoadGame(saveData);
    }

    /// <summary>
    /// Gets a <see cref="SaveData"/> object from the save file contents
    /// </summary>
    public SaveData GetSaveData()
    {
        string saveFileLocation = FilePathConstants.GetSaveFileLocation();
        string saveFileJsonContents = FilePathConstants.GetSafeFileContents(saveFileLocation, "Save Data", "Loading");
        if (saveFileJsonContents is null)
        {
            Debug.Log("No savedata was found");
            return null;
        }

        return JsonConvert.DeserializeObject<SaveData>(saveFileJsonContents);
    }
}
