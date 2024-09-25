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

        currentObject = GameManager.gm.dialogueObject;
        currentObject.Execute();
        // Generate first prompts
        prompts = Instantiate(promptsUIPrefab, FindObjectOfType<Canvas>().transform);
        for (int i = 0; i < 2; i++)
            CreatePromptButton();
        CreateBackButton();
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
    }

    public void SetQuestionsField(bool active) => questionsField.SetActive(active);

    public void OnDialogueComplete()
    {
        // Close dialogue field
        dialogueField.SetActive(false);

        // Execute next dialogue object
        currentObject = currentObject.Responses[0];
        currentObject.Execute();
    }

    // Write given dialogue to the screen
    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        dialogueField.SetActive(true);

        // Adjust the box containing the character's name
        dialogueField.GetComponentInChildren<TextField>().SetText(GameManager.gm.dialogueRecipient.characterName);

        animator.WriteDialogue(dialogue, pitch);
    }

    // Instantiate question buttons
    public void CreatePromptButtons(QuestionObject questionObject)
    {
        foreach (ResponseObject response in questionObject.Responses)
        {
            Button button = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            // Set button text in question form
            buttonText.text = GameManager.gm.GetPromptText(response.question);

            // Add event when clicking the button
            button.onClick.AddListener(() => OnButtonClick(response));
            CreateContinueButton();
            CreateBackButton();
            // Create new prompt
                       
            // TODO: back to home button
            
        }
        else
        {
            // TODO: end cycle
            GameManager.gm.EndCycle();
        }
        
    }

    // Starts writing response to the given question to the current character
    public void AskQuestion(Question question)
    {
        prompts.SetActive(false);
        dialogue.SetActive(true);

        List<string> answer = recipient.Answers[question];
        animator.WriteDialogue(answer, recipient.pitch);
    }

    // Unity buttons don't accept enums as parameters in functions, so use this instead
    public void AskQuestion(string questionType)
    {
        AskQuestion((Question)Enum.Parse(typeof(Question), questionType));

        // NOTE: Below is an old version of the code above, I just have it here in case the code above breaks :)
        //switch (questionType)
        //{
        //    case "Name": AskQuestion(Question.Name); break;
        //    case "Age": AskQuestion(Question.Age); break;
        //    case "Wellbeing": AskQuestion(Question.Wellbeing); break;
        //    case "Political": AskQuestion(Question.Political); break;
        //    case "Personality": AskQuestion(Question.Personality); break;
        //    case "Hobby": AskQuestion(Question.Hobby); break;
        //    case "CulturalBackground": AskQuestion(Question.CulturalBackground); break;
        //    case "Education": AskQuestion(Question.Education); break;
        //    case "CoreValues": AskQuestion(Question.CoreValues); break;
        //    case "ImportantPeople": AskQuestion(Question.ImportantPeople); break;
        //    case "PositiveTrait": AskQuestion(Question.PositiveTrait); break;
        //    case "NegativeTrait": AskQuestion(Question.NegativeTrait); break;
        //    case "OddTrait": AskQuestion(Question.OddTrait); break;
        //    default: Debug.Log("Invalid question string"); break;
        //}
    }

    public void AskQuestion(Question question, Button button)
    {
        DestroyButtons();
        AskQuestion(question);
    }

    public void AskQuestion(string question, Button button)
    {
        DestroyButtons();
        AskQuestion(question);
    }

    private void CreatePromptButton()
    {
        if (recipient.RemainingQuestions.Count > 0)
        {
            // Get random question that has not been asked yet
            int questionIndex = new System.Random().Next(recipient.RemainingQuestions.Count);
            Question buttonType = recipient.RemainingQuestions[questionIndex];

            // Remove the question from list of questions to be asked
            //recipient.RemainingQuestions.RemoveAt(questionIndex);

            Button button = Instantiate(buttonPrefab, prompts.transform).GetComponent<Button>();
            button.gameObject.tag = "Button";
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            buttonText.text = GetPromptText(buttonType);
            buttonText.enableAutoSizing = false;
            buttonText.fontSize = 40;
            button.onClick.AddListener(() => AskQuestion(buttonType.ToString(), button));
        }
        else
        {
            Debug.Log("No more questions to ask this character.");
            // TODO: In de selectscene duidelijk maken dat dit character geen vragen meer kan beantwoorden
        }
    }

    /// <summary>
    /// Creates the button to go back to the NPCSelect screen.
    /// </summary>
    private void CreateBackButton()
    {
        Button backbutton = Instantiate(buttonPrefab, prompts.transform).GetComponent<Button>();
        backbutton.name = "backButton";
        backbutton.gameObject.tag = "Button";
        TMP_Text buttonText = backbutton.GetComponentInChildren<TMP_Text>();

        buttonText.text = "Talk to someone else";
        buttonText.enableAutoSizing = false;
        buttonText.fontSize = 40;
        backbutton.onClick.AddListener(() => BacktoNPCScreen());
    }

    /// <summary>
    /// Helper function for CreateBackButton.
    /// Sends the player back to the NPCSelect scene
    /// </summary>
    private void BacktoNPCScreen()
    {
        DestroyButtons();
        GameManager.gm.UnloadDialogueScene();
        GameManager.gm.ToggleNPCSelectScene();
    }

    /// <summary>
    /// Creates the button to ask another question to the same NPC
    /// </summary>
    private void CreateContinueButton()
    {
        Button button = Instantiate(buttonPrefab, prompts.transform).GetComponent<Button>();
        button.gameObject.tag = "Button";
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        buttonText.text = "Ask another question";
        buttonText.enableAutoSizing = false;
        buttonText.fontSize = 40;
        button.onClick.AddListener(() => ContinueTalking());
    }

    /// <summary>
    /// Helper function for CreateContinueButton.
    /// Lets the player ask a question to the NPC
    /// </summary>
    private void ContinueTalking()
    {
        Debug.Log("Continue");
        DestroyButtons();
        for (int i = 0; i < 2; i++)
        {
            CreatePromptButton();
            Debug.Log("Create Question");
        }

        CreateBackButton();
    }

    /// <summary>
    /// Destroys all buttons with the "Button" tag currently in the scene.
    /// If a button should not be destroyed do not give it the "Button" tag .
    /// </summary>
    private void DestroyButtons()
    {
        var buttons = GameObject.FindGameObjectsWithTag("Button");
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i]);
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