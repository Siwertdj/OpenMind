// This program has been developed by students from the bachelor Computer Science at Utrecht University within the Software Project course.
// © Copyright Utrecht University (Department of Information and Computing Sciences)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Instances of this class act as special buttons for the NPCSelect scene.
/// </summary>
public class SelectOption : MonoBehaviour
{
    public  CharacterInstance character;
    private Image             avatar;
    private TMP_Text          characterNameText;
    
    /// <summary>
    /// On startup, set the sprite and name to that of the proper character and check whether it is active or not.
    /// </summary>
    void Start()
    {
        avatar = GetComponentInChildren<Image>();
        //if 
        //avatar.sprite = character.avatar;

        characterNameText = GetComponentInChildren<TMP_Text>();
        characterNameText.text = character.characterName;

        if (!character.isActive)
            SetInactive();
    }

    /// <summary>
    /// If a character is inactive grey out the button.
    /// </summary>
    private void SetInactive()
    {
        avatar.color = new Color(0.6f,0.6f,0.6f,0.6f);
        characterNameText.alpha = 0.6f;
    }
    
}
