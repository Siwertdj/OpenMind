// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;
using UnityEngine.EventSystems;

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
    [SerializeField] private bool audioEnabled = true;
    [SerializeField] private bool overrideDefaultSpeed = true;

    private Coroutine outputCoroutine;
    private AudioSource audioSource;

    /// <summary>
    /// Is there dialogue currently on the screen?
    /// </summary>
    public bool InDialogue { get; private set; } = false;

    /// <summary>
    /// Is dialogue currently being written?
    /// </summary>
    public bool IsOutputting { get; private set; } = false;

    private List<string> currentDialogue;
    private int dialogueIndex = 0;
    private string currentSentence = "";

    [NonSerialized] public UnityEvent OnDialogueComplete = new();

    /// <summary>
    /// Sets the properties of the text when loaded
    /// </summary>
    void Awake()
    {
        if (text == null)
            return;

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
        if (!IsOutputting) // Don't start writing something new if something is already being written
        {
            dialogueIndex = 0;
            if (audioEnabled && audioSource != null)
                audioSource.pitch = pitch;

            InDialogue = true;
            currentDialogue = output;
            WriteSentence(output[dialogueIndex]);
        }
    }

    /// <summary>
    /// An overload of WriteDialogue() for single lines.
    /// </summary>
    /// <param name="output">The line that is written</param>
    /// <param name="pitch">The pitch of the characters voice.</param>
    public void WriteDialogue(string output, float pitch = 1)
    {
        // Convert the output to a list of strings
        // and then execute the WriteDialogue() function
        WriteDialogue(new List<string> { output }, pitch);
    }

    /// <summary>
    /// Immediately stops writing new dialogue to the screen.
    /// Does <b>not</b> skip dialogue or do anything other than stopping the writing coroutine.
    /// </summary>
    public void CancelWriting()
    {
        if (IsOutputting)
        {
            IsOutputting = false;
            StopCoroutine(outputCoroutine);
        }
    }

    /// <summary>
    /// Helper function for <see cref="WriteDialogue"/> which writes a single sentence.
    /// </summary>
    /// <param name="output">The current sentence which needs to be written</param>
    private void WriteSentence(string output)
    {
        if (!IsOutputting)
        {
            IsOutputting = true;
            currentSentence = output;
            outputCoroutine = StartCoroutine(WritingAnimation(output, 0));

            if (SettingsManager.sm.ttsEnabled)
                SettingsManager.sm.tts.Speak(currentSentence);
        }
    }

    /// <summary>
    /// Skips dialogue that is being written
    /// </summary>
    public void SkipDialogue()
    {
        // Don't do anything if the game is paused
        if (GameManager.gm?.IsPaused == true)
            return;

        if (!InDialogue)
            return;

        if (IsOutputting)
        {
            // Write full sentence and then stop writing
            IsOutputting = false;
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
    private IEnumerator WritingAnimation(string output, int stringIndex)
    {
        // Don't write if the game is paused
        // '?' is used to make sure there is already an instance of the GameManager
        while (GameManager.gm?.IsPaused == true) 
            yield return null;

        // If a new sentence is started, first clear the old sentence
        if (stringIndex == 0)
            text.text = "";

        // Make sure the sentence is not finished
        if (stringIndex < output.Length)
        {
            // Write the current letter
            text.text += output[stringIndex];
            if (output[stringIndex] != ' ' && stringIndex % 2 == 0 && audioEnabled && audioSource != null)
                audioSource.Play();

            // Wait and continue with next letter
            float delay = overrideDefaultSpeed ? delayInSeconds : SettingsManager.sm.TalkingDelay;
            yield return new WaitForSeconds(delay);
            outputCoroutine = StartCoroutine(WritingAnimation(output, stringIndex + 1));
        }
        else
        {
            // If sentence is finished, stop outputting
            IsOutputting = false;
            dialogueIndex++;

            // If there are more sentences, start writing the next sentence after s seconds
            if (dialogueIndex < currentDialogue.Count)
            {
                yield return new WaitForSeconds(delayAfterSentence);

                if (dialogueIndex >= currentDialogue.Count)
                    Debug.LogError("dialogueIndex is greater than the amount of dialogue");

                if (dialogueIndex < currentDialogue.Count)
                    WriteSentence(currentDialogue[dialogueIndex]);
            }
        }
    }

#region Test Variables
#if UNITY_INCLUDE_TESTS
    public float Test_DelayInSeconds
    { 
        get { return delayInSeconds; }
        set { delayInSeconds = value; } 
    }

    public float Test_DelayAfterSentence
    {
        get { return delayAfterSentence; }
    }

    public void Test_SetTextComponent(TMP_Text text) => this.text = text;
#endif
#endregion
}
