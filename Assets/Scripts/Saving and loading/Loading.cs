using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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
        SaveData saveData;
        string saveFileLocation = FilePathConstants.GetSaveFileLocation();
        string saveFileJsonContents = FilePathConstants.GetSafeFileContents(saveFileLocation, "Save Data");
        if (saveFileJsonContents is null)
            return;
        
        //check if the notebook file exists, if not, stop the loading
        if (!File.Exists(FilePathConstants.GetNoteBookLocation()))
            return;
        
        saveData = JsonConvert.DeserializeObject<SaveData>(saveFileJsonContents);
        
    }
}
