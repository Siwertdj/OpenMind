using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ImageDialogueObject : DialogueObject
{
    public ImageDialogueObject(GameObject[] background)
    {
        this.background = background;
    }

    public override void Execute()
    {
        var dm = DialogueManager.dm;

        dm.ReplaceBackground(background);

        // If no response if given, add a TerminateDialogueObject response
        if(Responses.Count <= 0)
            Responses.Add(new TerminateDialogueObject());
    }
}
