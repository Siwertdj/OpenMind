using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dm;

    [SerializeField] private GameObject dialogueField;
    [SerializeField] private GameObject questionsField;
    [SerializeField] private DialogueAnimator animator;
    [SerializeField] private GameObject inputField;

    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private SpriteRenderer avatar;

    public GameObject background;

    public UnityEvent OnEndDialogue;

    public DialogueObject currentObject;

    public string inputText;

    // Start is called before the first frame update
    void Start()
    {
        dm = this;

        // Add event listener to check when dialogue is complete
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);

        OnEndDialogue.AddListener(GameManager.gm.CheckEndCycle);

        avatar.sprite = GameManager.gm.dialogueRecipient.avatar;
        currentObject = GameManager.gm.dialogueObject;
        currentObject.Execute();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse input to skip current dialogue
        if (Input.GetMouseButtonDown(0) && animator.InDialogue && !EventSystem.current.IsPointerOverGameObject())
            animator.SkipDialogue();
    }

    /// <summary>
    /// Sets prompts to parameter value.
    /// </summary>
    /// <param name="active"></param>
    public void SetQuestionsField(bool active) => questionsField.SetActive(active);

    /// <summary>
    /// Executed when the dialogue animator has finished writing the dialogue.
    /// </summary>
    public void OnDialogueComplete()
    {
        // Close dialogue field
        dialogueField.SetActive(false);

        if (GameManager.gm.isEpilogue)
            CreateOpenQuestion();
        
        // Execute next dialogue object
        currentObject = currentObject.Responses[0];
        currentObject.Execute();
    }

    /// <summary>
    /// Write the given dialogue to the screen using the dialogue animator.
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="pitch"></param>
    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        // Enable the dialogue field
        dialogueField.SetActive(true);
        
        // Adjust the box containing the character's name
        if (GameManager.gm.dialogueObject != null)
            dialogueField.GetComponentInChildren<TextField>().SetText(GameManager.gm.dialogueRecipient.characterName);

        // Animator write dialogue to the screen.
        animator.WriteDialogue(dialogue, pitch);
    }

    /// <summary>
    /// Instantiates question (and return) buttons to the screen.
    /// </summary>
    /// <param name="questionObject"></param>
    public void CreatePromptButtons(QuestionObject questionObject)
    {
        foreach (ResponseObject response in questionObject.Responses)
        {
            Button button = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();
            button.name = "questionButton";
            button.gameObject.tag = "Button";

            // Set button text in question form
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = GameManager.gm.GetPromptText(response.question);
            buttonText.enableAutoSizing = false;
            buttonText.fontSize = 40;

            // Add event when clicking the button
            button.onClick.AddListener(() => OnButtonClick(response));
        }

        // Add the button to return to the other characters
        CreateBackButton();
    }

    /// <summary>
    /// Executed when a question button is pressed.
    /// </summary>
    /// <param name="response"></param>
    public void OnButtonClick(ResponseObject response)
    {
        DestroyButtons();

        // Remove questions field
        questionsField.SetActive(false);

        // Write dialogue when button is pressed
        currentObject = response;
        currentObject.Execute();
    }

    /// <summary>
    /// Creates the buttons and the text field for the open questions.
    /// </summary>
    private void CreateOpenQuestion()
    {
        // Enable the input field.
        inputField.SetActive(true);
        
        // Create the enter button.
        Button enterButton = Instantiate(buttonPrefab, inputField.transform).GetComponent<Button>();
        enterButton.name = "enterButton";
        enterButton.gameObject.tag = "Button";

        TMP_Text buttonText = enterButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Enter";
        buttonText.enableAutoSizing = false;
        buttonText.fontSize = 40;
        enterButton.onClick.AddListener(() => AnswerOpenQuestion());
    }
    
    /// <summary>
    /// Creates the button to go back to the NPCSelect screen.
    /// </summary>
    private void CreateBackButton()
    {
        Button backButton = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();
        backButton.name = "backButton";
        backButton.gameObject.tag = "Button";

        TMP_Text buttonText = backButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Talk to someone else";
        buttonText.enableAutoSizing = false;
        buttonText.fontSize = 40;
        backButton.onClick.AddListener(() => BacktoNPCScreen());
    }

    /// <summary>
    /// Continues the dialogue after answering the open question.
    /// </summary>
    private void AnswerOpenQuestion()
    {
        DestroyButtons();
        
        // Assign the text from the inputField to inputText.
        // TODO: can write the answers from the open questions to somewhere.
        inputText = inputField.GetComponent<TMP_InputField>().text;
        
        // Disable the input field.
        inputField.SetActive(false);
        
        // Reset the text from the input field.
        inputField.GetComponentInChildren<TMP_InputField>().text = "";
        
        // Go to the next part of the dialogue.
        currentObject = currentObject.Responses[0];
        currentObject.Execute();
    }
    /// <summary>
    /// Helper function for CreateBackButton.
    /// Sends the player back to the NPCSelect scene
    /// </summary>
    private void BacktoNPCScreen()
    {
        DestroyButtons();
        currentObject = new TerminateDialogueObject(SceneController.sc.ToggleNPCSelectScene);
        currentObject.Execute();
    }

    /// <summary>
    /// Creates the button to ask another question to the same NPC
    /// </summary>
    private void CreateContinueButton()
    {
        Button button = Instantiate(buttonPrefab, dialogueField.transform).GetComponent<Button>();
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
            currentObject.Execute();
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
            Destroy(buttons[i]);
    }
}

/// <summary>
/// An enum containing all possible questions in the game.
/// </summary>
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