// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manager class for the notebook scene.
/// </summary>
public class NotebookManager : MonoBehaviour
{
    public GameObject inputField;
    public GameObject characterCustomInput;
    public GameObject characterInfo;
    public Button personalButton;
    [NonSerialized] public NotebookData notebookData;
    private CharacterInstance currentCharacter;
    private int currentCharacterId;
    private Button selectedButton;
    [SerializeField] private Button[] nameButtons;

    [Header("Prefab References")]
    [SerializeField] private GameObject logObjectPrefab;
    [SerializeField] private GameObject inputObjectPrefab;
    [SerializeField] private GameObject pagePrefab;

    private int currentPageIndex = 0;

    private Queue<GameObject> allCharacterInfo = new();

    /// <summary>
    /// On startup, go to the personal notes and make sure the correct data is shown
    /// </summary>
    void Start()
    {
        // close character notes
        characterInfo.SetActive(false);
        // Open personal notes
        inputField.SetActive(true);
        // assign character names to buttons
        InitializeCharacterButtons();
        // get notebookdata
        notebookData = GameManager.gm.notebookData;
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();
        selectedButton = personalButton;
        personalButton.interactable = false;
    }
    
    /// <summary>
    /// Initialize the character buttons, use their names as the button text and add the button event.
    /// </summary>
    public void InitializeCharacterButtons()
    {
        // Initialise all buttons for which there are characters
        for (int i = 0; i < GameManager.gm.currentCharacters.Count; i++)
        {
            int id = i;
            var button = nameButtons[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = 
                GameManager.gm.currentCharacters[i].characterName;

            button.onClick.AddListener(()=>OpenCharacterTab(id));
        }
        // Set any remaining buttons to inactive
        for (int i = GameManager.gm.currentCharacters.Count; i < nameButtons.Length; i++)
        {
            nameButtons[i].gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Open the personal notes tab and load the notes.
    /// </summary>
    public void OpenPersonalNotes()
    {
        // Save character notes
        SaveNotes();
        // Close the character tab 
        characterInfo.SetActive(false);
        // activate input
        inputField.SetActive(true);
        inputField.GetComponent<TMP_InputField>().text = notebookData.GetPersonalNotes();
        // Make button clickable
        ChangeButtons(personalButton);
    }
    
    /// <summary>
    /// Open a character tab and load and display the notes on that character.
    /// </summary>
    private void OpenCharacterTab(int id)
    {
        currentCharacterId = id;

        // Destroy info from the previous character
        foreach (Transform page in characterInfo.transform)
            Destroy(page.gameObject);

        // Save notes
        SaveNotes();

        // Deactivate the personal notes tab if it's opened
        if (inputField.activeInHierarchy)
            inputField.SetActive(false);

        // Activate written character notes
        characterInfo.SetActive(true);

        // Get the character instance
        currentCharacter = GameManager.gm.currentCharacters[id];

        // Create the custom input field object
        var inputObject = Instantiate(inputObjectPrefab);
        inputObject.GetComponent<TMP_InputField>().text = notebookData.GetCharacterNotes(currentCharacter);
        characterCustomInput = inputObject; // Also set the reference so that it can be saved
        allCharacterInfo.Enqueue(inputObject);

        for (int i = 0; i < 4; i++)
        {


            // Create objects for all q&a pairs
            var log = notebookData.GetAnswers(currentCharacter);
            foreach (var (question, answer) in log)
            {
                var logObject = Instantiate(logObjectPrefab).GetComponent<NotebookLogObject>();
                logObject.SetText(question, answer);

                allCharacterInfo.Enqueue(logObject.gameObject);

                // Make sure the layout group is displayed properly
                LayoutRebuilder.ForceRebuildLayoutImmediate(logObject.GetComponent<RectTransform>());
            }
        }

        CreateCharacterPages();

        // Make button clickable
        ChangeButtons(nameButtons[id]);
    }

    /// <summary>
    /// Navigates to the page which is <paramref name="direction"/> pages away
    /// from the current page. Will navigate to the previous/next character tab
    /// if the page index is not reachable.
    /// </summary>
    public void NavigatePages(int direction)
    {
        int childCount = characterInfo.transform.childCount;
        int newIndex = currentPageIndex + direction;

        if (newIndex < 0)
        {
            // The index is less than 0, so navigate to previous character
            NavigateCharacters(currentCharacterId - 1);
        }
        else if (newIndex >= childCount)
        {
            // The index is greater than the amount of pages, so navigate to next character
            NavigateCharacters(currentCharacterId + 1);
        }
        else
        {
            // The index is within the current character's bounds, so navigate to given page
            var prevPage = characterInfo.transform.GetChild(currentPageIndex).gameObject;
            var newPage = characterInfo.transform.GetChild(currentPageIndex + direction).gameObject;

            prevPage.SetActive(false);
            newPage.SetActive(true);

            currentPageIndex = newIndex;
        }
    }

    /// <summary>
    /// Navigates to the character tab with the given id.
    /// </summary>
    private void NavigateCharacters(int id)
    {
        // Set the id so that we remain within the correct bounds
        if (id >= GameManager.gm.currentCharacters.Count)
            id = 0;
        else if (id < 0)
            id = GameManager.gm.currentCharacters.Count - 1;

        OpenCharacterTab(id);
    }

    /// <summary>
    /// Create all pages in one go. Creates multiple GameObjects each containing
    /// a part of the notebook data regarding the selected character. All pages apart
    /// from the first are automatically set to inactive.
    /// </summary>
    private void CreateCharacterPages()
    {
        currentPageIndex = 0;

        // Create the first page
        var page = Instantiate(pagePrefab, characterInfo.transform);

        while (allCharacterInfo.Count > 0)
        {
            // Dequeue the object
            var go = allCharacterInfo.Dequeue();
            go.GetComponent<RectTransform>().SetParent(page.transform, false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(page.GetComponent<RectTransform>());

            if (IsPageOverflowing(page.GetComponent<RectTransform>()))
            {
                // Page is overflowing, so create a new page
                page = Instantiate(pagePrefab, characterInfo.transform);

                // Add object to this new page instead
                go.GetComponent<RectTransform>().SetParent(page.transform, false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(page.GetComponent<RectTransform>());

                // TODO: Find an alternative to this, as it is quite slow
                // It is currently necessary to make sure the layout size is set immediately
                Canvas.ForceUpdateCanvases();

                // Set the new page to be inactive
                page.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Returns true if the collective height of the children is greater than
    /// the height of the given gameObject.
    /// </summary>
    /// <param name="parent">The RectTransform of the gameObject to be checked, 
    /// this gameObject must also have a VerticalLayoutGroup component.
    /// </param>
    public bool IsPageOverflowing(RectTransform parent)
    {
        // Get the VerticalLayoutGroup component        
        if (!parent.TryGetComponent<VerticalLayoutGroup>(out var layoutGroup))
        {
            Debug.LogError("VerticalLayoutGroup not found!");
            return false;
        }

        // Get padding and spacing from the VerticalLayoutGroup
        float spacing = layoutGroup.spacing;
        float padding = layoutGroup.padding.top + layoutGroup.padding.bottom;

        // Calculate the total height of all children
        float totalHeight = 0f;
        int childCount = parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = parent.GetChild(i).GetComponent<RectTransform>();
            if (child != null)
            {
                totalHeight += child.rect.height;
                if (i > 0) totalHeight += spacing; // Add spacing between items
            }
        }

        totalHeight += padding;

        // Compare the total height of content to the height of the layout group container
        float containerHeight = parent.rect.height;
        return totalHeight > containerHeight;
    }

    /// <summary>
    /// Make the character log visible or not.
    /// </summary>
    public void ToggleCharacterInfo()
    {
        characterInfo.SetActive(!characterInfo.activeInHierarchy);
    }
    
    /// <summary>
    /// Save the notes on the (character) inputfield to the notebookdata.
    /// </summary>
    public void SaveNotes()
    {
        if (inputField.activeInHierarchy)
        {
            // Save the written personal text to the notebook data
            notebookData.UpdatePersonalNotes(inputField.GetComponent<TMP_InputField>().text);
        }
        else
        {
            // Save the written character text to the notebook data
            notebookData.UpdateCharacterNotes(currentCharacter, 
                characterCustomInput.GetComponent<TMP_InputField>().text);
        }
    }
    
    /// <summary>
    /// Make the clicked button non-interactable and make the last clicked buttons interactable again.
    /// </summary>
    private void ChangeButtons(Button clickedButton)
    {
        selectedButton.interactable = true;
        selectedButton = clickedButton;
        selectedButton.interactable = false;
    }

    #region Test Variables
    #if UNITY_INCLUDE_TESTS
    public Button[] Test_GetNameButtons() => nameButtons;
    #endif
    #endregion
}
