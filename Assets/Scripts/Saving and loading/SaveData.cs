using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Contains data that can be saved to a file.
/// </summary>
public class SaveData
{
    public int[] activeCharacters;                      // A list of the active characters.
    public int[] inactiveCharacters;                    // A list of the inactive characters.
    public int culprit;                                 // The culprit.
    public int questionsRemaining;                      // The amount of questions remaining for this cycle.
    public (int, List<Question>)[] remainingQuestions;  // The questions that are still available.
    public string[] sceneStack;                         // All scenes that are loaded.
    public string personalNotes;                        // The notes the player has made.
    public (int, string)[] characterNotes;              // The notes the player has made of the .
    public (int, List<Question>)[] askedQuestions;      // The questions that have already been asked by the player.
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
    /// Gets the location to the save file.
    /// Uses "Application.persistentDataPath", which is the standard directory for save data.
    /// </summary>
    public static string GetSaveFileLocation() => Path.Combine(Application.persistentDataPath, playerSaveDataFileName);

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
