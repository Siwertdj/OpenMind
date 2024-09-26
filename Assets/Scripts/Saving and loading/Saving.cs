using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class Saving : MonoBehaviour
{
    /// <summary>
    /// Saves data to a file.
    /// All data saved to a file is stored in <see cref="SaveData"/>>.
    ///
    /// This method results in a warning if the gamemanager is not loaded, after which it will exit the function.
    /// </summary>
    public void Save()
    {
        
        GameManager gameManager;
        try
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Cannot save data when the gamemanger is not loaded.\nSaving failed");
            return;
        }

        CharacterInstance[] active = gameManager.currentCharacters.FindAll(c => c.isActive).ToArray();
        CharacterInstance[] inactive = gameManager.currentCharacters.FindAll(c => !c.isActive).ToArray();
        string notebookLocation = Application.dataPath + FilePathConstants.notebookFilePath;
        string noteBookData;
        try
        {
            noteBookData = File.ReadAllText(notebookLocation);
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogError($"A specified directory does not exist in filepath {notebookLocation}, got error: {e}.\nSaving failed");
            return;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError($"Couldn't find the file with filepath {notebookLocation}, got error: {e}.\nSaving failed");
            return;
        }
        catch (IOException e)
        {
            Debug.LogError($"Something went wrong when opening the file, got error: {e}.\nSaving failed");
            return;
        }
        
        List<Question>[] remaingQuestions = active.Select(a => a.RemainingQuestions).ToArray();
        
        SaveData saveData = new SaveData
        {
            activeCharacters = active.Select(c =>c.id).ToArray(),
            inactiveCharacters = inactive.Select(c =>c.id).ToArray(),
            culprit = gameManager.GetCulprit().id,
            questionsRemaining = gameManager.GetQuestionsRemaining(),
            remainingQuestions = remaingQuestions,
            noteBookData = noteBookData
        };

        string jsonString = JsonConvert.SerializeObject(saveData);
        string directoryLocation = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + FilePathConstants.playerSaveDataFileName;
        string fileLocation = directoryLocation + @"\" + FilePathConstants.playerSaveDataFolderName;
        
        if (!Directory.Exists(directoryLocation))
            Directory.CreateDirectory(directoryLocation);
        
        File.WriteAllText(fileLocation,jsonString);
    }
}
