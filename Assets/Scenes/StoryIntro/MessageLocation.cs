using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageLocation : MonoBehaviour
{
    public                 TextMessage tmi;
    [SerializeField] private GameObject          tm;
    
    // Start is called before the first frame update
    void Start()
    {
        tm = tmi.message; 
    }
    
}
