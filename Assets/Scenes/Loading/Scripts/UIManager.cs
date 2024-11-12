// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager class for UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gameButtons;
    [SerializeField] private GameObject transitionCanvas;
    [SerializeField] private TextMeshProUGUI transitionText;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float fadeTime = 0.5f;

    private Coroutine transitionCoroutine;

    /// <summary>
    /// Opens the GameMenu-scene, hides the UI buttons
    /// </summary>
    public void OpenMenu()
    {
        GameManager.gm.IsPaused = true;
        gameButtons.SetActive(false);
        SceneManager.LoadScene("GameMenuScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Closes the menu of the game, reveals the UI buttons
    /// </summary>
    public void CloseMenu()
    {
        GameManager.gm.IsPaused = false;
        gameButtons.SetActive(true);
    }
    

    /// <summary>
    /// Starts a transition-animation, using coroutines.
    /// </summary>
    /// <param name="text">Text to be displayed during the transition.</param>
    public void Transition(string text)
    {
        UpdateTransitionText(text);
        transitionCoroutine = StartCoroutine(TransitionAnimation());
    }

    /// <summary>
    /// Updates the text to be displayed in the transition.
    /// </summary>
    /// <param name="text">Text to be displayed during the transition.</param>
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