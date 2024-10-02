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
    [SerializeField] private float transitionDuration;
    
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
        transitionCanvas.SetActive(true);
        yield return new WaitForSeconds(transitionDuration);
        transitionCanvas.SetActive(false);
    }

}
