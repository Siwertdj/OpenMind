using UnityEngine;

public class CompanionDialogue : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        Debug.Log(gameManager);
        Character culprit = gameManager.GetCulprit();
        Character randOther = gameManager.GetRandomCharacterNoCulprit();

        string randTraitCulprit = culprit.GetRandomTrait(gameManager.random);
        string randTraitOther = randOther.GetRandomTrait(gameManager.random);
        
        //TODO: wait until I have a dialogue box to put this in
        Debug.Log(randTraitCulprit); 
        Debug.Log(randTraitOther);
    }
}
