using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMessageInstance : MonoBehaviour
{
    [SerializeField] private TextMessage        textMessage;
    private string             messageContent;
    private TextMessage.Sender sender;
    public GameObject         message; 
 
        
    /// <summary>
    /// The constructor for <see cref="CharacterInstance"/>.
    /// Sets this instances variables to the information from <see cref="data"/> 
    /// </summary>
    /// <param name="data">A set of <see cref="CharacterData"/></param>
    public TextMessageInstance(TextMessage textMessage)
    {
        Debug.Log("instantiate");
        this.textMessage = textMessage;
        messageContent = textMessage.messageContent;
        sender = textMessage.sender;
        message = textMessage.message;
        
        Text text = message.GetComponentInChildren<Text>();
        text.text = messageContent; 
    }
    
    public void Start()
    {
        Debug.Log("content: " + messageContent);
        Debug.Log("sender: " + sender);
        messageContent = textMessage.messageContent;
        sender = textMessage.sender;
        message = textMessage.message;
        
        Text text = message.GetComponentInChildren<Text>();
        text.text = messageContent; 
    }
}
