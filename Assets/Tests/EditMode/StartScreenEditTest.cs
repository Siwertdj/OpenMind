using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor.SceneManagement;

public class StartScreenTest : MonoBehaviour
{
    private StartMenuManager sm;
    
    [OneTimeSetUp]

    public void Setup()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Menus/StartScreenScene.unity");
        sm = GameObject.Find("StartMenuManager").GetComponent<StartMenuManager>();
    }
    
}
