// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Handles putting dialogue on the screen
/// </summary>
public class DialogueAnimator : MonoBehaviour
{
    [SerializeField] private float delayInSeconds = 0.07f; // The delay between each letter being put on the screen
    [SerializeField] private float delayAfterSentence = 1.5f; // The delay to write a new sentence after the previous sentence is finished
    
    private TMP_Text text;
    private Coroutine outputCoroutine;
    private AudioSource audioSource;

    [FormerlySerializedAs("InDialogue")] public bool inDialogue = false; // Is there dialogue on the screen?
    private bool isOutputting = false;
    private List<string> currentDialogue;
    private List<Emotion> currentEmotion = new List<Emotion>();
    private GameObject[] background;
    private int dialogueIndex = 0;
    private string currentSentence = "";

    public UnityEvent OnDialogueComplete;

    /// <summary>
    /// Sets the properties of the text when loaded
    /// </summary>
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.enableAutoSizing = false;
        text.fontSize = 40;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Puts dialogue on screen.
    /// </summary>
    /// <param name="output">The text that is written</param>
    /// <param name="moods">The list of moods for sentences></param>
    /// <param name="background">The list of sprites loaded></param>
    /// <param name="pitch">The pitch of the characters voice.</param>
    public void WriteDialogue(List<string> output, List<Emotion> emotions, GameObject[] background, float pitch = 1)
    {


        if (!isOutputting) // Don't start writing something new if something is already being written
        {
            dialogueIndex = 0;
            audioSource.pitch = pitch;

            inDialogue = true;
            currentDialogue = output;

            //currentMood = moods;
            //not every time is mood given, because not always does it matter. Those times it should be neutral expression
            if (emotions == null)
                currentEmotion.Add(Emotion.Neutral);
            else
                currentEmotion = emotions;

            this.background = background;

            //if (currentMood.Count < 1)
            //    Debug.Log("uhh test");
            //    currentMood.Add(DialogueObject.Mood.Neutral);
            //change facial expression at start
            

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
            ChangeEmotion(background, currentEmotion, dialogueIndex);
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
            //ChangeMood(background, currentMood, dialogueIndex);
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
        inDialogue = false;
        OnDialogueComplete.Invoke();
    }

    //change the player background to the correct expression (currently only debug.log is done. No actual change is made
    private void ChangeEmotion(GameObject[] background, List<Emotion> emotions, int dialogueIndex)
    {
        foreach (GameObject bg in background)
        {
            if (bg.CompareTag("Player"))
            {
                Debug.Log(emotions[dialogueIndex]);
            }
        }
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

                //another place to changedialogue
                //ChangeMood(background, currentMood, dialogueIndex);

                if (dialogueIndex < currentDialogue.Count)
                    WriteSentence(currentDialogue[dialogueIndex]);
            }
        }
    }
}
