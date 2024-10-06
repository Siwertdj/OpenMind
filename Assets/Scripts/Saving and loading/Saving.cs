using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class Saving : MonoBehaviour
{
    /// <summary>
    /// Saves data to a file.
    /// All data saved to a file is stored in <see cref="SaveData"/>>.
    /// When accessing the notebook file, all relevant exceptions are caught and result in an error in the debug console and results in nothing being saved.
    /// When the specified directory and or file to save to cannot be found, these will be created anew.
    ///
    /// This method results in an error in the debug console, if the gamemanager is not loaded, after which it will exit the function.
    /// </summary>
    public void Save()
    {
        GameManager gameManager = GameManager.gm;
        //check if the gamemanger is loaded
        if (gameManager is null)    
            Debug.LogError("Cannot save data when the gamemanger is not loaded.\nSaving failed");

        //check if all characters have a unique id
        bool allUniqueID = true;
        for (int i = 0; i < gameManager.currentCharacters.Count; i++)
            for (int j = i+1; j < gameManager.currentCharacters.Count; j++)
                if (gameManager.currentCharacters[i].id == gameManager.currentCharacters[j].id)
                    allUniqueID = false;

        if (!allUniqueID)
        {
            Debug.LogError("Not all character ids were unique, this is going to cause issues when loading characters.\nSaving failed.");
            return;
        }

        CharacterInstance[] active = gameManager.currentCharacters.FindAll(c => c.isActive).ToArray();
        CharacterInstance[] inactive = gameManager.currentCharacters.FindAll(c => !c.isActive).ToArray();
        
        (int, List<Question>)[] remainingQuestions = active.Select(a => (a.id, a.RemainingQuestions)).ToArray();
        (int, List<Question>)[] askedQuestions = gameManager.currentCharacters.Select(a => (a.id, a.AskedQuestions)).ToArray();
        (int, string)[] characterNotes = gameManager.notebookData.GetAllNotes();
        //saves the scene stack, excluding the loading scene
        string[] sceneStack = new string[SceneManager.sceneCount-1];
        for (int i = 0; i < sceneStack.Length; i++)
            sceneStack[i] = SceneManager.GetSceneAt(i+1).name;

        SaveData saveData = new SaveData
        {
            activeCharacters = active.Select(c => c.id).ToArray(),
            inactiveCharacters = inactive.Select(c => c.id).ToArray(),
            culprit = gameManager.GetCulprit().id,
            questionsRemaining = gameManager.AmountOfQuestionsRemaining(),
            remainingQuestions = remainingQuestions,
            sceneStack = sceneStack,
            personalNotes = gameManager.notebookData.GetPersonalNotes(),
            characterNotes = characterNotes,
            askedQuestions = askedQuestions,
        };

        string jsonString = JsonConvert.SerializeObject(saveData);
        string directoryLocation = FilePathConstants.GetSaveFileDirectory();
        string fileLocation = FilePathConstants.GetSaveFileLocation();
        
        if (!Directory.Exists(directoryLocation))
            Directory.CreateDirectory(directoryLocation);
        
        File.WriteAllText(fileLocation,jsonString);
    }
}
