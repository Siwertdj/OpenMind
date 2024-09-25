using System.Collections.Generic;

/// <summary>
/// Contains all data that is saved to a file
/// </summary>
public class SaveData
{
    public int[] activeCharacters;
    public int[] inactiveCharacters;
    public int culprit;
    public int questionsRemaining;
    public List<Question>[] remainingQuestions;
    public string noteBookData;
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
    public const string playerSaveDataFolderName = "PlayerSaveData";
    
    /// <summary>
    /// The name of the save file of the player save data.
    /// </summary>
    public const string playerSaveDataFileName = "saveData.txt";

    /// <summary>
    /// The filepath to where the notebook data is stored. The "root" of this filepath is Assets.
    /// </summary>
    public const string notebookFilePath = "Notebook/notes.txt";
}
