using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class CharacterIcon : MonoBehaviour
{
    [SerializeField] private Image avatarImageRef;

    public void SetAvatar(CharacterInstance character)
    {        
        // Set the correct sprite
        avatarImageRef.sprite = character.avatar;

        // Wait for layout to be initialized before setting scale & pos
        StartCoroutine(SetSizeAndPosition(character));
    }

    private IEnumerator SetSizeAndPosition(CharacterInstance character)
    {
        yield return new WaitForEndOfFrame();

        var rectTransform = avatarImageRef.GetComponent<RectTransform>();

        // Set the correct size
        float width = 1.8f * Mathf.Abs(GetComponent<RectTransform>().rect.height);
        float height = width / (character.avatar.rect.width / character.avatar.rect.height);
        rectTransform.sizeDelta = new Vector2(width, height);

        // Set the image location to the center of the face
        rectTransform.pivot = character.data.facePivot;
        rectTransform.localPosition = new Vector2(0, 0);
    }
}
