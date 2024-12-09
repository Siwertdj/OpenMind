// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// An abstract class containing the blueprint for the possible dialogue options.
/// Possible children are: ContentDialogueObject, DialogueDialogueQuestionObject, ResponseDialogueObject, and TerminateDialogueObject.
/// </summary>
public abstract class DialogueObject
{
    protected GameObject[] background;

    /// <summary>
    /// The possible responses to the dialogue object (when picturing a tree structure, these are the children of the object)
    /// </summary>
    public List<DialogueObject> Responses { get; set; } = new();

    /// <summary>
    /// Executes the logic of the given dialogue object
    /// </summary>
    public abstract void Execute();
}

/// <summary>
/// A child of DialogueObject. Executing this object simply writes its text to the screen.
/// A response must be set manually, otherwise the response is a TerminateDialogueObject.
/// </summary>
public class ContentDialogueObject : DialogueObject
{
    [CanBeNull] public List<string> dialogue;
    [CanBeNull] public Sprite   image; 

    /// <summary>
    /// The constructor for <see cref="ContentDialogueObject"/>.
    /// </summary>
    /// <param name="dialogue">The text</param>
    /// <param name="background">The background</param>
    public ContentDialogueObject([CanBeNull] List<string> dialogue, [CanBeNull] Sprite image, GameObject[] background)
    {
        // Set this object's local variables to match the parameter-values of the constructor
        this.dialogue = dialogue;
        this.image = image; 
        this.background = background;
    }

    /// <summary>
    /// Writes the text to the screen
    /// </summary>
    public override void Execute()
    {
        var dm = DialogueManager.dm;

        dm.ReplaceBackground(background);
        dm.PrintImage(image);
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
    /// <summary>
    /// Unloads the scene and loads NPCSelect
    /// </summary>
    public override void Execute()
    {
        // Invokes event, listener invokes CheckEndCycle, which loads NPCSelect.
        // Also pass along the currentObject, which is used for the Epilogue scene.
        DialogueManager.dm.onEndDialogue.Raise(DialogueManager.dm, DialogueManager.dm.currentObject);
    }
}
