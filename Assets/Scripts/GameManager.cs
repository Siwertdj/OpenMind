using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Makes this GameManager persistent throughout the scenes.
        DontDestroyOnLoad(this.gameObject);
        
        
        LoadDialogueScene();
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
}
