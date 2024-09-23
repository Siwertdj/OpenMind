using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueObject
{
    public abstract List<DialogueObject> Response { get; set; }

    public abstract void Execute();
}

public class SpeakingObject : DialogueObject
{
    public List<string> dialogue;

    private List<DialogueObject> _response = new();
    public override List<DialogueObject> Response
    {
        get { return _response; }
        set { _response = value; }
    }

    public SpeakingObject(List<string> dialogue)
    {
        this.dialogue = dialogue;
    }

    public override void Execute()
    {
        DialogueManager.dm.WriteDialogue(dialogue);
    }
}

public class TerminateDialogueObject : DialogueObject
{

    private List<DialogueObject> _response = new();
    public override List<DialogueObject> Response
    {
        get { return _response; }
        set { _response = value; }
    }

    public TerminateDialogueObject()
    {

    }

    public override void Execute()
    {
        Debug.Log("Executing Terminate Dialogue Object");
        GameManager.gm.UnloadDialogueScene();
    }
}
