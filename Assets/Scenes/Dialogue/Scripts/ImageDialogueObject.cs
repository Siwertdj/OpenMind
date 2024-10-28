using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDialogueObject : DialogueObject
{
    public override void Execute()
    {
        // If no response if given, add a TerminateDialogueObject response
        if(Responses.Count <= 0)
            Responses.Add(new TerminateDialogueObject());
    }
}
