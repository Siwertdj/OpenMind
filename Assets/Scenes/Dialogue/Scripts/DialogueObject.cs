using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class containing the blueprint for the possible dialogue options.
/// Possible children are: SpeakingObject, QuestionObject, ResponseObject, and TerminateDialogueObject.
/// </summary>
public abstract class DialogueObject
{
    /// <summary>
    /// The possible responses to the dialogue object (when picturing a tree structure, these are the children of the object)
    /// </summary>
    public abstract List<DialogueObject> Responses { get; set; }

    /// <summary>
    /// Executes the logic of the given dialogue object
    /// </summary>
    public abstract void Execute();
}

/// <summary>
/// A child of DialogueObject. Executing this object simply writes its text to the screen.
/// A response must be set manually, otherwise the response is a TerminateDialogueObject.
/// </summary>
public class SpeakingObject : DialogueObject
{
    public List<string> dialogue;
    public GameObject[] background;

    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    public SpeakingObject(List<string> dialogue, GameObject[] background)
    {
        this.dialogue = dialogue;
        this.background = background;
    }

    public override void Execute()
    {
        var dm = DialogueManager.dm;
        dm.ReplaceBackground(background);

        dm.WriteDialogue(dialogue);

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

    Action post;

    public TerminateDialogueObject() { }

    public TerminateDialogueObject(Action post)
    {
        this.post = post;
    }

    public override void Execute()
    {
        Debug.Log("Terminating dialogue");
        DialogueManager.dm.OnEndDialogue.Invoke();
        SceneController.sc.ToggleDialogueScene();

        // Invoke post function if given
        post?.Invoke();
    }
}
