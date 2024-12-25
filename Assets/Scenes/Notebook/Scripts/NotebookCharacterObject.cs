using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookCharacterObject : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] private CharacterIcon characterIcon;
    [SerializeField] private TMP_Text nameText;

    public void SetInfo(CharacterInstance character)
    {
        nameText.text = character.characterName;
        characterIcon.SetAvatar(character);

        if (SettingsManager.sm != null)
            nameText.fontSize = SettingsManager.sm.GetFontSize() * 1.4f;
    }
}