using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseObject : DialogueObject
{
    public Question question;
    public List<string> dialogue;

    private List<DialogueObject> _response = new();
    public override List<DialogueObject> Response
    {
        get { return _response; }
        set { _response = value; }
    }

    public ResponseObject(Question question)
    {
        this.question = question;
    }

    public override void Execute()
    {
        Debug.Log("Executing Response Object");

        if (GameManager.gm.HasQuestionsLeft() && GameManager.gm.dialogueRecipient.RemainingQuestions.Count > 0)
            Response.Add(new QuestionObject());
        else
            Response.Add(new TerminateDialogueObject());

        DialogueManager.dm.AskQuestion(question);
    }   
}
