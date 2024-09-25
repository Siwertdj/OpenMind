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