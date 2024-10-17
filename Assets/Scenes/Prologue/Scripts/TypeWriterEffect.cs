using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypeWriterEffect : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private string text;
    private Coroutine typeWriterCoroutine;

    public float typingSpeed = 0.1f; 
    
    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        text = textComponent.text;
        textComponent.text = "";
        typeWriterCoroutine = StartCoroutine(TypeWriterCoroutine());
    }

    IEnumerator TypeWriterCoroutine()
    {
        for (int i = 0; i < text.Length; i++)
        {
            textComponent.text += text[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
