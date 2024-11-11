// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

/// <summary>
/// The manager for the dialogue scene
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue animator reference")]
    [SerializeField] private DialogueAnimator animator;

    [Header("Fields")]
    [SerializeField] private GameObject dialogueField;
    [SerializeField] private GameObject questionsField;
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject backgroundField;
    [SerializeField] private GameObject characterNameField;

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;

    [Header("Events")]
    public GameEvent onEndDialogue;

    [NonSerialized] public string inputText;
    [NonSerialized] public static DialogueManager dm;
    [NonSerialized] public CharacterInstance currentRecipient;
    [NonSerialized] public DialogueObject currentObject;
    
    /// <summary>
    /// Sets DialogueManager variables (currentObject & dialogueRecipient) and executes the starting DialogueObject.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data">Should be an array where element 0 is the dialogue recipient, 
    /// and element 1 is the starting dialogue object.</param>
    public void StartDialogue(Component sender, params object[] data)
    {
        // Set static DialogueManager sm
        dm = this;

        // Retrieve and set the dialogue object
        if (data[0] is DialogueObject dialogueObject)
        {
            currentObject = dialogueObject;
        }
        // Retrieve and set the dialogue recipient (if given)
        if (data.Length > 1 && data[1] is CharacterInstance recipient)
        {
            currentRecipient = recipient;
            characterNameField.SetActive(true);
        } 
        // No dialogue recipient given, so we remove the character name field
        else
        {
            characterNameField.SetActive(false);
        }
        
        // Execute the starting object
        currentObject.Execute();

        // Add event listener to check when dialogue is complete
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);
    }

    /// <summary>
    /// Executed when the dialogue animator has finished writing the dialogue.
    /// </summary>
    public void OnDialogueComplete()
    {
        // Close dialogue field
        dialogueField.SetActive(false);
        characterNameField.SetActive(false);
        
        // If we are in the Epilogue GameState and the next response object is an OpenResponseObject, create the open question.
        if (GameManager.gm.gameState == GameManager.GameState.Epilogue && currentObject.Responses[0] is OpenResponseObject)
            CreateOpenQuestion();

        ExecuteNextObject();
    }

    /// <summary>
    /// Gets the current object's first response and executes it.
    /// </summary>
    public void ExecuteNextObject()
    {
        currentObject = currentObject.Responses[0];
        currentObject.Execute();
    }

    /// <summary>
    /// Write the given dialogue to the screen using the dialogue animator.
    /// </summary>
    /// <param name="dialogue">The text that needs to be written</param>
    /// <param name="pitch">The pitch of the character</param>
    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        // Enable the dialogue field
        dialogueField.SetActive(true);
        
        // Adjust the box containing the character's name
        if (currentRecipient != null)
        {
            characterNameField.SetActive(true);
            characterNameField.GetComponentInChildren<TMP_Text>().text = currentRecipient.characterName;
        }

        // Animator write dialogue to the screen.
        pitch = currentRecipient == null ? 1 : currentRecipient.pitch;
        animator.WriteDialogue(dialogue, pitch);
    }

    /// <summary>
    /// Replaces the current dialogue background with the given background.
    /// </summary>
    /// <param name="newBackground">The background that will replace the current background.</param>
    public void ReplaceBackground(GameObject[] newBackground)
    {
        Transform parent = backgroundField.transform;

        // Remove old background
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        // Instantiate new background
        foreach (GameObject element in newBackground)
            Instantiate(element).transform.parent = parent;

    }

    /// <summary>
    /// Instantiates question (and return) buttons to the screen.
    /// </summary>
    /// <param name="questionObject">A <see cref="QuestionObject"/> containing the questions and responses</param>
    public void InstantiatePromptButtons(QuestionObject questionObject)
    {
        // Instantiate button containing each response
        foreach (ResponseObject response in questionObject.Responses)
        {
            // Instantiate and set parent
            Button button = Instantiate(buttonPrefab, questionsField.transform).GetComponent<Button>();

            // Set Unity inspector values
            button.name = "questionButton";
            button.gameObject.tag = "Button";

            // Set button text in question form
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = GetPromptText(response.question);

            // Set styling for button
            buttonText.enableAutoSizing = false;
            buttonText.fontSize = 40;

            // Add event when clicking the button
            button.onClick.AddListener(() => OnButtonClick(response));
        }

        // Add the button to return to the other characters
        CreateBackButton();

        // Set the buttons to be visible
        questionsField.SetActive(true);
    }

    /// <summary>
    /// Executed when a question button is pressed.
    /// </summary>
    /// <param name="response">A <see cref="ResponseObject"/> containing the response</param>
    public void OnButtonClick(ResponseObject response)
    {
        // Remove buttons from screen
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
        enterButton.transform.position = inputField.transform.position + new Vector3(0f, -300f, 0f);

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

        ExecuteNextObject();
    }
    
    /// <summary>
    /// Helper function for CreateBackButton.
    /// Sends the player back to the NPCSelect scene
    /// </summary>
    private void BacktoNPCScreen()
    {
        DestroyButtons();
        // TODO: Combineer met het unloaden van Dialoguescene
        currentObject = new TerminateDialogueObject();
        currentObject.Execute();
    }

    #region [DEPRECATED] Continue Button
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
    #endregion

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
    
    /// <summary>
    /// Gets the text for the buttons that prompt specific questions.
    /// </summary>
    /// <param name="questionType">The type of question that is being prompted.</param>
    /// <returns></returns>
    public string GetPromptText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "What's your name?",
            Question.Age => "How old are you?",
            Question.LifeGeneral => "How's life?",
            Question.Inspiration => "Is there anyone that inspires you?",
            Question.Sexuality => "What is your sexual orientation?",
            Question.Wellbeing => "How are you doing?",
            Question.Political => "What are your political thoughts?",
            Question.Personality => "Can you describe what your personality is like?",
            Question.Hobby => "What are some of your hobbies?",
            Question.CulturalBackground => "What is your cultural background?",
            Question.Religion => "Are you religious?",
            Question.Education => "What is your education level?",
            Question.CoreValues => "What core values are the most important to you?",
            Question.ImportantPeople => "Who are the most important people in your life?",
            Question.PositiveTrait => "What do you think is your best trait?",
            Question.NegativeTrait => "What is a bad trait you may have?",
            Question.OddTrait => "Do you have any odd traits?",
            Question.SocialIssues => "What social issues are you interested in?",
            Question.EducationSystem => "What is you opinion on the Dutch school system?",
            Question.Lottery => "If you win the lottery, what would you do?",
            Question.Diet => "Do you have any dietary restrictions?",
            _ => "",
        };
    }
}

/// <summary>
/// An enum containing all possible questions in the game.
/// </summary>
public enum Question
{
    Name,
    Age,
    LifeGeneral,
    Inspiration,
    Sexuality,
    Wellbeing,
    Political,
    Hobby,
    CulturalBackground,
    Religion,
    Education,
    CoreValues,
    Personality,
    ImportantPeople,
    PositiveTrait,
    NegativeTrait,
    OddTrait,
    SocialIssues,
    EducationSystem,
    Lottery,
    Diet
}