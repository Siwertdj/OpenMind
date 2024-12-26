// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// An expansion of Unity's built-in button that raises a GameEvent when clicked.
/// </summary>
public class GameButton : Button
{
    public GameEvent gameEvent;

    private new void Start()
    {
        // add method below to listeners of the onclick.
        // This method raises the aforementioned event 
        onClick.AddListener(RaiseEvent);
    }
    
    private void RaiseEvent()
    { 
        gameEvent.Raise(this);
    }
}