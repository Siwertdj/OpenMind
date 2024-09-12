using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogue;
    [SerializeField] private DialogueAnimator animator;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject promptsUIPrefab;

    // private DialoguePrompter prompter;
    private GameObject promptsUI;
    private DialogueRecipient recipient = new();

    // Start is called before the first frame update
    void Start()
    {
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);

        promptsUI = Instantiate(promptsUIPrefab, FindObjectOfType<Canvas>().transform);
        for (int i = 0; i < 2; i++)
            CreatePromptButton();

        recipient.answers.Add(
            QuestionType.Political, 
            new List<string> { "I think that political stuff is really stuff", "it really is..." });
        recipient.answers.Add(
            QuestionType.Personality,
            new List<string> { "I'm someone who really stuffs", "I stuff all the time fr", "pluh" });
        recipient.answers.Add(
            QuestionType.Hobby,
            new List<string> { "hobby" });
        recipient.answers.Add(
            QuestionType.CulturalBackground,
            new List<string> { "the culture" });
        recipient.answers.Add(
            QuestionType.Education,
            new List<string> { "edutationasd" });
        recipient.answers.Add(
            QuestionType.CoreValues,
            new List<string> { "cor vales" });
        recipient.answers.Add(
            QuestionType.ImportantPeople,
            new List<string> { "people be important" });
        recipient.answers.Add(
            QuestionType.PositiveTrait,
            new List<string> { "i am just great" });
        recipient.answers.Add(
            QuestionType.NegativeTrait,
            new List<string> { "dont have any" });
        recipient.answers.Add(
            QuestionType.OddTrait,
            new List<string> { "... eeeeeeee ... ee. .. ..." });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AskQuestion(QuestionType.Political);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            AskQuestion(QuestionType.Personality);

        if (Input.GetMouseButtonDown(0) && animator.inDialogue)
            animator.SkipDialogue();
    }

    public void OnDialogueComplete()
    {
        Debug.Log("Dialogue is complete");
        dialogue.SetActive(false);
        promptsUI.SetActive(true);

        // Create new prompt
        CreatePromptButton();
    }

    // Unity buttons don't accept enums as parameters in functions, so use this instead
    public void AskQuestion(string questionType)
    {
        switch (questionType)
        {
            case "Political": AskQuestion(QuestionType.Political); break;
            case "Personality": AskQuestion(QuestionType.Personality); break;
            case "Hobby": AskQuestion(QuestionType.Hobby); break;
            case "CulturalBackground": AskQuestion(QuestionType.CulturalBackground); break;
            case "Education": AskQuestion(QuestionType.Education); break;
            case "CoreValues": AskQuestion(QuestionType.CoreValues); break;
            case "ImportantPeople": AskQuestion(QuestionType.ImportantPeople); break;
            case "PositiveTrait": AskQuestion(QuestionType.PositiveTrait); break;
            case "NegativeTrait": AskQuestion(QuestionType.NegativeTrait); break;
            case "OddTrait": AskQuestion(QuestionType.OddTrait); break;            
            default: Debug.Log("Invalid question string"); break;
        }
    }

    public void AskQuestion(QuestionType question)
    {
        Debug.Log($"Asking question {question}");

        promptsUI.SetActive(false);
        dialogue.SetActive(true);

        List<string> answer = recipient.GetAnswer(question);
        animator.WriteDialogue(answer);
    }

    public void AskQuestion(string question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    public void AskQuestion(QuestionType question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    private void CreatePromptButton()
    {
        // Get random question that has not been asked yet
        int questionIndex = new System.Random().Next(recipient.remainingQuestions.Count);
        QuestionType buttonType = recipient.remainingQuestions[questionIndex];

        // Remove the question from list of questions to be asked
        recipient.remainingQuestions.RemoveAt(questionIndex);
        
        Button button = Instantiate(buttonPrefab, promptsUI.transform).GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        buttonText.text = GetPromptText(buttonType);
        button.onClick.AddListener(() => AskQuestion(buttonType.ToString(), button));
    }

    private string GetPromptText(QuestionType questionType)
    {
        return questionType switch
        {
            QuestionType.Political => "What are your political thoughts?",
            QuestionType.Personality => "Can you describe what your personality is like?",
            QuestionType.Hobby => "What are some of your hobbies?",
            QuestionType.CulturalBackground => "What is your cultural background?",
            QuestionType.Education => "What is your education level?",
            QuestionType.CoreValues => "What core values are the most important to you?",
            QuestionType.ImportantPeople => "Who are the most important people in your life?",
            QuestionType.PositiveTrait => "What do you think is your best trait?",
            QuestionType.NegativeTrait => "What is a bad trait you may have?",
            QuestionType.OddTrait => "Do you have any odd traits?",
            _ => "",
        };
    }
}

public enum QuestionType
{
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