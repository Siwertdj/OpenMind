using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 

public class CanvasManager : MonoBehaviour
{
    // Object within the scene
    public TextMeshProUGUI PrologueText;
    public Button NextButton;
    public Image Image;
    public Toggle ImageToggler; 
    // Variables with a connection to the scene. 
    public Sprite[] Sprites; // Contains all sprites that are shown in this scene.
    public float DisplayTime = 2f; // Determines how many seconds the image is visible.
    public RectTransform PrologueTextRectTransform; 
    private TextMeshProUGUI buttonText; 
    // Variables with only a meaning in this code. 
    private int clickCounter; // Keeps track of the amount of times the button is clicked. 
    private bool coroutineIsRunning; // Enables a lock system for coroutines. 
        
    
    /// <summary>
    /// Start is called before the first frame update and makes sure each component
    /// is in the right state for the start of the scene. 
    /// </summary>
    void Start()
    {
        // First text that is shown. 
        PrologueText.text =
            "This game is developed with the help of students from Utrecht University with an educational purpose.";
        // Connect methods that handle events of togglers and buttons. 
        ImageToggler.onValueChanged.AddListener(OnToggleValueChanged);
        NextButton.onClick.AddListener(ButtonClicked);
        // Disactivate object that are not yet required. 
        ImageToggler.gameObject.SetActive(false);  
        Image.enabled = false; 
        // Initialize required components. 
        buttonText = NextButton.GetComponentInChildren<TextMeshProUGUI>(); 
    }

    /// <summary>
    /// ButtonClicked handles the set up of the screen for different parts of the
    /// prologue scene. The set up depends on the number of times the 'next' button
    /// is clicked. When a coroutine is currently running, this method will not do
    /// anything. 
    /// </summary>
    private void ButtonClicked()
    {
        if (!coroutineIsRunning) // When coroutine is running, do nothing. 
        {
            clickCounter++;
            switch (clickCounter)
            {
                case 1:
                    PrologueTextRectTransform.sizeDelta = new Vector2(PrologueTextRectTransform.sizeDelta.x, -400f);
                    PrologueText.text =
                        "In order to get into the right mindset for the purpose of the game, you will first be introduced to the theme and purpose of the game.";
                    break;
                case 2:
                    PrologueText.text = "Not everything is as it seems..";
                    buttonText.text = "Start";
                    break;
                case 3:
                    buttonText.text = "Next";
                    PrologueText.text = "Next, you will be shown an image.";
                    break;
                case 4:
                    PrologueText.text = "";
                    ShowColorImage();
                    break;
                case 5:
                    PrologueText.text = "The correct answer is grey! The image shown was actually black and white.";
                    buttonText.text = "Show again";
                    break;
                case 6:
                    PrologueTextRectTransform.sizeDelta = new Vector2(PrologueTextRectTransform.sizeDelta.x, 900f);
                    PrologueText.text =
                        "By adding a grid of colored lines, your brain makes you believe the image is actually in color.";
                    buttonText.text = "Next";
                    Image.enabled = true;
                    ImageToggler.gameObject.SetActive(true);
                    break;
                default:
                    PrologueText.text = "You've reached the end";
                    break;
            }
        }
    }

    /// <summary>
    /// When the value of the 'Show Lines' toggler is changed, this method is called. 
    /// When the toggle is on, the sprite with the colored lines is shown.
    /// When the toggle is off, the sprite without the colored lines is shown. 
    /// </summary>
    /// <param name="isOn"></param>
    public void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            // TODO: let the image shown depend on the version of the game that is played
            Image.sprite = Sprites[0];
        }
        else
        {
            Image.sprite = Sprites[1];
        }
    }
    
    /// <summary>
    /// Method that is called when the user clicked on the 'Next' button after
    /// it is said that an image will be shown. This method will call the
    /// coroutine that actually performs the showing and hiding of the image.
    /// This is the way coroutines work. 
    /// </summary>
    private void ShowColorImage()
    {
        Image.sprite = Sprites[0];
        // TODO: currently, the image that will be shown is hard coded. In the final version this image depends on the version of the story that will be played. 
        StartCoroutine(ShowAndHide());
    }
    
    /// <summary>
    /// This method is a coroutine that is called when an image has to be shown
    /// for an amount of time in which there can not be any other functionality. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowAndHide()
    {
        coroutineIsRunning = true;                              // Lock 
        Image.enabled = true;                                   // Show the image
        yield return new WaitForSeconds(DisplayTime);           // Wait DisplayTime amount of seconds
        Image.enabled = false;                                  // Hide the image
        PrologueText.text = "What color was the butterfly?";    // This line must be here, because it has to be shown after the image has disappeared.
        coroutineIsRunning = false;                             // Unlock
    }
    
}
