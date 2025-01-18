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
    private                GameObject        characterCustomInput;
    private                TMP_InputField    personalCustomInput;
    private                CharacterInstance currentCharacter;
    private                int               currentCharacterIndex;
    private                int               currentPageIndex;
    private                Button            selectedButton;
    [NonSerialized] public NotebookData      notebookData;

    [Header("Settings")]
    [SerializeField] private float tabAnimationDuration;
    [SerializeField] private float expandedTabHeight;
    [SerializeField] private float collapsedTabHeight;

    [Header("Field References")]
    [SerializeField] private GameObject characterInfo;
    [SerializeField] private GameObject personalInfo;

    [Header("Tab Select Button References")]
    [SerializeField] private GameButton personalButton;
    [SerializeField] private GameButton[] nameButtons;

    [Header("Component References")]
    [SerializeField] private TMP_Text currentTabText;

    [Header("Prefab References")]
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private GameObject inactiveNotePrefab;
    [SerializeField] private GameObject introObjectPrefab;
    [SerializeField] private GameObject inputObjectPrefab;
    [SerializeField] private GameObject titleObjectPrefab;

    /// <summary>
    /// On startup, go to the personal notes and make sure the correct data is shown
    /// </summary>
    private void Start()
    {
        // Assign character names to buttons
        InitializeTabButtons();

        // Get notebookdata
        notebookData = GameManager.gm.notebookData;
        
        // Create personal notes
        CreatePersonalPage();

        // Open custom notes page
        OpenPersonalNotes();

        // Add listener to recreate tab when font size is changed
        SettingsManager.sm.OnTextSizeChanged.AddListener(OnTextSizeChanged);
    }
    
    /// <summary>
    /// Initialize the tab buttons (custom notes & character tabs), 
    /// For characters, use their names as the button text and add the button event.
    /// </summary>
    public void InitializeTabButtons()
    {
        // Initialise all buttons for which there are characters
        var characters = GameManager.gm.currentCharacters;
        for (int i = 0; i < characters.Count; i++)
        {
            int id = i;
            var button = nameButtons[i];
            
            // Set the icon avatar
            var icon = button.GetComponentInChildren<CharacterIcon>();
            icon.SetAvatar(characters[i]);

            // Inactive characters should have a different looking icon
            if (!characters[i].isActive)
            {
                icon.BackgroundColor = new Color(0.7f, 0.7f, 0.7f);
                icon.OverlayColor = new Color(0.7f, 0.7f, 0.7f);
            }

            button.onClick.AddListener(()=>OpenCharacterTab(id));
        }

        // Set any remaining buttons to inactive
        for (int i = characters.Count; i < nameButtons.Length; i++)
        {
            nameButtons[i].gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Open the personal notes tab and load the notes.
    /// </summary>
    public void OpenPersonalNotes()
    {
        // An id of -1 signifies the custom notes tab
        currentCharacterIndex = -1;

        // Close the character tab 
        characterInfo.SetActive(false);

        // Activate written personal notes  
        personalInfo.SetActive(true);

        // Make button clickable
        ChangeButtons(personalButton);
        
        // Set the appropriate footer text
        currentTabText.text = "Personal Notes";
    }
    
    /// <summary>
    /// Create the personal notes page.
    /// The page consists of the "personal notes" title and the inputfield.
    /// </summary>
    private void CreatePersonalPage()
    {
        // Create personal notes title object  
        var titleObject = Instantiate(titleObjectPrefab);  
        titleObject.GetComponent<NotebookTitleObject>().SetInfo("Personal Notes");  
  
        // Create the custom input field object  
        var inputObject = Instantiate(inputObjectPrefab);  
        var inputObjectField = inputObject.GetComponent<TMP_InputField>();

        // Set the saved text to the notebook
        inputObjectField.text = notebookData.GetPersonalNotes();
        inputObjectField.placeholder.GetComponentInChildren<TMP_Text>().text
            = "Write down your thoughts!";
        
        inputObjectField.onEndEdit.AddListener(_ => SavePersonalData());  
  
        inputObjectField.pointSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_SMALL_TEXT;  
        personalCustomInput = inputObjectField; // Also set the reference so that it can be saved  

        // Create the personal notes page  
        var pagePersonal = Instantiate(pagePrefab, personalInfo.transform);
        
        //Set its parent with a vertical layout group component  
        titleObject.GetComponent<RectTransform>().SetParent(pagePersonal.transform, false);
        inputObject.GetComponent<RectTransform>().SetParent(pagePersonal.transform, false);
        
        // Force rebuild the layout so the height values are correct  
        LayoutRebuilder.ForceRebuildLayoutImmediate(pagePersonal.GetComponent<RectTransform>());
    }
    
    /// <summary>
    /// Open a character tab and load and display the notes on that character.
    /// </summary>
    private void OpenCharacterTab(int id)
    {
        // If id is out of bounds, open personal notes
        if (id < 0 || id >= GameManager.gm.currentCharacters.Count)
        {
            OpenPersonalNotes();
            return;
        }

        currentCharacterIndex = id;
        
        // Destroy info from the previous character
        // Keep track of number of pages so we display the correct number
        int prevPageCount = characterInfo.transform.childCount;
        foreach (Transform page in characterInfo.transform)
            Destroy(page.gameObject);

        // Deactivate the personal notes tab if it's opened and remove that page
        if (personalInfo.gameObject.activeInHierarchy)
            personalInfo.SetActive(false);
        
        // Activate written character notes
        characterInfo.SetActive(true);

        // Get the character instance
        currentCharacter = GameManager.gm.currentCharacters[id];

        // The queue which will hold all the character's info
        // This info will later be divided into pages
        Queue<GameObject> allCharacterInfo = new();

        // If character is inactive, create note object
        if (!currentCharacter.isActive)
        {
            var inactiveNoteObject = Instantiate(inactiveNotePrefab);
            var noteText = inactiveNoteObject.GetComponentInChildren<TMP_Text>();

            noteText.fontSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_SMALL_TEXT;
            noteText.text = $"Note: {currentCharacter.characterName} {GameManager.gm.story.victimDialogue}";

            allCharacterInfo.Enqueue(inactiveNoteObject);
        }

        // Create icon & name object
        var introObject = Instantiate(introObjectPrefab);
        introObject.GetComponent<NotebookCharacterObject>().SetInfo(currentCharacter);
        allCharacterInfo.Enqueue(introObject);

        // Create the custom input field object
        var inputObject = Instantiate(inputObjectPrefab);
        var inputObjectField = inputObject.GetComponent<TMP_InputField>();
        
        inputObjectField.text = notebookData.GetCharacterNotes(currentCharacter);
        inputObjectField.placeholder.GetComponentInChildren<TMP_Text>().text
            = notebookData.GetCharacterPlaceholder(currentCharacter);
        
        inputObjectField.onEndEdit.AddListener(_ => SaveCharacterData());
        
        inputObjectField.pointSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_SMALL_TEXT;
        characterCustomInput = inputObject; // Also set the reference so that it can be saved
        allCharacterInfo.Enqueue(inputObject);

        CreateCharacterPages(allCharacterInfo);

        // Make button clickable
        ChangeButtons(nameButtons[id]);

        // Set appropriate footer text
        currentTabText.text = 
            currentCharacter.characterName + "\n" +
            "Page " + (currentPageIndex + 1) + "/" + 
            (characterInfo.transform.childCount - prevPageCount);

    }

    /// <summary>
    /// Navigates to the page which is <paramref name="direction"/> pages away
    /// from the current page. Will navigate to the previous/next character tab
    /// if the page index is not reachable.
    /// </summary>
    public void NavigatePages(int direction)
    {
        int newIndex = currentPageIndex + direction;

        if (newIndex < 0)
        {
            // The index is less than 0, so navigate to previous character
            NavigateCharacters(currentCharacterIndex - 1);
        }
        else if (newIndex >= characterInfo.transform.childCount)
        {
            // The index is greater than the amount of pages, so navigate to next character
            NavigateCharacters(currentCharacterIndex + 1);
        }
        else
        {
            // The index is within the current character's bounds, so navigate to given page
            var prevPage = characterInfo.transform.GetChild(currentPageIndex).gameObject;
            var newPage = characterInfo.transform.GetChild(currentPageIndex + direction).gameObject;

            prevPage.SetActive(false);
            newPage.SetActive(true);

            currentPageIndex = newIndex;        
            
            // Set appropriate footer text
            currentTabText.text =
                currentCharacter.characterName + "\n" +
                "Page " + (currentPageIndex + 1) + "/" + characterInfo.transform.childCount;
        }
    }

    /// <summary>
    /// Navigates to the character tab with the given id.
    /// </summary>
    private void NavigateCharacters(int id)
    {
        // Set the id so that we remain within the correct bounds
        int characterCount = GameManager.gm.currentCharacters.Count;
        if (id > characterCount)
            id = -1;
        else if (id < -1)
            id = characterCount - 1;

        OpenCharacterTab(id);
    }

    /// <summary>
    /// Create all pages in one go. Creates multiple GameObjects each containing
    /// a part of the notebook data regarding the selected character. All pages apart
    /// from the first are automatically set to inactive.
    /// </summary>
    private void CreateCharacterPages(Queue<GameObject> allCharacterInfo)
    {
        currentPageIndex = 0;

        // Create the first page
        var page = Instantiate(pagePrefab, characterInfo.transform);

        // While there are still notebook objects to be placed
        while (allCharacterInfo.Count > 0)
        {
            // Dequeue an object
            var go = allCharacterInfo.Dequeue();

            // Set its parent with a vertical layout group component
            go.GetComponent<RectTransform>().SetParent(page.transform, false);

            // Force rebuild the layout so the height values are correct
            LayoutRebuilder.ForceRebuildLayoutImmediate(page.GetComponent<RectTransform>());

            // If it doesn't fit, make a new page and place it in there
            // If it does fit, move on to the next object
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
    /// Save the notes of the character inputfield to the notebookdata.
    /// </summary>
    public void SaveCharacterData()
    {
        // Save the written character text to the notebook data
        notebookData.UpdateCharacterNotes(currentCharacter, 
            characterCustomInput.GetComponent<TMP_InputField>().text);
    }

    /// <summary>
    /// Save the notes of the personal inputfield to the notebookdata.
    /// </summary>
    public void SavePersonalData()
    {
        // Save the written personal text to the notebook data
        notebookData.UpdatePersonalNotes(personalCustomInput.GetComponent<TMP_InputField>().text);
    }
    
    /// <summary>
    /// Make the clicked button non-interactable and make the last clicked buttons interactable again.
    /// </summary>
    private void ChangeButtons(Button clickedButton)
    {
        if (selectedButton != null)
        {
            selectedButton.interactable = true;

            // Collapse the previously clicked button
            selectedButton.GetComponent<NotebookTabButton>().AnimateTab(
                collapsedTabHeight, tabAnimationDuration);
        }

        // Expand the clicked button
        clickedButton.GetComponent<NotebookTabButton>().AnimateTab(
            expandedTabHeight, tabAnimationDuration);

        selectedButton = clickedButton;
        selectedButton.interactable = false;
    }

    /// <summary>
    /// What happens when the player changes text size settings.
    /// Resets current page and apply new values.
    /// </summary>
    private void OnTextSizeChanged()
    {
        // Reopen character tab (automatically applies settings)
        OpenCharacterTab(currentCharacterIndex);
    }

    #region Test Variables
#if UNITY_INCLUDE_TESTS
    public TMP_InputField Test_PersonalInputField { get { return personalCustomInput; } }
    public Button Test_GetPersonalButton() => personalButton;
    public Button[] Test_GetNameButtons() => nameButtons;
    public GameObject Test_CharacterInfoField { get { return characterInfo; } }
    #endif
    #endregion
}
