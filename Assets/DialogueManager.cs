using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject prompts;
    [SerializeField] private GameObject dialogue;
    [SerializeField] private DialogueAnimator animator;
    [SerializeField] private GameObject buttonPrefab;

    // private DialoguePrompter prompter;
    private DialogueRecipient recipient = ;

    List<QuestionType> questions = new();

    // Start is called before the first frame update
    void Start()
    {
        animator.OnDialogueComplete.AddListener(OnDialogueComplete);

        recipient.answers.Add(
            QuestionType.Political, 
            new List<string> { "I think that political stuff is really stuff", "it really is..." });
        recipient.answers.Add(
            QuestionType.Personality,
            new List<string> { "I'm someone who really stuffs", "I stuff all the time fr", "pluh" });
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
        prompts.SetActive(true);
    }

    // Unity buttons don't accept enums as parameters in functions, so use this instead
    public void AskQuestion(string questionType)
    {
        switch (questionType)
        {
            case "political": AskQuestion(QuestionType.Political); break;
            case "personality": AskQuestion(QuestionType.Personality); break;
            case "hobby": AskQuestion(QuestionType.Hobby); break;
            case "culture": AskQuestion(QuestionType.CulturalBackground); break;
            case "education": AskQuestion(QuestionType.Education); break;
            case "values": AskQuestion(QuestionType.CoreValues); break;
            case "importantpeople": AskQuestion(QuestionType.ImportantPeople); break;
            case "positivetrait": AskQuestion(QuestionType.PositiveTrait); break;
            case "negativetrait": AskQuestion(QuestionType.NegativeTrait); break;
            case "oddtrait": AskQuestion(QuestionType.OddTrait); break;            
            default: break;
        }
    }

    public void AskQuestion(QuestionType question)
    {
        prompts.SetActive(false);
        dialogue.SetActive(true);

        List<string> answer = recipient.GetAnswer(question);
        animator.WriteDialogue(answer);
    }

    public void CreatePromptButton()
    {
        Button button = Instantiate(buttonPrefab).GetComponent<Button>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        
        
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