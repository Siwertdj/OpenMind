using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Button = UnityEngine.UI.Button;

public class SelectOption : MonoBehaviour
{
    public CharacterInstance character;

    private Image avatar;
    private TMP_Text characterNameText;
    
    // Start is called before the first frame update
    void Start()
    {
        avatar = GetComponentInChildren<Image>();
        avatar.sprite = character.avatar;

        characterNameText = GetComponentInChildren<TMP_Text>();
        characterNameText.text = character.characterName;        
    }

    private void SetInactive()
    {
        avatar.color = new Color(100,100,100,100);
    }
    
}
