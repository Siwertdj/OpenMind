using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if DEBUG
using static UnityEditor.EditorUtility;
#endif

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
    private HashSet<string> ignores = new HashSet<string>();
    
    /// <summary>
    /// A bool that determines whether pops are disabled
    /// </summary>
    private bool DisablePopups = false;
    
    private void Awake()
    {
        #if DEBUG
        // DontDestroyOnLoad(gameObject);
        // IsDebug = true;
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
        #if DEBUG
        if (!(DisablePopups || ignores.Contains(condition) || type == LogType.Log))
        {
            int result = DisplayDialogComplex(type.ToString(),
                $"Condition: {condition}\nStacktrace:\n{stackTrace}",
                "OK", "Ignore this message from now on.", "Disable all pop-ups");
            
            if (result == 1)
                ignores.Add(condition);
            else if (result == 2)
                DisablePopups = true;
        }
        #endif
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }
}
