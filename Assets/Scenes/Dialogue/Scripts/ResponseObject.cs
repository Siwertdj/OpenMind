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
    public GameObject[] background;
    
    public List<DialogueObject> _responses = new();
    public override List<DialogueObject> Responses
    {
        get { return _responses; }
        set { _responses = value; }
    }

    /// <summary>
    /// The constructor.
    /// </summary>
    /// <param name="question">The question that this is a response to</param>
    /// <param name="background">The background</param>
    public ResponseObject(Question question, GameObject[] background)
    {
        this.question = question;
        this.background = background;
    }

    /// <summary>
    /// Gives a response to the question that was just asked
    /// </summary>
    public override void Execute()
    {
        var dm = DialogueManager.dm;

        List<string> answer = GetQuestionResponse(question);
        if (GameManager.gm.HasQuestionsLeft() && DialogueManager.dm.currentRecipient.RemainingQuestions.Count > 0)
            Responses.Add(new QuestionObject(background));
        // If there are no more questions remaining give a TerminateDialogueObject as a response
        else
            Responses.Add(new TerminateDialogueObject());

        dm.ReplaceBackground(background);
        dm.WriteDialogue(answer, DialogueManager.dm.currentRecipient.pitch);
    }
    /// <summary>
    /// Gets character's response to the given question
    /// </summary>
    /// <param name="question">The question that needs a response.</param>
    /// <returns>The answer to the given question.</returns>
    // 
    private List<string> GetQuestionResponse(Question question)
    {
        GameManager.gm.AssignAmountOfQuestionsRemaining(GameManager.gm.AmountOfQuestionsRemaining() - 1);

        CharacterInstance character = DialogueManager.dm.currentRecipient;
        character.RemainingQuestions.Remove(question);
        
        // Write answers to notebook
        character.AskedQuestions.Add(question);
        
        // Return answer to the question
        return character.Answers[question];
    }
}
