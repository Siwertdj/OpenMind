// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a container for dialogueobjects. it has a list of dialogueobjects, rather than a
/// dialogueobject having 'responses', it could be sequential
/// </summary>
[CreateAssetMenu(fileName = "newDialogue", menuName = "Dialogue")]
public class ScriptableDialogueObject : ScriptableObject
{
    [Range(0.5f, 2f)] public float voicePitch = 1;

    [SerializeField] public DialogueData[] lines;
    
    // TODO: Method to extract as DialogueObjects
    public DialogueObject[] GetDialogue()
    {
        DialogueObject[] output = new DialogueObject[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            var data = lines[i];
            output[i] = foo(data);
        }

        return output;
    }

    public DialogueObject foo(DialogueData data)
    {
        DialogueObject output;
        if (data.text != null && data.image == null)
        {
            output = new SpeakingDialogueObject(data.text, null);
        }
        else if (data.text != null && data.image != null)
        {
            // Add image above background
        }
        else if (data.text == null && data.image != null)
        {
            output = new ImageDialogueObject(data.image, null);
        }
        else // (data.text == null && data.image == null)
        {
            // empty dialogueobject?
            output = null;
        }
        return output;
    }
}

/// <summary>
/// A DialogueData object can take both a text and and image.
/// </summary>
[Serializable]
public class DialogueData
{
    [SerializeField] public List<string> text;
    [SerializeField] public Sprite image;
}
