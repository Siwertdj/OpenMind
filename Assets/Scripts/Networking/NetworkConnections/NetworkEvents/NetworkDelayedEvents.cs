// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections.Generic;

/// <summary>
/// Acts similar to <see cref="NetworkEvents"/>, but with a delay.
/// </summary>
public class NetworkDelayedEvents
{
    private Dictionary<string, object> inputData = new();
    private NetworkEvents              events    = new();
    
    /// <summary>
    /// Raises all events that are subscribed to the given signature.
    /// If clear is set to true, all events regarding the given signature will be removed after called.
    /// </summary>
    public string Raise(string signature, bool clear, string eventName) =>
        events.Raise(signature, inputData[signature], clear, eventName);
    
    /// <summary>
    /// Adds data to the given signature for when the event can be called.
    /// If data has already been given to a signature, it is overridden with the new data.
    /// </summary>
    public void InputData(string signature, object data) =>
        inputData[signature] = data;
    
    /// <summary>
    /// Subscribes the given action to the given signature.
    /// </summary>
    public void Subscribe(string signature, Action<object> action) =>
        events.Subscribe(signature, action);
    
}
