// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using UnityEngine;

/// <summary>
/// Script containing methods to be used on button click in the gameover scene.
/// Each method calls a method from <see cref="GameManager"/>.
/// </summary>
public class ButtonBehaviour : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    /// <summary>
    /// Calls <see cref="GameManager.EndGame"/>
    /// </summary>
    public void EndGame()
    {
        gameManager.EndGame();
    }
    
    /// <summary>
    /// Calls <see cref="GameManager.RestartStoryScene"/>
    /// </summary>
    public void Restart()
    {
        gameManager.RestartStoryScene();
    }
    
    /// <summary>
    /// Calls <see cref="GameManager.RetryStoryScene"/>
    /// </summary>
    public void Retry()
    {
        gameManager.RetryStoryScene();
    }
}
