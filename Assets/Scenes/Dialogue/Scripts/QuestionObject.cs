using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A child of DialogueObject. Executing this object places questions on the screen, with ResponseObjects as responses.
/// </summary>
public class QuestionObject : DialogueObject
{
    public List<Question> questions = new();

    private List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    public override void Execute()
    {
        var dm = DialogueManager.dm;

        GenerateQuestions();

        // Add response to each question to list of responses
        foreach (Question question in questions)
            Responses.Add(new ResponseObject(question));

        dm.SetQuestionsField(true);
        dm.CreatePromptButtons(this);
    }

    private void GenerateQuestions()
    {
        // The number of question options to give the player
        // (This value should possibly be public and adjustable from the GameManager)
        int questionsOnScreen = 2;

        //Debug.Log(string.Join(", ", GameManager.gm.dialogueRecipient.RemainingQuestions));

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
}
