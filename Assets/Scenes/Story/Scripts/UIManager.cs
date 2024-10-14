using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gameButtons;
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private TextMeshProUGUI transitionText;
    private Coroutine transitionCoroutine;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float fadeTime = 0.5f;
    
    /// <summary>
    /// Opens the menu of the game, hides the UI buttons
    /// </summary>
    public void OpenMenu()
    {
        // Hide buttoncanvas
        // Reveal menucanvas
        gameButtons.SetActive(false);
        gameMenu.SetActive(true);
    }

    /// <summary>
    /// Closes the menu of the game, reveals the UI buttons
    /// </summary>
    public void CloseMenu()
    {
        // Reveal buttoncanvas
        // Hide menucanvas
        gameButtons.SetActive(true);
        gameMenu.SetActive(false);
    }

    /// <summary>
    /// Starts a transition-animation, using coroutines.
    /// The text to be displayed during this transtition is passed as the parameter in a string.
    /// </summary>
    /// <param name="text"></param>
    public void Transition(string text)
    {
        UpdateTransitionText(text);
        transitionCoroutine = StartCoroutine(TransitionAnimation());
    }

    /// <summary>
    /// Updates the text to be displayed in the transition.
    /// </summary>
    /// <param name="text"></param>
    private void UpdateTransitionText(string text) { transitionText.text = text; }

    /// <summary>
    /// This is the coroutine that animates the scenetransition.
    /// </summary>
    IEnumerator TransitionAnimation()
    {
        //TODO: Magick numbers
        transitionCanvas.SetActive(true);

        // Use a canvas group to adjust alpha value of all children
        CanvasGroup canvasGroup = transitionCanvas.GetComponent<CanvasGroup>();

        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null; // this waits for a single frame
        }
        canvasGroup.alpha = 1f;

        // Mid-point
        yield return new WaitForSeconds(transitionDuration);

        // Fade back to the game
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        transitionCanvas.SetActive(false);
    }

}
