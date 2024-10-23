using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Contains data that can be saved to a file.
/// </summary>
public class SaveData
{
    public int                     storyId;
    public int[]                   activeCharacterIds;
    public int[]                   inactiveCharacterIds;
    public int                     culpritId;
    public (int, List<Question>)[] remainingQuestions;
    public string                  personalNotes;
    public (int, string)[]         characterNotes;
    public (int, List<Question>)[] askedQuestionsPerCharacter;
    public int                     numQuestionsAsked;
}

/// <summary>
/// This class holds the filepaths to important file locations regarding saving and loading player data.
/// Adjust these paths if files are moved around.
/// </summary>
public static class FilePathConstants
{    
    /// <summary>
    /// The name of the save file of the player save data.
    /// </summary>
    private const string playerSaveDataFileName = "saveData.txt";

    /// <summary>
    /// Gets the location of the folder where the save file should be stored. This is used for checking if this folder exists
    /// </summary>
    public static string GetSaveFolderLocation() => Path.Combine(Application.persistentDataPath, "SaveData");
    
    /// <summary>
    /// Gets the location to the save file.
    /// Uses "Application.persistentDataPath", which is the standard directory for save data.
    /// </summary>
    public static string GetSaveFileLocation() => Path.Combine(GetSaveFolderLocation(), playerSaveDataFileName);

    /// <summary>
    /// Checks if the save file exists.
    /// </summary>
    public static bool DoesSaveFileLocationExist() => File.Exists(GetSaveFileLocation());

    /// <summary>
    /// A safe way to read files that handles a bunch of exceptions.
    /// </summary>
    /// <returns>File contents if no exception is thrown, otherwise null.</returns>
    public static string GetSafeFileContents(string fileLocation, string typeOfContent, string typeOfAction)
    {
        string fileContents;
        try
        {
            fileContents = File.ReadAllText(fileLocation);
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogError($"A specified directory does not exist when accessing {typeOfContent} content in filepath {fileLocation}, got error: {e}.\n{typeOfAction} failed");
            return null;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError($"Couldn't find the {typeOfContent} content with filepath {fileLocation}, got error: {e}.\n{typeOfAction} failed");
            return null;
        }
        catch (IOException e)
        {
            Debug.LogError($"Something went wrong when opening and reading the {typeOfContent} content, got error: {e}.\n{typeOfAction} failed");
            return null;
        }

        return fileContents;
    }
}
