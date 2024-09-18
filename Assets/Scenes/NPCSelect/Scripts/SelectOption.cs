using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Button = UnityEngine.UI.Button;

public class SelectOption : MonoBehaviour
{
    public Image avatar;
    public int characterId;
    public TMP_Text characterNameText;
    
    // Start is called before the first frame update
    void Awake()
    {
        avatar = this.gameObject.GetComponentInChildren<Image>();
        characterNameText = GetComponentInChildren<TMP_Text>();
    }

    private void Inactive()
    {
        avatar.color = new Color(100,100,100,100);
    }
    
}
