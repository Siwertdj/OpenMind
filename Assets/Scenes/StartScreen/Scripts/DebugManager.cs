using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        DontDestroyOnLoad(gameObject);
        string root = @"cd\";
        string cdCommand = $@"cd C:\Users\sande\RiderProjects\OpenMind";
        string gitCommand = "git rev-parse --abbrev-ref HEAD";
        
        Process process = new Process();
        ProcessStartInfo info = new ProcessStartInfo();
        
        info.FileName = "cmd.exe";
        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        
        process.StartInfo = info;
        process.Start();
        
        using (StreamWriter sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                sw.WriteLine(root);
                sw.WriteLine(cdCommand);
                sw.WriteLine(gitCommand);
            }
        }
        
        string output = process.StandardOutput.ReadToEnd();
        string branchName = output.Split("\n")[^3];
        IsDebug = branchName != "main";
        Debug.unityLogger.logEnabled = IsDebug;
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }
    
    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        //TODO: add handling of messages
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }
}
