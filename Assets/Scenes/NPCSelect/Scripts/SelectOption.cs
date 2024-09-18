using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class SelectOption : MonoBehaviour
{
    public Image avatar;
    public int characterId;
    
    // Start is called before the first frame update
    void Awake()
    {
        avatar = this.gameObject.GetComponentInChildren<Image>();
    }

    private void Inactive()
    {
        avatar.color = new Color(100,100,100,100);
    }
    
}
