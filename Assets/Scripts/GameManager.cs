using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool notebookOn = false;

    // Start is called before the first frame update
    void Start()
    {
        // Makes this GameManager persistent throughout the scenes.
        DontDestroyOnLoad(this.gameObject);
        
        
        //LoadDialogueScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnloadDialogueScene();
        }
    }
    
    public void LoadDialogueScene()
    {
        SceneManager.LoadScene("Dialogue Test", LoadSceneMode.Additive);
    }

    public void UnloadDialogueScene()
    {
        SceneManager.UnloadSceneAsync("Dialogue Test");
    }

    public void ToggleNotebookScene()
    {
        if (notebookOn)
        {

            SceneManager.UnloadSceneAsync("NotebookScene");
            notebookOn = false;
        }
        else
        {
            SceneManager.LoadScene("NotebookScene", LoadSceneMode.Additive);
            notebookOn= true;
        }
    }
}
