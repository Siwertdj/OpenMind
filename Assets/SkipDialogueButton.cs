using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkipDialogueButton : MonoBehaviour
{
    [SerializeField] private Button skipButton;

    void Update()
    {
        // Check for tap/click
        if (Input.GetMouseButtonDown(0))
        {
            // Don't do anything if the player clicked a UI element
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                Debug.Log("Pointer is over GameObject");
                return;
            }

            Debug.Log("Pointer is not over GameObject");

            skipButton.onClick.Invoke();
        }
    }
}
