using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gameButtons;

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
}
