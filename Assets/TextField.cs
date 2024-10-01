using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextField : MonoBehaviour
{
    private TMP_Text textObject;

    void Awake()
    {
        textObject = GetComponent<TMP_Text>();
    }

    public void SetText(string text)
    {
        textObject.text = text;
    }
}
