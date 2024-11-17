// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class to handle events regarding networking.
/// </summary>
public class NetworkEvents
{
    private Dictionary<string, List<Action<object>>> events = new ();
    
    /// <summary>
    /// Raises all events that are subscribed to the given signature.
    /// If clear is set to true, all events regarding the given signature will be removed after called.
    /// </summary>
    public void Raise(string signature, object data, bool clear = true)
    {
        if (!events.ContainsKey(signature))
            return;
        
        foreach (var action in events[signature])
            action(data);
        
        if (clear)
            events[signature] = new List<Action<object>>();
    }
    
    /// <summary>
    /// Subscribes the given action to the given signature.
    /// </summary>
    public void Subscribe(string signature, Action<object> action)
    {
        if (!events.ContainsKey(signature))
            events.Add(signature, new List<Action<object>>());
        
        events[signature].Add(action);
    }
}
