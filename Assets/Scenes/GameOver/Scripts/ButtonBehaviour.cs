using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehaviour : MonoBehaviour
{
    public void EndGame()
    {
        Debug.Log("End game.");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    public void Restart()
    {
        Debug.Log("Restart Level.");
        SceneManager.LoadScene("StoryScene");
    }
}
