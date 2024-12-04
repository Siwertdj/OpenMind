using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(GameButton))]
public class GameButtonEditor : Editor
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
#endif