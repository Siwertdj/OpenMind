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
    [SerializeField] private CharacterData[] characters;
    
    private GameObject prompts;
    private CharacterInstance recipient;

    // Start is called before the first frame update
    void Start()
    {
        recipient = GameManager.gm.dialogueRecipient;
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
        GameManager.gm.numTalked += 1;
     
        dialogue.SetActive(false);
        prompts.SetActive(true);
        
        if (GameManager.gm.HasQuestionsLeft())
        {
            // Create new prompt
            CreatePromptButton();            
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
        animator.WriteDialogue(answer);
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
            Question buttonType = recipient.RemainingQuestions[questionIndex];

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

    private string GetPromptText(Question questionType)
    {
        return questionType switch
        {
            Question.Name => "What is your name?",
            Question.Age => "How old are you?",
            Question.Wellbeing => "How are you doing?",
            Question.Political => "What are your political thoughts?",
            Question.Personality => "Can you describe what your personality is like?",
            Question.Hobby => "What are some of your hobbies?",
            Question.CulturalBackground => "What is your cultural background?",
            Question.Education => "What is your education level?",
            Question.CoreValues => "What core values are the most important to you?",
            Question.ImportantPeople => "Who are the most important people in your life?",
            Question.PositiveTrait => "What do you think is your best trait?",
            Question.NegativeTrait => "What is a bad trait you may have?",
            Question.OddTrait => "Do you have any odd traits?",
            _ => "",
        };
    }

    private CharacterInstance GenerateRecipient() => new(characters[new System.Random().Next(characters.Length)]);
    
    private CharacterInstance GetRecipient(int id) => new(characters[id]);
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