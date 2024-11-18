// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manager class for the NPCSelect scene.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float buttonFadeDuration;
    [SerializeField] private float scrollDuration;

    [Header("Prefabs")]
    [SerializeField] private GameObject optionPrefab;

    [Header("References")]
    [SerializeField] private GameButton confirmSelectionButton;
    [SerializeField] private NPCSelectScroller scroller;
    [SerializeField] private TextMeshProUGUI headerText;

    private Coroutine fadeCoroutine;

    /// <summary>
    /// On startup, set the selectionType of the scene, set the headertext and generate the selectable options.
    /// </summary>
    private void Start()
    {
        SetSceneType();
        SetHeaderText();
        GenerateOptions();

        scroller.OnCharacterSelected.AddListener(EnableSelectionButton);
        scroller.NoCharacterSelected.AddListener(DisableSelectionButton);
        scroller.scrollDuration = scrollDuration;
    }

    /// <summary>
    /// Set the selectionType variable.
    /// If the number of characters has reached the minimum amount, and the player has no more questions left,
    /// set the selectionType variable to decidecriminal.
    /// </summary>
    private void SetSceneType()
    {
        if (!GameManager.gm.EnoughCharactersRemaining())
            GameManager.gm.gameState = GameManager.GameState.CulpritSelect;
    }

    /// <summary>
    /// Change the Header text if the culprit needs to be chosen.
    /// </summary>
    /// <param name="sceneType"> Can take "dialogue" or "decidecriminal" as value. </param>
    private void SetHeaderText()
    {
        if (GameManager.gm.gameState == GameManager.GameState.CulpritSelect)
            headerText.text = "Who do you think it was?";
        else
            headerText.text = "Who do you want to talk to?";
    }
    
    /// <summary>
    /// Generates the selectOption objects for the characters.
    /// </summary>
    private void GenerateOptions()
    {
        // Create a SelectOption object for every character in currentCharacters.
        for (int i = 0; i < GameManager.gm.currentCharacters.Count; i++)
        {
            var character = GameManager.gm.currentCharacters[i];

            // Create a new SelectOption object.
            SelectOption newOption = Instantiate(optionPrefab).GetComponent<SelectOption>();
            newOption.character = character;

            // Set the parent & position of the object
            newOption.transform.SetParent(scroller.transform.GetChild(0).GetChild(i), false);
            newOption.transform.position = newOption.transform.parent.position;
        }
    }

    #region Selection Button Logic
    /// <summary>
    /// Event for when a character is selected.
    /// </summary>
    /// <param name="option"> The character space on which has been clicked on. </param>
    /// TODO: Add an intermediate choice for the culprit. (if everyone agrees with the storyline epilogue)
    public void SelectionButtonClicked(CharacterInstance character)
    {
        // Only active characters can be talked to.
        if (!character.isActive)
            return;

        // Start the epilogue scene if CulpritSelect is active
        if (GameManager.gm.gameState == GameManager.GameState.CulpritSelect)
        {
            // Set the FinalChosenCulprit variable to the chosen character in GameManager.
            GameManager.gm.FinalChosenCuplrit = character;

            // Set the hasWon variable to true if the correct character has been chosen, else set it to false.
            GameManager.gm.hasWon = GameManager.gm.GetCulprit().characterName == character.characterName;

            // Load the epilogue scene.
            GameManager.gm.StartEpilogueDialogue(character);
        }
        else
        {
            // No special gamestate, so we start dialogue with the given character
            GameManager.gm.StartDialogue(character);
        }
    }
    /// <summary>
    /// Enables the character selection button & sets it to the selected character.
    /// </summary>
    private void EnableSelectionButton()
    {
        var button = confirmSelectionButton;

        // Set appropriate text whether or not selected character is active
        var text = button.GetComponentInChildren<TMP_Text>();
        string characterName = scroller.SelectedCharacter.characterName;

        // Button text should not be interactable if character is not active
        if (scroller.SelectedCharacter.isActive)
        {
            button.interactable = true;

            // Set the button text depending on the gameState
            text.text = GameManager.gm.gameState == GameManager.GameState.CulpritSelect ?
                    $"Approach {characterName}" : $"Talk to {characterName}";
        }
        else
        {
            button.interactable = false;
            text.text = $"{characterName} {GameManager.gm.story.victimDialogue}";
        }

        // Force the button to change state immediately
        Canvas.ForceUpdateCanvases();

        // Add appropriate "start dialogue" button for selected character
        button.onClick.AddListener(() => SelectionButtonClicked(scroller.SelectedCharacter));

        // Fade the button in
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeIn(button.GetComponent<CanvasGroup>(), buttonFadeDuration));
    }

    /// <summary>
    /// Disables the character selection button & removes its listeners.
    /// </summary>
    private void DisableSelectionButton()
    {
        var button = confirmSelectionButton;
        button.onClick.RemoveAllListeners();

        // Fade the button out
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut(button.GetComponent<CanvasGroup>(), buttonFadeDuration));
    }
    #endregion

    #region Fade Logic
    /// <summary>
    /// Fades an object in using a canvas group.
    /// </summary>
    /// <param name="cg">The canvas group of the object to be faded</param>
    /// <param name="duration">The duration of the fade</param>
    private IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        Debug.Log("Fading in");
        // First, set the given gameObject to active
        cg.gameObject.SetActive(true);

        // Make the object interactable in advance to make it more responsive
        cg.interactable = true;

        float startingAlpha = cg.alpha;
        float time = 0;
        while (time <= duration)
        {
            time += Time.deltaTime;

            // Use lerp to interpolate the fade
            cg.alpha = Mathf.Lerp(startingAlpha, 1, time / duration);

            yield return null;
        }
        Debug.Log("Finished fading in");
    }

    /// <summary>
    /// Fades an object out using a canvas group.
    /// </summary>
    /// <param name="cg">The canvas group of the object to be faded</param>
    /// <param name="duration">The duration of the fade</param>
    private IEnumerator FadeOut(CanvasGroup cg, float duration)
    {
        Debug.Log("Fading out");
        // Make the object non-interactable to prevent player mistakes
        cg.interactable = true;

        float startingAlpha = cg.alpha;
        float time = 0;
        while (time <= duration)
        {
            time += Time.deltaTime;

            // Use lerp to interpolate the fade
            cg.alpha = Mathf.Lerp(startingAlpha, 0, time / duration);

            yield return null;
        }

        // Finally, disable the given gameObject
        cg.gameObject.SetActive(false);
        Debug.Log("Finished fading out");
    }
    #endregion

    #region Test Variables
    #if UNITY_INCLUDE_TESTS

    public void FadeIn_Test(CanvasGroup cg, float duration) => StartCoroutine(FadeIn(cg, duration));
    public void FadeOut_Test(CanvasGroup cg, float duration) => StartCoroutine(FadeOut(cg, duration));

#endif
    #endregion
}
