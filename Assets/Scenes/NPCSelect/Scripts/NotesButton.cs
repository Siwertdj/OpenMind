using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesButton : MonoBehaviour
{
    public void toggleNotes()
    {
        // Use the gamemanager to toggle the notebook scene
        FindObjectOfType<SceneController>().ToggleNotebookScene();
    }
}
