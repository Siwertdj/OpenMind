using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionObject : DialogueObject
{
    public List<Question> questions = new();

    private List<DialogueObject> _response = new();
    public override List<DialogueObject> Response
    {
        get { return _response; }
        set { _response = value; }
    }

    public QuestionObject()
    {

    }

    private void GenerateQuestions()
    {
        // The number of question options to give the player
        // (This value should possibly be public and adjustable from the GameManager)
        int questionsOnScreen = 2;

        Debug.Log(string.Join(", ", GameManager.gm.dialogueRecipient.RemainingQuestions));

        // Generate random list of questions
        if (GameManager.gm.HasQuestionsLeft())
        {
            List<Question> possibleQuestions = new(GameManager.gm.dialogueRecipient.RemainingQuestions);
            for (int i = 0; i < questionsOnScreen; i++)
            {
                if (possibleQuestions.Count <= 0)
                    continue;

                int questionIndex = new System.Random().Next(possibleQuestions.Count);
                questions.Add(possibleQuestions[questionIndex]);
                possibleQuestions.RemoveAt(questionIndex);
            }
        }
    }

    public override void Execute()
    {
        Debug.Log("Executing Question Object");

        GenerateQuestions();

        // Add response to each question to list of responses
        foreach (Question question in questions)
            Response.Add(new ResponseObject(question));

        DialogueManager.dm.SetQuestionsField(true);
        DialogueManager.dm.CreatePromptButtons(this);
    }
}
