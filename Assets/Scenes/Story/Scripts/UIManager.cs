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

    public void Transition(string text)
    {
        UpdateTransitionText(text);
        transitionCoroutine = StartCoroutine(TransitionAnimation());
    }

    private void UpdateTransitionText(string text) { transitionText.text = text; }

    IEnumerator TransitionAnimation()
    {
        transitionCanvas.SetActive(true);
        yield return new WaitForSeconds(transitionDuration);
        transitionCanvas.SetActive(false);
    }

}
