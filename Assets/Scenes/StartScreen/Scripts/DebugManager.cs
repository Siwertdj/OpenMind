using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using static UnityEditor.EditorUtility;

/// <summary>
/// Manages all debug options. Every debug message and error handling should go through this class
/// </summary>
public class DebugManager : MonoBehaviour
{
    /// <summary>
    /// A boolean that shows whether the game is in debug mode or not. This boolean should be used when displaying any debug messages or other debug stuff.
    /// The value of this boolean is determined by whether the current branch is the main branch or not.
    /// </summary>
    public static bool IsDebug { get; private set; }
    
    private void Awake()
    {
        #if DEBUG
        DontDestroyOnLoad(gameObject);
        Debug.unityLogger.logEnabled = IsDebug;
        #endif
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }
    
    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
            DisplayDialog(type.ToString(), $"Condition: {condition}\nStacktrace:\n{stackTrace}",
                "OK");
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }
}
