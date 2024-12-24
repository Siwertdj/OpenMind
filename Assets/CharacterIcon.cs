using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class CharacterIcon : MonoBehaviour
{
    private const float ZOOM_FACTOR = 1.8f;

    [SerializeField] private Image avatarImageRef;

    private CharacterInstance character;

    public void SetAvatar(CharacterInstance character)
    {
        this.character = character;

        // Set the correct sprite
        avatarImageRef.sprite = character.avatarEmotions.Where(es => es.Item1 == Emotion.Neutral).First().Item2;

        // Set the image location to the center of the face
        var rectTransform = avatarImageRef.GetComponent<RectTransform>();
        rectTransform.pivot = character.data.facePivot;
        rectTransform.localPosition = Vector2.zero;

        SetAvatarSize();
    }

    private void OnRectTransformDimensionsChange()
    {
        // Check edge case
        if (character == null) 
            return;

        // Rescale the avatar if the icon scale has changed
        SetAvatarSize();
    }

    /// <summary>
    /// Set the size of the avatar to match the size of the icon.
    /// </summary>
    private void SetAvatarSize()
    {
        // The ratio between the width & height of the character's sprite
        float ratio = character.avatarEmotions.Where(
            es => es.Item1 == Emotion.Neutral).First().Item2.rect.width /
            character.avatarEmotions.Where(
            es => es.Item1 == Emotion.Neutral).First().Item2.rect.height;

        // Set the avatar size according to the icon's size
        float width = ZOOM_FACTOR * Mathf.Abs(GetComponent<RectTransform>().rect.height);
        float height = width / ratio;
        avatarImageRef.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }
}
