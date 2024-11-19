// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDialogueObject : DialogueObject
{
    
    // Use the background for the image
    // Empty dialogue/lines - displays no text or textwindow
    // Retain the tap-function to continue.
    
    public ImageDialogueObject(GameObject[] background)
    {
        this.background = background;
    }

    public override void Execute()
    {
        // If no response if given, add a TerminateDialogueObject response
        if(Responses.Count <= 0)
            Responses.Add(new TerminateDialogueObject());
    }
}
