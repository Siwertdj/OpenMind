using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dm;

    [SerializeField] private GameObject dialogueField;
    [SerializeField] private GameObject questionsField;
    [SerializeField] private DialogueAnimator animator;

    [SerializeField] private GameObject buttonPrefab;

    public DialogueObject currentObject;

    // Start is called before the first frame update
    void Start()
    {
        dm = this;

        // Add event listener to check when dialogue is complete
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);

        StartCharacterDialogue(GameManager.gm.dialogueRecipient);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse input to skip current dialogue
        if (Input.GetMouseButtonDown(0) && animator.InDialogue)
            animator.SkipDialogue();
    }

    public void StartCharacterDialogue(CharacterInstance character)
    {
        Debug.Log($"Talking to {character.characterName}");
        currentObject = new SpeakingObject(character.GetGreeting());
        currentObject.Response.Add(new QuestionObject());
        currentObject.Execute();
    }

    public void SetQuestionsField(bool active) => questionsField.SetActive(active);

    public void OnDialogueComplete()
    {
        dialogueField.SetActive(false);
        
        if (GameManager.gm.HasQuestionsLeft())
        {
            // TODO: back to home button

            // Create the next set of questions
            currentObject = currentObject.Response[0];
            currentObject.Execute();            
        }
        else
        {
            // TODO: end cycle
            GameManager.gm.EndCycle();
        }        
    }

    // Write given dialogue to the screen
    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        dialogueField.SetActive(true);

        animator.WriteDialogue(dialogue, pitch);
    }

    // Starts writing response to the given question to the current character
    public void AskQuestion(Question question)
    {
        GameManager.gm.numQuestionsAsked += 1;

        CharacterInstance character = GameManager.gm.dialogueRecipient;
        character.RemainingQuestions.Remove(question);

        // Adjust the box containing the character's name
        dialogueField.GetComponentInChildren<TextField>().SetText(character.characterName);

        // Get and write the answer to the question
        List<string> answer = character.Answers[question];
        WriteDialogue(answer);
    }

    // Instantiate question buttons
    public void CreatePromptButtons(QuestionObject questionObject)
    {
        foreach (ResponseObject response in questionObject.Response)
        {
            Button button = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            // Set button text in question form
            buttonText.text = GameManager.gm.GetPromptText(response.question);

            // Add event when clicking the button
            button.onClick.AddListener(() => OnButtonClick(response));
        }
    }

    // When a question button is pressed, do things necessary to write dialogue
    public void OnButtonClick(ResponseObject response)
    {
        // Destroy buttons
        for (int i = 0; i < questionsField.transform.childCount; i++)
            Destroy(questionsField.transform.GetChild(i).gameObject);

        // Remove questions field
        questionsField.SetActive(false);

        // Write dialogue when button is pressed
        currentObject = response;
        currentObject.Execute();
    }
}

public enum Question
{
    Name,
    Age,
    Wellbeing,
    Political,
    Hobby,
    CulturalBackground,
    Education,
    CoreValues,
    Personality,
    ImportantPeople,
    PositiveTrait,
    NegativeTrait,
    OddTrait
}