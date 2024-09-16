using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour
{
    public void EndGame()
    {
        GameManager.Instance.EndGame();
    }
    public void Restart()
    {
        GameManager.Instance.RestartStoryScene();
    }
    public void Retry()
    {
        GameManager.Instance.RetryStoryScene();
    }
}
