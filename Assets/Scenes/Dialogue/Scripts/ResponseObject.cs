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
    public List<string> dialogue;

    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    public ResponseObject(Question question)
    {
        this.question = question;
    }

    public override void Execute()
    {
        var dm = DialogueManager.dm;

        List<string> answer = GetQuestionResponse(question);

        if (GameManager.gm.HasQuestionsLeft() && GameManager.gm.dialogueRecipient.RemainingQuestions.Count > 0)
            Responses.Add(new QuestionObject());
        else
            Responses.Add(new TerminateDialogueObject());

        dm.WriteDialogue(answer);
    }

    // Gets character's response to the given question
    private List<string> GetQuestionResponse(Question question)
    {
        GameManager.gm.numQuestionsAsked += 1;

        CharacterInstance character = GameManager.gm.dialogueRecipient;
        character.RemainingQuestions.Remove(question);

        // Return answer to the question
        return character.Answers[question];
    }
}
