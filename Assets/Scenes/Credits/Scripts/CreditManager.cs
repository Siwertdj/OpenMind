using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private RectTransform creditTransform;
    [SerializeField] private RectTransform thanksTransform;
    [SerializeField] private GameButton button;

    private float duration = 20;

    // Start is called before the first frame update
    async void Start()
    {
        await ScrollCredits();
        button.interactable = true;
    }

    public Task ScrollCredits()
    {
        var tcs = new TaskCompletionSource<bool>();

        float creditStartY = creditTransform.position.y;
        float creditEndY = Screen.height + creditTransform.rect.height / 2;
        float thanksStartY = thanksTransform.position.y;
        float thanksEndY = Screen.height / 2f;

        DoCredits(creditStartY, creditEndY, thanksStartY, thanksEndY, tcs);

        return tcs.Task;
    }

    public void DoCredits(float creditStartY, float creditEndY, float thanksStartY, float thanksEndY, TaskCompletionSource<bool> tcs = null)
    {
        StartCoroutine(AnimateScroll(duration, creditStartY, creditEndY, thanksStartY, thanksEndY, tcs));
    }

    private IEnumerator AnimateScroll(
        float duration, 
        float creditStartY, float creditEndY, 
        float thanksStartY, float thanksEndY, 
        TaskCompletionSource<bool> tcs)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;

            float y = (creditEndY - creditStartY) * (time / duration);
            creditTransform.position = new Vector2(creditTransform.position.x, creditStartY + y);

            Debug.Log(thanksTransform.position.y < thanksEndY);
            if (thanksTransform.position.y < thanksEndY)
            {
                thanksTransform.position = new Vector2(thanksTransform.position.x, thanksStartY + y);
            }

            yield return null;
        }

        tcs?.SetResult(true);
    }

    public void CloseCredits()
    {
        if (SceneManager.GetSceneByName("StartScreenScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CreditsScene");
        }
    }
}
