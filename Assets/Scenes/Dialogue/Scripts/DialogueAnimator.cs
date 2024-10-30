// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;

/// <summary>
/// Handles putting dialogue on the screen
/// </summary>
public class DialogueAnimator : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField] private TMP_Text text;

    [Header("Settings")]
    [SerializeField] private float delayInSeconds = 0.07f; // The delay between each letter being put on the screen
    [SerializeField] private float delayAfterSentence = 1.5f; // The delay to write a new sentence after the previous sentence is finished

    private Coroutine outputCoroutine;
    private AudioSource audioSource;

    public bool InDialogue { get; private set; } = false; // Is there dialogue on the screen?

    private bool isOutputting = false; // Is dialogue currently being written?
    private List<string> currentDialogue;
    private int dialogueIndex = 0;
    private string currentSentence = "";

    [NonSerialized] public UnityEvent OnDialogueComplete = new();

    /// <summary>
    /// Sets the properties of the text when loaded
    /// </summary>
    void Awake()
    {
        text.enableAutoSizing = false;
        text.fontSize = 40;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Puts dialogue on screen.
    /// </summary>
    /// <param name="output">The text that is written</param>
    /// <param name="pitch">The pitch of the characters voice.</param>
    public void WriteDialogue(List<string> output, float pitch = 1)
    {
        if (!isOutputting) // Don't start writing something new if something is already being written
        {
            dialogueIndex = 0;
            audioSource.pitch = pitch;

            InDialogue = true;
            currentDialogue = output;
            WriteSentence(output[dialogueIndex]);
        }
    }

    /// <summary>
    /// Helper function for <see cref="WriteDialogue"/> which writes a single sentence.
    /// </summary>
    /// <param name="output">The current sentence which needs to be written</param>
    private void WriteSentence(string output)
    {
        if (!isOutputting)
        {
            isOutputting = true;
            currentSentence = output;
            outputCoroutine = StartCoroutine(WritingAnimation(output, 0));
        }
    }

    /// <summary>
    /// Skips dialogue that is being written
    /// </summary>
    public void SkipDialogue()
    {
        if (!InDialogue)
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

    /// <summary>
    /// Closes dialogue once it is finished
    /// </summary>
    private void EndDialogue()
    {
        // Close dialogue
        InDialogue = false;
        OnDialogueComplete.Invoke();
    }

    /// <summary>
    /// Writes the text to the screen letter by letter
    /// </summary>
    /// <param name="output">The text that needs to be written</param>
    /// <param name="stringIndex">The index of the letter that is being written</param>
    /// <returns></returns>
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
            if (output[stringIndex] != ' ' && stringIndex % 2 == 0)
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

                if (dialogueIndex >= currentDialogue.Count)
                    Debug.Log("Index out of boudns?????");

                if (dialogueIndex < currentDialogue.Count)
                    WriteSentence(currentDialogue[dialogueIndex]);
            }
        }
    }
}
