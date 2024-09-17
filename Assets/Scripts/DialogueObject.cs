using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDialogue", menuName = "Dialogue")]
public class DialogueObject : ScriptableObject
{
    public int id;
    public Question questionType;
    public string answerText;
}
