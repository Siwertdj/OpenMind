// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// An expansion of Unity's built-in button that raises a GameEvent when clicked.
/// </summary>
public class GameButton : Button
{
    public GameEvent gameEvent;

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

[CustomEditor(typeof(GameButton))]
public class GameButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        // Access the target script
        GameButton gameButton = (GameButton)target;

        // Add a custom field for the GameEvent
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("OnClick GameEvent", EditorStyles.boldLabel);

        // Create GUIContent with a label and a tooltip
        GUIContent gameEventLabel = new GUIContent(
            "Game Event",                // Label
            "This should be set to the \"OnClick\" GameEvent" // Tooltip
        );

        gameButton.gameEvent = (GameEvent)EditorGUILayout.ObjectField(
            gameEventLabel,
            gameButton.gameEvent,
            typeof(GameEvent),
            false
        );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Default Button Settings", EditorStyles.boldLabel);
        base.OnInspectorGUI(); // Draws the default Button Inspector UI

        if (GUI.changed)
        {
            EditorUtility.SetDirty(gameButton); // Mark the object as dirty to save changes
        }
    }
}