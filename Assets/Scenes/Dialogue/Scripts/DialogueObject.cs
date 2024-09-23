using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueObject
{
    public abstract DialogueObject Response { get; set; }

    public abstract void Execute();
}

public class SpeakingObject : DialogueObject
{
    public override DialogueObject Response { get; set; }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}

public class TerminateDialogueObject : DialogueObject
{
    public override DialogueObject Response { get; set; }

    public TerminateDialogueObject()
    {

    }

    public override void Execute()
    {
        Debug.Log("Ending dialogue");
    }
}
