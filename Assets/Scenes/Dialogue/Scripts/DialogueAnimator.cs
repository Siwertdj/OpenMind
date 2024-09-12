using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueAnimator : MonoBehaviour
{
    [SerializeField] private float delayInSeconds = 0.07f; // The delay between each letter being put on the screen
    [SerializeField] private float delayAfterSentence = 1.5f; // The delay to write a new sentence after the previous sentence is finished
    
    private TMP_Text text;
    private Coroutine outputCoroutine;
    private AudioSource audioSource;

    public bool inDialogue = false; // Is there dialogue on the screen?
    private bool isOutputting = false; // Is currently being written?
    private List<string> currentDialogue;
    private int dialogueIndex = 0;
    private string currentSentence = "";

    public UnityEvent OnDialogueComplete;

    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void WriteDialogue(List<string> output)
    {
        if (!isOutputting) // Don't start writing something new if something is already being written
        {
            dialogueIndex = 0;

            inDialogue = true;
            currentDialogue = output;
            WriteSentence(output[dialogueIndex]);
        }
    }

    private void WriteSentence(string output)
    {
        if (!isOutputting)
        {
            isOutputting = true;
            currentSentence = output;
            outputCoroutine = StartCoroutine(WritingAnimation(output, 0));
        }
    }

    public void SkipDialogue()
    {
        if (!inDialogue)
            return;

        if (isOutputting)
        {
            // Write full sentence and then stop writing
            isOutputting = false;
            StopCoroutine(outputCoroutine);
            text.text = currentSentence;
            dialogueIndex++;
        }
        else if (dialogueIndex < currentDialogue.Count)
        {
            WriteSentence(currentDialogue[dialogueIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // Close dialogue
        inDialogue = false;
        OnDialogueComplete.Invoke();
    }

    IEnumerator WritingAnimation(string output, int stringIndex)
    {
        // If a new sentence is started, first clear the old sentence
        if (stringIndex == 0)
            text.text = "";

        // Make sure the sentence is not finished
        if (stringIndex < output.Length)
        {
            // Write the current letter
            text.text += output[stringIndex];
            if (output[stringIndex] != ' ' && stringIndex % 3 == 0)
                audioSource.Play();

            // Wait and continue with next letter
            yield return new WaitForSeconds(delayInSeconds);
            outputCoroutine = StartCoroutine(WritingAnimation(output, stringIndex + 1));
        }
        else
        {
            // If sentence is finished, stop outputting
            isOutputting = false;
            dialogueIndex++;

            // If there are more sentences, start writing the next sentence after s seconds
            if (dialogueIndex < currentDialogue.Count)
            {
                yield return new WaitForSeconds(delayAfterSentence);
                WriteSentence(currentDialogue[dialogueIndex]);
            }
            else
            {
                yield return new WaitForSeconds(delayAfterSentence);
                EndDialogue();
            }
        }
    }
}
