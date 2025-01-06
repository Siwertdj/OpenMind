// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

/// <summary>
/// The manager for the dialogue scene
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset customFont;

    [Header("Dialogue animator reference")]
    [SerializeField] private DialogueAnimator animator;

    [Header("Fields")]
    [SerializeField] private GameObject dialogueField;
    [SerializeField] private GameObject imageField;
    [SerializeField] private GameObject questionsField;
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject backgroundField;
    [SerializeField] private GameObject characterNameField;
    [SerializeField] private GameObject phoneField;

    [Header("Prefabs")]
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject avatarPrefab; // A prefab containing a character
    [SerializeField] private GameObject[] backgroundPrefabs; // The list of backgrounds for use in character dialogue
    [SerializeField] private GameObject phoneDialogueBoxPrefab;

    [Header("Events")]
    public GameEvent onEndDialogue;
    public UnityEvent onEpilogueEnd;
    public GameEvent stopLoadIcon;

    
    [FormerlySerializedAs("testDialogueObject")]
    [Header("Test variables")]
    [SerializeField] private DialogueContainer testDialogueContainer;
    
    // Variables
    [NonSerialized] public        string            inputText;
    [NonSerialized] public        List<string>      playerAnswers;
    [NonSerialized] public static DialogueManager   dm;
    [NonSerialized] public        CharacterInstance currentRecipient;
    [NonSerialized] public        DialogueObject    currentObject;
    private                       Component         dialogueStarter;
    
    // In this awake, we initialize some components in case it is loaded in isolation.
    // It does not need to rely on GameManager to be active, but it needs an eventsystem
    private void Awake()
    {
        if (GameObject.Find("EventSystem") == null)
        {
            Instantiate(eventSystemPrefab);
            StartDialogue(null, testDialogueContainer.GetDialogue());
        }
        
        // Set static DialogueManager instance
        dm = this;
    }

    /// <summary>
    /// Sets DialogueManager variables (currentObject & dialogueRecipient) and executes the starting DialogueObject.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data">Should be an array where element 0 is the dialogue recipient, 
    /// and element 1 is the starting dialogue object.</param>
    public void StartDialogue(Component sender, params object[] data)
    {
        // Save the sender of the event that started dialogue
        dialogueStarter = sender;
        
        // Change the text size
        characterNameField.GetComponentInChildren<TMP_Text>().enableAutoSizing = false;
        ChangeTextSize();
        
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

        // Add event listener to check when dialogue is complete
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);
        
        // Execute the starting object to begin dialogue
        currentObject.Execute();
    }

    /// <summary>
    /// Executed when the dialogue animator has finished writing the dialogue.
    /// </summary>
    public void OnDialogueComplete()
    {
        // If the current object is a TerminateDialogueObject, don't do anything,
        // just wait for the scene to unload.
        if (currentObject is TerminateDialogueObject)
            return;

        // Close dialogue field
        dialogueField.SetActive(false);
        characterNameField.SetActive(false);

        ExecuteNextObject();
    }

    /// <summary>
    /// Gets the current object's first response and executes it.
    /// </summary>
    private void ExecuteNextObject()
    {
        currentObject = currentObject.Responses.First();
        currentObject.Execute();
    }

    /// <summary>
    /// Write the given dialogue to the screen using the dialogue animator.
    /// </summary>
    /// <param name="dialogue">The text that needs to be written</param>
    /// <param name="pitch">The pitch of the character</param>
    public void WriteDialogue(List<string> dialogue, float pitch = 1)
    {
        // The text of this dialogue object is null, so we dont open the window.
        // This can be in case of a pause, an image, etc.
        if (dialogue == null)
        {
            dialogueField.SetActive(false);
            ExecuteNextObject();
        }
        else
        {
            // Enable the dialogue field
            dialogueField.SetActive(true);

            // Adjust the box containing the character's name
            if (currentRecipient != null)
            {
                characterNameField.SetActive(true);
                characterNameField.GetComponentInChildren<TMP_Text>().text =
                    currentRecipient.characterName;
            }

            // Animator write dialogue to the screen.
            pitch = currentRecipient == null ? 1 : currentRecipient.pitch;
            animator.WriteDialogue(dialogue, pitch);
        }
    }

    public void WritePhoneDialogue(string newMessage, List<string> previousMessages)
    {
        imageField.SetActive(false);
        questionsField.SetActive(false);
        dialogueField.SetActive(false);
        phoneField.SetActive(true);

        // Remove previous messages
        foreach (Transform child in phoneField.transform)
            Destroy(child.gameObject);

        // Write previous messages
        foreach (string message in previousMessages)
        {
            var prevMessageObject = Instantiate(phoneDialogueBoxPrefab, phoneField.transform);
            prevMessageObject.GetComponent<ResizingTextBox>().SetText(message);
        }

        // Write current message
        var messageObject = Instantiate(phoneDialogueBoxPrefab, phoneField.transform);
        messageObject.GetComponent<ResizingTextBox>().SetText(newMessage);
    }

    public void PrintImage(Sprite newImage)
    {
        if (newImage == null)
            imageField.SetActive(false);
        else
        {
            // Enable the image field
            imageField.SetActive(true);

            // Set the content of the image
            imageField.GetComponent<Image>().sprite = newImage;
        }
    }
    
    /// <summary>
    /// Creates a background for the coming dialogue.
    /// </summary>
    /// <param name="story">The storyobject that contains the backgrounds for the dialogue.</param>
    /// <param name="character">The character the dialogue will be with.</param>
    /// <param name="background">The background for the dialogue.</param>
    /// <returns></returns>
    public GameObject[] CreateDialogueBackground(StoryObject story, CharacterInstance character = null, GameObject background = null)
    {
        List<GameObject> background_ = new();

        // If the passed background is null, we use 'dialogueBackground' as the default. Otherwise, we use the passed one.
        background_.Add(background == null ? story.dialogueBackground : background);

        // If a character is given, add that as well with the proper emotion
        if (character != null)
        {
            avatarPrefab.GetComponent<Image>().sprite = 
                character.avatarEmotions.First(es => es.Item1 == Emotion.Neutral).Item2;
            background_.Add(avatarPrefab);
        }

        return background_.ToArray();
    }
    
    /// <summary>
    /// Replaces the current dialogue background with the given background.
    /// </summary>
    /// <param name="newBackground">The background that will replace the current background.</param>
    public void ReplaceBackground(GameObject[] newBackground, Emotion? emotion = null)
    {
        if (newBackground != null)
        {
            if (newBackground.Length > 0)
            {
                Transform parent = backgroundField.transform;

                // Remove old background
                foreach (Transform child in parent)
                    Destroy(child.gameObject);

                // Instantiate new background
                foreach (GameObject prefab in newBackground)
                {
                    var image = Instantiate(prefab).GetComponent<Image>();
                    image.rectTransform.SetParent(parent, false);
                    
                }
            }
        }
        // Set emotion
        if (emotion.HasValue)
        {
            foreach (Transform child in backgroundField.transform)
            {
                if (child.gameObject.name == "Character Avatar(Clone)")
                {
                    if (currentRecipient != null)
                        child.GetComponent<Image>().sprite =
                            currentRecipient.avatarEmotions.First(es => es.Item1 == emotion.Value).Item2;
                }
            }
        }
    }


    /// <summary>
    /// Instantiates question (and return) buttons to the screen.
    /// </summary>
    /// <param name="questionDialogueObject">A <see cref="QuestionDialogueObject"/> containing the questions and responses</param>
    public void InstantiatePromptButtons(QuestionDialogueObject questionDialogueObject)
    {
        // Instantiate button containing each responseDialogue
        foreach (ResponseDialogueObject response in questionDialogueObject.Responses)
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
            buttonText.fontSize = SettingsManager.sm.GetFontSize();
            buttonText.font = customFont;

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
    /// <param name="responseDialogue">A <see cref="ResponseDialogueObject"/> containing the responseDialogue</param>
    public void OnButtonClick(ResponseDialogueObject responseDialogue)
    {
        // Remove buttons from screen
        DestroyButtons();

        // Remove questions field
        questionsField.SetActive(false);

        // Write dialogue when button is pressed
        currentObject = responseDialogue;
        currentObject.Execute();
    }

    /// <summary>
    /// Creates the buttons and the text field for the open questions.
    /// </summary>
    public void CreateOpenQuestion(List<string> dialogue)
    {
        // Enable the input field.
        inputField.SetActive(true);
        inputField.gameObject.GetComponentInChildren<TMP_InputField>().text = "";

        animator.InOpenQuestion = true;        
        WriteDialogue(dialogue);

        // TODO: Save answer somewhere?
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
        buttonText.fontSize = SettingsManager.sm.GetFontSize();
        buttonText.font = customFont;
        backButton.onClick.AddListener(() => BacktoNPCScreen());
    }

    /// <summary>
    /// Continues the dialogue after answering the open question.
    /// </summary>
    public void AnswerOpenQuestion()
    {        
        // Assign the text from the inputField to inputText and add it to the list of answers.
        inputText = inputField.GetComponentInChildren<TMP_InputField>().text;
        // Can use this to write the inputText to somewhere, here..

        // Disable the input field.
        inputField.SetActive(false);
        
        // Reset the text from the input field.
        inputText = "";

        animator.InOpenQuestion = false;
        ExecuteNextObject();
    }

    
    /// <summary>
    /// Helper function for CreateBackButton.
    /// Sends the player back to the NPCSelect scene
    /// TODO: This button should take into account other situations dialgoue may appear beside in the gameloop
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
    
    #region TextSize

    /// <summary>
    /// Change the fontSize of the tmp_text components when a different textSize is chosen in the settings menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    // TODO: could be made private.
    public void OnChangedTextSize(Component sender, params object[] data)
    {
        // Set the fontSize.
        if (data[0] is int fontSize)
        {
            // Change the characterNameField fontSize
            characterNameField.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
            // Change the animator text fontSize
            animator.ChangeTextSize(fontSize);
            // Change the question and return button fontSize if they are present.
            foreach (Button b in questionsField.GetComponentsInChildren<Button>())
            {
                TMP_Text buttonText = b.GetComponentInChildren<TMP_Text>();
                buttonText.fontSize = fontSize;
            }
        }
    }

    /// <summary>
    /// Change the fontSize of the tmp_text components (excluding questions and return button)
    /// </summary>
    // TODO: could be made private.
    public void ChangeTextSize()
    {
        // Set the fontSize.
        int fontSize = SettingsManager.sm.GetFontSize();
        characterNameField.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
        animator.ChangeTextSize(fontSize);
    }

    #endregion
    
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