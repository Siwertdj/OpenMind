using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator AnimateCredits()
    {
        float upperEdge = Screen.height;

    }

    public void CloseCredits()
    {
        if (SceneManager.GetSceneByName("StartScreenScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CreditsScene");
        }
    }
}
