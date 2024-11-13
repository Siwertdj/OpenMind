using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSelectScroller : MonoBehaviour
{
    private Transform scrollable;
    private Transform[] children;

    private int _selectedChild;
    private int selectedChild 
    { 
        get { return _selectedChild; } 
        set 
        {
            // Make sure the target value is not too big or small
            int target = value;
            target = Mathf.Clamp(target, 0, children.Length - 1);
            Debug.Log($"target: {target}");

            _selectedChild = target;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        scrollable = transform.GetChild(0);

        // Populate list of children
        children = new Transform[scrollable.childCount];
        for (int i = 0; i < children.Length; i++)
            children[i] = scrollable.GetChild(i);

        selectedChild = children.Length / 2;

        Debug.Log($"Length of list: {children.Length}");
    }

    //TODO: Should be replaced by a swiping motion instead of buttons
    public void NavigateLeft()
    {
        selectedChild -= 1;
        Debug.Log($"Selected child: {selectedChild}");
        NavigateToChild();
    }

    public void NavigateRight()
    {
        selectedChild += 1;
        Debug.Log($"Selected child: {selectedChild}");
        NavigateToChild();
    }

    private void NavigateToChild()
    {
        var parentRect = (RectTransform)transform;
        var offset = parentRect.rect.size / 2 - (Vector2)children[selectedChild].localPosition;
        Debug.Log($"Offset: {offset}");
        parentRect.localPosition += (Vector3)offset;
    }
}
