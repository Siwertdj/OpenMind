// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The manager class for the StorySelectScene
/// </summary>
public class StorySelectionManager : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] public StoryObject[] stories;
    
    // Game Events
    [Header("Events")]
    public GameEvent onIntroLoaded;

    private void Awake()
    {
        UpdateButtons();
    }

    /// <summary>
    /// Prepares buttons in StorySelect-scene based on UserData.
    /// </summary>
    public void UpdateButtons()
    {
        // TODO: Preferably find a way to do this without 'GameObject.Find'
        if (FetchUserData.Loader.GetUserDataValue(FetchUserData.UserDataQuery.storyAWon))
        {
            // story b unlocked
            GameObject.Find("StoryB_Button").GetComponent<GameButton>().interactable = true;
            // enable 'complete'-text
            GameObject.Find("StoryA_Button").GetComponent<GameButton>().GetComponentsInChildren<TMP_Text>()[2].gameObject.SetActive(true);
        }
        if (FetchUserData.Loader.GetUserDataValue(FetchUserData.UserDataQuery.storyBWon))
        {
            // story c unlocked
            GameObject.Find("StoryC_Button").GetComponent<GameButton>().GetComponentInChildren<GameButton>().interactable = true;
            // enable 'complete'-text
            GameObject.Find("StoryB_Button").GetComponent<GameButton>().GetComponentsInChildren<TMP_Text>()[2].gameObject.SetActive(true);
        }
        if (FetchUserData.Loader.GetUserDataValue(FetchUserData.UserDataQuery.storyCWon))
        {
            // enable 'complete'-text
            GameObject.Find("StoryC_Button").GetComponent<GameButton>().GetComponentsInChildren<TMP_Text>()[2].gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Starts Story A
    /// </summary>
    public void StoryASelected()
    {
        StartIntro(0);
    }
    
    /// <summary>
    /// Starts Story B
    /// </summary>
    public void StoryBSelected()
    {
        StartIntro(1);
    }
    
    /// <summary>
    /// Starts Story C
    /// </summary>
    public void StoryCSelected()
    {
        StartIntro(2);
    }

    /// <summary>
    /// Starts the intro belonging to the story the player chose
    /// </summary>
    /// <param name="storyid">The story that the player chose</param>
    void StartIntro(int storyid)
    {
        StartCoroutine(LoadIntro(storyid));
    }
    
    /// <summary>
    /// Loads the appropriate intro scene
    /// </summary>
    /// <param name="storyid">The story that the player chose</param>
    IEnumerator LoadIntro(int storyid)
    {
        // Start the loadscene-operation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("IntroStoryScene", LoadSceneMode.Additive);
        
        // Within this while-loop, we wait until the scene is done loading. We check this every frame
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        onIntroLoaded.Raise(this, stories[storyid]);
        
        // Finally, when the data has been sent, we then unload our currentscene
        SceneManager.UnloadSceneAsync("StorySelectScene");  // unload this scene; no longer necessary
    }
    
}
