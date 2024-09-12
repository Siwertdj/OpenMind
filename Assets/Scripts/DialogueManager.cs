using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogue;
    [SerializeField] private DialogueAnimator animator;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject promptsUIPrefab;
    
    private GameObject prompts;
    private DialogueRecipient recipient;

    // Start is called before the first frame update
    void Start()
    {
        // Generate answers for recipient
        // TODO: should be done in a different (more versatile) way later
        recipient = GenerateRecipient();

        // Add event listener to check when dialogue is complete
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);

        // Generate first prompts
        prompts = Instantiate(promptsUIPrefab, FindObjectOfType<Canvas>().transform);
        for (int i = 0; i < 2; i++)
            CreatePromptButton();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse input to skip current dialogue
        if (Input.GetMouseButtonDown(0) && animator.InDialogue)
            animator.SkipDialogue();
    }

    public void OnDialogueComplete()
    {
        dialogue.SetActive(false);
        prompts.SetActive(true);

        // Create new prompt
        CreatePromptButton();
    }

    public void AskQuestion(QuestionType question)
    {
        prompts.SetActive(false);
        dialogue.SetActive(true);

        List<string> answer = recipient.GetAnswer(question);
        animator.WriteDialogue(answer);
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

    public void AskQuestion(QuestionType question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    public void AskQuestion(string question, Button button)
    {
        Destroy(button.gameObject);
        AskQuestion(question);
    }

    private void CreatePromptButton()
    {
        if (recipient.RemainingQuestions.Count > 0)
        {
            // Get random question that has not been asked yet
            int questionIndex = new System.Random().Next(recipient.RemainingQuestions.Count);
            QuestionType buttonType = recipient.RemainingQuestions[questionIndex];

            // Remove the question from list of questions to be asked
            recipient.RemainingQuestions.RemoveAt(questionIndex);

            Button button = Instantiate(buttonPrefab, prompts.transform).GetComponent<Button>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            buttonText.text = GetPromptText(buttonType);
            button.onClick.AddListener(() => AskQuestion(buttonType.ToString(), button));
        }
        else
        {
            Debug.Log("No more questions remaining; prompt not created");
        }
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

    private DialogueRecipient GenerateRecipient()
    {
        DialogueRecipient dialogueRecipient = new();

        dialogueRecipient.Name = "J. Gorbingson";

        dialogueRecipient.Answers.Add(
            QuestionType.Political,
            new List<string> { "I think that political stuff is really stuff", "it really is..." });
        dialogueRecipient.Answers.Add(
            QuestionType.Personality,
            new List<string> { "I'm someone who really stuffs", "I stuff all the time fr", "pluh" });
        dialogueRecipient.Answers.Add(
            QuestionType.Hobby,
            new List<string> { "hobby" });
        dialogueRecipient.Answers.Add(
            QuestionType.CulturalBackground,
            new List<string> { "the culture" });
        dialogueRecipient.Answers.Add(
            QuestionType.Education,
            new List<string> { "edutationasd" });
        dialogueRecipient.Answers.Add(
            QuestionType.CoreValues,
            new List<string> { "cor vales" });
        dialogueRecipient.Answers.Add(
            QuestionType.ImportantPeople,
            new List<string> { "people be important" });
        dialogueRecipient.Answers.Add(
            QuestionType.PositiveTrait,
            new List<string> { "i am just great" });
        dialogueRecipient.Answers.Add(
            QuestionType.NegativeTrait,
            new List<string> { "dont have any" });
        dialogueRecipient.Answers.Add(
            QuestionType.OddTrait,
            new List<string> { "... eeeeeeee ... ee. .. ..." });

        return dialogueRecipient;
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