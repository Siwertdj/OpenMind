using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSelectScroller : MonoBehaviour
{
    private Transform scrollable;
    private Transform[] children;

    private bool isNavigating = false;
    private Coroutine scrollCoroutine;

    private GameObject navButtonLeft;
    private GameObject navButtonRight;

    private SwipeDetector swipeDetector;
    public CharacterInstance SelectedCharacter { get; private set; }

    [SerializeField] private float scrollDuration;

    [NonSerialized] public UnityEvent OnCharacterSelected = new();
    [NonSerialized] public UnityEvent NoCharacterSelected = new();

    private int _selectedChild;
    private int selectedChild 
    { 
        get { return _selectedChild; } 
        set 
        {
            // Make sure the target value is not too big or small
            _selectedChild = Mathf.Clamp(value, 0, children.Length - 1);

            // Set selected character
            SelectedCharacter = children[_selectedChild].GetComponentInChildren<SelectOption>().character;

            // Remove correct button if the child is on either edge
            if (_selectedChild == 0)
            {
                navButtonLeft.SetActive(false);
            }
            else if (_selectedChild == children.Length - 1)
                navButtonRight.SetActive(false);
            else
            {
                navButtonLeft.SetActive(true);
                navButtonRight.SetActive(true);
            }
        } 
    }

    private void Start()
    {
        // Add swipe listeners
        swipeDetector = GetComponent<SwipeDetector>();
        swipeDetector.OnSwipeLeft.AddListener(NavigateRight);
        swipeDetector.OnSwipeRight.AddListener(NavigateLeft);

        try
        {
            navButtonLeft = transform.GetChild(1).gameObject;
            navButtonRight = transform.GetChild(2).gameObject;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "NPC Select navigation buttons were not found.\n" +
                "They should be child index 1 and 2 of the scroller.\n" +
                e);
        }

        scrollable = transform.GetChild(0);

        // Populate list of children
        Debug.Log("GameManager Instance: " + GameManager.gm);
        children = new Transform[GameManager.gm.currentCharacters.Count];
        for (int i = 0; i < children.Length; i++)
            children[i] = scrollable.GetChild(i);

        selectedChild = children.Length / 2;
        StartCoroutine(InstantNavigate());
    }

    //TODO: Should be replaced by a swiping motion instead of buttons
    public void NavigateLeft()
    {
        if (selectedChild > 0)
        {
            selectedChild -= 1;
            NavigateToChild(selectedChild);
        }
    }

    public void NavigateRight()
    {
        if (selectedChild < children.Length - 1)
        {
            selectedChild += 1;
            NavigateToChild(selectedChild);
        }
    }

    private bool CanNavigate()
    {
        return true;
    }

    private void NavigateToChild(int childIndex)
    {
        if (isNavigating)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(NavigationCoroutine(childIndex));
    }

    private IEnumerator InstantNavigate()
    {
        yield return new WaitForEndOfFrame();
        scrollable.localPosition = GetTargetPos(selectedChild);
        OnCharacterSelected.Invoke();
    }

    private IEnumerator NavigationCoroutine(int childIndex)
    {
        NoCharacterSelected.Invoke();

        isNavigating = true;
        float time = 0;

        var startPos = scrollable.localPosition;
        var endPos = GetTargetPos(childIndex);

        while (time < scrollDuration)
        {
            time += UnityEngine.Time.deltaTime;

            // Mathf.SmoothStep makes the "animation" ease in and out
            float t = Mathf.SmoothStep(0, 1, Mathf.Clamp01(time / scrollDuration));

            // Vector3.Lerp creates a linear interpolation between the given positions
            scrollable.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        OnCharacterSelected.Invoke();
        isNavigating = false;
    }

    private Vector3 GetTargetPos(int childIndex)
    {
        return new Vector2(
            -children[childIndex].localPosition.x,
            scrollable.localPosition.y);
    }
}
