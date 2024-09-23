using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class NotebookManager : MonoBehaviour
{
    public GameObject inputField;
    private string notesFilePath = "Assets\\Scenes\\Notebook\\";

    // Start is called before the first frame update
    void Start()
    {
        // Read notes previously saved to notes.txt
        using StreamReader reader = new(notesFilePath + "notes.txt");
        string fetchedNotes = reader.ReadToEnd();
        Debug.Log("Fetched notes: " + fetchedNotes);
        inputField.GetComponent<TMP_InputField>().text = fetchedNotes;  // Put said notes into the inputfield
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveNotes()
    {
        Debug.Log("Saving...");
        // Write written notes from the TMP input field to notes.txt to save notes after closing scene
        string savedNotes = inputField.GetComponent<TMP_InputField>().text;
        using (StreamWriter writer = new (notesFilePath + "notes.txt"))
        {
            writer.WriteLine(savedNotes);
        }

        Debug.Log("Saved notes: " + savedNotes);
    }
}
