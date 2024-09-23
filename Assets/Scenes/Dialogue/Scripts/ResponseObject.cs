using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseObject : DialogueObject
{
    public List<string> dialogue;

    private DialogueObject _response;
    public override DialogueObject Response
    {
        get { return _response; }
        set { _response = value; }
    }

    private CharacterInstance character;

    public ResponseObject(List<string> dialogue, CharacterInstance character)
    {
        this.dialogue = dialogue;
        this.character = character;

        
    }

    public override void Execute()
    {
        if (GameManager.gm.HasQuestionsLeft())
        {
            int questionsOnScreen = 2;

            List<Question> questions = new();
            List<Question> possibleQuestions = new(character.RemainingQuestions);
            for (int i = 0; i < questionsOnScreen; i++)
            {
                if (possibleQuestions.Count <= 0)
                    continue;

                int questionIndex = new System.Random().Next(possibleQuestions.Count);
                questions.Add(possibleQuestions[questionIndex]);
                possibleQuestions.RemoveAt(questionIndex);
            }

            Debug.Log($"Questions: {string.Join(", ", questions)}");
            Response = new QuestionObject(questions.ToArray());
        }
        else
        {
            Response = new TerminateDialogueObject();
        }

        DialogueManager.dm.WriteDialogue(dialogue);
    }   
}
