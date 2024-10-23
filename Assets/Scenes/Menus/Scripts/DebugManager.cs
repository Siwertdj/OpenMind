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
    public static bool IsDebug { get; private set; } = false;
    
    /// <summary>
    /// A list of all conditions to be ignored.
    /// </summary>
    private HashSet<string> _ignores = new HashSet<string>(); 
    
    private void Awake()
    {
        #if DEBUG
        DontDestroyOnLoad(gameObject);
        IsDebug = true;
        #endif
        
        Debug.unityLogger.logEnabled = IsDebug;
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }
    
    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (!(_ignores.Contains(condition) || type == LogType.Log))
        {
            if (!DisplayDialog(type.ToString(), $"Condition: {condition}\nStacktrace:\n{stackTrace}",
                    "OK", "Ignore this message from now on."))
            {
                _ignores.Add(condition);
            }
        }
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }
}
