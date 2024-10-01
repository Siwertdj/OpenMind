using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 

public class CanvasManager : MonoBehaviour
{
    public TextMeshProUGUI PrologueText;
    public Button NextButton;
    public Image Image;
    public Sprite[] Sprites;
    public Toggle ImageToggler; 
    
    private TextMeshProUGUI buttonText; 
    private int counter = 0;
    //private int spriteIndex = 0;
    
    public float DisplayTime = 2f; // Determines how many seconds the image is visible.
    
        
    // Start is called before the first frame update
    void Start()
    {
        Image.enabled = false;
        ImageToggler.onValueChanged.AddListener(OnToggleValueChanged);
        ImageToggler.gameObject.SetActive(false);  
        PrologueText.text =
            "This game is developed with the help of students from Utrecht University with an educational purpose.";
        buttonText = NextButton.GetComponentInChildren<TextMeshProUGUI>(); 
        NextButton.onClick.AddListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        counter++;

        switch (counter)
        {
            case 1:
                PrologueText.text = "In order to get into the right mindset for the purpose of the game, you will first be introduced to the theme and purpose of the game.";
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
                ShowColorImage();
                break; 
            case 5:
                PrologueText.text = "The right answer is 'grey'! The image shown was actually black and white.";
                buttonText.text = "Show me again";
                break; 
            case 6:
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

    private void ShowColorImage()
    {
        Image.sprite = Sprites[0];
        StartCoroutine(ShowAndHide());
        
    }
    
    private IEnumerator ShowAndHide()
    {
        Image.enabled = true; 
        yield return new WaitForSeconds(DisplayTime);
        Image.enabled = false; 
        PrologueText.text = "What color was the butterfly?";
    }
    
    public void OnToggleValueChanged(bool isOn)
    {
        if(isOn) Image.sprite = Sprites[0];
        else
        {
            Image.sprite = Sprites[1];
        }
    }
    
    
}
