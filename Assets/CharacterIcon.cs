using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    [SerializeField] private Image avatarImageRef;

    public void SetAvatar(CharacterInstance character)
    {
        // Set the correct sprite
        avatarImageRef.sprite = character.avatar;
        
        // Set the image location to the center of the face
        var rectTransform = avatarImageRef.GetComponent<RectTransform>();
        rectTransform.sizeDelta = character.avatar.rect.size;

        float x = character.data.facePivot.x * rectTransform.rect.size.x
            - rectTransform.rect.size.x / 2;

        float y =
            character.data.facePivot.y * rectTransform.rect.size.y
            - rectTransform.rect.size.y / 2;

        avatarImageRef.transform.localPosition = new Vector2(
            -x, -y);
            //-(character.data.facePivot.y * rectTransform.rect.size.y
            //- rectTransform.rect.size.y / 2));

        Debug.Log("New pos: " + avatarImageRef.transform.localPosition +
         ", Name: " + character.characterName +
         ", Pivot: " + character.data.facePivot +
         ", Rect size: " + rectTransform.rect.size);
    }

    private void Update()
    {
        Debug.Log("pos: " + avatarImageRef.transform.localPosition);
    }
}
