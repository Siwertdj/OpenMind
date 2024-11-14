using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSelectScroller : MonoBehaviour
{
    private Transform scrollable;
    private Transform[] children;

    private bool isNavigating = false;
    private Coroutine scrollCoroutine;

    [SerializeField] private float scrollDuration;

    private int _selectedChild;
    private int selectedChild 
    { 
        get { return _selectedChild; } 
        set 
        {
            // Make sure the target value is not too big or small
            _selectedChild = Mathf.Clamp(value, 0, children.Length - 1);
        } 
    }

    private void Start()
    {
        scrollable = transform.GetChild(0);

        // Populate list of children
        children = new Transform[scrollable.childCount];
        for (int i = 0; i < children.Length; i++)
            children[i] = scrollable.GetChild(i);

        selectedChild = children.Length / 2;
        StartCoroutine(InstantNavigate());
    }

    private IEnumerator InstantNavigate()
    {
        yield return new WaitForEndOfFrame();
        scrollable.localPosition = GetTargetPos(selectedChild);
    }

    //TODO: Should be replaced by a swiping motion instead of buttons
    public void NavigateLeft()
    {
        selectedChild -= 1;
        NavigateToChild(selectedChild);
    }

    public void NavigateRight()
    {
        selectedChild += 1;
        NavigateToChild(selectedChild);
    }

    private void NavigateToChild(int childIndex)
    {
        if (isNavigating)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(NavigationCoroutine(childIndex));
    }

    private IEnumerator NavigationCoroutine(int childIndex)
    {
        Debug.Log("Navigating");
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

        isNavigating = false;
    }

    private Vector3 GetTargetPos(int childIndex)
    {
        return new Vector2(
            -children[childIndex].localPosition.x,
            scrollable.localPosition.y);
    }
}
