using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookTabButton : MonoBehaviour
{
    private Coroutine animationCoroutine;

    public void AnimateTab(float newHeight, float duration)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            TabAnimationCoroutine(newHeight, duration));
    }

    private IEnumerator TabAnimationCoroutine(float newHeight, float duration)
    {
        var rect = GetComponent<RectTransform>();

        // Set the pivot to the object's bottom
        rect.pivot = new Vector2(rect.pivot.x, 0);

        float originalHeight = rect.sizeDelta.y;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;

            float height = Mathf.Lerp(
                originalHeight, newHeight,
                time / duration);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);

            yield return null;
        }

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
    }
}
