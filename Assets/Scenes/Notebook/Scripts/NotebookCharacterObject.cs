using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookCharacterObject : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text nameText;

    public void SetInfo(CharacterInstance character)
    {
        nameText.text = character.characterName;
        avatarImage.sprite = character.avatar;
    }
}