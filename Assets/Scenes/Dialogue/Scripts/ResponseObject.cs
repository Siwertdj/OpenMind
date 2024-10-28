using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A child of DialogueObject. Executing this object writes a response to the given question to the screen.
/// A response can be either a new QuestionObject, or a TerminateDialogueObject if there are no more questions available.
/// </summary>
public class ResponseObject : DialogueObject
{
    public Question question;

    public ResponseObject(Question question, GameObject[] background)
    {
        this.question = question;
        this.background = background;
    }

    public override void Execute()
    {
        var dm = DialogueManager.dm;

        List<string> answer = GetQuestionResponse(question);

        if (GameManager.gm.HasQuestionsLeft() && DialogueManager.dm.currentRecipient.RemainingQuestions.Count > 0)
            Responses.Add(new QuestionObject(background));
        else
            Responses.Add(new TerminateDialogueObject());

        dm.ReplaceBackground(background);
        dm.WriteDialogue(answer, DialogueManager.dm.currentRecipient.pitch);
    }

    // Gets character's response to the given question
    private List<string> GetQuestionResponse(Question question)
    {
        GameManager.gm.numQuestionsAsked++;

        CharacterInstance character = DialogueManager.dm.currentRecipient;
        character.RemainingQuestions.Remove(question);
        
        // Write answers to notebook
        character.AskedQuestions.Add(question);
        
        // Return answer to the question
        return character.Answers[question];
    }
}
