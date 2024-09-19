using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void EndGame()
    {
        gameManager.EndGame();
    }
    public void Restart()
    {
        gameManager.RestartStoryScene();
    }
    public void Retry()
    {
        gameManager.RetryStoryScene();
    }
}
