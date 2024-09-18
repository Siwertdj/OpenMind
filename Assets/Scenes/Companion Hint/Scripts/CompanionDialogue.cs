using System.Collections.Generic;
using UnityEngine;

public class CompanionDialogue : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        Debug.Log(gameManager);
        CharacterInstance culprit = gameManager.GetCulprit();
        CharacterInstance randOther = gameManager.GetRandomCharacterNoCulprit();

        List<string> randTraitCulprit = culprit.GetRandomTrait();
        List<string> randTraitOther = randOther.GetRandomTrait();
        
        //TODO: wait until I have a dialogue box to put this in
        Debug.Log(string.Join(", ", randTraitCulprit)); 
        Debug.Log(string.Join(", ", randTraitOther));
    }
}
