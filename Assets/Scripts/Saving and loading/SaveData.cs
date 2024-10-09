using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Contains all data that is saved to a file
/// </summary>
public class SaveData
{
    public int[] activeCharacters;
    public int[] inactiveCharacters;
    public int culprit;
    public int questionsRemaining;
    public (int, List<Question>)[] remainingQuestions;
    public string[] sceneStack;
    public string personalNotes;
    public (int, string)[] characterNotes;
    public (int, List<Question>)[] askedQuestions;
}

/// <summary>
/// This class holds the filepaths to important file locations regarding saving and loading player data.
/// Adjust these paths if files are moved around.
/// </summary>
public static class FilePathConstants
{
    /// <summary>
    /// The folder from the root folder where the save file resides.
    /// So if the asset folder had the path root/Assets, the save file would be saved in root/<see cref="folderName"/>>.
    /// </summary>
    private const string playerSaveDataFolderName = "Assets/Data";
    
    /// <summary>
    /// The name of the save file of the player save data.
    /// </summary>
    private const string playerSaveDataFileName = "saveData.txt";
    
    /// <summary>
    /// Gets the location to the directory when the save file is stored.
    /// </summary>
    public static string GetSaveFileDirectory() => Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + playerSaveDataFolderName;
    
    /// <summary>
    /// Gets the location to the save file.
    /// </summary>
    public static string GetSaveFileLocation() => GetSaveFileDirectory() + @"\" + playerSaveDataFileName;

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
