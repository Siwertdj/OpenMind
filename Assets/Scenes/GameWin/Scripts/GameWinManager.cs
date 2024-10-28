// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using UnityEngine;

/// <summary>
/// Script containing methods to be used on button click in the gamewin scene.
/// Each method calls a method from <see cref="GameManager"/>.
/// </summary>
public class GameWinManager : MonoBehaviour
{
    /// <summary>
    /// Calls <see cref="GameManager.RestartStoryScene"/>
    /// </summary>
    public void Restart()
    {
        GameManager.gm.RestartStoryScene();
    }

    /// <summary>
    /// Calls <see cref="GameManager.EndGame"/>
    /// </summary>
    public void Quit()
    {
        GameManager.gm.EndGame();
    }
}
