// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
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

    /// <summary>
    /// The constructor for <see cref="SpeakingObject"/>.
    /// </summary>
    /// <param name="dialogue">The text</param>
    /// <param name="background">The background</param>
    public SpeakingObject(List<string> dialogue, GameObject[] background)
    {
        this.dialogue = dialogue;
        this.background = background;
    }

    /// <summary>
    /// Writes the text to the screen
    /// </summary>
    public override void Execute()
    {
        var dm = DialogueManager.dm;

        dm.ReplaceBackground(background);
        dm.WriteDialogue(dialogue);

        // If no response is given, terminate dialogue
        if (Responses.Count <= 0)
        {
            Debug.LogError("There was no response given");
            Responses.Add(new TerminateDialogueObject());
        }
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

    /// <summary>
    /// Unloads the scene and loads NPCSelect
    /// </summary>
    public override void Execute()
    {
        // Invokes event, listener invokes CheckEndCycle, which loads NPCSelect.
        // Also pass along the currentObject, which is used for the Epilogue scene.
        DialogueManager.dm.onEndDialogue.Raise(DialogueManager.dm, DialogueManager.dm.currentObject);
        
        // Invoke post function if given
        post?.Invoke();
    }
}
