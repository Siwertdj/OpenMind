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
        currentObject = new ResponseObject(character.GetGreeting(), character);
        currentObject.Execute();
    }

    public void SetQuestionsField(bool active) => questionsField.SetActive(active);

    public void OnDialogueComplete()
    {
        dialogueField.SetActive(false);
        
        if (GameManager.gm.HasQuestionsLeft())
        {
            // TODO: back to home button
            Debug.Log($"Current dialogue object: {currentObject.GetType()}");
            currentObject = currentObject.Response;
            Debug.Log("Updating dialogue object");
            Debug.Log($"It be {currentObject.GetType()}");
            currentObject.Execute();            
        }
        else
        {
            // TODO: end cycle
            GameManager.gm.EndCycle();
        }        
    }

    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        dialogueField.SetActive(true);

        animator.WriteDialogue(dialogue, pitch);
    }

    // Starts writing response to the given question to the current character
    public void AskQuestion(Question question)
    {
        GameManager.gm.numQuestionsAsked += 1;
        questionsField.SetActive(false);

        CharacterInstance recipient = GameManager.gm.dialogueRecipient;
        dialogueField.GetComponentInChildren<TextField>().SetText(recipient.characterName);

        List<string> answer = recipient.Answers[question];
        WriteDialogue(answer);
    }

    // Unity buttons don't accept enums as parameters in functions, so use this instead
    public void AskQuestion(string questionType)
    {
        AskQuestion((Question)Enum.Parse(typeof(Question), questionType));
    }

    public void AskQuestion(Question question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    public void AskQuestion(string question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    public void CreatePromptButton(Question question)
    {
        Button button = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        buttonText.text = GameManager.gm.GetPromptText(question);
        button.onClick.AddListener(() => AskQuestion(question, button));
    }

    public void CreateRandomPromptButton()
    {
        // Get random question that has not been asked yet
        CharacterInstance recipient = GameManager.gm.dialogueRecipient;
        int questionIndex = new System.Random().Next(recipient.RemainingQuestions.Count);
        Question question = recipient.RemainingQuestions[questionIndex];
        CreatePromptButton(question);
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