using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A child of DialogueObject. Executing this object places a text field on the screen, so that the player can answer an open question.
/// </summary>
public class OpenResponseObject : DialogueObject
{
    // The answer of the open question.
    private string answer = "";

    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }
    public override void Execute()
    {
        GameManager gm = GameManager.gm;

        Debug.Log("openresponse");
        // Bool which ensures that an OpenResponseObject as response only gets added if there is more than 1 speakingObjectText remaining.
        // (The epilogue should not end with the open question and then go straight to the GameWin/GameOver scene)
        if (gm.remainingDialogueScenario.Count > 1)
        {
            List<string> speakingObjectText = new List<string>();
            // Assign the first element of the list to speakingObjectText.
            if (gm.remainingDialogueScenario.Count > 0)
                speakingObjectText = gm.remainingDialogueScenario[0];
            // Remove the first element of the list (so that the remainder of the list can be passed to OpenResponseObject).
            gm.remainingDialogueScenario.RemoveAt(0);
            
            // Create the DialogueObject to add as response for the current object
            // and add another OpenResponseObject as response for the SpeakingObject.
            DialogueObject next = new SpeakingObject(speakingObjectText);
            next.Responses.Add(new OpenResponseObject());
            Responses.Add(next);
        }
        else
        {
            // Add a SpeakingObject with the last part of the dialogue as parameter.
            Responses.Add(new SpeakingObject(gm.remainingDialogueScenario[0]));
        }
    }
}
