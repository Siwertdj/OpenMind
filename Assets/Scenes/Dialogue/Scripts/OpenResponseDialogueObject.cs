    // This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A child of DialogueObject.
/// Executing this object places a text field on the screen, so that the player can answer an open question.
/// </summary>
public class OpenResponseDialogueObject : DialogueObject
{
    // The answer of the open question.
    public string answer = "";

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="background">The background</param>
    public OpenResponseDialogueObject(GameObject[] background)
    {
        this.background = background;
    }

    /// <summary>
    /// Creates on open text field in which the player can type their response
    /// </summary>
    public override void Execute()
    {
        GameManager gm = GameManager.gm;

        // Bool which ensures that an OpenResponseDialogueObject as response only gets added if there is more than 1 speakingObjectText remaining.
        // (The epilogue should not end with the open question and then go straight to the GameWin/GameOver scene)
        if (gm.remainingDialogueScenario.Count > 1)
        {
            List<string> speakingObjectText = new List<string>();
            // Assign the first element of the list to speakingObjectText.
            if (gm.remainingDialogueScenario.Count > 0)
                speakingObjectText = gm.remainingDialogueScenario[0];
            // Remove the first element of the list (so that the remainder of the list can be passed to OpenResponseDialogueObject).
            gm.remainingDialogueScenario.RemoveAt(0);
            
            // Create the DialogueObject to add as response for the current object
            // and add another OpenResponseDialogueObject as response for the ContentDialogueObject.
            DialogueObject next = new ContentDialogueObject(speakingObjectText,null, background);
            next.Responses.Add(new OpenResponseDialogueObject(background));
            Responses.Add(next);
        }
        else
        {
            // Add a ContentDialogueObject with the last part of the dialogue as parameter.
            Responses.Add(new ContentDialogueObject(gm.remainingDialogueScenario[0],null, background));
        }
    }
}
