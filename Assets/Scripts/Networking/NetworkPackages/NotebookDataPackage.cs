// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)


using System.Collections.Generic;

public class NotebookDataPackage
{
    public Dictionary<int, string> characterNotes = new();
    public string                  personalNotes;
    
    public NotebookDataPackage(NotebookData data)
    {
        foreach (CharacterInstance character in GameManager.gm.currentCharacters)
            characterNotes.Add(character.id, data.GetCharacterNotes(character));
        
        personalNotes = data.GetPersonalNotes();
    }
    
    public NotebookDataPackage(NetworkPackage data)
    {
        NotebookDataPackage notebookDataPackage = data.GetData<NotebookDataPackage>();
        characterNotes = notebookDataPackage.characterNotes;
        personalNotes = notebookDataPackage.personalNotes;
    }
    
    public NetworkPackage CreatePackage()
    {
        return NetworkPackage.CreatePackage(this);
    }
    
    public NotebookData ConvertToNotebookData()
    {
        Dictionary<CharacterInstance, NotebookPage> pages =
            new Dictionary<CharacterInstance, NotebookPage>();
        
        foreach (var keyValuePair in characterNotes)
        {
            CharacterInstance instance =
                GameManager.gm.currentCharacters.Find(cc => cc.id == keyValuePair.Key);
            pages.Add(instance, new NotebookPage(instance));
        }
    }
}
