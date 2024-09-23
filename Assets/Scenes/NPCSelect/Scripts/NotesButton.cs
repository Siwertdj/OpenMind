using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesButton : MonoBehaviour
{
    public void toggleNotes()
    {
        GameManager.gm.ToggleNotebookScene();
    }
}
