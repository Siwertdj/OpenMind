using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GreetingManager : MonoBehaviour
{
    GameManager gm;
    TMP_Text textField;
    
    private void Awake()
    {
        gm = GameManager.gm;
        textField = GetComponentInChildren<TMP_Text>();
    }

    public void UpdatePeopleGreeted(Component sender, params object[] data)
    {
        Debug.Log("Dialogue ended, update peoplegreeted.");
        int currentGreeted = gm.AmountCharactersGreeted();
        int total = gm.currentCharacters.Count;
        textField.text = $"People greeted: {currentGreeted}/{total}";
        if (currentGreeted == total)
            Destroy(this.gameObject);;
    }
}
