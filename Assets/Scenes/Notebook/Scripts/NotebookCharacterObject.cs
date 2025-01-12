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
        characterIcon.BackgroundColor = new Color(0, 0, 0, 0.2f);

        if (SettingsManager.sm != null)
            nameText.fontSize = SettingsManager.sm.GetFontSize() * SettingsManager.M_LARGE_TEXT;
    }
}