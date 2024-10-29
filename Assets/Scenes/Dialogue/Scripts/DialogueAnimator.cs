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

    public bool InDialogue = false; // Is there dialogue on the screen?
    private bool isOutputting = false; // Is currently being written?
    private List<string> currentDialogue;
    private List<DialogueObject.Mood> currentMood = new List<DialogueObject.Mood>();
    private GameObject[] background;
    private int dialogueIndex = 0;
    private string currentSentence = "";

    public UnityEvent OnDialogueComplete;

    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.enableAutoSizing = false;
        text.fontSize = 40;
        audioSource = GetComponent<AudioSource>();
    }

    public void WriteDialogue(List<string> output, List<DialogueObject.Mood> moods, GameObject[] background, float pitch = 1)
    {


        if (!isOutputting) // Don't start writing something new if something is already being written
        {
            dialogueIndex = 0;

            audioSource.pitch = pitch;

            InDialogue = true;
            currentDialogue = output;

            //currentMood = moods;
            //not every time is mood given, because not always does it matter. Those times it should be neutral expression
            if (moods == null)
                currentMood.Add(DialogueObject.Mood.Neutral);
            else
                currentMood = moods;

            this.background = background;

            //if (currentMood.Count < 1)
            //    Debug.Log("uhh test");
            //    currentMood.Add(DialogueObject.Mood.Neutral);
            //change facial expression at start
            

            WriteSentence(output[dialogueIndex]);
        }
    }

    private void WriteSentence(string output)
    {
        if (!isOutputting)
        {
            ChangeMood(background, currentMood, dialogueIndex);
            isOutputting = true;
            currentSentence = output;
            outputCoroutine = StartCoroutine(WritingAnimation(output, 0));
        }
    }

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
            //ChangeMood(background, currentMood, dialogueIndex);
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
        InDialogue = false;
        OnDialogueComplete.Invoke();
    }

    //change the player background to the correct expression (currently only debug.log is done. No actual change is made
    private void ChangeMood(GameObject[] background, List<DialogueObject.Mood> moods, int dialogueIndex)
    {
        foreach (GameObject bg in background)
        {
            if (bg.CompareTag("Player"))
            {
                Debug.Log(moods[dialogueIndex]);
            }
        }
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
            else
            {
                // NOTE: Uncomment the lines below if we want dialogue to automatically end

                //yield return new WaitForSeconds(delayAfterSentence);
                //EndDialogue();
            }
        }
    }
}
