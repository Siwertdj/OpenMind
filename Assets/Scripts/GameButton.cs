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
    [Header("Game Event")]
    [SerializeField] private GameEvent gameEvent;

    private new void Start()
    {
        // Retrieve gameevent from assets folder to reference
        //gameEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets/Data/Events/OnClick.asset");
        // add method below to listeners of the onclick.
        // This method raises the aforementioned event 
        onClick.AddListener(RaiseEvent);
    }
    
    private void RaiseEvent()
    { 
        gameEvent.Raise(this);
    }
}
