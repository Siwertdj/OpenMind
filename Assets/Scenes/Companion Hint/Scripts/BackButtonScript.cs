using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonScript : MonoBehaviour
{
    /// <summary>
    /// Hides the companion scene
    /// </summary>
    public void GoBack() => FindObjectOfType<GameManager>().ToggleCompanionHintScene();
}
