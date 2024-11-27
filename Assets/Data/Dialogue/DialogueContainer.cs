// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a container for dialogueobjects. it has a list of dialogueobjects, rather than a
/// dialogueobject having 'responses', it could be sequential
/// </summary>
[CreateAssetMenu(fileName = "newDialogue", menuName = "Dialogue")]
[Serializable]
public class DialogueContainer : ScriptableObject
{
    // Voicepitch of the dialogue
    [Range(0.5f, 2f)] public float voicePitch = 1;
    // lines of the dialogue in an array
    [SerializeField] public DialogueData[] lines;
    // Background image, takes an array and orders these into one background.
    [SerializeField] GameObject[] background;
    
    /// <summary>
    /// Takes an array of DialogueData, creates DialogueObjects of these,
    /// and strings them each sequentially as responses of the previous.
    /// This creates a sequential dialogue, during which backgrounds,
    /// images and dialogue-lines can vary.
    /// </summary>
    /// <returns></returns>
    public DialogueObject GetDialogue()
    {
        // Create the first piece of dialogue and initialize it as output.
        // we will add on to its responses in the for-loop below.
        DialogueObject output = CreateDialogueObject(lines[0]);
        for (int i = 1; i < lines.Length; i++)
        {
            // Get dialogue-data from the correct line
            var data = lines[i];
            AppendToLeaf(output, CreateDialogueObject(data));
        }

        return output;
    }

    /// <summary>
    /// Appends a dialogueobject to the lead of a dialogue-tree.
    /// It finds the lowest-level node with no responses and adds
    /// the given dialogueobject as a response of that.
    /// </summary>
    public void AppendToLeaf(DialogueObject node, DialogueObject newLeaf)
    {
        // if the node has no responses, add the newleaf to its responses
        if (node.Responses.Count == 0)
            node.Responses.Add(newLeaf);
        // else, we recurse
        else
            AppendToLeaf(node.Responses.First(), newLeaf);
    }

    public DialogueObject CreateDialogueObject(DialogueData data)
    {
        // We create a new ContentDialogueObject, containing text, image or background
        // The first of these two could be null, but that is handled by the ContentDialogueObject.
        List<string> text = new List<string>();
        text.Add(data.line);
        return new ContentDialogueObject(text, data.image, background);
        
    }
}

/// <summary>
/// A DialogueData object can take both a text and and image.
/// </summary>
[Serializable]
public class DialogueData
{
    [SerializeField] public string line;
    [SerializeField] public Sprite  image;
}

