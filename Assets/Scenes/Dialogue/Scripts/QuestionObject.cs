using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionObject : DialogueObject
{
    public Question[] questions;

    private DialogueObject _response;
    public override DialogueObject Response
    {
        get { return _response; }
        set { _response = value; }
    }

    public QuestionObject(Question[] questions)
    {
        this.questions = questions;
    }

    public override void Execute()
    {
        Response = new TerminateDialogueObject();

        DialogueManager.dm.SetQuestionsField(true);

        Debug.Log("Creating prompts");

        foreach (var question in questions)
            DialogueManager.dm.CreatePromptButton(question);
    }
}
