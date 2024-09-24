using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class containing the blueprint for the possible dialogue options.
/// Possible children are: SpeakingObject, QuestionObject, ResponseObject, and TerminateDialogueObject.
/// </summary>
public abstract class DialogueObject
{
    public abstract List<DialogueObject> Responses { get; set; }

    public abstract void Execute();
}

/// <summary>
/// A child of DialogueObject. Executing this object simply writes its text to the screen.
/// A response must be set manually, otherwise the response is simply a TerminateDialogueObject.
/// </summary>
public class SpeakingObject : DialogueObject
{
    public List<string> dialogue;

    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    public SpeakingObject(List<string> dialogue)
    {
        this.dialogue = dialogue;
    }

    public override void Execute()
    {
        DialogueManager.dm.WriteDialogue(dialogue);

        // If no response is given, terminate dialogue
        if (Responses.Count <= 0)
            Responses.Add(new TerminateDialogueObject());
    }
}

/// <summary>
/// A child of DialogueObject. Executing this object unloads the dialogue scene with no response.
/// </summary>
public class TerminateDialogueObject : DialogueObject
{
    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    public override void Execute()
    {
        Debug.Log("Executing Terminate Dialogue Object");

        // TODO: This should just close the dialogue scene and open the next one, but that doesn't exist yet...
        GameManager.gm.EndCycle();
    }
}
